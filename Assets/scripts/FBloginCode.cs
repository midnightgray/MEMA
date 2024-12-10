using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Networking;
using System.IO;
using System.Linq;

using UnityEngine.UI;

public class FBloginCode : MonoBehaviour
{
    public GameObject panel;
    public GetData getData;
    string userId;
    string userName;

    private string baseURL = "https://mema-server.netlify.app/.netlify/functions/mema_api";
    private void Start()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(OnInitComplete, OnHideUnity);
        }
        else
        {
            FB.ActivateApp();
        }
    }

    // private void addlogs(string message){
    //     logs.text += message;
    // }

    private void OnInitComplete()
    {
        Debug.Log("Facebook SDK initialized");
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void LoginWithFacebook()
    {
        if (!FB.IsLoggedIn)
        {
            FB.LogInWithReadPermissions(new List<string> { "public_profile"}, OnLoginComplete);
        }
        else
        {
            Debug.Log("Already logged in with Facebook");
        }
    }

    private void OnLoginComplete(ILoginResult result)
    {
        if (result.Error == null)
        {
            FB.API("/me?fields=id,name", HttpMethod.GET, OnUserDataReceived);
        }
        else
        {
            Debug.Log($"Facebook login failed: {result.Error}");
        }
    }

    private void OnUserDataReceived(IGraphResult result)
    {
        if (result.Error == null)
        {
            userId = result.ResultDictionary["id"].ToString();
            userName = result.ResultDictionary["name"].ToString();
            Debug.Log($"User ID: {userId}, Name: {userName}");
            panel.gameObject.SetActive (true);
            string userIdFB = userId + "@fb.com";
            StartCoroutine(FBLoginMove(userIdFB, userName));
        }
        else
        {
            Debug.Log($"Failed to get user data: {result.Error}");
        }
    }

    public void LogoutFromFacebook()
    {
        if (FB.IsLoggedIn)
        {
            FB.LogOut();
            Debug.Log("Logged out from Facebook");
        }
        else
        {
            Debug.Log("Not logged in with Facebook");
        }
    }

     IEnumerator FBLoginMove(string email, string fbname)
    {
        FbSignInData fbSignInData = new FbSignInData
        {
            email = email,
            fbname = fbname
        };

        string jsonData = JsonUtility.ToJson(fbSignInData);
        UnityWebRequest request = new UnityWebRequest(baseURL + "/sign_in_fb", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonString = request.downloadHandler.text;
            string token = JsonUtility.FromJson<JsonResponse>(jsonString).token;
            string profileusername = JsonUtility.FromJson<JsonResponse>(jsonString).username;
            string profileemail = JsonUtility.FromJson<JsonResponse>(jsonString).email;

            PlayerPrefs.SetString("token", token);
            PlayerPrefs.SetString("email", profileemail);
            PlayerPrefs.SetString("username", fbname);
            PlayerPrefs.SetString("FBlogin", "true");
            PlayerPrefs.Save();
            StartCoroutine(GetRecentLogin());
        }
        else
        {
            panel.gameObject.SetActive (false);
        }
    }

    private IEnumerator CallGetAllDataAndWait()
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
            panel.gameObject.SetActive (false);
            SceneManager.LoadScene("Home");
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

    [System.Serializable]
    private class RecentLogin
    {
        public string devicename;
        public string locationname;
        public string datetime;
    }

    [System.Serializable]
    private class LocationData
    {
        public string country_name;
        public string city;
    }

    [System.Serializable]
    private class JsonResponse
    {
        public string token;
        public string username;
        public string email;
    }

    [System.Serializable]
    private class FbSignInData
    {
        public string email;
        public string fbname;
    }
}
