using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.SceneManagement; 
using System.Text.RegularExpressions;
using System;
using System.IO;
using System.Linq;
using System.Globalization;

public class HomeCode : MonoBehaviour
{
    public TextMeshProUGUI Datetoday;
    public TextMeshProUGUI username;
    public TextMeshProUGUI TotalPrice; 
    public TextMeshProUGUI MonthlyExpenditure;
    public TextMeshProUGUI TotalSavings; 
    public TextMeshProUGUI TargetSavings; 
    private string DateToday;




    void Update()
    {
        DateTime currentDate = DateTime.Now;

        string formattedDate = currentDate.ToString("MMMM dd, yyyy hh:MM:ss tt");
        
        Datetoday.text = formattedDate;
    }

    void Awake ()
    {
        string profile_username = PlayerPrefs.GetString("username", "noname");
        username.text = "Hello " + profile_username;
        DateToday = DateTime.Now.ToString("yyyy-MM-dd");
        StartCoroutine(LoadCurrentExpenseData());
    }

    IEnumerator LoadCurrentExpenseData()
    {
        string filePathexpenses = Application.persistentDataPath + "/expensesData.json";
        string filePathCategories = Application.persistentDataPath + "/categoryData.json";
        string filePathGoals = Application.persistentDataPath + "/goalsData.json";
        float totalExpenses = 0;
        float MonthlyExpenses = 0;

        if (File.Exists(filePathexpenses))
        {
            string expensesJsonData = File.ReadAllText(filePathexpenses);
            string categoriesjsonData = File.ReadAllText(filePathCategories);
            string formattedDate = "";
            ExpensesDataList loadedExpensesDataList = JsonUtility.FromJson<ExpensesDataList>(expensesJsonData);
            CategoryDataList loadedCategoryDataList = JsonUtility.FromJson<CategoryDataList>(categoriesjsonData);
            foreach (var expense in loadedExpensesDataList.data)
            {
                if (DateTime.TryParse(expense.expensedate, out DateTime dateTime))
                {
                    formattedDate = dateTime.ToString("yyyy-MM-dd");
                }
                if (DateToday == formattedDate)
                {
                    float totalCost = expense.quantity * expense.price;
                    totalExpenses += totalCost;
                }
                DateTime ExpenseDateTime = DateTime.ParseExact(formattedDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                if (ExpenseDateTime.Year == DateTime.Now.Year && ExpenseDateTime.Month == DateTime.Now.Month)
                {
                    float totalCost = expense.quantity * expense.price;
                    MonthlyExpenses += totalCost;
                }
            }
            TotalPrice.text = "₱ " + totalExpenses.ToString("F2");
            MonthlyExpenditure.text = "₱ " + MonthlyExpenses.ToString("F2");
        }
        else
        {
            Debug.LogWarning("data file not found: " + filePathexpenses);
        }

        if (File.Exists(filePathGoals))
        {
            string goalsJsonData = File.ReadAllText(filePathGoals);
            GoalsDataList loadedGoalsDataList = JsonUtility.FromJson<GoalsDataList>(goalsJsonData);
            
            if (PlayerPrefs.HasKey("selected_goal_id"))
            {
                string id = PlayerPrefs.GetString("selected_goal_id", "1");
                var goalWithID = loadedGoalsDataList.data.FirstOrDefault(goal => goal.id == int.Parse(id));
                TotalSavings.text = "₱ " + goalWithID.totalsavings.ToString("F2");
                TargetSavings.text = "₱ " + goalWithID.targetsavings.ToString("F2");

                PlayerPrefs.SetString("selected_total_savings", goalWithID.totalsavings.ToString("F2"));
                PlayerPrefs.Save();
            }
        }




        yield return null;
    }


}
