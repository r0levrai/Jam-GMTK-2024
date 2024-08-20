using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIHelpsceneManager : MonoBehaviour
{
	[SerializeField] private UIDocument uiTitleSceen;
	private Button mainMenuButton;

	private void Awake()
	{
		VisualElement root = uiTitleSceen.rootVisualElement;
		mainMenuButton = root.Q<Button>("BackButton");
	}

	private void Start()
	{
		mainMenuButton.clicked += () => SceneManager.LoadSceneAsync(0);
	}

}