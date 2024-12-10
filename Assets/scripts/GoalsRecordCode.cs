using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.IO;
using System.Linq;
using System;
using System.Globalization;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GoalsRecordCode : MonoBehaviour
{
    public GameObject ExpenseItem;
     private List<Image> starButtonList = new List<Image>();

    private void Awake()
    {
        StartCoroutine(LoadExpenseData()); 
    }

    IEnumerator LoadExpenseData()
    {
        string filePathexpenses = Application.persistentDataPath + "/goalsData.json";

        if (File.Exists(filePathexpenses))
        {
            string goalsJsonData = File.ReadAllText(filePathexpenses);
            GoalsDataList loadedGoalsDataList = JsonUtility.FromJson<GoalsDataList>(goalsJsonData);
            foreach (var goalsData in loadedGoalsDataList.data)
            {
                GameObject obj = Instantiate(ExpenseItem); 
                obj.transform.SetParent(this.gameObject.transform);
                obj.name = goalsData.id.ToString();
                
                Image starImage = obj.transform.Find("Star").GetComponent<Image>();

                Button button = starImage.gameObject.AddComponent<Button>();
                button.onClick.AddListener(() => SelectedStar(starImage, goalsData));

                if(PlayerPrefs.HasKey("selected_goal_id")
                && goalsData.id.ToString() == PlayerPrefs.GetString("selected_goal_id", "1"))
                {
                    Sprite star_black = Resources.Load<Sprite>("star_black");
                    starImage.sprite = star_black;
                }
                starButtonList.Add(starImage);

                Image ViewArea = obj.transform.Find("ViewArea").GetComponent<Image>();
                Button buttonViewArea = ViewArea.gameObject.AddComponent<Button>();
                buttonViewArea.onClick.AddListener(() => SelectedGoal(goalsData));
                
                Transform In_Descr = obj.transform.Find("In_Descr");
                Transform goal_name = In_Descr.Find("goal_name");
                TextMeshProUGUI goal_nameTM = goal_name.GetComponent<TextMeshProUGUI>();
                goal_nameTM.text = goalsData.goalname;

                Transform dateobj = In_Descr.Find("Date");
                TextMeshProUGUI dateTM = dateobj.GetComponent<TextMeshProUGUI>();


                if (DateTime.TryParse(goalsData.goaldate, out DateTime date))
                {
                    dateTM.text = date.ToString("MMMM dd, yyyy");
                }
                else
                {
                    Console.WriteLine("Invalid date format: " + goalsData.goaldate);
                }
                

                Transform amount = In_Descr.Find("Amount");
                TextMeshProUGUI amountTM = amount.GetComponent<TextMeshProUGUI>();
                amountTM.text = "₱ " + goalsData.targetsavings.ToString("F2");
            }
        }
        else
        {
            Debug.LogWarning("data file not found: " + filePathexpenses);
        }

        yield return null;
    }

    private void SelectedStar(Image imageComponent, GoalsData goalsData){
        foreach (var image in starButtonList)
        {
            Sprite star_white = Resources.Load<Sprite>("star_white");
            image.sprite = star_white;
        }
        Sprite star_black = Resources.Load<Sprite>("star_black");
        imageComponent.sprite = star_black;
        PlayerPrefs.SetString("selected_goal_id", goalsData.id.ToString());
        PlayerPrefs.SetString("selected_goalname", goalsData.goalname);
        PlayerPrefs.SetString("selected_total_savings", goalsData.totalsavings.ToString("F2"));
        PlayerPrefs.SetString("selected_target_savings", goalsData.targetsavings.ToString("F2"));
        PlayerPrefs.SetString("selected_goal_date", goalsData.goaldate);
        PlayerPrefs.SetString("previousscene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
    }

    private void SelectedGoal(GoalsData goalsData){
        // Debug.Log("selected goal: " + id);
        PlayerPrefs.SetString("viewed_goal_id", goalsData.id.ToString());
        PlayerPrefs.SetString("viewed_goalname", goalsData.goalname);
        PlayerPrefs.SetString("viewed_total_savings", goalsData.totalsavings.ToString("F2"));
        PlayerPrefs.SetString("viewed_target_savings", goalsData.targetsavings.ToString("F2"));
        PlayerPrefs.SetString("viewed_goal_date", goalsData.goaldate);
        PlayerPrefs.SetString("previousscene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
        SceneManager.LoadScene("ViewGoals");  
    }
}
