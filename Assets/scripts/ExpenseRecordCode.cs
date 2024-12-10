using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.IO;
using System.Linq;

public class ExpenseRecordCode : MonoBehaviour
{
    public GameObject ExpenseItem;

    private void Awake()
    {
        StartCoroutine(LoadExpenseData()); 
    }

    IEnumerator LoadExpenseData()
    {
        string filePathexpenses = Application.persistentDataPath + "/expensesData.json";
        string filePathCategories = Application.persistentDataPath + "/categoryData.json";

        if (File.Exists(filePathexpenses))
        {
            string expensesJsonData = File.ReadAllText(filePathexpenses);
            string categoriesjsonData = File.ReadAllText(filePathCategories);
            ExpensesDataList loadedExpensesDataList = JsonUtility.FromJson<ExpensesDataList>(expensesJsonData);
            CategoryDataList loadedCategoryDataList = JsonUtility.FromJson<CategoryDataList>(categoriesjsonData);
            foreach (var expenseData in loadedExpensesDataList.data)
            {
                CategoryData selectedCategory = loadedCategoryDataList.data.FirstOrDefault(category => category.id == expenseData.categoryid);
                float total = expenseData.price*expenseData.quantity;
                GameObject obj = Instantiate(ExpenseItem); 
                obj.transform.SetParent(this.gameObject.transform);
                TextMeshProUGUI textMeshPro = obj.GetComponent<TextMeshProUGUI>();
                textMeshPro.text = expenseData.expensename;
                Transform childTransform = obj.transform.Find("Categorytxt");
                TextMeshProUGUI category = childTransform.GetComponent<TextMeshProUGUI>();
                category.text = selectedCategory.categoryname;
                Transform Amount = obj.transform.Find("Amount");
                TextMeshProUGUI AmountText = Amount.GetComponent<TextMeshProUGUI>();
                AmountText.text = "₱ " + total.ToString();
                Debug.Log(expenseData.expensedate);
            }
        }
        else
        {
            Debug.LogWarning("data file not found: " + filePathexpenses);
        }

        yield return null;
    }
}
