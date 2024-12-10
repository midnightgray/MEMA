using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.IO;
using System.Linq;
using System;

public class ExpenseView : MonoBehaviour
{

    public TextMeshProUGUI TMexpensename;
    public TextMeshProUGUI TMquantity;
    public TextMeshProUGUI TMprice;
    public TextMeshProUGUI TMcategoryname;
    public TextMeshProUGUI TMDescription;
    public TextMeshProUGUI TMDate;
    public Image imageComponent;
    public GameObject overlay;
    public GameObject deleteexpense;
    public GetData getData;
    public NotificationHandler notificationHandler;
    private string expenseID;
    private string baseURL = "https://mema-server.netlify.app/.netlify/functions/mema_api";
    // private string baseURL = "http://localhost:8888/.netlify/functions/mema_api";
    private void Awake()
    {
        string expenseName = PlayerPrefs.GetString("expenseName", "noname");
        expenseID = PlayerPrefs.GetString("expenseID", "0");
        StartCoroutine(LoadExpenseView()); 
    }

    IEnumerator LoadExpenseView()
    {
        string filePathexpenses = Application.persistentDataPath + "/expensesData.json";
        string filePathCategories = Application.persistentDataPath + "/categoryData.json";

        if (File.Exists(filePathexpenses))
        {
            string expensesJsonData = File.ReadAllText(filePathexpenses);
            string categoriesjsonData = File.ReadAllText(filePathCategories);
            string formattedDate = "";
            ExpensesDataList loadedExpensesDataList = JsonUtility.FromJson<ExpensesDataList>(expensesJsonData);
            CategoryDataList loadedCategoryDataList = JsonUtility.FromJson<CategoryDataList>(categoriesjsonData);
        
            var expensesWithid = loadedExpensesDataList.data.FirstOrDefault(expense => expense.id == int.Parse(expenseID));
            var categoryWithid = loadedCategoryDataList.data.FirstOrDefault(category => category.id == expensesWithid.categoryid);
            if (DateTime.TryParse(expensesWithid.expensedate, out DateTime dateTime))
            {
                formattedDate = dateTime.ToString("yyyy-MM-dd");
            }
            TMexpensename.text = expensesWithid.expensename;
            TMquantity.text = expensesWithid.quantity.ToString();
            TMprice.text = expensesWithid.price.ToString("F2");
            TMcategoryname.text = categoryWithid.categoryname;
            TMDescription.text = expensesWithid.description;
            TMDate.text = formattedDate;
            string iconName= "";
            switch (categoryWithid.categoryname)
            {
                case "Clothes":
                    iconName = "icons8-t-shirt-100";
                    break;
                case "Food":
                    iconName = "icons8-poultry-leg-96 (1)";
                    break;
                case "Utilities":
                    iconName = "wrench-solid-240";
                    break;
                case "Others":
                    iconName = "wallet-solid-240";
                    break;
                default:
                    iconName = "wallet-solid-240";
                    break;
            }
            Sprite newSprite = Resources.Load<Sprite>(iconName);
            if (newSprite != null)
            {

                imageComponent.sprite  = newSprite;
            }
            else
            {
                Debug.LogError("Failed to load the sprite: ");
            }
        }
        else
        {
            Debug.LogWarning("data file not found: " + filePathexpenses);
        }
        yield return null;
    }

    public void EditExpense()
    {
        PlayerPrefs.SetString("date", TMDate.text);
        PlayerPrefs.SetString("expensename", TMexpensename.text);
        PlayerPrefs.SetString("quantity", TMquantity.text);
        PlayerPrefs.SetString("price", TMprice.text);
        PlayerPrefs.SetString("categoryname", TMcategoryname.text);
        PlayerPrefs.SetString("description", TMDescription.text);
        PlayerPrefs.SetString("id", expenseID);
        PlayerPrefs.Save();
    }

    public void DeleteExpense()
    {
        ShowOverlay();
        StartCoroutine(DeleteExpenses(expenseID));
    }

    IEnumerator DeleteExpenses(string id){
        ExpenseData expenseData = new ExpenseData
        {
            id = id
        };
        string token = PlayerPrefs.GetString("token", "None");

        string jsonData = JsonUtility.ToJson(expenseData);
        UnityWebRequest request = new UnityWebRequest(baseURL + "/delete_expense", "POST");
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
            Debug.Log("fail" + request.responseCode);

        }
    }

    private IEnumerator CallGetAllDataAndWait()
    {
        if (getData != null)
        {
            getData.GetAllData();

            // Wait until both coroutines finish
            string completedTime = DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt");
            yield return StartCoroutine(notificationHandler.AddNotification("Deleted expense.", "1", completedTime));
            yield return StartCoroutine(getData.FetchAndSaveNotifications());
            yield return StartCoroutine(getData.FetchAndSaveExpenses());
            yield return StartCoroutine(getData.FetchAndSaveCategories());

            // Now both coroutines have finished
            deleteexpense.gameObject.SetActive(true);
            Debug.Log("Data retrieval and processing completed.");
        }
        else
        {
            Debug.LogError("DataManager instance is null.");
        }
    }

    // === for overlay

    public void ShowOverlay()
    {
        overlay.gameObject.SetActive(true);
        Debug.Log("Show overlay");
    }

    [System.Serializable]
    private class ExpenseData
    {
        public string id;
    }
}