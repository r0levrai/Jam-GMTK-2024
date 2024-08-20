using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIScoreCard : MonoBehaviour
{
	[SerializeField] private UIDocument uiTitleSceen;
	private Button playAgainButton;
	public VisualTreeAsset sheet;

	private void Awake()
	{
		VisualElement root = uiTitleSceen.rootVisualElement;
		playAgainButton = root.Q<Button>("BackButton");
	}

	private void Start()
	{
		playAgainButton.clicked += () => SetPreviousCard();
	}

	private void SetPreviousCard()
    {
		GetComponent<UIDocument>().visualTreeAsset = sheet;
    }
}