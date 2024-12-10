using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Collections.Generic;

public class Calendar : MonoBehaviour
{
    public TextMeshProUGUI monthDayYear;
    public GridLayoutGroup dayGrid;
    public Boolean isNotCurrentDate;
    private DateTime currentDate;
    private List<GameObject> dayButtons = new List<GameObject>();
    public Boolean isForRreminder;

    void Start()
    {
        if (isForRreminder){
                string reminderdate = PlayerPrefs.GetString("reminderdate", "noname");
                DateTime reminderdatetime = DateTime.ParseExact(reminderdate, "yyyy-MM-ddTHH:mm:ss.fffZ", null);
                currentDate = reminderdatetime;
        }
        else if (isNotCurrentDate){
            if (PlayerPrefs.GetString("previousscene", "") == "Home")
            {
                string goaldate = PlayerPrefs.GetString("selected_goal_date", "noname");
                DateTime Goaldatetime = DateTime.ParseExact(goaldate, "yyyy-MM-ddTHH:mm:ss.fffZ", null);
                currentDate = Goaldatetime;
            }
            else 
            {
                string goaldate = PlayerPrefs.GetString("viewed_goal_date", "noname");
                DateTime Goaldatetime = DateTime.ParseExact(goaldate, "yyyy-MM-ddTHH:mm:ss.fffZ", null);
                currentDate = Goaldatetime;
            }
        }
        else{
            currentDate = DateTime.Now;
        }
        UpdateCalendar();
    }

    void UpdateCalendar()
    {
        monthDayYear.text = currentDate.ToString("MMMM dd, yyyy");
        UpdateDayGrid();
    }

    void UpdateDayGrid()
    {
        foreach (Transform child in dayGrid.transform)
        {
            Destroy(child.gameObject);
        }
        DateTime firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
        int daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
        DayOfWeek startDayOfWeek = firstDayOfMonth.DayOfWeek;
        for (int i = 0; i < 7; i++)
        {
            GameObject dayOfWeek = new GameObject("DayOfWeek", typeof(TextMeshProUGUI));
            dayOfWeek.transform.SetParent(dayGrid.transform);
            TextMeshProUGUI text = dayOfWeek.GetComponent<TextMeshProUGUI>();
            text.text = ((DayOfWeek)i).ToString().Substring(0, 3);
            text.alignment = TextAlignmentOptions.Center;
        }

        for (int i = 0; i < (int)startDayOfWeek; i++)
        {
            GameObject emptyDay = new GameObject("EmptyDay", typeof(TextMeshProUGUI));
            emptyDay.transform.SetParent(dayGrid.transform);
            TextMeshProUGUI text = emptyDay.GetComponent<TextMeshProUGUI>();
            text.text = "";
            text.alignment = TextAlignmentOptions.Center;
        }

        for (int day = 1; day <= daysInMonth; day++)
        {
            DateTime currentDay = firstDayOfMonth.AddDays(day - 1);
            GameObject dayObject = new GameObject("Day", typeof(Button));
            dayObject.transform.SetParent(dayGrid.transform);
            Button button = dayObject.GetComponent<Button>();
            TextMeshProUGUI text = dayObject.AddComponent<TextMeshProUGUI>();
            text.text = day.ToString();
            text.alignment = TextAlignmentOptions.Center;
            if (day == currentDate.Day){
                text.color = Color.red;
            }

            button.onClick.AddListener(() => OnDayClick(button, currentDay));
            dayButtons.Add(dayObject);
        }
    }
    void OnDayClick(Button clickedButton, DateTime clickedDate)
    {
        foreach (var obj in dayButtons)
        {
            TextMeshProUGUI textcal = obj.GetComponentInChildren<TextMeshProUGUI>();
            textcal.color = Color.white;
        }
        TextMeshProUGUI text = clickedButton.GetComponentInChildren<TextMeshProUGUI>();
        text.color = Color.red;
        monthDayYear.text = clickedDate.ToString("MMMM dd, yyyy");
    }

    public void GoToPreviousMonth()
    {
        currentDate = currentDate.AddMonths(-1);
        dayButtons.Clear();
        UpdateCalendar();
    }

    public void GoToNextMonth()
    {
        currentDate = currentDate.AddMonths(1);
        dayButtons.Clear();
        UpdateCalendar();
    }
}
