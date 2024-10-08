using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Linq;

public class NetworkManager : MonoSingleton<NetworkManager>
{
    public string[] servers;
    public string postDrawingsRoute;
    public string getLastNDrawingsRoute;
    public string[] categories;
    public string[] reactions;
    //[TextArea(1, 1000)] public string sslCertificatePublicKey;
    void Awake()
    {
        if (FindObjectsOfType<NetworkManager>().Length > 1)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
}

[Serializable]
public struct ReceivedDrawingList
{
    public ReceivedDrawing[] drawings;
    public ReceivedMetadata meta;
}

[Serializable]
public struct ReceivedMetadata
{
    public int page;
    public int take; // number of drawings per page, correspon to the 'n' sent parametter
    public int itemCount;
    public int pageCount;
    public bool hasPreviousPage;
    public bool hasNextPage;
}

[Serializable]
public struct ReceivedDrawing
{
    public int id;
    public string createdDate;
    public string userName;
    public string drawingName; // object the game asked to draw
    public string background;
    public float score;
    public int like;
    public int funny;
    public int bad;
    public Draw.UnitySuxxWith2DVector[] linesPoints;
    public float[] linesWidth;
    public int[] linesColorIndex;
}

[Serializable]
public struct SentDrawing
{
    // public int id; // automaticaly added by the backend
    // public string createdDate; // automaticaly added by the backend
    public string userName;
    public string drawingName; // object the game asked to draw
    public string background;
    public float score;
    public Draw.UnitySuxxWith2DVector[] linesPoints;
    public float[] linesWidth;
    public int[] linesColorIndex;

    public SentDrawing(ReceivedDrawing data)
    {
        this.userName = data.userName;
        this.drawingName = data.drawingName;
        this.background = data.background;
        this.score = data.score;
        this.linesPoints = data.linesPoints;
        this.linesWidth = data.linesWidth;
        this.linesColorIndex = data.linesColorIndex;
    }
}

public class NetworkedDrawing
{
    public ReceivedDrawing data;
    public NetworkedDrawing(
        Draw.UnitySuxxWith2DVector[] linesPoints,
        float[] linesWidth,
        int[] linesColorIndex,
        int? id = null, // automaticaly added by the backend
        string createdDate = null, // automaticaly added by the backend
        string userName = "guest",
        string drawingName = "test", // object the game asked to draw
        string background = "test",
        float score = float.NegativeInfinity
    ) {
        data.id = id.GetValueOrDefault(-1);
        data.createdDate = null;
        data.userName = userName;
        data.drawingName = drawingName;
        data.background = background;
        data.score = score;
        data.linesPoints = linesPoints;
        data.linesWidth = linesWidth;
        data.linesColorIndex = linesColorIndex;
    }

    public NetworkedDrawing(Draw.LineRendererData data,
        int? id = null, // automaticaly added by the backend
        string createdDate = null, // automaticaly added by the backend
        string userName = "guest",
        string drawingName = "test",
        string background = "test",
        float score = float.NegativeInfinity
    ) : this(data.linesPoints, data.linesWidth, data.linesColorIndex, id, createdDate, userName, drawingName, background, score) { }

