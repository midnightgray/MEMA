using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;

public class NotificationHandler : MonoBehaviour
{
    private string baseURL = "https://mema-server.netlify.app/.netlify/functions/mema_api"; 
    public IEnumerator AddNotification(string description, string notificationtype, string datetime){
        Notification notification = new Notification
        {
            description = description,
            notificationtype = notificationtype,
            datetime = datetime
        };
        string token = PlayerPrefs.GetString("token", "None");
        Debug.Log(token);

        string jsonData = JsonUtility.ToJson(notification);
        UnityWebRequest request = new UnityWebRequest(baseURL + "/add_notification", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + token);
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
   
            Debug.Log("Add notification success");
        }else{
            Debug.Log("fail" + request.responseCode);
        }
    }

    [System.Serializable]
    private class Notification
    {
        public string description;
        public string notificationtype;
        public string datetime;
    }
}
