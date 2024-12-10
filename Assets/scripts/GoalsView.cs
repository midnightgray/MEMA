using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.IO;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class GoalsView : MonoBehaviour
{

    public TextMeshProUGUI GoalsName;
    public TextMeshProUGUI TotalSavings;
    public GetData getData;
    public NotificationHandler notificationHandler;
    private string goalID;
    public GameObject overlay;
    public GameObject deletegoal;
    private string baseURL = "https://mema-server.netlify.app/.netlify/functions/mema_api";
    public TextMeshProUGUI TargetSavings;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetString("previousscene", "") == "Home")
        {
            GoalsName.text = PlayerPrefs.GetString("selected_goalname", "");
            TotalSavings.text = PlayerPrefs.GetString("selected_total_savings", "");
            TargetSavings.text = PlayerPrefs.GetString("selected_target_savings", "");
        }
        else 
        {
            GoalsName.text = PlayerPrefs.GetString("viewed_goalname", "");
            TotalSavings.text = PlayerPrefs.GetString("viewed_total_savings", "");
            TargetSavings.text = PlayerPrefs.GetString("viewed_target_savings", "");  
        }
    }

    public void GoToEditGoals(){
        SceneManager.LoadScene("EditGoals");  
    }

    public void DeleteGoals()
    {
        if (PlayerPrefs.GetString("previousscene", "") == "Home")
        {
            goalID = PlayerPrefs.GetString("selected_goal_id", "noname");
        }
        else 
        {
            goalID = PlayerPrefs.GetString("viewed_goal_id", "noname");
        }
        ShowOverlay();
        StartCoroutine(DeleteGoal(goalID));
    }

    IEnumerator DeleteGoal(string id){
        GoalData goalData = new GoalData
        {
            id = id
        };
        string token = PlayerPrefs.GetString("token", "None");

        string jsonData = JsonUtility.ToJson(goalData);
        UnityWebRequest request = new UnityWebRequest(baseURL + "/delete_goals", "POST");
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
            yield return StartCoroutine(notificationHandler.AddNotification("Deleted goal.", "2", completedTime));
            yield return StartCoroutine(getData.FetchAndSaveNotifications());

            // Wait until both coroutines finish
            yield return StartCoroutine(getData.FetchAndSaveGoals());

            // Now both coroutines have finished
            deletegoal.gameObject.SetActive(true);
            Debug.Log("Data retrieval and processing completed.");
        }
        else
        {
            Debug.LogError("DataManager instance is null.");
        }
    }

    // === for overlay

    public void ShowOverlay()
    {
        overlay.gameObject.SetActive(true);
        Debug.Log("Show overlay");
    }

    [System.Serializable]
    private class GoalData
    {
        public string id;
    }
}
