using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; 

public class ExpenseTranstion : MonoBehaviour
{
    public TextMeshProUGUI item;
    public void ViewExpense(){
        PlayerPrefs.SetString("expenseName", item.text);
        PlayerPrefs.SetString("expenseID", this.name);
        PlayerPrefs.Save();
        Debug.Log("ID: "+ this.name +" item Name: " + item.text);
        SceneManager.LoadScene("ViewScene"); 
    }
}
