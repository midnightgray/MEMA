using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Google;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;

using UnityEngine.Networking;
using UnityEngine.SceneManagement;
// using Google.SignIn;

public class GoogleSignInCode : MonoBehaviour
{

    // public TextMeshProUGUI logs; 
    public GameObject panel;
    public GetData getData;
    public string webClientId = "856822273258-rdpbk36d27s0iqd5a1brkmt3otaeorsn.apps.googleusercontent.com";
    private GoogleSignInConfiguration configuration;
    private string baseURL = "https://mema-server.netlify.app/.netlify/functions/mema_api";
    private void Awake(){
        configuration = new GoogleSignInConfiguration { WebClientId = webClientId, RequestEmail = true, RequestIdToken = false };
    }

    public void SignInWithGoogle(){
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = false;
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        Debug.Log("OnAuthenticationFinished");
        if (task.IsFaulted)
        {
            using (IEnumerator<Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                    Debug.Log("Got Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    Debug.Log("Got Unexpected Exception?!?" + task.Exception);
                }
            }
        }
        else if (task.IsCanceled)
        {
            Debug.Log("Canceled");
        }
        else
        {
            Debug.Log("Welcome: " + task.Result.DisplayName + "!");
            Debug.Log("Email = " + task.Result.Email);
            Debug.Log("Email = " + task.Result.Email);
            panel.gameObject.SetActive (true);
            StartCoroutine(GoogleLoginMove(task.Result.Email, task.Result.DisplayName));
        }
    }

    public void OnSignOut()
    {
        GoogleSignIn.DefaultInstance.SignOut();
    }

    IEnumerator GoogleLoginMove(string email, string googlename)
    {
        GoogleSignInData googleSignInData = new GoogleSignInData
        {
            email = email,
            googlename = googlename
        };

        string jsonData = JsonUtility.ToJson(googleSignInData);
        UnityWebRequest request = new UnityWebRequest(baseURL + "/sign_in_google", "POST");
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
            PlayerPrefs.SetString("username", googlename);
            PlayerPrefs.SetString("googlesignin", "true");
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
    private class GoogleSignInData
    {
        public string email;
        public string googlename;
    }

}
