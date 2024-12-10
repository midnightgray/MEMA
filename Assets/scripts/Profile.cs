using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.Networking;
using System.IO;
using System.Linq;
using System;

public class Profile : MonoBehaviour
{
    private string baseURL = "https://mema-server.netlify.app/.netlify/functions/mema_api";
    private string token;

    public GameObject panel;
    public GameObject logout;
    public GameObject deleteAccount;
    public GameObject success;
    public GameObject help;
    public TextMeshProUGUI email;
    public TextMeshProUGUI username;

    void Awake ()
    {
        string profile_username = PlayerPrefs.GetString("username", "noname");
        string profile_email = PlayerPrefs.GetString("email", "0");
        Debug.Log   (profile_username);
        email.text = profile_email;
        username.text = profile_username;
    }
    void Start()
    {
        panel.gameObject.SetActive (false);
        logout.gameObject.SetActive (false);
        deleteAccount.gameObject.SetActive (false);
        success.gameObject.SetActive (false);
    }
    public void ShowPanel()
	{
		panel.gameObject.SetActive (true);
        Debug.Log("show Panel");
	}

    public void ShowLogout()
	{
        ShowPanel();
		logout.gameObject.SetActive (true);
        Debug.Log("show logout");
	}

    public void ShowDeleteAccount()
	{
        ShowPanel();
		deleteAccount.gameObject.SetActive (true);
        Debug.Log("show logout");
	}

    public void ShowDeleteSuccess()
	{
        ShowPanel();
        deleteAccount.gameObject.SetActive (false);
		StartCoroutine(DeleteAccount()); 
        Debug.Log("show logout");
	}

    public void ShowHelp()
	{
        ShowPanel();
		help.gameObject.SetActive (true);
	}

    public void HidePanel()
    {
        panel.gameObject.SetActive (false);
        logout.gameObject.SetActive (false);
        deleteAccount.gameObject.SetActive (false);
        success.gameObject.SetActive (false);
        help.gameObject.SetActive (false);
    }

    public void SignInScene() {  
        SceneManager.LoadScene("Sign In");  
    }

    public IEnumerator DeleteAccount()
    {
        token = PlayerPrefs.GetString("token", "None");
        UnityWebRequest request = new UnityWebRequest(baseURL + "/delete_user", "POST");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + token);
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            Debug.Log(jsonResponse);
            success.gameObject.SetActive (true);
        }else{
            Debug.Log("delete_user fail");
        }
    }
}
