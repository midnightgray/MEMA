using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReminderType_Code : MonoBehaviour
{
    public GameObject[] reminderCheck;
    public Button remindertypesave;
    public TextMeshProUGUI remindertypename;

    void Start()
    {
        foreach (var item in reminderCheck)
        {
            item.gameObject.SetActive (false);
        }
        remindertypesave.interactable = false;
        remindertypename.text = "Urgent";
    }

    public void Urgentclick()
    {
        foreach (var item in reminderCheck)
        {
            item.gameObject.SetActive (false);
        }
        reminderCheck[0].gameObject.SetActive(true);
        remindertypesave.interactable = true;
        PlayerPrefs.SetString("remindertype", "1");
        PlayerPrefs.Save();
        remindertypename.text = "Urgent";
    }

    public void Favoritesclick()
    {
        foreach (var item in reminderCheck)
        {
            item.gameObject.SetActive (false);
        }
        reminderCheck[1].gameObject.SetActive(true);
        remindertypesave.interactable = true;
        PlayerPrefs.SetString("remindertype", "2");
        PlayerPrefs.Save();
        remindertypename.text = "Favorites";
    }
}
