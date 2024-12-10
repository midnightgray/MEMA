using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

public class GetData : MonoBehaviour
{
    private string baseURL = "https://mema-server.netlify.app/.netlify/functions/mema_api";
    // private string baseURL = "http://localhost:8888/.netlify/functions/mema_api";
    private string token;
    private static GetData instance;
    void Awake()
    {
        token = PlayerPrefs.GetString("token", "None");
    }

    public static GetData Instance
    {
        get { return instance; }
    }

    public void GetAllData(){
        token = PlayerPrefs.GetString("token", "None");
    }

    public IEnumerator FetchAndSaveCategories()
    {
        UnityWebRequest request = new UnityWebRequest(baseURL + "/get_categories", "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            CategoryDataList categoryDataList = JsonUtility.FromJson<CategoryDataList>(jsonResponse);
            SaveCategoryToJson(categoryDataList.data);
        }else{
            Debug.Log("get_categories fail");
        }
    }

    void SaveCategoryToJson(List<CategoryData> categories)
    {
        string jsonData = JsonUtility.ToJson(new CategoryDataList { data = categories });
        string filePath = Application.persistentDataPath + "/categoryData.json";
        File.WriteAllText(filePath, jsonData);

        Debug.Log("Category data saved to: " + filePath);
    }

    public IEnumerator FetchAndSaveExpenses()
    {
        UnityWebRequest request = new UnityWebRequest(baseURL + "/get_expenses", "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + token);
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            ExpensesDataList categoryDataList = JsonUtility.FromJson<ExpensesDataList>(jsonResponse);
            SaveExpensesToJson(categoryDataList.data);
        }else if(request.responseCode == 401){
            string filepath = Application.persistentDataPath + "/expensesData.json";
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
                Debug.Log(filepath + "deleted successfully.");
            }
        }else{
            Debug.Log("get_expenses fail");
        }
    }

    void SaveExpensesToJson(List<ExpensesData> categories)
    {
        string jsonData = JsonUtility.ToJson(new ExpensesDataList { data = categories });
        string filePath = Application.persistentDataPath + "/expensesData.json";
        File.WriteAllText(filePath, jsonData);

        Debug.Log("expenses data saved to: " + filePath);
    }

    public IEnumerator FetchAndSaveGoals()
    {
        Debug.Log("FetchAndSaveGoals");
        UnityWebRequest request = new UnityWebRequest(baseURL + "/get_goals", "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + token);
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            GoalsDataList goalDataList = JsonUtility.FromJson<GoalsDataList>(jsonResponse);
            SaveGoalToJson(goalDataList.data);
        }else if(request.responseCode == 401){
            string filepath = Application.persistentDataPath + "/goalsData.json";
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
                Debug.Log(filepath + "deleted successfully.");
            }
        }else{
            Debug.Log("get_goals fail:" + request.responseCode);
        }
    }

    void SaveGoalToJson(List<GoalsData> goals)
    {
        string jsonData = JsonUtility.ToJson(new GoalsDataList { data = goals });
        string filePath = Application.persistentDataPath + "/goalsData.json";
        File.WriteAllText(filePath, jsonData);

        Debug.Log("goals data saved to: " + filePath);
    }

    public IEnumerator FetchAndSaveReminders()
    {
        Debug.Log("FetchAndSaveReminder");
        UnityWebRequest request = new UnityWebRequest(baseURL + "/get_reminders", "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + token);
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            ReminderDataList reminderDataList = JsonUtility.FromJson<ReminderDataList>(jsonResponse);
            SaveReminderToJson(reminderDataList.data);
        }else if(request.responseCode == 401){
            string filepath = Application.persistentDataPath + "/remindersData.json";
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
                Debug.Log(filepath + "deleted successfully.");
            }
        }else{
            Debug.Log("get_goals fail:" + request.responseCode);
        }
    }

    void SaveReminderToJson(List<ReminderData> reminder)
    {
        string jsonData = JsonUtility.ToJson(new ReminderDataList { data = reminder });
        string filePath = Application.persistentDataPath + "/remindersData.json";
        File.WriteAllText(filePath, jsonData);

        Debug.Log("reminders data saved to: " + filePath);
    }

    public IEnumerator FetchAndSaveLogins()
    {
        Debug.Log("FetchAndSaveLogins");
        UnityWebRequest request = new UnityWebRequest(baseURL + "/get_recentlogin", "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + token);
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            RecentLoginDataList recentLoginDataList = JsonUtility.FromJson<RecentLoginDataList>(jsonResponse);
            SaveReminderToJson(recentLoginDataList.data);
        }else if(request.responseCode == 401){
            string filepath = Application.persistentDataPath + "/recentLoginData.json";
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
                Debug.Log(filepath + "deleted successfully.");
            }
        }else{
            Debug.Log("get_goals fail:" + request.responseCode);
        }
    }

    void SaveReminderToJson(List<RecentLoginData> reminder)
    {
        string jsonData = JsonUtility.ToJson(new RecentLoginDataList { data = reminder });
        string filePath = Application.persistentDataPath + "/recentLoginData.json";
        File.WriteAllText(filePath, jsonData);

        Debug.Log("reminders data saved to: " + filePath);
    }

    public IEnumerator FetchAndSaveNotifications()
    {
        Debug.Log("FetchAndSaveNotifications");
        UnityWebRequest request = new UnityWebRequest(baseURL + "/get_notification", "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + token);
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            NotificationDataList notificationDataList = JsonUtility.FromJson<NotificationDataList>(jsonResponse);
            SaveNotificationToJson(notificationDataList.data);
        }else if(request.responseCode == 401){
            string filepath = Application.persistentDataPath + "/notificationData.json";
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
                Debug.Log(filepath + "deleted successfully.");
            }
        }else{
            Debug.Log("get_notification fail:" + request.responseCode);
        }
    }

    void SaveNotificationToJson(List<NotificationData> notification)
    {
        string jsonData = JsonUtility.ToJson(new NotificationDataList { data = notification });
        string filePath = Application.persistentDataPath + "/notificationData.json";
        File.WriteAllText(filePath, jsonData);

        Debug.Log("notification data saved to: " + filePath);
    }
}

