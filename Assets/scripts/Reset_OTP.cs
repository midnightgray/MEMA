using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Reset_OTP : MonoBehaviour
{
    public TMP_InputField[] inputFields;
    private string baseURL = "https://mema-server.netlify.app/.netlify/functions/mema_api";

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
            string resettoken = JsonUtility.FromJson<JsonResponse>(jsonString).token;
            PlayerPrefs.SetString("token", resettoken);
            PlayerPrefs.Save();
            Debug.Log("Token: " + resettoken);
            SceneManager.LoadScene("Change Password");
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
        }
    }

    [System.Serializable]
    private class JsonResponse
    {
        public string token;
    }

    [System.Serializable]
    private class OTPData
    {
        public string otp;
    }
}

