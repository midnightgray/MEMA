using UnityEngine;
using UnityEngine.UI;

public class Darkmode : MonoBehaviour
{
    // Reference to UI elements that will change color in dark mode
    public Text textElement;
    public Image imageElement;

    private void Start()
    {
        // Check if dark mode preference is stored in PlayerPrefs
        if (PlayerPrefs.HasKey("DarkMode"))
        {
            // Apply dark mode based on stored preference
            bool darkModeEnabled = PlayerPrefs.GetInt("DarkMode") == 1;
            SetDarkMode(darkModeEnabled);
        }
        else
        {
            // Default to light mode if no preference is found
            SetDarkMode(false);
        }
    }

    public void ToggleDarkMode()
    {
        // Toggle dark mode
        bool darkModeEnabled = !IsDarkModeEnabled();
        SetDarkMode(darkModeEnabled);

        // Save dark mode preference to PlayerPrefs
        PlayerPrefs.SetInt("DarkMode", darkModeEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void SetDarkMode(bool enableDarkMode)
    {
        // Adjust UI element colors based on dark mode preference
        if (enableDarkMode)
        {
            // Dark mode colors
            textElement.color = Color.white;
            imageElement.color = Color.gray;
        }
        else
        {
            // Light mode colors
            textElement.color = Color.black;
            imageElement.color = Color.white;
        }
    }

    private bool IsDarkModeEnabled()
    {
        // Check if dark mode is currently enabled
        return PlayerPrefs.GetInt("DarkMode") == 1;
    }
}
