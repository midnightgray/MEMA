using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Linq;

public class sceneTransition : MonoBehaviour
{
    private static string previousscene;
    private static string[] list = {"","","","",""};

    public void exitgame() {  
        Debug.Log("exitgame");  
        Application.Quit();  
    }

    public void LoadPreviousScene()
    {
        Debug.Log(previousscene);

        int current = list.Length - 1;
        

        if(list[0] != "")
        {
            for(int i = 4; i >= 0 ; i--)
            {
                if(list[i] != "")
                {
                    SceneManager.LoadScene(list[i]);
                    list[i] = "";

                    if(i == 0)
                    {
                        for(int j = 0; j < list.Length; j++)
                            {
                                if(list[j] != "")
                                {
                                    list[j] = "";
                                }
                            }
                    }
                    break;
                }
            }
        }
        else
        {
            SceneManager.LoadScene(previousscene);
        }
        for(int z = 0; z < list.Length; z++)
        {
            Debug.Log("List: " + list[z]);
        }
        

    }

    //transition to papuntang home
    public void HomeScene() {  
        previousscene = SceneManager.GetActiveScene().name;

        for(int i = 0; i < list.Length; i++)
        {
            if(list[i] != "")
            {
                list[i] = "";
            }
        }

        SceneManager.LoadScene("Home");  
        for(int z = 0; z < list.Length; z++)
        {
            Debug.Log("List: " + list[z]);
        }
    }  
    public void ReportsScene() {  
        previousscene = SceneManager.GetActiveScene().name;

        for(int i = 0; i < list.Length; i++)
        {
            if(list[i] != "")
            {
                list[i] = "";
            }
        }

        SceneManager.LoadScene("Reports");    
        for(int z = 0; z < list.Length; z++)
        {
            Debug.Log("List: " + list[z]);
        }
    }  
    public void ProfileScene() {  
        previousscene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Profile");  
    } 
    public void RecentLoginsScene() {  
        previousscene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("RecentLogin");  
    } 
    public void EspensesRecordsScene() {  
        previousscene = SceneManager.GetActiveScene().name;

        for(int i = 0; i < list.Length; i++)
        {
            if(list[i] != "")
            {
                list[i] = "";
            }
        }

        for(int i = 0; i < list.Length; i++)
        {
            if(list[i] == "")
            {
                list[i] += SceneManager.GetActiveScene().name;
                break;
            }
        }
        
        SceneManager.LoadScene("ExpensesRecords");    
        for(int z = 0; z < list.Length; z++)
        {
            Debug.Log("List: " + list[z]);
        }
    } 
    public void ViewCategoryScene() { 
        previousscene = SceneManager.GetActiveScene().name; 
        SceneManager.LoadScene("ViewScene");  
    }
    public void NewExpenseScene() {  
        previousscene = SceneManager.GetActiveScene().name;

        for(int i = 0; i < list.Length; i++)
        {
            if(list[i] == "")
            {
                list[i] += SceneManager.GetActiveScene().name;
                break;
            }
        }

        SceneManager.LoadScene("NewExpScene");    
        for(int z = 0; z < list.Length; z++)
        {
            Debug.Log("List: " + list[z]);
        }
    }
    public void ClothingScene() {  
        previousscene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Clothing");  
    }
    public void CategoriesScene() {  
        previousscene = SceneManager.GetActiveScene().name;

        for(int i = 0; i < list.Length; i++)
        {
            if(list[i] == "")
            {
                list[i] += SceneManager.GetActiveScene().name;
                break;
            }
        }

        SceneManager.LoadScene("Categories");    
        for(int z = 0; z < list.Length; z++)
        {
            Debug.Log("List: " + list[z]);
        }
    }
    public void ExpenseEditScene() { 
        previousscene = SceneManager.GetActiveScene().name; 
        SceneManager.LoadScene("EditScene");  
    }
    public void AccInfoScene() {  
        previousscene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("AccountInformation");  
    }
    public void SignUpScene() {  
        previousscene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Sign Up");  
    }
    public void SISU_OTPScene() {  
        previousscene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("SISU_OTP");  
    }
    public void ResetPassScene() {  
        previousscene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Reset Password");  
    }
    public void SignInScene() {  
        previousscene = SceneManager.GetActiveScene().name;
        PlayerPrefs.DeleteKey("token");
        PlayerPrefs.DeleteAll();
        string filePathexpenses = Application.persistentDataPath + "/expensesData.json";

        string filePathCategories = Application.persistentDataPath + "/categoryData.json";

        string filePathGoals = Application.persistentDataPath + "/goalsData.json";
        // Check if the file exists before attempting to delete it
        if (File.Exists(filePathexpenses))
        {
            File.Delete(filePathexpenses);
            Debug.Log(filePathexpenses + "deleted successfully.");
        }

        if (File.Exists(filePathCategories))
        {
            File.Delete(filePathCategories);
            Debug.Log(filePathCategories + "deleted successfully.");
        }

         if (File.Exists(filePathGoals))
        {
            File.Delete(filePathGoals);
            Debug.Log(filePathGoals + "deleted successfully.");
        }


        PlayerPrefs.Save();
        SceneManager.LoadScene("Sign In");  
    }
    public void Reset_OTPScene() {  
        previousscene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Reset_OTP");  
    }
    public void UpdatePassScene() {  
        previousscene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Update Password");  
    }
    public void TermsScene() {  
        previousscene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Terms and Condition");  
    }
    public void NewRemindersScene() {  
        previousscene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("NewReminders");  
    }
    public void SwitchaccScene() {
        previousscene = SceneManager.GetActiveScene().name;  
        SceneManager.LoadScene("SwitchAcc");  
    }
    public void RemindersScene() {  
        previousscene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("1Reminder");  
    }
    public void RemindersALLScene() {  
        previousscene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("2Reminder");  
    }
    public void RemindersUrgentScene() {
        previousscene = SceneManager.GetActiveScene().name;  
        SceneManager.LoadScene("3UrgentlReminders");  
    }
    public void RemindersFavoriteScene() {  
        previousscene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("4FavReminders");  
    }
    public void RemindersTodayScene() {  
        previousscene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("5TodayReminders");  
    }
    public void RemindersCompleteScene() { 
        previousscene = SceneManager.GetActiveScene().name; 
        SceneManager.LoadScene("6CompleteReminders");  
    }
    public void GoalsHomeScene() { 
        previousscene = SceneManager.GetActiveScene().name;

        for(int i = 0; i < list.Length; i++)
        {
            if(list[i] != "")
            {
                list[i] = "";
            }
        }

        SceneManager.LoadScene("Goals");    
        for(int z = 0; z < list.Length; z++)
        {
            Debug.Log("List: " + list[z]);
        }
    }
    public void CreateGoalScene() { 
        previousscene = SceneManager.GetActiveScene().name; 
        SceneManager.LoadScene("CreateGoal");  
    }
    public void NotificationScene() { 
        previousscene = SceneManager.GetActiveScene().name; 
        SceneManager.LoadScene("Notifications");  
    }
    public void CurrentExpensesScene() { 
        previousscene = SceneManager.GetActiveScene().name; 

        for(int i = 0; i < list.Length; i++)
        {
            if(list[i] == "")
            {
                list[i] += SceneManager.GetActiveScene().name;
                break;
            }
        }

        SceneManager.LoadScene("CurrentExpense");    
        for(int z = 0; z < list.Length; z++)
        {
            Debug.Log("List: " + list[z]);
        }
    }
    public void SavingsLog() { 
        previousscene = SceneManager.GetActiveScene().name; 
        SceneManager.LoadScene("Savings Log");  
    }

    public void ViewgoalsfromHome() { 
        previousscene = SceneManager.GetActiveScene().name; 
        PlayerPrefs.SetString("previousscene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
        SceneManager.LoadScene("ViewGoals");    }
    public void EditgoalsfromHome() { 
        previousscene = SceneManager.GetActiveScene().name; 
        PlayerPrefs.SetString("previousscene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
        SceneManager.LoadScene("EditGoals"); 
    }
}
