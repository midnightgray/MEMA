using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.IO;
using System.Linq;
using MemaData;
using System;
public class Reminders1 : MonoBehaviour
{

    public TextMeshProUGUI ALL;
    public TextMeshProUGUI urgent;
    public TextMeshProUGUI favorites;
    public TextMeshProUGUI due;
    public TextMeshProUGUI completed;


   private void Awake()
    {
        StartCoroutine(LoadRemindersData()); 
    }

    IEnumerator LoadRemindersData()
    {
        string filePathreminders = Application.persistentDataPath + "/remindersData.json";
        int allcount = 0;
        int urgentcount = 0;
        int favoritescount = 0;
        int duecount = 0;
        int completedcount = 0;

        if (File.Exists(filePathreminders))
        {
            int comparisonResult = 0;
            string remindersJsonData = File.ReadAllText(filePathreminders);
            ReminderDataList loadedRemindersDataList = JsonUtility.FromJson<ReminderDataList>(remindersJsonData);
            foreach (var reminderData in loadedRemindersDataList.data)
            {
                string enumName = Enum.GetName(typeof(ReminderType), reminderData.remindertypeid);
                Debug.Log(reminderData.remindername +  ":" + enumName);
                allcount += 1;

                if (DateTime.TryParse(reminderData.reminderdate, out DateTime dateTime))
                    {
                        DateTime currentDate = DateTime.Now;
                        comparisonResult = DateTime.Compare(dateTime.Date, currentDate.Date);
                    }
                    if (enumName == "Completed"){
                        completedcount += 1;
                    }
                    else if (comparisonResult > 0)
                    {
                        if (enumName == "Urgent")
                        {
                            urgentcount += 1;
                        }
                        if (enumName == "Favorites")
                        {
                            favoritescount += 1;
                        }
                    }
                    else 
                    {
                        duecount += 1;
                    }
            }
            ALL.text = allcount.ToString();
            urgent.text = urgentcount.ToString();
            favorites.text = favoritescount.ToString();
            due.text = duecount.ToString();
            completed.text = completedcount.ToString();
        }
        else
        {
            Debug.LogWarning("data file not found: " + filePathreminders);
        }
        yield return null;
    }

}
