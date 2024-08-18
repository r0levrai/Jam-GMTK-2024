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
		eraser = toolGroup.Q<RadioButton>("Eraser");
		undoButton = toolGroup.Q<Button>("RedoButton");
		GroupBox brushGroup = toolGroup.Q<GroupBox>("BrushSelection");
		smallBrush = brushGroup.Q<RadioButton>("SmallBrush");
		mediumBrush = brushGroup.Q<RadioButton>("MediumBrush");
		bigBrush = brushGroup.Q<RadioButton>("BigBrush");
	}

	[SerializeField] private UIDocument uiMainDocument;
	private Label promptLabel;
	private Button submitButton, nextButton;
	private GroupBox toolGroup;
	private RadioButton pensilBlack, pensilRed, pensilBlue, pensilWhite, eraser;
	private Button undoButton;
	private RadioButton smallBrush, mediumBrush, bigBrush;

	private void Start()
	{
		submitButton.clicked += () => GameplayManager.Instance.Submit();
		nextButton.clicked += () => Debug.Log("next step !");
		pensilBlack.RegisterCallback<ClickEvent>(evt => { Draw.Instance.OnButtonBrushBlackPress();});
		pensilRed.RegisterCallback<ClickEvent>(evt => { Draw.Instance.OnButtonBrushRedPress();});
		pensilBlue.RegisterCallback<ClickEvent>(evt => { Draw.Instance.OnButtonBrushBluePress(); });
		pensilWhite.RegisterCallback<ClickEvent>(evt => { Draw.Instance.OnButtonBrushWhitePress();});
		//eraser.RegisterCallback<ClickEvent>(evt => { Draw.Instance.;});
		undoButton.clicked += () => Draw.Instance.OnButtonUndoPress();
		smallBrush.RegisterCallback<ClickEvent>(evt => { Draw.Instance.OnButtonWidthDownPress(); });
		mediumBrush.RegisterCallback<ClickEvent>(evt => { Draw.Instance.OnMediumBrushPress(); });
		bigBrush.RegisterCallback<ClickEvent>(evt => { Draw.Instance.OnButtonWidthUpPress(); });

		RegisterMouseEvent(submitButton);
		RegisterMouseEvent(pensilBlack);
		RegisterMouseEvent(pensilRed);
		RegisterMouseEvent(pensilBlue);
		RegisterMouseEvent(pensilWhite);
		RegisterMouseEvent(eraser);
		RegisterMouseEvent(undoButton);
		RegisterMouseEvent(smallBrush);
		RegisterMouseEvent(mediumBrush);
		RegisterMouseEvent(bigBrush);
	}

	void RegisterMouseEvent(VisualElement elem)
	{
		elem.RegisterCallback<MouseEnterEvent>(_ => Draw.Instance.drawable = false);
		elem.RegisterCallback<MouseLeaveEvent>(_ => Draw.Instance.drawable = true);
	}

	public void FillTitle(string title)
	{
		//animation ???
		promptLabel.text = title;
		SoundManager.Instance.PlaySound("write");
	}

	public void ActiveTools(bool active = true)
	{
		submitButton.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
		toolGroup.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
		Draw.Instance.drawable = active;
	}
	public void ActiveNextButton(bool active = true)
	{
		nextButton.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
	}
}
