using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;

public class SignInCode : MonoBehaviour
{
    private static string previousscene;
    public TMP_InputField emailInputField;
    public TMP_InputField  passwordInputField;
    public TextMeshProUGUI TextError;
    public GameObject panel;
    public GetData getData;

    public FBloginCode fblogin;
    public GoogleSignInCode googleSignInCode;
    private string baseURL = "https://mema-server.netlify.app/.netlify/functions/mema_api";
    // private string baseURL = "http://localhost:8888/.netlify/functions/mema_api";
    public void showpassword()
    {
        if (passwordInputField.contentType == TMP_InputField.ContentType.Password)
        {
            passwordInputField.contentType = TMP_InputField.ContentType.Standard;
        }
        else
        {
            passwordInputField.contentType = TMP_InputField.ContentType.Password;
        }
        passwordInputField.ForceLabelUpdate();
        
        
    }

    void Awake()
    {
        panel.gameObject.SetActive (true);
        if (PlayerPrefs.HasKey("token"))
        {
            string token = PlayerPrefs.GetString("token", "noname");
            StartCoroutine(AutoLogin(token));
        }
        else
        {
            panel.gameObject.SetActive (false);
        }
    }

    public void LoginButton(){
        panel.gameObject.SetActive (true);
        StartCoroutine(LoginCoroutine(emailInputField.text, passwordInputField.text));
    }

    IEnumerator LoginCoroutine(string email, string password)
    {
        SignInData signInData = new SignInData
        {
            email = email,
            password = password
        };

        string jsonData = JsonUtility.ToJson(signInData);
        UnityWebRequest request = new UnityWebRequest(baseURL + "/sign_in", "POST");
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
            PlayerPrefs.SetString("username", profileusername);
            PlayerPrefs.Save();
            panel.gameObject.SetActive (false);
            SceneManager.LoadScene("SISU_OTP");
            previousscene = SceneManager.GetActiveScene().name;
        }
        else
        {
            panel.gameObject.SetActive (false);
            TextError.text = "Invalid Email or Password";
        }
    }

    IEnumerator AutoLogin(string token)
    {
        UnityWebRequest request = new UnityWebRequest(baseURL + "/auto_signin", "POST");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + token);
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonString = request.downloadHandler.text;
            string profileusername = JsonUtility.FromJson<JsonResponse>(jsonString).username;
            string profileemail = JsonUtility.FromJson<JsonResponse>(jsonString).email;

            PlayerPrefs.SetString("token", token);
            PlayerPrefs.SetString("email", profileemail);
            PlayerPrefs.SetString("username", profileusername);
            PlayerPrefs.Save();

            LoadAllData("autosignin");
        }
        else
        {
            panel.gameObject.SetActive (false);
            fblogin.LogoutFromFacebook();
            googleSignInCode.OnSignOut();
        }
    }

    private void LoadAllData(string signtype){
        if (signtype == "autosignin"){
            StartCoroutine(CallGetAllDataAndWait("Home"));
        }
        else if(signtype == "SISU"){
            StartCoroutine(CallGetAllDataAndWait("SISU_OTP"));
        }
        else {
            Debug.Log("getData failed");
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
            panel.gameObject.SetActive (false);
            SceneManager.LoadScene(scenetogo);
            previousscene = "Sign In";
            Debug.Log("Data retrieval and processing completed.");
        }
        else
        {
            Debug.LogError("DataManager instance is null.");
        }
    }

    [System.Serializable]
    private class JsonResponse
    {
        public string token;
        public string username;
        public string email;
    }

    [System.Serializable]
    private class SignInData
    {
        public string email;
        public string password;
    }

    //Load previous scene
    public void LoadPreviousScene()
    {
        Debug.Log(previousscene);
        SceneManager.LoadScene(previousscene);
    }
}
