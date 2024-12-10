using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System.Linq;
using System;
using TMPro;


public class PieChartCode : MonoBehaviour
{
    public GameObject pieChartImage;
    public GameObject pieChartLabel;
    public Boolean legend;
    public Boolean monthly;

    void Start()
    {
        // DrawPieChart();
        StartCoroutine(LoadCategoryData()); 
    }

    IEnumerator LoadCategoryData()
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
                if (monthly)
                {
                    expensesWithCategoryId = loadedExpensesDataList.data.Where(expense => expense.categoryid == category.id
                    && DateTime.ParseExact("2023-12-01T16:00:00.000Z", "yyyy-MM-ddTHH:mm:ss.fffZ", null).Date.Year == DateTime.Now.Date.Year
                    && DateTime.ParseExact("2023-12-01T16:00:00.000Z", "yyyy-MM-ddTHH:mm:ss.fffZ", null).Date.Month == DateTime.Now.Date.Month);
                }
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
            float totalPie = 1;
            float newX = -300.0f; 
            float newY = 120.0f; 
            float newZ = 0;
            int index = 0;
            foreach (var category in PieChartList)
            {
                float hue = (float)index / PieChartList.Count; // Distribute hues evenly
                Color pieColor = Color.HSVToRGB(hue, 0.4f, 1.0f);
                // Color pieColor = new Color(totalPie, UnityEngine.Random.value, UnityEngine.Random.value, 1.0f);
                GameObject obj = Instantiate(pieChartImage);
                obj.transform.SetParent(this.gameObject.transform);
                obj.transform.localPosition = Vector3.zero;
                Image imageComponent = obj.GetComponent<Image>();
                imageComponent.color = pieColor;
                imageComponent.fillAmount = 0f;
                StartCoroutine(AnimateFillAmount(imageComponent, totalPie));
                totalPie -= category.Values.First()/totalExpenses;

                if (legend)
                {
                    GameObject objLabel = Instantiate(pieChartLabel);
                    Image imageComponentLabel = objLabel.GetComponent<Image>();
                    imageComponentLabel.color = pieColor;
                    objLabel.transform.SetParent(this.gameObject.transform);
                    objLabel.transform.localPosition = Vector3.zero;
                    objLabel.transform.localPosition = new Vector3(newX, newY, newZ);
                    newY -= 60.0f;
                    string targetTextMeshCategory = "Category";
                    TextMeshProUGUI targetCategory = objLabel.GetComponentsInChildren<TextMeshProUGUI>(true)
                    .FirstOrDefault(textMeshPro => textMeshPro.name == targetTextMeshCategory);
                    targetCategory.text = category.Keys.First();

                    string targetTextMeshPercentage = "Percentage";
                    TextMeshProUGUI targetPercentage = objLabel.GetComponentsInChildren<TextMeshProUGUI>(true)
                    .FirstOrDefault(textMeshPro => textMeshPro.name == targetTextMeshPercentage);
                    float percentage = (category.Values.First()/totalExpenses) * 100;
                    targetPercentage.text = percentage.ToString("F2") + "%";
                }
                index++;
            }
        }
        else
        {
            Debug.LogWarning("data file not found: " + filePathexpenses);
        }

        yield return null;
    }

    IEnumerator AnimateFillAmount(Image fillImage, float totalPie)
    {
        float elapsedTime = 0f;
        float startFillAmount = fillImage.fillAmount;
        float targetFillAmount = totalPie;
        float animationDuration = 1.3f;

        while (elapsedTime < animationDuration)
        {
            fillImage.fillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, elapsedTime / animationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the fillAmount reaches exactly 1 at the end
        fillImage.fillAmount = targetFillAmount;
    }


    void DrawPieChart()
    {

        GameObject obj = Instantiate(pieChartImage);
        obj.transform.SetParent(this.gameObject.transform);
        obj.transform.localPosition = Vector3.zero;
    }

    
}
