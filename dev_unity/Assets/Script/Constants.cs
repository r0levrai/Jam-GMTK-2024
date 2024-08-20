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

	public int GetIndexImage() => (int)targetZoomValue;
	public Sprite GetCurrentImage(int index) => imagesBackground[index];
    public NetworkedDrawing GetPlayerDrawing() => new(
        Draw.Instance.GetDrawingData(), null, null, "guest",
        "TOADD: object name", GetIndexImage().ToString(), -1f
    );

}
