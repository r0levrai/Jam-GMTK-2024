using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ZoomManager : MonoBehaviour
{
	[SerializeField] private UIDocument uiDocument;
	[SerializeField] private Transform[] environment;
	[SerializeField] private InputAction zoomAction;
	[SerializeField] private InputAction moveAction;
	[SerializeField] private float zoomSpeed = 0.1f;
	[SerializeField] private float moveSpeed = 1f;
	[SerializeField] private float transitionDuration = 0.6f;
	[SerializeField] private float spaceAround = 0f;

	[HideInInspector] public int currentIndex = 1;
	[HideInInspector] public float targetZoom = 0;

	private List<Vector3> initialSizesEnvironment = new List<Vector3>();
	private Queue<int> zoomLevelQueue = new Queue<int>();
	private bool canZoom = true;
	private Slider zoomSlider;
	private Camera cam;

	public static ZoomManager Instance;
	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		cam = Camera.main;
		zoomSlider = uiDocument.rootVisualElement.Q<Slider>("scaleSlider");
		zoomSlider.lowValue = 0f;
		zoomSlider.highValue = environment.Length-.1f;
		zoomSlider.value = currentIndex;
		zoomSlider.RegisterValueChangedCallback(OnZoomSliderChanged);

		environment[currentIndex].gameObject.SetActive(true);
		for (int i = 0; i < environment.Length; i++)
		{
			initialSizesEnvironment.Add(environment[i].localScale);
			if (i == currentIndex) continue;
			environment[i].gameObject.SetActive(false);
		}

		zoomAction.performed += OnZoom;
		zoomAction.Enable();
		moveAction.performed += OnMove;
		moveAction.Enable();
	}


	private void OnDestroy()
	{
		Instance = null;
		zoomSlider.UnregisterValueChangedCallback(OnZoomSliderChanged);
		zoomAction.performed -= OnZoom;
		zoomAction.Disable();
		moveAction.performed -= OnMove;
		moveAction.Disable();
	}

	private float timeElapsed = 0, duration = 0.1f;
	private void Update()
	{
		if (canZoom && timeElapsed<duration)
		{
			environment[currentIndex].localScale = Vector3.Lerp(environment[currentIndex].transform.localScale, initialSizesEnvironment[currentIndex]*targetZoom, timeElapsed / duration);
			timeElapsed += Time.deltaTime;
		}

		ProcessNextZoomLevel();
	}

	private void OnZoom(InputAction.CallbackContext context)
	{
		Vector2 scrollInput = context.ReadValue<Vector2>();
		zoomSlider.value += scrollInput.y * zoomSpeed;
	}

	private void OnZoomSliderChanged(ChangeEvent<float> evt)
	{
		targetZoom = (evt.newValue%1f)+1;
		timeElapsed = 0;

		if (evt.newValue<currentIndex || evt.newValue >= currentIndex+1)
		{
			zoomLevelQueue.Enqueue((int)evt.newValue);
		}
	}

	private void ProcessNextZoomLevel()
	{
		if (zoomLevelQueue.Count > 0 && canZoom)
		{
			int targetIndex = zoomLevelQueue.Dequeue();

			if (targetIndex < currentIndex) //recul d'un niveau de zoom
			{
				StartCoroutine(TransitionToPreviousImage(targetIndex));
			}
			else if (targetIndex > currentIndex) //avance d'un niveau de zoom
			{
				StartCoroutine(TransitionToNextImage(targetIndex));
			}
		}
		//if (Camera.main.orthographicSize < bounds.size.y / 4) //if (Camera.main.orthographicSize > bounds.size.y / 2)
	}


	private void OnMove(InputAction.CallbackContext context)
	{
		Vector2 delta = context.ReadValue<Vector2>();
		Transform currentImage = environment[currentIndex];
		Vector3 newPosition = currentImage.position + (new Vector3(delta.x * moveSpeed, delta.y * moveSpeed, 0) * Time.deltaTime);
		currentImage.position = ClampPositionToBounds(newPosition);		
	}

	private Vector3 ClampPositionToBounds(Vector3 newPosition)
	{
		Vector3 objectSize = environment[currentIndex].GetComponent<SpriteRenderer>().bounds.size / 2;
		Vector3 minScreenBounds = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)) * spaceAround;
		Vector3 maxScreenBounds = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)) * spaceAround;
		Vector3 clampedPosition = newPosition;

		clampedPosition.x = Mathf.Clamp(newPosition.x, minScreenBounds.x + (maxScreenBounds.x - minScreenBounds.x) - objectSize.x, maxScreenBounds.x - (maxScreenBounds.x - minScreenBounds.x) + objectSize.x);
		clampedPosition.y = Mathf.Clamp(newPosition.y, minScreenBounds.y + (maxScreenBounds.y - minScreenBounds.y) - objectSize.y, maxScreenBounds.y - (maxScreenBounds.y - minScreenBounds.y) + objectSize.y);

		return clampedPosition;
	}


	//////////// Animations ////////////
	private IEnumerator TransitionToNextImage(int targetIndex)
	{
		canZoom = false;
		while (currentIndex < targetIndex)
		{
			Transform currentImage = environment[currentIndex];
			Transform nextImage = environment[currentIndex + 1];
			Vector3 startPoint = currentImage.GetChild(0).position;
			Vector3 targetPos = nextImage.position;
			Vector3 startScale = currentImage.transform.localScale;
			nextImage.localScale = Vector3.zero;
			nextImage.position = startPoint;

			nextImage.gameObject.SetActive(true);

			float timeElapsed = 0f;
			while (timeElapsed < transitionDuration)
			{
				nextImage.position = Vector3.Lerp(startPoint, targetPos, timeElapsed / transitionDuration);
				nextImage.transform.localScale = Vector3.Lerp(Vector3.zero, initialSizesEnvironment[currentIndex + 1] * targetZoom, timeElapsed / transitionDuration);
				currentImage.transform.localScale = Vector3.Lerp(startScale, initialSizesEnvironment[currentIndex] * 2, timeElapsed / transitionDuration);
				timeElapsed += Time.deltaTime;
				yield return null;
			}

			currentImage.gameObject.SetActive(false);
			currentImage.localScale = initialSizesEnvironment[currentIndex];
			nextImage.localScale = initialSizesEnvironment[currentIndex + 1] * targetZoom;
			currentIndex++;
		}
		canZoom = true;
	}

	private IEnumerator TransitionToPreviousImage(int targetIndex)
	{
		canZoom = false;
		while (currentIndex > targetIndex)
		{
			Transform currentImage = environment[currentIndex];
			Transform previousImage = environment[currentIndex - 1];
			Vector3 endPoint = previousImage.GetChild(0).position;
			Vector3 startPos = currentImage.position;
			previousImage.localScale = initialSizesEnvironment[currentIndex - 1] * 2;

			previousImage.gameObject.SetActive(true);

			float timeElapsed = 0f;
			while (timeElapsed < transitionDuration)
			{
				currentImage.position = Vector3.Lerp(startPos, endPoint, timeElapsed / transitionDuration);
				previousImage.localScale = Vector3.Lerp(initialSizesEnvironment[currentIndex - 1] * 2, initialSizesEnvironment[currentIndex - 1] * targetZoom, timeElapsed / transitionDuration);
				currentImage.localScale = Vector3.Lerp(initialSizesEnvironment[currentIndex], Vector3.zero, timeElapsed / transitionDuration);
				timeElapsed += Time.deltaTime;
				yield return null;
			}

			currentImage.gameObject.SetActive(false);
			currentImage.localScale = initialSizesEnvironment[currentIndex];
			currentImage.position = startPos;
			previousImage.localScale = initialSizesEnvironment[currentIndex - 1] * targetZoom;
			currentIndex--;
		}
		canZoom = true;
	}


}
