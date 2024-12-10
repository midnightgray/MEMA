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
public class SISU_OTP : MonoBehaviour
{
    public TMP_InputField[] inputFields;
    public GameObject overLay;
    public TextMeshProUGUI ErrorOTP;
    public Button resendButton;
    private string baseURL = "https://mema-server.netlify.app/.netlify/functions/mema_api";
    public GetData getData;
    private float countdownTime = 120f;

    private void Start()
    {
        if (inputFields != null && inputFields.Length > 0)
        {
            for (int i = 0; i < inputFields.Length; i++)
            {
                int currentIndex = i;
                inputFields[i].onValueChanged.AddListener(newValue =>
                {
                    OnInputValueChanged(newValue, currentIndex);
                });
            }
        }
        resendButton.interactable = false;
        StartCoroutine(EnableButtonAfterDelay());
    }

    IEnumerator EnableButtonAfterDelay()
    {
        while (countdownTime > 0)
        {
            // Update the UI text with the remaining time
            string minutes = Mathf.Floor(countdownTime / 60).ToString("00");
            string seconds = (countdownTime % 60).ToString("00");
            Debug.Log(minutes + ":" + seconds);
            // Wait for one second
            yield return new WaitForSeconds(1f);

            // Decrease the remaining time
            countdownTime--;
        }
        resendButton.interactable = true;
    }

    public void resendOTP(){
        Debug.Log("OTP sent.");
        StartCoroutine(resendOTPCoroutine());
        resendButton.interactable = false;
        countdownTime = 120f;
        StartCoroutine(EnableButtonAfterDelay());
        
    }

    private void OnInputValueChanged(string newValue, int currentIndex)
    {
        inputFields[currentIndex].text = newValue.ToUpper();
        if (newValue.Length > 0 && currentIndex < inputFields.Length - 1)
        {
            inputFields[currentIndex + 1].Select();
        }
    }

    public void verifyOTPButton(){
        overLay.gameObject.SetActive (true);
        StartCoroutine(verifyOTPCoroutine());
    }


    IEnumerator verifyOTPCoroutine()
    {
        string otp = string.Join("", inputFields.Select(inputField => inputField.text));
        OTPData signInData = new OTPData
        {
            otp = otp
        };
        string token = PlayerPrefs.GetString("token", "None");
        Debug.Log(token);

        string jsonData = JsonUtility.ToJson(signInData);
        UnityWebRequest request = new UnityWebRequest(baseURL + "/confirm_otp", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + token);
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonString = request.downloadHandler.text;
            string token_new = JsonUtility.FromJson<JsonResponse>(jsonString).token;
            PlayerPrefs.SetString("token", token_new);
            PlayerPrefs.Save();
            Debug.Log("Token: " + token_new);
            StartCoroutine(GetRecentLogin());
        }
        else
        {
            overLay.gameObject.SetActive (false);
            ErrorOTP.text = "OTP is incorrect or expired.";
            Debug.Log(request.downloadHandler.text);
        }
    }

    IEnumerator resendOTPCoroutine()
    {
        string token = PlayerPrefs.GetString("token", "None");

        UnityWebRequest request = new UnityWebRequest(baseURL + "/resend_otp", "POST");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + token);
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log(request.downloadHandler.text);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
        }
    }

    private IEnumerator CallGetAllDataAndWait(string scenetogo)
    {
        if (getData != null)
        {
            getData.GetAllData();

            // Wait until both coroutines finish
            yield return StartCoroutine(getData.FetchAndSaveExpenses());
            yield return StartCoroutine(getData.FetchAndSaveCategories());
            yield return StartCoroutine(getData.FetchAndSaveGoals());
            yield return StartCoroutine(getData.FetchAndSaveLogins());
            yield return StartCoroutine(getData.FetchAndSaveReminders());

            // Now both coroutines have finished
            overLay.gameObject.SetActive (false);
            SceneManager.LoadScene(scenetogo);
            Debug.Log("Data retrieval and processing completed.");
        }
        else
        {
            Debug.LogError("DataManager instance is null.");
        }
    }

    IEnumerator GetRecentLogin()
    {
        string apiKey = "323b793a448a49de8bced023017afc98";
        string apiUrl = "https://api.ipgeolocation.io/ipgeo?apiKey=" + apiKey;
        string deviceModel = SystemInfo.deviceModel;

        using (UnityWebRequest www = UnityWebRequest.Get(apiUrl))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                // Parse the JSON response
                string jsonResponse = www.downloadHandler.text;
                LocationData locationData = JsonUtility.FromJson<LocationData>(jsonResponse);
                 string datetime = DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt");

                string locationname = locationData.city + ", " + locationData.country_name;
                Debug.Log(deviceModel);
                Debug.Log(locationname);
                Debug.Log(datetime);
                StartCoroutine(AddRecentLogin(deviceModel, locationname, datetime));
            }
            else
            {
                Debug.LogError("Failed to get location by IP. Error: " + www.error);
            }
        }
    }

    IEnumerator AddRecentLogin(string devicename, string locationname, string datetime){
        RecentLogin recentLogin = new RecentLogin
        {
            devicename = devicename,
            locationname = locationname,
            datetime = datetime
        };
        string token = PlayerPrefs.GetString("token", "None");
        Debug.Log(token);

        string jsonData = JsonUtility.ToJson(recentLogin);
        UnityWebRequest request = new UnityWebRequest(baseURL + "/add_recentlogin", "POST");
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
                StartCoroutine(CallGetAllDataAndWait("Home"));
            }
            else{
                Debug.Log("getData failed");
            }
            
            Debug.Log("success");
        }else{
            Debug.Log("fail" + request.responseCode);
        }
    }

    [System.Serializable]
    private class RecentLogin
    {
        public string devicename;
        public string locationname;
        public string datetime;
    }

    [System.Serializable]
    private class OTPData
    {
        public string otp;
    }

    [System.Serializable]
    private class JsonResponse
    {
        public string token;
    }

    [System.Serializable]
    private class LocationData
    {
        public string country_name;
        public string city;
    }
}
