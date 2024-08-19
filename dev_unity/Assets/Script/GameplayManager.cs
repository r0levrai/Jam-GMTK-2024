using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class GameplayManager : MonoBehaviour
{
	[SerializeField] private List<ObjectToDrawn> listObjects = new();
	[SerializeField] private List<ListSprites> stickers;
	[SerializeField] private UIDocument uiDocument;
	[SerializeField] private ScreenshotDrawing screenshotDrawing;
	[SerializeField] private Classify classifier;
	[SerializeField] private Classify.MyPair[] result;


	[SerializeField] private SpriteRenderer referenceSprite;
	[SerializeField] private RectTransform playerSize;
	[SerializeField] private TMP_Text playerSizeText;
	[SerializeField] private RectTransform trueSize;
	[SerializeField] private TMP_Text trueSizeText;

	private int objectIndex;
	private Label stampedLabel;
	private VisualElement imageContainer;
	private AnimationCurve curve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);


	public static GameplayManager Instance;
	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
        NewGame();
		var root = uiDocument.rootVisualElement;
		stampedLabel = root.Q<Label>("stampedLabel");
		imageContainer = root.Q<VisualElement>("sticker");

		playerSize.localScale = Vector3.zero;
		trueSize.localScale = Vector3.zero;
	}

	public void NewGame()
    {
        objectIndex = UnityEngine.Random.Range(0, listObjects.Count);
		UIManager.Instance.FillTitle($"Draw {listObjects[objectIndex].name}<br>to scale!");
		referenceSprite.sprite = listObjects[objectIndex].spriteObject;
		UIManager.Instance.ActiveTools(true);
	}

	public (Bounds, Vector3) CheckSize()
    {
        LineRenderer[] renderers = Draw.Instance.linesListUndo.ToArray();
        if (!(renderers != null && renderers.Length > 0)) return (new Bounds(), Vector3.zero);

        Bounds lineBounds = renderers[0].bounds;
        for (int i = 0; i < renderers.Length; i++)
        {
            lineBounds.Encapsulate(renderers[i].bounds);
		}

		Vector3 sizeobject = lineBounds.size * ZoomManager.Instance.currentUnitScaleInMeter;

        return (lineBounds, sizeobject);
	}

	private int note = 0;
	public void Submit()
    {
		UIManager.Instance.ActiveTools(false);
		Texture2D texture = ScreenshotDrawing.Instance.GetDrawing();
		//texture.Resize(224, 224);
		result = classifier.Classification(texture);

		(Bounds, Vector3) boundsSizeDrawing = CheckSize();

        float sizeDrawn;
        bool isVertical;
        if (boundsSizeDrawing.Item1.size.y >= boundsSizeDrawing.Item1.size.x) {
            isVertical = true;
            sizeDrawn = boundsSizeDrawing.Item2.y;
        } else {
            isVertical = false;
            sizeDrawn = boundsSizeDrawing.Item2.x;
        }

        float realSize = listObjects[objectIndex].sizeInMeter;
		trueSizeText.text = $"{realSize} m";
		playerSizeText.text = $"{sizeDrawn} m";

        if (sizeDrawn > realSize) note = (int)((sizeDrawn - ((sizeDrawn - realSize) * 2)) * 10 / realSize) + 1;
        else note = (int)(sizeDrawn *10 / realSize) +1;

		StartCoroutine(EndAnimation(boundsSizeDrawing, isVertical));

	}
	
	private IEnumerator ShowStampedText()
	{
        switch (note)
		{
			case <= 0:
				stampedLabel.text = $"{note}/10<br>What a score...";
				note = 0;
				break;
			case 1:
				stampedLabel.text = $"{note}/10";
				break;
			case 2:
				stampedLabel.text = $"{note}/10";
				break;
			case 3:
				stampedLabel.text = $"{note}/10";
				break;
			case 4:
				stampedLabel.text = $"{note}/10";
				break;
			case 5:
				stampedLabel.text = $"{note}/10";
				break;
			case 6:
				stampedLabel.text = $"{note}/10";
				break;
			case 7:
				stampedLabel.text = $"{note}/10";
				break;
			case 8:
				stampedLabel.text = $"{note}/10";
				break;
			case 9:
				stampedLabel.text = $"{note}/10 !!!";
				break;
			case 10:
				stampedLabel.text = $"{note}/10<br>INCREDIBLE !";
				break;
		}

		if (stickers[note].sprites.Count == 1)
			imageContainer.style.backgroundImage = new StyleBackground(stickers[note].sprites[0]);
		else if(stickers[note].sprites.Count > 1)
			imageContainer.style.backgroundImage = new StyleBackground(stickers[note].sprites[UnityEngine.Random.Range(0, stickers[note].sprites.Count)]);

		yield return new WaitForSeconds(.6f);
		stampedLabel.AddToClassList("stamped-text-active");
		stampedLabel.RemoveFromClassList("stamped-text-inactive");

		imageContainer.style.opacity = 0;

		Rect bounds = uiDocument.rootVisualElement.worldBound;
		float randomX = UnityEngine.Random.Range(bounds.width * 0.25f, bounds.width * 0.75f);
		float randomY = UnityEngine.Random.Range(bounds.height * 0.25f, bounds.height * 0.75f);
		float randomRotation = UnityEngine.Random.Range(-45f, 45f);

		imageContainer.style.left = randomX - imageContainer.resolvedStyle.width / 2;
		imageContainer.style.top = randomY - imageContainer.resolvedStyle.height / 2;
		imageContainer.style.transformOrigin = new TransformOrigin(50, 50, 0);
		imageContainer.style.rotate = new Rotate(randomRotation);
		imageContainer.style.height = 500;
		imageContainer.style.width = 500;

		yield return new WaitForSeconds(.6f);
		imageContainer.style.width = 300;
		imageContainer.style.height = 300;
		imageContainer.style.opacity = 1;
	}

	private IEnumerator EndAnimation((Bounds, Vector3) boundsSizeDrawing, bool isVertical = true)
	{
		float elapsedTime = 0.0f, duration;
		
		//show player size
		duration = .2f;
		while (elapsedTime < duration)
		{
			elapsedTime += Time.deltaTime;
			playerSize.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, curve.Evaluate(elapsedTime / duration));
			yield return null;
		}

		//grow ref image
		referenceSprite.transform.position = new Vector3(-1.9f, boundsSizeDrawing.Item1.min.y, 0);
		if (!isVertical) referenceSprite.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
		float targetHeight = (listObjects[objectIndex].sizeInMeter / ZoomManager.Instance.currentUnitScaleInMeter) / referenceSprite.sprite.bounds.size.y;
		Vector3 initialScale = new((listObjects[objectIndex].sizeInMeter / ZoomManager.Instance.currentUnitScaleInMeter) / referenceSprite.sprite.bounds.size.x, 0,1);
		float initialHeight = initialScale.y;
		
		duration = 1;
		elapsedTime = 0;
		while (elapsedTime < duration)
		{
			elapsedTime += Time.deltaTime;
			float newHeight = Mathf.Lerp(initialHeight, targetHeight*0.8f, elapsedTime / duration);
			referenceSprite.transform.localScale = new Vector3(initialScale.x, newHeight, initialScale.z);
			yield return null;
		}
		duration = 1;
		elapsedTime = 0;
		initialHeight += targetHeight * 0.8f;
		Transform camTransform = Camera.main.transform;
		Vector3 originalPosCam = camTransform.position;
		float sizePOVCam = Camera.main.orthographicSize;
		while (elapsedTime < duration)
		{
			elapsedTime += Time.deltaTime;
			float newHeight = Mathf.Lerp(initialHeight, targetHeight, elapsedTime / duration);
			referenceSprite.transform.localScale = new Vector3(initialScale.x, newHeight, initialScale.z);
			camTransform.localPosition = Vector3.Lerp(camTransform.localPosition, originalPosCam + UnityEngine.Random.insideUnitSphere * 1, Time.deltaTime * 3);
			Camera.main.orthographicSize -= 0.1f * Time.deltaTime * 3;
			yield return null;
		}
		referenceSprite.transform.localScale = new Vector3(initialScale.x, targetHeight, initialScale.z);
		camTransform.localPosition = originalPosCam;
		Camera.main.orthographicSize = sizePOVCam;

		//show true size
		duration = .2f;
		elapsedTime = 0;
		while (elapsedTime < duration)
		{
			elapsedTime += Time.deltaTime;
			trueSize.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, curve.Evaluate(elapsedTime / duration));
			yield return null;
		}
		yield return new WaitForSeconds(.5f);

		yield return StartCoroutine(ShowStampedText());

		UIManager.Instance.ActiveNextButton(true);
	}

	[Serializable]
    public struct ObjectToDrawn {
        public string name;
        public float sizeInMeter;
		public Sprite spriteObject;
		public string category;
    }

	[Serializable]
	public struct ListSprites
	{
		public List<Sprite> sprites;
	}
}
