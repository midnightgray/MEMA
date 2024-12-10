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

public class UpdateSavingsCode : MonoBehaviour
{

    
    public TMP_InputField savingsadded;
    public GetData getData;
    private string goalID;
    public GameObject overlay;
    public GameObject savingsSaved;
    private string baseURL = "https://mema-server.netlify.app/.netlify/functions/mema_api";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UpdateSavingsButton()
    {
        goalID = PlayerPrefs.GetString("viewed_goal_id", "noname");

        ShowOverlay();
        StartCoroutine(UpdateSavings(goalID, savingsadded.text));
    }

    IEnumerator UpdateSavings(string id, string totalsavings){
        GoalData goalData = new GoalData
        {
            id = id,
            totalsavings = totalsavings
        };
        string token = PlayerPrefs.GetString("token", "None");

        string jsonData = JsonUtility.ToJson(goalData);
        UnityWebRequest request = new UnityWebRequest(baseURL + "/update_savings", "POST");
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

            // Wait until both coroutines finish
            yield return StartCoroutine(getData.FetchAndSaveGoals());

            // Now both coroutines have finished
            savingsSaved.gameObject.SetActive(true);
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
        public string totalsavings;
    }
}
