using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class GameplayManager : MonoBehaviour
{
	[SerializeField] private List<ObjectToDrawn> listObjects = new();
	[SerializeField] private List<String> SnarkyCommentList = new();
	[SerializeField] private List<String> IntroductionCommentList = new();
	[SerializeField] private List<String> ObjectCategeoryNoneCommentList = new();
	[SerializeField] private List<String> ObjectNotFoundList = new();
	[SerializeField] private List<String> ObjectFoundList = new();
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
	private Label stampedLabel, bananaCommentary, bananaScale;
	private VisualElement stickerContainer, bananaFace, bananaRating;
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
		stickerContainer = root.Q<VisualElement>("sticker");
		bananaRating = root.Q<VisualElement>("bananaRating");
		bananaFace = bananaRating.Q<VisualElement>("banana");
		bananaScale = bananaRating.Q<Label>("bananaScale");
		bananaCommentary = bananaRating.Q<Label>("bananaCommentary");

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
		result = classifier.Classification(texture);
		string ClassifierPhrase;
		float ClassifierScore;
		bool objectFound = false;
		int objectFoundIndex = -1;
		for (int i = 0; i <10; i++)
        {
			if (result[i].label == listObjects[objectIndex].category)
			{
				objectFound = true;
				objectFoundIndex = i;
			}
		}

		if (listObjects[objectIndex].category == "none")
		{
			ClassifierPhrase = ObjectCategeoryNoneCommentList[UnityEngine.Random.Range(0, ObjectCategeoryNoneCommentList.Count)];
			ClassifierScore = result[0].score;

		}
		else if (objectFound)
		{
			ClassifierPhrase = ObjectFoundList[UnityEngine.Random.Range(0, ObjectFoundList.Count)];
			ClassifierScore = result[objectFoundIndex].score;
		}
		else
        {
			ClassifierPhrase = ObjectNotFoundList[UnityEngine.Random.Range(0, ObjectNotFoundList.Count)];
			ClassifierScore = result[0].score;

		}
		string x = listObjects[objectIndex].name;
		string y = result[0].label;
		ClassifierPhrase = ClassifierPhrase.Replace("X", x);
		ClassifierPhrase = ClassifierPhrase.Replace("Y", y);
		print(ClassifierPhrase + "    " + ClassifierScore);

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

		//image bananaFace
		bananaScale.text = "scale";
		bananaCommentary.text = "commentary blah blah blah";

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
			stickerContainer.style.backgroundImage = new StyleBackground(stickers[note].sprites[0]);
		else if(stickers[note].sprites.Count > 1)
			stickerContainer.style.backgroundImage = new StyleBackground(stickers[note].sprites[UnityEngine.Random.Range(0, stickers[note].sprites.Count)]);

		yield return new WaitForSeconds(.6f);
		stampedLabel.AddToClassList("stamped-text-active");
		stampedLabel.RemoveFromClassList("stamped-text-inactive");

		stickerContainer.style.opacity = 0;

		Rect bounds = uiDocument.rootVisualElement.worldBound;
		float randomX = UnityEngine.Random.Range(bounds.width * 0.2f, bounds.width * 0.8f);
		float randomY = UnityEngine.Random.Range(bounds.height * 0.2f, bounds.height * 0.8f);
		float randomRotation = UnityEngine.Random.Range(-45f, 45f);

		stickerContainer.style.left = randomX - stickerContainer.resolvedStyle.width / 2;
		stickerContainer.style.top = randomY - stickerContainer.resolvedStyle.height / 2;
		stickerContainer.style.transformOrigin = new TransformOrigin(50, 50, 0);
		stickerContainer.style.rotate = new Rotate(randomRotation);
		stickerContainer.style.height = 500;
		stickerContainer.style.width = 500;

		yield return new WaitForSeconds(.6f);
		stickerContainer.style.width = 300;
		stickerContainer.style.height = 300;
		stickerContainer.style.opacity = 1;

		yield return new WaitForSeconds(.8f);
		bananaRating.AddToClassList("banana-active");
		bananaRating.RemoveFromClassList("stamped-text-inactive");
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
