using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.IO;
using System.Linq;
using System;

public class ReportsCode : MonoBehaviour
{
    public GameObject historyItem; 

    private void Awake()
    {      
        StartCoroutine(LoadExpensePerCategoryData()); 
    }

    IEnumerator LoadExpensePerCategoryData()
    {
        string filePathexpenses = Application.persistentDataPath + "/expensesData.json";
        string filePathCategories = Application.persistentDataPath + "/categoryData.json";
        float totalExpenses = 0;

        if (File.Exists(filePathexpenses))
        {
            string expensesJsonData = File.ReadAllText(filePathexpenses);
            string categoriesjsonData = File.ReadAllText(filePathCategories);
            List<Dictionary<int, float>> PieChartList = new List<Dictionary<int, float>>();
            Dictionary<int, Color> ColorDict = new Dictionary<int, Color>();
            ExpensesDataList loadedExpensesDataList = JsonUtility.FromJson<ExpensesDataList>(expensesJsonData);
            CategoryDataList loadedCategoryDataList = JsonUtility.FromJson<CategoryDataList>(categoriesjsonData);

            foreach (var category in loadedCategoryDataList.data)
            {
                var expensesWithCategoryId = loadedExpensesDataList.data.Where(expense => expense.categoryid == category.id);
                float totalCost = expensesWithCategoryId.Sum(expense => expense.quantity * expense.price);
                totalExpenses += totalCost;
                if (totalCost > 0){
                    Dictionary<int, float> categoryPair = new Dictionary<int, float>
                    {
                        { category.id, totalCost }
                    };
                    PieChartList.Add(categoryPair);
                }
            }
            PieChartList = PieChartList.OrderByDescending(d => d.Values.First()).ToList();
            int index = 0;
            foreach (var category in PieChartList)
            {
                float hue = (float)index / PieChartList.Count; // Distribute hues evenly
                Color pieColor = Color.HSVToRGB(hue, 0.4f, 1.0f);
                ColorDict.Add(category.Keys.First(), pieColor);
                index++;
            }
            var sortedExpenses = loadedExpensesDataList.data
                .OrderByDescending(expense => DateTime.Parse(expense.expensedate))
                .ToList();
            foreach (var expense in sortedExpenses)
            {
                    float totalCost = expense.quantity * expense.price;
                    GameObject obj = Instantiate(historyItem);
                    obj.transform.SetParent(this.gameObject.transform);
                    TextMeshProUGUI report_itemname = obj.transform.Find("txt_ItemName").GetComponent<TextMeshProUGUI>();
                    report_itemname.text = expense.expensename;
                    if (DateTime.TryParse(expense.expensedate, out DateTime dateTime))
                    {
                        TextMeshProUGUI  report_date= obj.transform.Find("txt_Date").GetComponent<TextMeshProUGUI>();
                        report_date.text = dateTime.ToString("MMM dd, yyyy");
                    }
                    
                    TextMeshProUGUI report_price = obj.transform.Find("txt_price").GetComponent<TextMeshProUGUI>();
                    report_price.text = "₱ " + totalCost.ToString("F2");

                    Image report_cat = obj.transform.Find("circ_category").GetComponent<Image>();
                    report_cat.color = ColorDict[expense.categoryid];
            }
        }
        else
        {
            Debug.LogWarning("data file not found: " + filePathexpenses);
        }

        yield return null;
    }
}
