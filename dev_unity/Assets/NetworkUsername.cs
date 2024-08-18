using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class NetworkUsername : MonoBehaviour
{
    public TMPro.TextMeshProUGUI pseudoText;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetRequest("https://itch.io/api/1/me/me"));
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                // Show the response text on the pseudoText UI element
                pseudoText.text = webRequest.downloadHandler.text;
            }
        }
    }
}