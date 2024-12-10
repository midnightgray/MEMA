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

public class Reminder2 : MonoBehaviour
{
    public GameObject reminderItem;

    private void Awake()
    {
        StartCoroutine(LoadreminderData()); 
    }

    IEnumerator LoadreminderData()
    {
        string filePathreminders = Application.persistentDataPath + "/remindersData.json";

        if (File.Exists(filePathreminders))
        {
            string formattedDate = "";
            string reminderJsonData = File.ReadAllText(filePathreminders);
            ReminderDataList loadedReminderDataList = JsonUtility.FromJson<ReminderDataList>(reminderJsonData);
            foreach (var remindersData in loadedReminderDataList.data)
            {
                if (DateTime.TryParse(remindersData.reminderdate, out DateTime dateTime))
                {
                    DateTime currentDate = DateTime.Now;
                    int comparisonResult = DateTime.Compare(dateTime.Date, currentDate.Date);
                    formattedDate = dateTime.ToString("MMM dd, yyyy");
                }
                GameObject obj = Instantiate(reminderItem); 
                obj.transform.SetParent(this.gameObject.transform);
                Transform childTransform = obj.transform.Find("Amount");
                TextMeshProUGUI amount = childTransform.GetComponent<TextMeshProUGUI>();
                amount.text = "Php " + remindersData.price.ToString("F2");
                Transform titleTransform = obj.transform.Find("Title");
                TextMeshProUGUI title = titleTransform.GetComponent<TextMeshProUGUI>();
                title.text = remindersData.remindername;
                Transform categoryTransform = obj.transform.Find("Category");
                TextMeshProUGUI category = categoryTransform.GetComponent<TextMeshProUGUI>();
                category.text = remindersData.category;

                Transform date = obj.transform.Find("Date");
                TextMeshProUGUI duedate = date.GetComponent<TextMeshProUGUI>();
                duedate.text = formattedDate;

                Button button = obj.gameObject.AddComponent<Button>();
                button.onClick.AddListener(() => viewreminders(remindersData));
                
            }
            
        }
        else
        {
            Debug.LogWarning("data file not found: " + filePathreminders);
        }

        yield return null;
    }
    private void viewreminders(ReminderData remindersData)
    {
        PlayerPrefs.SetInt("reminderid", remindersData.id);
        PlayerPrefs.SetString("reminderdate", remindersData.reminderdate);
        PlayerPrefs.SetString("remindertime", remindersData.remindertime);
        PlayerPrefs.SetString("remindername", remindersData.remindername);
        PlayerPrefs.SetString("remindercategory", remindersData.category);
        PlayerPrefs.SetString("reminderprice", remindersData.price.ToString("F2"));
        PlayerPrefs.SetInt("remindertypeid", remindersData.remindertypeid);
        PlayerPrefs.Save();
        if (remindersData.remindertypeid == 4){
            SceneManager.LoadScene("ViewReminderCompleted");
        }else{

            SceneManager.LoadScene("ViewReminder");
        }
    }
}
