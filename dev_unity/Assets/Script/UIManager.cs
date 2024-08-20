using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance;
	private void Awake()
	{
		Instance = this;

		promptLabel = uiMainDocument.rootVisualElement.Q<Label>("Prompt");
		submitButton = uiMainDocument.rootVisualElement.Q<Button>("Submit");
		nextButton = uiMainDocument.rootVisualElement.Q<Button>("Next");
		toolGroup = uiMainDocument.rootVisualElement.Q<GroupBox>("ToolSelection");
		pensilBlack = toolGroup.Q<RadioButton>("PencilBlack");
		pensilRed = toolGroup.Q<RadioButton>("PencilRed");
		pensilBlue = toolGroup.Q<RadioButton>("PencilBlue");
		pensilWhite = toolGroup.Q<RadioButton>("PencilWhite");
		eraser = toolGroup.Q<Button>("ClearButton");
		undoButton = toolGroup.Q<Button>("BackButton");
		redoButton = toolGroup.Q<Button>("RedoButton");
		GroupBox brushGroup = toolGroup.Q<GroupBox>("BrushSelection");
		smallBrush = brushGroup.Q<RadioButton>("SmallBrush");
		mediumBrush = brushGroup.Q<RadioButton>("MediumBrush");
		bigBrush = brushGroup.Q<RadioButton>("BigBrush");
	}

	[SerializeField] private UIDocument uiMainDocument;
	private Label promptLabel;
	private Button submitButton, nextButton;
	private GroupBox toolGroup;
	private RadioButton pensilBlack, pensilRed, pensilBlue, pensilWhite;
	private Button undoButton, eraser, redoButton;
	private RadioButton smallBrush, mediumBrush, bigBrush;

	private readonly string[] soundHoverNames = new string[2] { "hover1", "hover2" };

	private void Start()
	{
		submitButton.clicked += () => GameplayManager.Instance.Submit();
		nextButton.clicked += () => Debug.Log("next step !");
		pensilBlack.RegisterCallback<ClickEvent>(evt => { Draw.Instance.OnButtonBrushBlackPress();	SoundManager.Instance.PlayOneShot("penSelect1"); });
		pensilRed.RegisterCallback<ClickEvent>(evt => { Draw.Instance.OnButtonBrushRedPress();		SoundManager.Instance.PlayOneShot("penSelect2"); });
		pensilBlue.RegisterCallback<ClickEvent>(evt => { Draw.Instance.OnButtonBrushBluePress();	SoundManager.Instance.PlayOneShot("penSelect3"); });
		pensilWhite.RegisterCallback<ClickEvent>(evt => { Draw.Instance.OnButtonBrushWhitePress();	SoundManager.Instance.PlayOneShot("penSelect4"); });
		eraser.RegisterCallback<ClickEvent>(evt => { Draw.Instance.OnClearButtonPress();			SoundManager.Instance.PlayOneShot("paperNoise"); });
		undoButton.RegisterCallback<ClickEvent>(evt => { Draw.Instance.OnButtonUndoPress();			SoundManager.Instance.PlayOneShot("clickPapier"); });
		redoButton.RegisterCallback<ClickEvent>(evt => { Draw.Instance.OnButtonRedoPress();			SoundManager.Instance.PlayOneShot("clickPapier"); });
		smallBrush.RegisterCallback<ClickEvent>(evt => { Draw.Instance.OnSmallBrushPress();			SoundManager.Instance.PlayOneShot("penSelect1"); });
		mediumBrush.RegisterCallback<ClickEvent>(evt => { Draw.Instance.OnMediumBrushPress();		SoundManager.Instance.PlayOneShot("penSelect2"); });
		bigBrush.RegisterCallback<ClickEvent>(evt => { Draw.Instance.OnLargeBrushPress();			SoundManager.Instance.PlayOneShot("penSelect3"); });

		RegisterMouseEvent(submitButton);
		RegisterMouseEvent(pensilBlack);
		RegisterMouseEvent(pensilRed);
		RegisterMouseEvent(pensilBlue);
		RegisterMouseEvent(pensilWhite);
		RegisterMouseEvent(eraser);
		RegisterMouseEvent(undoButton);
		RegisterMouseEvent(redoButton);
		RegisterMouseEvent(smallBrush);
		RegisterMouseEvent(mediumBrush);
		RegisterMouseEvent(bigBrush);
	}

	void RegisterMouseEvent(VisualElement elem)
	{
		elem.RegisterCallback<MouseEnterEvent>(_ => { Draw.Instance.drawable = false; SoundManager.Instance.PlayOneShot(soundHoverNames[Random.Range(0, soundHoverNames.Length)]); });
		elem.RegisterCallback<MouseLeaveEvent>(_ => Draw.Instance.drawable = true);
	}

	public void FillTitle(string title)
	{
		promptLabel.text = title;
	}

	public void ActiveTools(bool active = true)
	{
		submitButton.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
		toolGroup.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
		promptLabel.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
		Draw.Instance.drawable = active;
	}
	public void ActiveNextButton(bool active = true)
	{
		nextButton.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
	}
}
