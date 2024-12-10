using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; 

public class CategoryItemTransition : MonoBehaviour
{
    public TextMeshProUGUI item;
    public void onclicked(){
        PlayerPrefs.SetString("itemName", item.text);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Clothing");  
    }
}
