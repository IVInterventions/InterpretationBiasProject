using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool isCBMIActive;

    public GameObject mainMenuPanel;
    public GameObject biasGamePanel;
    public GameObject postSessionPanel;
    public GameObject backgroundMainMenu;
    public GameObject backgroundBias;
    public GameObject settingsPanel;
    

    public GameObject cbmiManager;
   // private CBMITask cbmiTask;
    private TrialManager trialManager;

    public TMP_Text playerNameInputField;

    public string playerName;
    public int highScore;


    // Start is called before the first frame update
    void Start()
    {
       
        isCBMIActive = false;

        cbmiManager = GameObject.Find("CBM-IManager");
        //cbmiTask = cbmiManager.GetComponent<CBMITask>();
        trialManager = cbmiManager.GetComponent<TrialManager>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartCBMITask()
    {
        isCBMIActive = true;
        switchScreen("Trial");
        // cbmiTask.StartTrial();
        trialManager.StartTrial();
    }

    public void switchScreen(string scene)
    {
        switch(scene)
        {
            case "Trial":
            if (isCBMIActive)
            {
                backgroundBias.SetActive(true);
                biasGamePanel.SetActive(true);
                mainMenuPanel.SetActive(false);
                backgroundMainMenu.SetActive(false);
                postSessionPanel.SetActive(false);
                settingsPanel.SetActive(false);
                Debug.Log("Switching to Trial panels");
            }
                break;

            case "PostSession":
                backgroundBias.SetActive(true);
                postSessionPanel.SetActive(true);
                biasGamePanel.SetActive(false);
                mainMenuPanel.SetActive(false);
                backgroundMainMenu.SetActive(false);
                settingsPanel.SetActive(false);
                Debug.Log("Switching to PostSession panels");
                break;

            case "MainMenu":
                backgroundBias.SetActive(false);
                biasGamePanel.SetActive(false);
                mainMenuPanel.SetActive(true);
                backgroundMainMenu.SetActive(true);
                settingsPanel.SetActive(false);
                postSessionPanel.SetActive(false);
                Debug.Log("Switching to MainMenu panels");
                break;

            case "Settings":
                backgroundBias.SetActive(false);
                biasGamePanel.SetActive(false);
                mainMenuPanel.SetActive(false);
                backgroundMainMenu.SetActive(true);
                settingsPanel.SetActive(true);
                postSessionPanel.SetActive(false);
                Debug.Log("Switching to Settings panels");
                break;
        }
        //else
        //{
        //    backgroundBias.SetActive(false);
        //    biasGamePanel.SetActive(false);
        //    mainMenuPanel.SetActive(true);
        //    backgroundMainMenu.SetActive(true);
        //}
    }

    public void UpdateProfile()
    {

    }

    public void UpdateProfileName()
    {
        playerName = playerNameInputField.text;
    }


   /* Old Save System
    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
    }

    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        playerName = data.playerName;
        highScore = data.highScore;
    }
    */
}
