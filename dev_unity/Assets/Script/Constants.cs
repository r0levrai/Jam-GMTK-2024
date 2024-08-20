using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoSingleton<Constants>
{
	void Awake()
	{
        if (FindObjectsOfType<Constants>().Length > 1)
        {
            Destroy(gameObject);
        }
		DontDestroyOnLoad(gameObject);
	}

    public List<Sprite> imagesBackground = new List<Sprite>();
    public float targetZoomValue = 1f;
    public bool zoomEnable = true;

    public int GetIndexImage() => (int)targetZoomValue;
	public Sprite GetCurrentImage(int index) => imagesBackground[index];

    public NetworkedDrawing playerDrawing;
    public void SavePlayerDrawing(int objectIndex, int note)
    {
        playerDrawing = new(
            Draw.Instance.GetDrawingData(), null, null, "guest",
            objectIndex.ToString(), GetIndexImage().ToString(), (float)note
        );
        print($"Saved w/ background {GetIndexImage()}: {imagesBackground[GetIndexImage()].name}");
    }
    public NetworkedDrawing GetPlayerDrawing() => playerDrawing;
}
