using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameplayManager : MonoBehaviour
{
	[SerializeField] private List<ObjectToDrawn> listObjects = new();
	[SerializeField] private List<ListSprites> stickers;
	[SerializeField] private UIDocument uiDocument;

    private int objectIndex;
	private Label stampedLabel;
	private VisualElement imageContainer;

	public static GameplayManager Instance;
	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
        if (Input.GetKeyUp(KeyCode.E))
            Submit();
	}

	private void Start()
	{
        NewGame();

		var root = uiDocument.rootVisualElement;
		stampedLabel = root.Q<Label>("stampedLabel");
		imageContainer = root.Q<VisualElement>("sticker");
	}

	public void NewGame()
    {
        objectIndex = UnityEngine.Random.Range(0, listObjects.Count);
        FillUI();
    }

	public (Bounds, Vector3) CheckSize()
    {
        LineRenderer[] renderers = Draw.Instance.gameObject.GetComponentsInChildren<LineRenderer>();
        if (!(renderers != null && renderers.Length > 0)) return (new Bounds(), Vector3.zero);

        Bounds lineBounds = renderers[0].bounds;
        for (int i = 0; i < renderers.Length; i++)
        {
            lineBounds.Encapsulate(renderers[i].bounds);
		}

        Vector3 sizeobject = lineBounds.size * ZoomManager.Instance.oneUnitInMeter[ZoomManager.Instance.currentIndex];

        return (lineBounds, sizeobject);
	}

    public void FillUI()
    {
        string text = $"Draw {listObjects[objectIndex].name} to scale!";
        Debug.Log(text);
    }

    public void Submit()
    {
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

        float tailleReal = listObjects[objectIndex].sizeInMeter;
        int note;
        if (sizeDrawn > tailleReal) note = (int)((sizeDrawn - ((sizeDrawn - tailleReal) * 2)) * 10 / tailleReal) + 1;
        else note = (int)(sizeDrawn *10 / tailleReal) +1;

		StartCoroutine(ShowStampedText(note));
    }
	private IEnumerator ShowStampedText(int note)
	{
        switch (note)
		{
			case <= 0:
				stampedLabel.text = $"{note}/10<br>I didn't know that existed...";
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

	[Serializable]
    public struct ObjectToDrawn {
        public string name;
        public float sizeInMeter;
    }

	[Serializable]
	public struct ListSprites
	{
		public List<Sprite> sprites;
	}
}
