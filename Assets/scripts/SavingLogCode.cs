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
using JetBrains.Annotations;

public class SavingsLogCode : MonoBehaviour
{
    public GameObject ExpenseItem;
    public Button updateSavings;
    private List<GameObject> goalList = new List<GameObject>();

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
                
                Transform Goalname = obj.transform.Find("Goalname");
                TextMeshProUGUI goal_nameTM = Goalname.GetComponent<TextMeshProUGUI>();
                goal_nameTM.text = goalsData.goalname;

                Transform amountTxt = obj.transform.Find("amountTxt");
                TextMeshProUGUI amountTxtTM = amountTxt.GetComponent<TextMeshProUGUI>();
                amountTxtTM.text = goalsData.targetsavings.ToString("F2");
                
                Button button = obj.gameObject.AddComponent<Button>();
                button.onClick.AddListener(() => SelectedGoal(obj, goalsData));
                goalList.Add(obj);
            }
        }
        else
        {
            Debug.LogWarning("data file not found: " + filePathexpenses);
        }

        yield return null;
    }

        private void SelectedGoal(GameObject obj, GoalsData goalsData){
            // Debug.Log("selected goal: " + id);
            PlayerPrefs.SetString("viewed_goal_id", goalsData.id.ToString());
            PlayerPrefs.SetString("viewed_goalname", goalsData.goalname);
            PlayerPrefs.SetString("viewed_total_savings", goalsData.totalsavings.ToString("F2"));
            PlayerPrefs.SetString("viewed_target_savings", goalsData.targetsavings.ToString("F2"));
            PlayerPrefs.SetString("viewed_goal_date", goalsData.goaldate);
            PlayerPrefs.SetString("previousscene", SceneManager.GetActiveScene().name);
            PlayerPrefs.Save();

            foreach (var item in goalList)
            {
                Image img = item.GetComponent<Image>(); 
                img.color = ColorUtility.TryParseHtmlString("#CDE3E1", out Color color) ? color : img.color;
                Transform Goalname = item.transform.Find("Goalname");
                TextMeshProUGUI goal_nameTM = Goalname.GetComponent<TextMeshProUGUI>();
                goal_nameTM.color = ColorUtility.TryParseHtmlString("#008080", out Color txtcolor) ? txtcolor : img.color;
                Transform amountitemTxt = item.transform.Find("amountTxt");
                TextMeshProUGUI amountitemTxtTM = amountitemTxt.GetComponent<TextMeshProUGUI>();
                amountitemTxtTM.color = ColorUtility.TryParseHtmlString("#008080", out Color amtitemcolor) ? amtitemcolor : amountitemTxtTM.color;
            }  
            Image objimg = obj.GetComponent<Image>();
            objimg.color = ColorUtility.TryParseHtmlString("#008080", out Color colored) ? colored : objimg.color;
            Transform amountTxt = obj.transform.Find("amountTxt");
            TextMeshProUGUI amountTxtTM = amountTxt.GetComponent<TextMeshProUGUI>();
            amountTxtTM.color = ColorUtility.TryParseHtmlString("#CDE3E1", out Color amtcolor) ? amtcolor : objimg.color;
            Transform Goalnameobj = obj.transform.Find("Goalname");
            TextMeshProUGUI goal_nameTMobj = Goalnameobj.GetComponent<TextMeshProUGUI>();
            goal_nameTMobj.color = ColorUtility.TryParseHtmlString("#CDE3E1", out Color objcolor) ? objcolor : goal_nameTMobj.color;
            updateSavings.interactable = true;
        }
}
