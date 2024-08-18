using UnityEngine;

public class ScreenshotDrawing : MonoBehaviour
{
    public static ScreenshotDrawing Instance;
	private void Awake()
	{
		if(Instance == null)
			Instance = this;
	}

	public RenderTexture renderTexture;

    public Texture2D GetDrawing()
    {
        Texture2D drawingTexture = new(renderTexture.width, renderTexture.height);
		var old_rt = RenderTexture.active;
		RenderTexture.active = renderTexture;
        drawingTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        drawingTexture.Apply();
		RenderTexture.active = old_rt;
        return drawingTexture;
	}
}