[System.Serializable]
public class CategoryData
{
    public int id;
    public string categoryname;
}

[System.Serializable]
public class CategoryDataList
{
    public List<CategoryData> data;
}

[System.Serializable]
public class ExpensesDataList
{
    public List<ExpensesData> data;
}

[System.Serializable]
public class ExpensesData
{
    public int id;
    public int accountid;
    public int categoryid;
    public string expensename;
    public int quantity;
    public float price;
    public string description;
    public string expensedate;
}

[System.Serializable]
public class GoalsData
{
    public int id;
    public string goalname;
    public int accountid;
    public float totalsavings;
    public float targetsavings;
    public string goaldate;
}

[System.Serializable]
public class GoalsDataList
{
    public List<GoalsData> data;
}

[System.Serializable]
public class ReminderData
{
    public int id;
    public string accountid;
    public string reminderdate;
    public string remindertime;
    public string remindername;
    public string category;
    public float price;
    public int remindertypeid;
    public string completedtime;
}

[System.Serializable]
public class ReminderDataList
{
    public List<ReminderData> data;
}

[System.Serializable]
public class RecentLoginData
{
    public int id;
    public string accountid;
    public string devicename;
    public string locationname;
    public string datetime;
}

[System.Serializable]
public class RecentLoginDataList
{
    public List<RecentLoginData> data;
}

[System.Serializable]
public class NotificationData
{
    public int id;
    public int accountid;
    public string description;
    public int notificationtype;
    public string datetime;
}

[System.Serializable]
public class NotificationDataList
{
    public List<NotificationData> data;
}

namespace MemaData
{
    public enum ReminderType
    {
        Urgent = 1,
        Favorites,
        Due,
        Completed
    }
}

