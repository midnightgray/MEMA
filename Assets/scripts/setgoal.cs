using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Networking;
using System.IO;
using System.Linq;
using System;
public class setgoal : MonoBehaviour
{

    public GameObject overlay;
    public GameObject savegoal;

    public GetData getData;

    public NotificationHandler notificationHandler;

    public TMP_InputField Description;
    public TMP_InputField Price;
    public TextMeshProUGUI TMDate;
    private string baseURL = "https://mema-server.netlify.app/.netlify/functions/mema_api";
    // private string baseURL = "http://localhost:8888/.netlify/functions/mema_api";

    public static string ConvertToYYYYMMDD(string inputDate) =>
        DateTime.ParseExact(inputDate, "MMMM dd, yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

    // Start is called before the first frame update
    void Start()
    {
        overlay.gameObject.SetActive(false);
        savegoal.gameObject.SetActive(false);
    }

    public void ShowOverlay()
    {
        overlay.gameObject.SetActive(true);
        Debug.Log("Show Overlay");
    }

    public void HideOverlay()
    {
        overlay.gameObject.SetActive(false);
        savegoal.gameObject.SetActive(false);  
    }

    public void Savegoal()
    {
        ShowOverlay();
        string convertedDate = ConvertToYYYYMMDD(TMDate.text);
        Debug.Log("Description" + Description.text + " Price:" + Price.text + "Date" + convertedDate);
        StartCoroutine(AddGoal(Description.text, Price.text, convertedDate));
    }

    IEnumerator AddGoal(string goalname, string targetsavings, string goaldate){
        GoalData goalData = new GoalData
        {
            goalname = goalname,
            targetsavings = targetsavings,
            goaldate = goaldate
        };
        string token = PlayerPrefs.GetString("token", "None");
        Debug.Log(token);

        string jsonData = JsonUtility.ToJson(goalData);
        UnityWebRequest request = new UnityWebRequest(baseURL + "/add_goal", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + token);
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            if (getData != null){
                StartCoroutine(CallGetAllDataAndWait());
            }
            else{
                Debug.Log("getData failed");
            }
            
            Debug.Log("success");
        }else{
            Debug.Log("fail");
        }
    }

    private IEnumerator CallGetAllDataAndWait()
    {
        if (getData != null)
        {
            getData.GetAllData();
            string completedTime = DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt");
            yield return StartCoroutine(notificationHandler.AddNotification("Added new goal.", "2", completedTime));
            yield return StartCoroutine(getData.FetchAndSaveNotifications());
            // Wait until both coroutines finish
            yield return StartCoroutine(getData.FetchAndSaveGoals());

            // Now both coroutines have finished
            savegoal.gameObject.SetActive(true);
            Debug.Log("Data retrieval and processing completed.");
        }
        else
        {
            Debug.LogError("DataManager instance is null.");
        }
    }

    [System.Serializable]
    private class GoalData
    {
        public string goalname;
        public string targetsavings;
        public string goaldate;
    }
}
