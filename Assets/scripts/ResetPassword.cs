using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
public class ResetPassword : MonoBehaviour
{
    public TMP_InputField emailInputField;
    public TextMeshProUGUI TextError;
    private string baseURL = "https://mema-server.netlify.app/.netlify/functions/mema_api";

    public void ResetPassButton(){
        StartCoroutine(ResetPassCoroutine(emailInputField.text));
    }

    IEnumerator ResetPassCoroutine(string email)
    {
        SignInData signInData = new SignInData
        {
            email = email,
        };

        string jsonData = JsonUtility.ToJson(signInData);
        UnityWebRequest request = new UnityWebRequest(baseURL + "/forgot_password", "POST");
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

            PlayerPrefs.SetString("token", token);
            PlayerPrefs.Save();
            SceneManager.LoadScene("Reset_OTP");
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
            TextError.text = "Invalid Email";
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
        public string email;
    }
}
