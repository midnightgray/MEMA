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

public class NotificationCode : MonoBehaviour
{
    public GameObject notif;

    private void Awake()
    {
        StartCoroutine(LoadreminderData()); 
    }

    IEnumerator LoadreminderData()
    {
        string filePathNotifications = Application.persistentDataPath + "/notificationData.json";

        if (File.Exists(filePathNotifications))
        {
            string notificationJsonData = File.ReadAllText(filePathNotifications);
            NotificationDataList loadedNotifcationDataList = JsonUtility.FromJson<NotificationDataList>(notificationJsonData);
            loadedNotifcationDataList.data = loadedNotifcationDataList.data.OrderByDescending(x => 
                    DateTime.ParseExact(x.datetime, "MMMM dd, yyyy hh:mm tt", System.Globalization.CultureInfo.InvariantCulture)).ToList();
            foreach (var notificationData in loadedNotifcationDataList.data)
            {
                
                GameObject obj = Instantiate(notif); 
                
                obj.transform.SetParent(this.gameObject.transform);

                Transform titleTransform = obj.transform.Find("title");
                TextMeshProUGUI title = titleTransform.GetComponent<TextMeshProUGUI>();
                title.text = NotificationType(notificationData.notificationtype);

                Transform DescriptionTransform = obj.transform.Find("description");
                TextMeshProUGUI description = DescriptionTransform.GetComponent<TextMeshProUGUI>();
                description.text = notificationData.description;


                DateTime parsedDateTime = DateTime.ParseExact(notificationData.datetime, "MMMM dd, yyyy hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);

                Transform textTransform = obj.transform.Find("time");
                TextMeshProUGUI text = textTransform.GetComponent<TextMeshProUGUI>();
                text.text = parsedDateTime.ToString("hh:mm tt");

                textTransform = obj.transform.Find("date");
                text = textTransform.GetComponent<TextMeshProUGUI>();
                text.text = parsedDateTime.ToString("MMM dd, yyyy");

                Button button = obj.gameObject.AddComponent<Button>();
                button.onClick.AddListener(() => GotoList(notificationData.notificationtype));
            }
            
        }
        else
        {
            Debug.LogWarning("data file not found: " + filePathNotifications);
        }

        yield return null;
    }

    private void GotoList(int notiftype){
        string Scene = "";
        switch (notiftype)
        {
            case 1:
                Scene = "ExpensesRecords";
                break;
            case 2:
                Scene = "Goals";
                break;
            case 3:
                Scene = "1Reminder";
                break;
            case 4:
                Scene = "Profile";
                break;
            default:
                Scene = "Home";
                break;
        }
         SceneManager.LoadScene(Scene);  
        
    }

    private string NotificationType(int notiftype){
        string Title = "";
        switch (notiftype)
        {
            case 1:
                Title = "Expenses";
                break;
            case 2:
                Title = "Goals";
                break;
            case 3:
                Title = "Reminders";
                break;
            case 4:
                Title = "Profile";
                break;
            default:
                Title = "Expense";
                break;
        }
        return Title;
    }
}
