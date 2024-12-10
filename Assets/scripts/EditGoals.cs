using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Networking;
using System.IO;
using System.Linq;
using System;

public class EditGoals : MonoBehaviour
{
    public GameObject overlay;
    public GameObject savegoal;

    public GetData getData;

    public NotificationHandler notificationHandler;
    public TMP_InputField GoalsName;
    public TMP_InputField TargetSavings;
    public TextMeshProUGUI TMDate;
    private string id;
    private string baseURL = "https://mema-server.netlify.app/.netlify/functions/mema_api";
    // private string baseURL = "http://localhost:8888/.netlify/functions/mema_api";
    public static string ConvertToYYYYMMDD(string inputDate) =>
        DateTime.ParseExact(inputDate, "MMMM dd, yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetString("previousscene", "") == "Home")
        {
            id = PlayerPrefs.GetString("selected_goal_id", "");
            GoalsName.text = PlayerPrefs.GetString("selected_goalname", "");
            TargetSavings.text = PlayerPrefs.GetString("selected_target_savings", "");
        }
        else 
        {
            id = PlayerPrefs.GetString("viewed_goal_id", "");
            GoalsName.text = PlayerPrefs.GetString("viewed_goalname", "");
            TargetSavings.text = PlayerPrefs.GetString("viewed_target_savings", "");  
        }
        overlay.gameObject.SetActive(false);
        savegoal.gameObject.SetActive(false);
        
    }

    public void GoToViewGoals(){
        SceneManager.LoadScene("ViewGoals");  
    }

    public void SaveGoal()
    {
        string convertedDate = ConvertToYYYYMMDD(TMDate.text);
        if (PlayerPrefs.GetString("previousscene", "") == "Home")
        {
            PlayerPrefs.SetString("selected_goalname", GoalsName.text);
            PlayerPrefs.SetString("selected_target_savings", TargetSavings.text);
            PlayerPrefs.SetString("selected_goal_date", convertedDate + "T00:00:00.000Z");
        }
        else 
        {
            PlayerPrefs.SetString("viewed_goalname", GoalsName.text);
            PlayerPrefs.SetString("viewed_target_savings", TargetSavings.text);
            PlayerPrefs.SetString("viewed_goal_date", convertedDate + "");  
        }
        overlay.gameObject.SetActive(true);
        Debug.Log("Description" + GoalsName.text + " Price:" + TargetSavings.text + "Date" + convertedDate);
        StartCoroutine(EditGoal(id, GoalsName.text, TargetSavings.text, convertedDate));
    }

    IEnumerator EditGoal(string id, string goalname, string targetsavings, string goaldate){
        GoalData goalData = new GoalData
        {
            id = id,
            goalname = goalname,
            targetsavings = targetsavings,
            goaldate = goaldate
        };
        string token = PlayerPrefs.GetString("token", "None");
        Debug.Log(token);

        string jsonData = JsonUtility.ToJson(goalData);
        UnityWebRequest request = new UnityWebRequest(baseURL + "/edit_goals", "POST");
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
            yield return StartCoroutine(notificationHandler.AddNotification("edited goal.", "2", completedTime));
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
        public string id;
        public string goalname;
        public string targetsavings;
        public string goaldate;
    }
}
