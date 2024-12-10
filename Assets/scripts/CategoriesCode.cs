using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.IO;
using System.Linq;

public class CategoriesCode : MonoBehaviour
{
    public GameObject Clothe;
    public GameObject Food;
    public GameObject Others;
    public GameObject Utilities;
    public TextMeshProUGUI TotalPrice;

    private void Awake()
    {
        StartCoroutine(LoadCategoryData()); 
    }

    IEnumerator LoadCategoryData()
    {
        string filePathexpenses = Application.persistentDataPath + "/expensesData.json";
        string filePathCategories = Application.persistentDataPath + "/categoryData.json";
        float totalExpenses = 0;
        GameObject CategoryItem = Others;

        if (File.Exists(filePathexpenses))
        {
            string expensesJsonData = File.ReadAllText(filePathexpenses);
            string categoriesjsonData = File.ReadAllText(filePathCategories);
            ExpensesDataList loadedExpensesDataList = JsonUtility.FromJson<ExpensesDataList>(expensesJsonData);
            CategoryDataList loadedCategoryDataList = JsonUtility.FromJson<CategoryDataList>(categoriesjsonData);
            foreach (var category in loadedCategoryDataList.data)
            {
                var expensesWithCategoryId = loadedExpensesDataList.data.Where(expense => expense.categoryid == category.id);
                float totalCost = expensesWithCategoryId.Sum(expense => expense.quantity * expense.price);
                totalExpenses += totalCost;
                switch (category.categoryname)
                {
                    case "Clothes":
                        CategoryItem = Clothe;
                        break;
                    case "Food":
                        CategoryItem = Food;
                        break;
                    case "Utilities":
                        CategoryItem = Utilities;
                        break;
                    case "Others":
                        CategoryItem = Others;
                        break;
                    default:
                        CategoryItem = Others;
                        break;
                }
                if (totalCost > 0){
                    GameObject obj = Instantiate(CategoryItem); 
                    obj.transform.SetParent(this.gameObject.transform);
                    Transform parentTransform = obj.transform.Find("C_expense");
                    TextMeshProUGUI T_Categ = parentTransform.Find("T_Categ").GetComponent<TextMeshProUGUI>();
                    T_Categ.text = category.categoryname;
                    TextMeshProUGUI T_Cprice = parentTransform.Find("T_Cprice").GetComponent<TextMeshProUGUI>();
                    T_Cprice.text = totalCost.ToString("F2");
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