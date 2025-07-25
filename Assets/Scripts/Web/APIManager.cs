using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    public readonly static string URL_API = "https://private-624120-softgamesassignment.apiary-mock.com/v3/magicwords";
    public static APIManager Instance;

    // Make it an Instance, to manage all API calls from a simgle script
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator GetDialoguesAndAvatars(Action<DialoguesAndAvatarsData> onSuccess = null)
    {
        yield return SendGetRequest(
            $"{URL_API}",
            (resultJson) =>
            {
                DialoguesAndAvatarsData data = JsonUtility.FromJson<DialoguesAndAvatarsData>(resultJson);
                
                // Callback that sends the data fetched as unity classes, ready to use.
                onSuccess(data);
            });
    }

    public IEnumerator SendGetRequest(string url, Action<string> onSuccess)
    {
        using var request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            onSuccess(request.downloadHandler.text);
        }
    }
    
    // Use this to obtain the bytes of a png, to later transform into an image
    public IEnumerator SendGetRequestTexture(string url, Action<Texture2D> onSuccess, Action<string> onError)
    {
        using var request = UnityWebRequestTexture.GetTexture(url);
        
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError(request.error);
            Debug.Log(request.error);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            onSuccess(texture);
        }
    }
}