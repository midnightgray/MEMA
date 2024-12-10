using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using UnityEngine.Networking;
using System.IO;
using System.Linq;

using MemaData;

public class EditReminder : MonoBehaviour
{
    public GameObject Overlay;
    public GameObject savereminder;
    public GameObject remindertype;
    
    public TMP_InputField hour;
    public TMP_InputField min;
    public TMP_InputField meridiem;
    public TMP_InputField title;
    public TMP_InputField category;
    public TMP_InputField price;
    public TextMeshProUGUI date;
    public TextMeshProUGUI remindertypeText;

    public GetData getData;
    public NotificationHandler notificationHandler;
    private string reminderid = "1";

    private string baseURL = "https://mema-server.netlify.app/.netlify/functions/mema_api";
    
    public static string ConvertToYYYYMMDD(string inputDate) =>
        DateTime.ParseExact(inputDate, "MMMM dd, yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");


    // Start is called before the first frame update
    void Start()
    {
        Overlay.gameObject.SetActive (false);
        savereminder.gameObject.SetActive (false);
        remindertype.gameObject.SetActive (false);


        reminderid = PlayerPrefs.GetInt("reminderid", 1).ToString();
        title.text = PlayerPrefs.GetString("remindername", "");
        category.text = PlayerPrefs.GetString("remindercategory", "");
        price.text = PlayerPrefs.GetString("reminderprice", "");
        

        DateTime convertedTime12Hour = DateTime.ParseExact(PlayerPrefs.GetString("remindertime", ""), "HH:mm:ss", null);
        hour.text = convertedTime12Hour.ToString("hh");
        min.text = convertedTime12Hour.ToString("mm");
        meridiem.text = convertedTime12Hour.ToString("tt");

        string enumName = Enum.GetName(typeof(ReminderType), PlayerPrefs.GetInt("remindertypeid", 1));
        remindertypeText.text = enumName;

        PlayerPrefs.SetString("remindertype", PlayerPrefs.GetInt("remindertypeid", 1).ToString());
        PlayerPrefs.Save();

    }

    public void ShowOverlay()
    {
        Overlay.gameObject.SetActive (true);
        Debug.Log("Show overlay");
    }

    public void HideOverlay()
    {
        Overlay.gameObject.SetActive (false);
        savereminder.gameObject.SetActive (false);
        remindertype.gameObject.SetActive (false);
    }
    
    public void Remindertype()
    {
        ShowOverlay();
        remindertype.gameObject.SetActive(true);
        Debug.Log("Show remindertype");
    }

    public void Savereminder()
    {
        ShowOverlay();
        savereminder.gameObject.SetActive(true);
        Debug.Log("Show savereminder");
    }
    private string ConvertToMilitaryTime (string inputTime) => 
        DateTime.ParseExact(inputTime, "h:mmtt", System.Globalization.CultureInfo.InvariantCulture).ToString("HH:mm:ss");

    public void savedreminder()
    {
        
        string convertedDate = ConvertToYYYYMMDD(date.text);
        string convertedtime = ConvertToMilitaryTime(hour.text + ":" + min.text + meridiem.text);
        ShowOverlay();
        StartCoroutine(EditReminders(reminderid,
                                    convertedDate,
                                   convertedtime,
                                   title.text,
                                   category.text,
                                   price.text,
                                   PlayerPrefs.GetString("remindertype", "1")));
    }

    IEnumerator EditReminders(string id, string reminderdate, string remindertime, string remindername, string category, string price, string remindertypeid){
        ReminderData reminderData = new ReminderData
        {
            id = id,
            reminderdate = reminderdate,
            remindertime = remindertime,
            remindername = remindername,
            category = category,
            price = price,
            remindertypeid = remindertypeid,
        };
        string token = PlayerPrefs.GetString("token", "None");
        Debug.Log(token);

        string jsonData = JsonUtility.ToJson(reminderData);
        UnityWebRequest request = new UnityWebRequest(baseURL + "/edit_reminders", "POST");
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
            Debug.Log("fail" + request.responseCode);
        }
    }

    private IEnumerator CallGetAllDataAndWait()
    {
        if (getData != null)
        {
            getData.GetAllData();
            string completedTime = DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt");
            yield return StartCoroutine(notificationHandler.AddNotification("Edited a reminder.", "3", completedTime));
            yield return StartCoroutine(getData.FetchAndSaveNotifications());

            // Wait until coroutines finish
            yield return StartCoroutine(getData.FetchAndSaveReminders());

            // Now coroutines have finished
            Savereminder();
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
        public string reminderdate;
        public string remindertime;
        public string remindername;
        public string category;
        public string price;
        public string remindertypeid;
    }
}
