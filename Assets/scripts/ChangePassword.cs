using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.SceneManagement; 
using System.Text.RegularExpressions;

public class ChangePassword : MonoBehaviour
{
    public TMP_InputField  passwordInputField;
    public TMP_InputField  reenterpasswordInputField;
    public TextMeshProUGUI TextError;
    private string baseURL = "https://mema-server.netlify.app/.netlify/functions/mema_api";


    public void ChangePassButton(){

        if (passwordInputField.text != reenterpasswordInputField.text){
            Debug.Log("Password does not match!");
            TextError.text = "Password does not match!";
        }
        else if(string.IsNullOrEmpty(passwordInputField.text)
             || string.IsNullOrEmpty(reenterpasswordInputField.text)){
            Debug.Log("Password is empty!");
            TextError.text = "Password is empty!";
        }
        else{

            StartCoroutine(ChangePassCoroutine(passwordInputField.text));
        }
    }

    IEnumerator ChangePassCoroutine(string password)
    {
        SignInData signInData = new SignInData
        {
            password = password,
        };
        string token = PlayerPrefs.GetString("token", "None");
        Debug.Log(token);

        string jsonData = JsonUtility.ToJson(signInData);
        UnityWebRequest request = new UnityWebRequest(baseURL + "/update_password", "POST");
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
            PlayerPrefs.SetString("token", "None");
            PlayerPrefs.Save();
            SceneManager.LoadScene("Sign In");
        }
    }

    [System.Serializable]
    private class JsonResponse
    {
        public string token;
    }

    [System.Serializable]
    private class SignInData
    {
        public string password;
    }
}


