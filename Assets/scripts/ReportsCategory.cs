using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.IO;
using System.Linq;

public class ReportsCategory : MonoBehaviour
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
            List<Dictionary<string, float>> PieChartList = new List<Dictionary<string, float>>();
            ExpensesDataList loadedExpensesDataList = JsonUtility.FromJson<ExpensesDataList>(expensesJsonData);
            CategoryDataList loadedCategoryDataList = JsonUtility.FromJson<CategoryDataList>(categoriesjsonData);
            foreach (var category in loadedCategoryDataList.data)
            {
                var expensesWithCategoryId = loadedExpensesDataList.data.Where(expense => expense.categoryid == category.id);
                float totalCost = expensesWithCategoryId.Sum(expense => expense.quantity * expense.price);
                totalExpenses += totalCost;
                if (totalCost > 0){
                    Dictionary<string, float> categoryPair = new Dictionary<string, float>
                    {
                        { category.categoryname, totalCost }
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
                //ColorDict.Add(category.Keys.First(), pieColor);
                GameObject obj = Instantiate(historyItem);
                obj.transform.SetParent(this.gameObject.transform);


                TextMeshProUGUI report_categoryname = obj.transform.Find("txt_HC").GetComponent<TextMeshProUGUI>();
                report_categoryname.text = category.Keys.First();

                TextMeshProUGUI report_categorypercent = obj.transform.Find("txt_HCPercent").GetComponent<TextMeshProUGUI>();
                float percentage = (category.Values.First()/totalExpenses) * 100;
                report_categorypercent.text = percentage.ToString("F2") + "%";

                Image report_categorycolor = obj.transform.Find("circ_hc").GetComponent<Image>();
                report_categorycolor.color = pieColor;
                index++;
            }


            // foreach (var expense in loadedExpensesDataList.data)
            // {
            //         float totalCost = expense.quantity * expense.price;
            //         totalExpenses += totalCost;
            //         GameObject obj = Instantiate(historyItem);
            //         obj.transform.SetParent(this.gameObject.transform);
            // }    


        }
        else
        {
            Debug.LogWarning("data file not found: " + filePathexpenses);
        }

        yield return null;
    }
}
