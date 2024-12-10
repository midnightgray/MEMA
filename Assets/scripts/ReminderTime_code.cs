using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReminderTime_code : MonoBehaviour
{
    public TMP_InputField hour;
    public TMP_InputField min;
    public TMP_InputField meridiem;


    void Start()
    {
        hour.onEndEdit.AddListener(validatehourinput);
        min.onEndEdit.AddListener(validatemininput);
        meridiem.onEndEdit.AddListener(validatemeridieminput);
    }

    private void validatehourinput(string input)
    {
        if (int.TryParse(input, out int parsedvalue))
        {
            parsedvalue = Mathf.Clamp(parsedvalue, 1, 12);
            hour.text = parsedvalue.ToString();
        }
    }
   private void validatemininput(string input)
    {
        if (int.TryParse(input, out int parsedvalue))
        {
            parsedvalue = Mathf.Clamp(parsedvalue, 0, 59);
            if (parsedvalue < 10)
            {
                min.text = "0" + parsedvalue.ToString();
            }
            else 
            {
                min.text = parsedvalue.ToString();
            }
            
        }
    }
    private void validatemeridieminput(string input)
    {
        string upperinput = input.ToUpper();
        if (upperinput == "AM" || upperinput == "PM")
        {
            meridiem.text = upperinput;
        }
        else
        {
            meridiem.text = "AM";
        }
    }

    
}
