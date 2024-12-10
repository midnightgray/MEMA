using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.IO;
using System.Linq;
using System;

public class CurrentExpense : MonoBehaviour
{
    public GameObject ExpenseItem;
    public TextMeshProUGUI TotalPrice; 
    private string DateToday;

    private void Start()
    {
        DateToday = DateTime.Now.ToString("yyyy-MM-dd");
        StartCoroutine(LoadCurrentExpenseData()); 
        // GetData.FetchAndSaveExpensesCompleted += OnFetchAndSaveExpensesCompleted;
    }

    void OnFetchAndSaveExpensesCompleted(){
        if (this != null)
        {
            StartCoroutine(LoadCurrentExpenseData()); 
        }
        // StartCoroutine(LoadCurrentExpenseData()); 
    }

    IEnumerator LoadCurrentExpenseData()
    {
        string filePathexpenses = Application.persistentDataPath + "/expensesData.json";
        string filePathCategories = Application.persistentDataPath + "/categoryData.json";
        float totalExpenses = 0;

        if (File.Exists(filePathexpenses))
        {
            string expensesJsonData = File.ReadAllText(filePathexpenses);
            string categoriesjsonData = File.ReadAllText(filePathCategories);
            string formattedDate = "";
            ExpensesDataList loadedExpensesDataList = JsonUtility.FromJson<ExpensesDataList>(expensesJsonData);
            CategoryDataList loadedCategoryDataList = JsonUtility.FromJson<CategoryDataList>(categoriesjsonData);
            foreach (var expense in loadedExpensesDataList.data)
            {
                CategoryData selectedCategory = loadedCategoryDataList.data.FirstOrDefault(category => category.id == expense.categoryid);
                if (DateTime.TryParse(expense.expensedate, out DateTime dateTime))
                {
                    formattedDate = dateTime.ToString("yyyy-MM-dd");
                }
                if (DateToday == formattedDate)
                {
                    float totalCost = expense.quantity * expense.price;
                    totalExpenses += totalCost;
                    GameObject obj = Instantiate(ExpenseItem);
                    obj.transform.Find("Transition").name = expense.id.ToString();
                    obj.transform.SetParent(this.gameObject.transform);
                    Transform parentTransform = obj.transform.Find("C_expense");
                    TextMeshProUGUI T_Categ = parentTransform.Find("T_itemname").GetComponent<TextMeshProUGUI>();
                    T_Categ.text = expense.expensename;
                    TextMeshProUGUI T_Cprice = parentTransform.Find("T_Iprice").GetComponent<TextMeshProUGUI>();
                    T_Cprice.text = totalCost.ToString("F2");
                    
                    string iconName= "";
                    switch (selectedCategory.categoryname)
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
                    Transform imageTransform = obj.transform.Find("Image");
                    Sprite newSprite = Resources.Load<Sprite>(iconName);
                    if (newSprite != null)
                    {
                        Image imageComponent = imageTransform.GetComponent<Image>();
                        imageComponent.sprite  = newSprite;
                    }
                    else
                    {
                        Debug.LogError("Failed to load the sprite: ");
                    }
                }
            }
            TotalPrice.text = "Php " + totalExpenses.ToString("F2");
        }
        else
        {
            Debug.LogWarning("data file not found: " + filePathexpenses);
        }

        yield return null;
    }
}