using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class AddNewExpense : MonoBehaviour
{
    //for overlay
    public GameObject overlay;
    public GameObject saveexpense;
    public GetData getData;
    public TMP_InputField ExpenseName;
    public TMP_InputField CategoryName;
    public TMP_InputField Quantity;
    public TMP_InputField OrigPrice;
    public TMP_InputField Description;
    public TMP_InputField Month;
    public TMP_InputField Day;
    public TMP_InputField Year;
    public NotificationHandler notificationHandler;

    private string baseURL = "https://mema-server.netlify.app/.netlify/functions/mema_api";

    public static string ConvertToYYYYMMDD(string inputDate) =>
        DateTime.ParseExact(inputDate, "MMM d, yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

    void Start()
    {
        overlay.gameObject.SetActive(false);
        saveexpense.gameObject.SetActive(false);
    }

    public void SaveExpense(){
        string quantity = Regex.Replace(Quantity.text, "[^0-9]", "");
        string price = Regex.Replace(OrigPrice.text, @"[^\d.]", "");
        string Date = Month.text + " " + Day.text + " " + Year.text;

        string convertedDate = ConvertToYYYYMMDD(Date);
        string token = PlayerPrefs.GetString("token", "None");
        StartCoroutine(AddExpenses(ExpenseName.text, 
                                    CategoryName.text, 
                                    Description.text,
                                    OrigPrice.text,  
                                    Quantity.text, 
                                    convertedDate));

        ShowOverlay();
    }

    IEnumerator AddExpenses(string expensename, string categoryname, string description, string price, string quantity, string date){
        ExpenseData expenseData = new ExpenseData
        {
            itemname = expensename,
            category = categoryname,
            description = description,
            price = price,
            quantity = quantity,
            date = date,
        };
        string token = PlayerPrefs.GetString("token", "None");
        Debug.Log(token);

        string jsonData = JsonUtility.ToJson(expenseData);
        UnityWebRequest request = new UnityWebRequest(baseURL + "/add_expense", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + token);
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            if (getData != null){
                StartCoroutine(CallGetAllDataAndWait());
            }
            else{
                Debug.Log("getData failed");
            }
            Debug.Log("success");
        }else{
            Debug.Log("fail");
        }
    }

    private IEnumerator CallGetAllDataAndWait()
    {
        if (getData != null)
        {
            getData.GetAllData();

            // Wait until both coroutines finish
            string completedTime = DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt");
            yield return StartCoroutine(notificationHandler.AddNotification("Added new expense.", "1", completedTime));
            yield return StartCoroutine(getData.FetchAndSaveNotifications());
            
            yield return StartCoroutine(getData.FetchAndSaveExpenses());
            yield return StartCoroutine(getData.FetchAndSaveCategories());

            // Now both coroutines have finished
            saveexpense.gameObject.SetActive(true);
            Debug.Log("Data retrieval and processing completed.");
        }
        else
        {
            Debug.LogError("DataManager instance is null.");
        }
    }

    public void QuantityPlus()
    {
        int quantity = int.Parse(Regex.Replace(Quantity.text, "[^0-9]", ""));
        quantity += 1;
        Quantity.text = quantity.ToString();
    }

    public void QuantityMinus()
    {
        int quantity = int.Parse(Regex.Replace(Quantity.text, "[^0-9]", ""));
        if (quantity != 1)
        {
            quantity -= 1;
        }
        Quantity.text = quantity.ToString();

    }

    // === for overlay

    public void ShowOverlay()
    {
        overlay.gameObject.SetActive(true);
        Debug.Log("Show overlay");
    }

    public void HideOverlay()
    {
        overlay.gameObject.SetActive(false);
        saveexpense.gameObject.SetActive(false);
    }


    [System.Serializable]
    private class ExpenseData
    {
        public string itemname;
        public string category;
        public string description;
        public string price;
        public string quantity;
        public string date;
    }
}
