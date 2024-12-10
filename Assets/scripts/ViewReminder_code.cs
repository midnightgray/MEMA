using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.IO;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
using MemaData;

public class ViewReminder_code : MonoBehaviour
{
    public TextMeshProUGUI remindertimeText;
    public TMP_InputField remindernameText;
    public TMP_InputField categoryText;
    public TMP_InputField priceText;
    public TMP_InputField remindertypeidText;


    public GetData getData;
    private int reminderId;
    public GameObject overlay;
    public GameObject delete_reminder;
    public GameObject mark_reminder;
    public TextMeshProUGUI status;
    public TextMeshProUGUI completedRemindertime;
    public NotificationHandler notificationHandler;

    private string baseURL = "https://mema-server.netlify.app/.netlify/functions/mema_api";

    void Start()
    {
        string convertedTime12Hour = System.DateTime.ParseExact(PlayerPrefs.GetString("remindertime", ""), "HH:mm:ss", null).ToString("hh:mm tt");

        remindertimeText.text = convertedTime12Hour;
        remindernameText.text = PlayerPrefs.GetString("remindername", "");
        categoryText.text = PlayerPrefs.GetString("remindercategory", "");
        priceText.text = PlayerPrefs.GetString("reminderprice", "");
        string enumName = Enum.GetName(typeof(ReminderType), PlayerPrefs.GetInt("remindertypeid", 1));
        if (DateTime.TryParse(PlayerPrefs.GetString("reminderdate", ""), out DateTime dateTime))
        {
            DateTime currentDate = DateTime.Now;
            int comparisonResult = DateTime.Compare(dateTime.Date, currentDate.Date);
            if (comparisonResult <=0){
                enumName = Enum.GetName(typeof(ReminderType), 3);
            }
        }
        remindertypeidText.text = enumName;
        if(enumName == "Completed"){
            completedRemindertime.text = PlayerPrefs.GetString("completedtime", "");
        }
    }

    public void GoToEditReminder(){
        SceneManager.LoadScene("EditReminder");  
    }

    public void GoTo1Reminder(){
        SceneManager.LoadScene("1Reminder");  
    }

    public void HideOverlay()
    {
        overlay.gameObject.SetActive(false);
        delete_reminder.gameObject.SetActive(false);
        mark_reminder.gameObject.SetActive(false);
    }

    public void StartDeleteReminder()
    {
        reminderId = PlayerPrefs.GetInt("reminderid", 0);
        status.text = "Reminder Deleted!";
        overlay.gameObject.SetActive(true);
        // delete_reminder.gameObject.SetActive(true);
        StartCoroutine(DeleteReminder(reminderId.ToString()));
    }

    IEnumerator DeleteReminder(string id){
        ReminderData reminderData = new ReminderData
        {
            id = id
        };
        string token = PlayerPrefs.GetString("token", "None");

        string jsonData = JsonUtility.ToJson(reminderData);
        UnityWebRequest request = new UnityWebRequest(baseURL + "/delete_reminders", "POST");
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
                StartCoroutine(CallGetAllDataAndWait("Deleted a reminder."));
            }
            else{
                Debug.Log("getData failed");
            }
            Debug.Log("success");
        }else{
            Debug.Log("fail" + request.responseCode);

        }
    }

    public void CompleteReminderOverlay(){
        overlay.gameObject.SetActive(true);
        mark_reminder.gameObject.SetActive(true);
    }

    public void StartCompleteReminder()
    {
        reminderId = PlayerPrefs.GetInt("reminderid", 0);
        status.text = "Reminder Completed!";
        mark_reminder.gameObject.SetActive(false);
        string completedTime = DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt");
        StartCoroutine(CompleteReminder(reminderId.ToString(), completedTime));
    }

    IEnumerator CompleteReminder(string id, string completedTime){
        ReminderData reminderData = new ReminderData
        {
            id = id,
            completedTime = completedTime
        };
        string token = PlayerPrefs.GetString("token", "None");

        string jsonData = JsonUtility.ToJson(reminderData);
        UnityWebRequest request = new UnityWebRequest(baseURL + "/complete_reminders", "POST");
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
                StartCoroutine(CallGetAllDataAndWait("Completed a reminder."));
            }
            else{
                Debug.Log("getData failed");
            }
            Debug.Log("success");
        }else{
            Debug.Log("fail" + request.responseCode);

        }
    }

    private IEnumerator CallGetAllDataAndWait(string eventExecuted)
    {
        if (getData != null)
        {
            getData.GetAllData();
            string completedTime = DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt");
            yield return StartCoroutine(notificationHandler.AddNotification(eventExecuted, "3", completedTime));
            yield return StartCoroutine(getData.FetchAndSaveNotifications());

            // Wait until both coroutines finish
            yield return StartCoroutine(getData.FetchAndSaveReminders());

            // Now both coroutines have finished
            delete_reminder.gameObject.SetActive(true);
            Debug.Log("Data retrieval and processing completed.");
        }
        else
        {
            Debug.LogError("DataManager instance is null.");
        }
    }

    [System.Serializable]
    private class ReminderData
    {
        public string id;
        public string completedTime;
    }
}
