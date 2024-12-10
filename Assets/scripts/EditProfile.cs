using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;
public class EditProfile : MonoBehaviour
{
    public GameObject panel;
    public GameObject deletepfp;
    public GameObject updatepfp;
    public GameObject apply;
    public GameObject accDetailsCompleted;
    public GameObject discard;
    public TMP_InputField username;
    public TMP_InputField email;
    public Button changepass;
    public TextMeshProUGUI fbaccount;
    public Image loginIcon;

    private string baseURL = "https://mema-server.netlify.app/.netlify/functions/mema_api";
    // private string baseURL = "http://localhost:8888/.netlify/functions/mema_api";

    void Start()
    {
        panel.gameObject.SetActive (false);
        deletepfp.gameObject.SetActive (false);
        updatepfp.gameObject.SetActive (false);
        username.text = PlayerPrefs.GetString("username", "0");
        email.text = PlayerPrefs.GetString("email", "0");


        string fblogin = PlayerPrefs.GetString("FBlogin", "false");
        if(fblogin == "true"){
            username.interactable = false;
            email.interactable = false;
            changepass.interactable = false;
            fbaccount.text = PlayerPrefs.GetString("email", "0");
            Sprite newSprite = Resources.Load<Sprite>("fb-icon");
            loginIcon.sprite = newSprite;
        }

        string googlesignin = PlayerPrefs.GetString("googlesignin", "false");
        if(googlesignin == "true"){
            email.interactable = false;
            changepass.interactable = false;
            fbaccount.text = PlayerPrefs.GetString("email", "0");
            Sprite newSprite = Resources.Load<Sprite>("google-icon");
            loginIcon.sprite = newSprite;
        }
        
    }

    public void SaveAccDetails(){
        apply.gameObject.SetActive (false);
        StartCoroutine(EditUsernameEmail( username.text, email.text));
    }


    IEnumerator EditUsernameEmail(string new_username, string new_email){
        UserData userData = new UserData
        {
            new_username = new_username,
            new_email = new_email
        };
        string token = PlayerPrefs.GetString("token", "None");

        string jsonData = JsonUtility.ToJson(userData);
        UnityWebRequest request = new UnityWebRequest(baseURL + "/change_username_email", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + token);
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            accDetailsCompleted.gameObject.SetActive (true);
            PlayerPrefs.SetString("email", new_email);
            PlayerPrefs.SetString("username", new_username);
            Debug.Log("success");
        }else{
            Debug.Log("fail" + request.responseCode);

        }
    }

    public void ShowPanel()
	{
		panel.gameObject.SetActive (true);
        Debug.Log("show Panel");
	}
    public void ShowDeletepfp()
	{
        ShowPanel();
		deletepfp.gameObject.SetActive (true);
	}
    public void ShowUpdatepfp()
	{
        ShowPanel();
		updatepfp.gameObject.SetActive (true);
	}
    public void Showapply()
	{
        ShowPanel();
		apply.gameObject.SetActive (true);
	}
    public void ShowDiscard()
	{
        ShowPanel();
		discard.gameObject.SetActive (true);
	}
    public void HidePanel()
    {
        panel.gameObject.SetActive (false);
        deletepfp.gameObject.SetActive (false);
        updatepfp.gameObject.SetActive (false);
        apply.gameObject.SetActive (false);
        discard.gameObject.SetActive (false);
    }
    public void HidePaneldelete()
    {
        deletepfp.gameObject.SetActive (false);
    }

    [System.Serializable]
    private class UserData
    {
        public string new_username;
        public string new_email;
    }
}
