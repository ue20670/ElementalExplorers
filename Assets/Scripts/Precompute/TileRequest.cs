using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class TileRequest
{
    
    // public void GetAvaliableChunks(Action<string[]> callback)
    // {
    //     StartCoroutine(GetAvailableChunksRequest(callback));
    // }

    // private IEnumerator GetAvailableChunksRequest(Action<string[]> callback)
    // {
    //     string[] chunkList = null;
    //     string requestUrl = "http://127.0.0.1:5000/download/list/available_chunks.txt";
    //     using (UnityWebRequest webRequest = UnityWebRequest.Get(requestUrl))
    //     {
    //         webRequest.SendWebRequest();
    //         while (webRequest.result == UnityWebRequest.Result.InProgress)
    //         {
    //             yield return null;
    //         }
    //
    //         if (webRequest.result != UnityWebRequest.Result.Success)
    //         {
    //             Debug.LogError(webRequest.error);
    //             
    //         }
    //         else
    //         {
    //             Debug.Log("success");
    //             
    //             chunkList = webRequest.downloadHandler.text.Split('\n');
    //         }
    //
    //         callback.Invoke(chunkList);
    //     }
    // }

    public static void GetChunk(string filename, Action<PrecomputeChunk> onComplete)
    {
        // StartCoroutine(GetChunkRequest(filename, onComplete));
        
        string requestUrl = "http://127.0.0.1:5000/download/chunks/";
        UnityWebRequest request = UnityWebRequest.Get(requestUrl + filename);

        //make async request
        UnityWebRequestAsyncOperation webRequest = request.SendWebRequest();
        //process and invoke callback on async complete
        webRequest.completed += (AsyncOperation operation) =>
        {
            PrecomputeChunk chunk = null;
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                
            }
            else
            {
                Debug.Log("success");


                MemoryStream stream = new MemoryStream(request.downloadHandler.data);
                
                BinaryFormatter bf =
                    new BinaryFormatter();
                
                chunk = (PrecomputeChunk)bf.Deserialize(stream);
            }

            onComplete.Invoke(chunk);
        };

        
    } 

    // private IEnumerator GetChunkRequest(string filename, Action<PrecomputeChunk> onComplete)
    // {
    //     PrecomputeChunk chunk = null;
    //     string requestUrl = "http://127.0.0.1:5000/download/chunks/";
    //     using (UnityWebRequest webRequest = UnityWebRequest.Get(requestUrl + filename))
    //     {
    //         webRequest.SendWebRequest();
    //         while (webRequest.result == UnityWebRequest.Result.InProgress)
    //         {
    //             yield return null;
    //         }
    //
    //         if (webRequest.result != UnityWebRequest.Result.Success)
    //         {
    //             Debug.LogError(webRequest.error);
    //             
    //         }
    //         else
    //         {
    //             Debug.Log("success");
    //
    //
    //             MemoryStream stream = new MemoryStream(webRequest.downloadHandler.data);
    //             
    //             System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf =
    //                 new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
    //             
    //             chunk = (PrecomputeChunk)bf.Deserialize(stream);
    //         }
    //     }
    //     onComplete.Invoke(chunk);
    // }

}
