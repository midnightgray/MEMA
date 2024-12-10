using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Linq;
using System;

public class UpdatePWCode : MonoBehaviour
{
    public GameObject panel;
    public GameObject ConfirmOverlay;
    public GameObject changedPassSuccessful;
    public TMP_InputField Current;
    public TMP_InputField NewPass;
    public TMP_InputField retypeNewPass;
    public TextMeshProUGUI error;
    public TextMeshProUGUI samePWerror;
    public TextMeshProUGUI IncPW;
    public NotificationHandler notificationHandler;

    public GetData getData;
    private string baseURL = "https://mema-server.netlify.app/.netlify/functions/mema_api";
    
    void Start()
    {
        panel.gameObject.SetActive (false);
        ConfirmOverlay.gameObject.SetActive (false);
        samePWerror.gameObject.SetActive (false);
        error.gameObject.SetActive (false);
        IncPW.gameObject.SetActive (false);

    }
    public void ShowPanel()
	{
		panel.gameObject.SetActive (true);
        Debug.Log("show Panel");
	}
    public void ShowConfirmOverlay()
	{
        if (NewPass.text == Current.text)
        {
            error.gameObject.SetActive (true);
        }

        else if (NewPass.text != retypeNewPass.text)
        {
            samePWerror.gameObject.SetActive (true);
        }
        else 
        {
            ShowPanel();
            ConfirmOverlay.gameObject.SetActive (true);
            Debug.Log("show ConfirmOverlay");
        }  
	}
    
    public void HidePanel()
    {
        panel.gameObject.SetActive (false);
        ConfirmOverlay.gameObject.SetActive (false);
    }

    public void updatePass(){
        ConfirmOverlay.gameObject.SetActive (false);
        StartCoroutine(UpdatePass(Current.text, NewPass.text));
    }

    public IEnumerator UpdatePass(string old_password, string new_password)
    {
        SignInData signInData = new SignInData
        {
            old_password = old_password,
            new_password = new_password
        };
        string jsonData = JsonUtility.ToJson(signInData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        

        string token = PlayerPrefs.GetString("token", "None");
        UnityWebRequest request = new UnityWebRequest(baseURL + "/change_password", "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + token);
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;

            StartCoroutine(CallGetAllDataAndWait());
            Debug.Log(jsonResponse);

        }else{
            string jsonResponse = request.downloadHandler.text;
            panel.gameObject.SetActive (false);
            IncPW.gameObject.SetActive (true);
            Debug.Log(jsonResponse);
        }
    }

    private IEnumerator CallGetAllDataAndWait()
    {
        if (getData != null)
        {
            getData.GetAllData();
            string completedTime = DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt");
            yield return StartCoroutine(notificationHandler.AddNotification("Password has been changed.", "4", completedTime));
            yield return StartCoroutine(getData.FetchAndSaveNotifications());

            // Now both coroutines have finished
            changedPassSuccessful.gameObject.SetActive (true);
            Debug.Log("Data retrieval and processing completed.");
        }
        else
        {
            Debug.LogError("DataManager instance is null.");
        }
    }


    [System.Serializable]
    private class SignInData
    {
        public string old_password;
        public string new_password;
    }
}
