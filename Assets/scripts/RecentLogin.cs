using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
using System.ComponentModel.Design;
using UnityEngine.SceneManagement;

public class RecentLogin : MonoBehaviour
{
    public GameObject LoginItem;

    private void Awake()
    {
        StartCoroutine(LoadLoginData()); 
    }

    IEnumerator LoadLoginData()
    {
        string filePathrecentlogin = Application.persistentDataPath + "/recentLoginData.json";

        if (File.Exists(filePathrecentlogin))
        {
            string recentloginJsonData = File.ReadAllText(filePathrecentlogin);
            RecentLoginDataList loadedRecentLoginDataList = JsonUtility.FromJson<RecentLoginDataList>(recentloginJsonData);
            foreach (var data in loadedRecentLoginDataList.data)
            {
                GameObject obj = Instantiate(LoginItem); 
                obj.transform.SetParent(this.gameObject.transform);
                Transform textTransform = obj.transform.Find("Title");
                TextMeshProUGUI text = textTransform.GetComponent<TextMeshProUGUI>();
                text.text = data.devicename;

                textTransform = obj.transform.Find("Location");
                text = textTransform.GetComponent<TextMeshProUGUI>();
                text.text = data.locationname;
                DateTime parsedDateTime = DateTime.ParseExact(data.datetime, "MMMM dd, yyyy hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);

                textTransform = obj.transform.Find("Time");
                text = textTransform.GetComponent<TextMeshProUGUI>();
                text.text = parsedDateTime.ToString("hh:mm tt");

                textTransform = obj.transform.Find("Date");
                text = textTransform.GetComponent<TextMeshProUGUI>();
                text.text = parsedDateTime.ToString("MMM dd, yyyy");
            }
        }
        else
        {
            Debug.LogWarning("data file not found: " + filePathrecentlogin);
        }
        yield return null;
    }
}
