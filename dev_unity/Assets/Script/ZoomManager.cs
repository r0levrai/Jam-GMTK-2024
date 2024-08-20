using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
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

	private bool zoomEnable
	{
		get => Constants.Instance.zoomEnable;
		set => Constants.Instance.zoomEnable = value;
	}
	private float zoomValue = 0f;
	private float zoomVelocity = 0f;
	private Transform movingObject;
	private Camera cam;
	private List<Vector3> posImages = new List<Vector3>();

	public static ZoomManager Instance;
	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		cam = Camera.main;

		foreach (var img in zoomLevels)
		{
			posImages.Add(img.spriteRenderer.transform.localPosition);
		}

		zoomAction.performed += OnZoom;
		zoomAction.Enable();
		moveAction.performed += OnMove;
		moveAction.Enable();

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
		zoomAction.performed -= OnZoom;
		zoomAction.Disable();
		moveAction.performed -= OnMove;
		moveAction.Disable();
	}

	private void Update()
	{
		//Debug.Log(currentUnitScaleInMeter);
		if (Mathf.Abs(zoomValue - targetZoomValue) > 0.01f && zoomEnable)
		{
			zoomValue = Mathf.SmoothDamp(zoomValue, targetZoomValue, ref zoomVelocity, zoomDuration);
			SetZoomLevel(zoomValue);
			SoundManager.Instance.PlayMusicByZoomIndex(Constants.Instance.GetIndexImage());

			/*
			float normalizedValue = Mathf.InverseLerp((int)targetZoomValue, (int)targetZoomValue + 1, zoomValue);
			float scale = (zoomLevels[(int)targetZoomValue].zoomCurve.Evaluate(normalizedValue));
			movingObject.position = Vector3.Lerp(Vector3.zero, posImages[(int)targetZoomValue], scale);
			*/
			zoomLevels[(int)targetZoomValue].spriteRenderer.transform.position = Vector3.zero;
		}
	}

	/*====== move ======*/

	private void OnMove(InputAction.CallbackContext context)
	{
		Vector2 delta = context.ReadValue<Vector2>();
		Vector3 newPosition = movingObject.position + (new Vector3(delta.x * moveSpeed, delta.y * moveSpeed, 0) * Time.deltaTime);
		movingObject.position = newPosition;
		//movingObject.position = ClampPositionToBounds(newPosition);
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
		float zoomValueNow = targetZoomValue + (scrollInput.y * zoomScroll * Time.deltaTime);
		if (zoomValueNow < zoomLevels.Length - .1f && zoomValueNow >= 0)
		{
			targetZoomValue = zoomValueNow;
			SoundManager.Instance.PlaySound(soundZoomNames[UnityEngine.Random.Range(0, soundZoomNames.Length)]);
		}
	}

	private void SetZoomLevel(float value)
	{
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
