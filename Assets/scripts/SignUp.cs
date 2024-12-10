using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.SceneManagement; 
using System.Text.RegularExpressions;

public class SignUp : MonoBehaviour
{
    public TMP_InputField emailInputField;
    public TMP_InputField  passwordInputField;
    public TMP_InputField  reenterpasswordInputField;
    public Toggle  acceptTerms;
    public TMP_InputField usernameInputField;
    public TextMeshProUGUI TextError;

    public InputField inputPass;
    private string baseURL = "https://mema-server.netlify.app/.netlify/functions/mema_api";

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

    public void showRepassword()
    {
        if (reenterpasswordInputField.contentType == TMP_InputField.ContentType.Password)
        {
            reenterpasswordInputField.contentType = TMP_InputField.ContentType.Standard;
        }
        else
        {
            reenterpasswordInputField.contentType = TMP_InputField.ContentType.Password;
        }
        reenterpasswordInputField.ForceLabelUpdate();
        
        
    }

    public void SignUpButton(){

        if (!Regex.IsMatch(usernameInputField.text, "^[a-zA-Z0-9]+$"))
        {
            Debug.Log("username should contain only letters and numbers.");
            TextError.text = "username should contain only letters and numbers.";
        }
        else if(!Regex.IsMatch(emailInputField.text, @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$")){
            Debug.Log("Invalid email!");
            TextError.text = "Invalid email!";
        }
        else if (passwordInputField.text != reenterpasswordInputField.text){
            Debug.Log("Password does not match!");
            TextError.text = "Password does not match!";
        }
        else if(string.IsNullOrEmpty(emailInputField.text) 
             || string.IsNullOrEmpty(passwordInputField.text)
             || string.IsNullOrEmpty(reenterpasswordInputField.text)
             || string.IsNullOrEmpty(usernameInputField.text)){
            Debug.Log("Password or email is empty!");
            TextError.text = "Password or email is empty!";
        }
        else if (!acceptTerms.isOn){
            Debug.Log("Please accept our terms or ELSE!!!");
            TextError.text = "Please accept our terms or ELSE!!!";
        }
        else{

            StartCoroutine(SignUpCoroutine(emailInputField.text, passwordInputField.text, usernameInputField.text));
        }
    }

    IEnumerator SignUpCoroutine(string email, string password, string username)
    {
        SignInData signInData = new SignInData
        {
            email = email,
            password = password,
            username = username
        };

        string jsonData = JsonUtility.ToJson(signInData);
        UnityWebRequest request = new UnityWebRequest(baseURL + "/sign_up", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request
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
            SceneManager.LoadScene("SISU_OTP");
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
            TextError.text = "Username or Email already exist";
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
        public string username;
    }
}


