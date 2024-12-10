using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class DateforNewExp : MonoBehaviour
{
    public TMP_InputField dayInput;
    public TMP_InputField monthInput;
    public TMP_InputField yearInput;

    private void Start()
    {
        // Add listeners to validate input when it changes
        dayInput.onEndEdit.AddListener(ValidateDayInput);
        monthInput.onEndEdit.AddListener(ValidateMonthInput);
        yearInput.onEndEdit.AddListener(ValidateYearInput);
        monthInput.onSelect.AddListener(monthnumber);

        // Set default values to today's date
        if(SceneManager.GetActiveScene().name == "NewExpScene"){
            DateTime currentDate = DateTime.Now;
            dayInput.text = currentDate.Day.ToString() + ",";
            ValidateMonthInput(currentDate.Month.ToString());
            yearInput.text = currentDate.Year.ToString();
        }
        else{
            dayInput.text = dayInput.text + ",";
            ValidateMonthInput(monthInput.text);
        }
    }

    private void ValidateDayInput(string newValue)
    {
        // Validate the day input (you may want to add more validation logic)
        if (!string.IsNullOrEmpty(newValue) && int.TryParse(newValue, out int day))
        {
            // Clamp the day value between 1 and 31
            day = Mathf.Clamp(day, 1, 31);
            dayInput.text = day.ToString() + ",";
        }
    }

    private void monthnumber(string newValue)
    {
        string month_words; 

        switch(newValue)
        {
                case "Jan":
                    month_words = "1";break;
                case "Feb":
                    month_words = "2";break;
                case "Mar":
                    month_words = "3";break;
                case "Apr":
                    month_words = "4";break;
                case "May":
                    month_words = "5";break;
                case "Jun":
                    month_words = "6";break;
                case "Jul":
                    month_words = "7";break;
                case "Aug":
                    month_words = "8";break;
                case "Sept":
                    month_words = "9";break;
                case "Oct":
                    month_words = "10";break;
                case "Nov":
                    month_words = "11";break;
                default:
                    month_words = "12";break;
        }  
                monthInput.text = month_words;
    }
    private void ValidateMonthInput(string newValue)
    {
        // Validate the month input (you may want to add more validation logic)
        if (!string.IsNullOrEmpty(newValue) && int.TryParse(newValue, out int month))
        {
            string month_words = "January";
            // Clamp the month value between 1 and 12
            month = Mathf.Clamp(month, 1, 12);
            switch(month){
                case 1:
                    month_words = "Jan";break;
                case 2:
                    month_words = "Feb";break;
                case 3:
                    month_words = "Mar";break;
                case 4:
                    month_words = "Apr";break;
                case 5:
                    month_words = "May";break;
                case 6:
                    month_words = "Jun";break;
                case 7:
                    month_words = "Jul";break;
                case 8:
                    month_words = "Aug";break;
                case 9:
                    month_words = "Sept";break;
                case 10:
                    month_words = "Oct";break;
                case 11:
                    month_words = "Nov";break;
                default:
                    month_words = "Dec";break; 
                 
            }
            monthInput.text = month_words;
        }
    }

    private void ValidateYearInput(string newValue)
    {
        // Validate the year input (you may want to add more validation logic)
        if (!string.IsNullOrEmpty(newValue) && int.TryParse(newValue, out int year))
        {
            // Clamp the year value to a reasonable range (adjust as needed)
            year = Mathf.Clamp(year, 1900, 2100);
            yearInput.text = year.ToString();
        }
    }

    public DateTime GetSelectedDate()
    {
        // Assuming you have validated input, construct a DateTime object
        int day = int.Parse(dayInput.text);
        int month = int.Parse(monthInput.text);
        int year = int.Parse(yearInput.text);

        return new DateTime(year, month, day);
    }
}