    public NetworkedDrawing(ReceivedDrawing data)
    {
        this.data = data;
    }
    public static NetworkedDrawing FromJson(string json)
    {
        ReceivedDrawing data = JsonUtility.FromJson<ReceivedDrawing>(json);
        return new NetworkedDrawing(data);
    }
    public override string ToString()
    {
        return JsonUtility.ToJson(this.data) + "\n" + this. GetTimeDifference();
    }
    private static async Task<Nullable<ReceivedDrawingList>> Receive(int n, int page = 1, string category = "lasts", bool reverseOrder = false)
    {
        try
        {
            string route = NetworkManager.Instance.getLastNDrawingsRoute;
            UnityWebRequest webRequest = UnityWebRequest.Get(NetworkManager.Instance.servers[0] + $"{route}/{category}?n={n}&page={page}&order={(reverseOrder ? "ASC" : "DESC")}");
            //webRequest.certificateHandler = new CustomSSLCertificate();
            var response = await webRequest.SendWebRequestAsync();
            if (response.result == UnityWebRequest.Result.ConnectionError || response.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + response.error);
                return null;
            }
            else
            {
                string json = response.downloadHandler.text;
                Debug.Log("Success: " + json);
                return JsonUtility.FromJson<ReceivedDrawingList>(json);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Exception: " + e);
            return null;
        }
    }
    public static async Task<NetworkedDrawing[]> ReceiveLasts(int n, int page=1, string category="lasts", bool reverseOrder=false)
    {
        Debug.Assert(NetworkManager.Instance.categories.Contains(category), $"[NetworkManager] Unknown category {category}");
        Nullable<ReceivedDrawingList> data = await Receive(n, page, category, reverseOrder);
        if (data.HasValue)
        {
            return data.Value.drawings.Select(rd => new NetworkedDrawing(rd)).ToArray();
        }
        else
        {
            return new NetworkedDrawing[0];
        }
    }
    public static async Task<int> GetTotalNumber()
    {
        Nullable<ReceivedDrawingList> data = await Receive(1);
        return data.HasValue ? data.Value.meta.itemCount : 0;
    }
    public async Task<bool> Send()
    {
        try
        {
            string route = NetworkManager.Instance.postDrawingsRoute;
            string json = JsonUtility.ToJson(new SentDrawing(this.data));
            UnityWebRequest webRequest = UnityWebRequest.Post(NetworkManager.Instance.servers[0] + route, json, "application/json");
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
        catch (Exception e)
        {
            Debug.LogError("Exception: " + e);
            return false;
        }
    }
    public async Task<bool> SendReaction(string reaction)
    {
        Debug.Assert(NetworkManager.Instance.reactions.Contains(reaction),
            $"[NetworkManager] Unknown reaction {reaction}");
        try
        {
            string route = NetworkManager.Instance.postDrawingsRoute + $"/{this.data.id}/{reaction}";
            UnityWebRequest webRequest = UnityWebRequest.Post(NetworkManager.Instance.servers[0] + route, "", "application/json");
            //webRequest.certificateHandler = new CustomSSLCertificate();
            var response = await webRequest.SendWebRequestAsync();
            if (response.result == UnityWebRequest.Result.ConnectionError || response.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + response.error);
                return false;
            }
            Debug.Log("Success: " + response.downloadHandler.text);


            if(reaction == "like")
            {
                this.data.like += 1;
            }
            if (reaction == "funny")
            {
                this.data.funny += 1;
            }
            if (reaction == "bad")
            {
                this.data.bad += 1;
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Exception: " + e);
            return false;
        }
    }

    public Draw.LineRendererData GetDrawingData()
    {
        return new Draw.LineRendererData(data.linesPoints, data.linesWidth, data.linesColorIndex);
    }

    /// Input a string utc datetime in the format "2024-08-19T16:32:57.171Z"
    /// and compare it to current time.
    /// Output will be like "2 days ago", "34 minutes ago", etc.
    public string GetTimeDifference()
    {
        // Parse the input date string (ISO 8601 format)
        DateTime givenTime;
        try
        {
            givenTime = DateTime.Parse(data.createdDate).ToUniversalTime();
        }
        catch
        {
            return "? ago";
        }

        // Calculate the time difference with the current time in UTC
        DateTime currentTime = DateTime.UtcNow;
        TimeSpan difference = currentTime - givenTime;

        // Convert to a human-readable format
        if (difference.TotalDays >= 1)
        {
            return $"{(int)difference.TotalDays} days ago";
        }
        else if (difference.TotalHours >= 1)
        {
            return $"{(int)difference.TotalHours} hours ago";
        }
        else if (difference.TotalMinutes >= 1)
        {
            return $"{(int)difference.TotalMinutes} min ago";
        }
        else if (difference.TotalSeconds >= 1)
        {
            return $"{(int)difference.TotalSeconds} sec ago";
        }
        else
        {
            return "just now";
        }
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
