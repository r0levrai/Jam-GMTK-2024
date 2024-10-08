using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIEndScreenManager : MonoBehaviour
{
	[SerializeField] private UIDocument uiTitleSceen;
	private Button playAgainButton, mainMenuButton;

	private bool hasLoaded = false;

	private void Awake()
	{
		if(!hasLoaded)
        {
			VisualElement root = uiTitleSceen.rootVisualElement;
			playAgainButton = root.Q<Button>("AgainButton");
			mainMenuButton = root.Q<Button>("MenuButton");

			hasLoaded = true;
		}
		
	}

	private void Start()
	{
		playAgainButton.clicked += () => SceneManager.LoadSceneAsync(1);
		mainMenuButton.clicked += () => SceneManager.LoadSceneAsync(0);
	}
}
