using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ZoomManager : MonoBehaviour
{
	[System.Serializable]
	public class ZoomLevel
	{
		public SpriteRenderer spriteRenderer;
		public AnimationCurve zoomCurve;
		public float minScale = 1f;
		public float maxScale = 2f;
		public float refUnitInMeter = 1.0f; 
	}

	[SerializeField] private UIDocument uiDocument;
	[SerializeField] private ZoomLevel[] zoomLevels;
	[SerializeField] private InputAction zoomAction;
	[SerializeField] private InputAction moveAction;
	[SerializeField] private float zoomScroll = 0.1f;
	[SerializeField] private float moveSpeed = 1f;
	[SerializeField] private float zoomDuration = 0.15f;
	[SerializeField] private float spaceAround = 0f;

	public float currentUnitScaleInMeter
	{
		get => zoomLevels[(int)targetZoomValue].refUnitInMeter / zoomLevels[(int)targetZoomValue].spriteRenderer.transform.localScale.x;
	}
	private float targetZoomValue { 
		get => Constants.Instance.targetZoomValue;
		set => Constants.Instance.targetZoomValue = value; 
	}
	private float zoomValue = 0f;
	private float zoomVelocity = 0f;
	private Slider zoomSlider;
	private Transform movingObject;
	private Camera cam;

	public static ZoomManager Instance;
	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		cam = Camera.main;

		zoomAction.performed += OnZoom;
		zoomAction.Enable();
		moveAction.performed += OnMove;
		moveAction.Enable();

		zoomSlider = uiDocument.rootVisualElement.Q<Slider>("scaleSlider");
		zoomSlider.lowValue = 0f;
		zoomSlider.highValue = zoomLevels.Length-.1f;
		zoomSlider.value = targetZoomValue;
		zoomSlider.RegisterValueChangedCallback(OnZoomSliderChanged);

		for (int i = 0; i < transform.childCount; i++)
		{
			if (transform.GetChild(i).CompareTag("MovingObject"))
			{
				movingObject = transform.GetChild(i);
				break;
			}
		}

		targetZoomValue = zoomValue;
		SetZoomLevel(zoomValue);
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

	private void Update()
	{
		//Debug.Log(currentUnitScaleInMeter);
		if (Mathf.Abs(zoomValue - targetZoomValue) > 0.01f)
		{
			zoomValue = Mathf.SmoothDamp(zoomValue, targetZoomValue, ref zoomVelocity, zoomDuration);
			SetZoomLevel(zoomValue);
		}
	}

	/*====== move ======*/

	private void OnMove(InputAction.CallbackContext context)
	{
		Vector2 delta = context.ReadValue<Vector2>();
		Vector3 newPosition = movingObject.position + (new Vector3(delta.x * moveSpeed, delta.y * moveSpeed, 0) * Time.deltaTime);
		movingObject.position = ClampPositionToBounds(newPosition);
	}

	private Vector3 ClampPositionToBounds(Vector3 newPosition)
	{
		Vector3 objectSize = zoomLevels[(int)targetZoomValue].spriteRenderer.bounds.size / 2;
		Vector3 minScreenBounds = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)) * spaceAround;
		Vector3 maxScreenBounds = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)) * spaceAround;
		Vector3 clampedPosition = newPosition;

		clampedPosition.x = Mathf.Clamp(newPosition.x, minScreenBounds.x + (maxScreenBounds.x - minScreenBounds.x) - objectSize.x, maxScreenBounds.x - (maxScreenBounds.x - minScreenBounds.x) + objectSize.x);
		clampedPosition.y = Mathf.Clamp(newPosition.y, minScreenBounds.y + (maxScreenBounds.y - minScreenBounds.y) - objectSize.y, maxScreenBounds.y - (maxScreenBounds.y - minScreenBounds.y) + objectSize.y);

		return clampedPosition;
	}


	/*====== zoom ======*/

	private readonly string[] soundZoomNames = new string[5] { "scroll1", "scroll2", "scroll3", "scroll4", "scroll5" };

	private void OnZoom(InputAction.CallbackContext context)
	{
		Vector2 scrollInput = context.ReadValue<Vector2>();
		zoomSlider.value += scrollInput.y * zoomScroll * Time.deltaTime;
	}

	private void OnZoomSliderChanged(ChangeEvent<float> evt)
	{
		targetZoomValue = evt.newValue;
		//movingObject.position = ClampPositionToBounds(movingObject.position);
		//Debug.Log(targetZoomValue);
		SoundManager.Instance.PlaySound(soundZoomNames[Random.Range(0, soundZoomNames.Length)]);
	}

	private void SetZoomLevel(float value)
	{
		SoundManager.Instance.PlayMusicByZoomIndex(Constants.Instance.GetIndexImage());
		foreach (var level in zoomLevels)
		{
			int index = System.Array.IndexOf(zoomLevels, level);
			float normalizedValue = Mathf.InverseLerp(index, index+1, value);

			float scale = (level.zoomCurve.Evaluate(normalizedValue) * (level.maxScale - level.minScale)) + level.minScale;
			level.spriteRenderer.transform.localScale = Vector3.one * scale /*  * level.refUnitInMeter*/;
			level.spriteRenderer.gameObject.SetActive(scale > 0.01f && value>=index && value<=index+1);
		}
	}
}
