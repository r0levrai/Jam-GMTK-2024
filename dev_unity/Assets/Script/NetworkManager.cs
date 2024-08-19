using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

public class NetworkManager : MonoSingleton<NetworkManager>
{
    public string[] servers;
    public string postDrawingsRoute;
    public string getLastNDrawingsRoute;
    //[TextArea(1, 1000)] public string sslCertificatePublicKey;
}

[System.Serializable]
public struct NetworkedDrawingList
{
    public NetworkedDrawing[] drawings;
}

[System.Serializable]
public struct NetworkedDrawing
{
    // public int id; // automaticaly added by the backend
    // public Date date; // automaticaly added by the backend
    public string userName;
    public string drawingName; // object the game asked to draw
    public string background;
    public float score;
    public Draw.UnitySuxxWith2DVector[] linesPoints;
    public float[] linesWidth;
    public int[] linesColorIndex;

    public NetworkedDrawing(
        Draw.UnitySuxxWith2DVector[] linesPoints,
        float[] linesWidth,
        int[] linesColorIndex,
        string userName = "guest",
        string drawingName = "test", // object the game asked to draw
        string background = "test",
        float score = float.NegativeInfinity
    )
    {
        this.userName = userName;
        this.drawingName = drawingName;
        this.background = background;
        this.score = score;
        this.linesPoints = linesPoints;
        this.linesWidth = linesWidth;
        this.linesColorIndex = linesColorIndex;
    }
    public NetworkedDrawing(Draw.LineRendererData data,
        string userName = "guest",
        string drawingName = "test",
        string background = "test",
        float score = float.NegativeInfinity
    ) : this(data.linesPoints, data.linesWidth, data.linesColorIndex, userName, drawingName, background, score) { }

    public static NetworkedDrawing FromJson(string json)
    {
        NetworkedDrawing data = JsonUtility.FromJson<NetworkedDrawing>(json);
        return new NetworkedDrawing(data.linesPoints, data.linesWidth, data.linesColorIndex, data.userName, data.drawingName, data.background, data.score);
    }
    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public async Task<bool> Send()
    {
        string route = NetworkManager.Instance.postDrawingsRoute;
        UnityWebRequest webRequest = UnityWebRequest.Post(NetworkManager.Instance.servers[0] + route, this.ToJson(), "application/json");
        //webRequest.certificateHandler = new CustomSSLCertificate();
        var response = await webRequest.SendWebRequestAsync();
        if (response.result == UnityWebRequest.Result.ConnectionError || response.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + response.error);
            return false;
        }
        Debug.Log("Success: " + response.downloadHandler.text);
        return true;
    }
    public static async Task<NetworkedDrawing[]> ReceiveRandom(int n)
    {
        string route = NetworkManager.Instance.getLastNDrawingsRoute;
        UnityWebRequest webRequest = UnityWebRequest.Get(NetworkManager.Instance.servers[0] + $"{route}?n={n}");
        //webRequest.certificateHandler = new CustomSSLCertificate();
        var response = await webRequest.SendWebRequestAsync();
        if (response.result == UnityWebRequest.Result.ConnectionError || response.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + response.error);
            return new NetworkedDrawing[0];
        }
        else
        {
            string json = response.downloadHandler.text;
            Debug.Log("Success: " + json);
            NetworkedDrawing[] data = JsonUtility.FromJson<NetworkedDrawingList>(json).drawings;
            return data;
        }
    }

    public Draw.LineRendererData GetDrawingData()
    {
        return new Draw.LineRendererData(this.linesPoints, this.linesWidth, this.linesColorIndex);
    }
}

/* DONT WORK WITH WEB BUILDS as of 19/08/2024
public class CustomSSLCertificate : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        string pubKey = NetworkManager.Instance.sslCertificatePublicKey;
        if (pubKey == null || pubKey == "")  // bypass
        {
            return true;
        }
        else
        {
            X509Certificate2 certificate = new X509Certificate2(certificateData);
            string pk = certificate.GetPublicKeyString();
            if (!pk.Equals(pubKey))
            {
                Debug.LogError("SSL certificate mismatch: expected public key " + pk);
            }
            return pk.Equals(pubKey);
        }
    }
}*/

public static class UnityWebRequestAsyncExtensions
{
    public static async Task<UnityWebRequest> SendWebRequestAsync(this UnityWebRequest request)
    {
        var operation = request.SendWebRequest();

        while (!operation.isDone)
        {
            await Task.Yield(); // Avoid blocking the main thread.
        }

        return request;
    }
}