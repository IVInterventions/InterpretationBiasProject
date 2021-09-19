using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using System;

public class TrialManager : MonoBehaviour
{
    public TextMeshProUGUI wordText;
    public TextMeshProUGUI sentenceText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI sessionStatsText;
    public GameObject fixationCross;
    public Button relatedButton;
    public Button notRelatedButton;
    public Button continueButton;
    public GameObject correctParticle;
    public GameObject correctText;
    public GameObject incorrectText;
    public int rowsRelatable;  //Related / Unrelated W/S pairs for tutorial
    public bool shortTestMode;  //use less trials rows
    public int shortTestModeNumTrials;
    public int timerColumn;
    public int interpretationColumn;
    public int sessionColumn;
    public int sessionsCompleted;
    public int highScore;

    private bool beginNextTrial;
    private bool wordIsBenign;
    private bool wordIsUnrelated;
    private bool isTimerActive;
    private int spreadSheetLength;
    private float stopWatch;
    private string interpretation;

    private float totalTime;
    private int totalInterp;
    private int sessionScore;

    public GameManager gameManager;
    public AudioManager audioManager;
    public SpreadsheetManager spreadsheetManager;

    public bool continueButtonPressed;
    public List<int> rowsDone = new List<int>();

    //store times and interpretaions for calculating highscore
    private List<float> scoreTimes = new List<float>();
    private List<int> scoreInterp = new List<int>();

    //private static ES3Spreadsheet trialDataOutput;

    // Start is called before the first frame update
    void Start()
    {
        //Load ES3 spreadsheets
        var trialDataSpreadSheet = new ES3Spreadsheet();
        //trialDataSpreadSheet.Load(Application.persistentDataPath + "TrialDataInput.csv");

        trialDataSpreadSheet.Load(Path.Combine(Application.streamingAssetsPath, "SpreadSheets", "TrialDataInput.csv"));



        var trialDataOutput = new ES3Spreadsheet();
        //trialDataOutput.Load("SpreadSheets/TrialDataOutput.csv"); 
        trialDataOutput.Load(Path.Combine(Application.streamingAssetsPath, "SpreadSheets", "TrialDataOutput.csv"));

        // gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        // audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        // spreadsheetManager = GameObject.Find("SpreadsheetManager").GetComponent<SpreadsheetManager>();

        spreadsheetManager.SendInputSpreadSheet(trialDataSpreadSheet);
        spreadsheetManager.SendOutputSpreadsheet(trialDataOutput);

        sessionsCompleted = PlayFabManager.PFM.sessionsCompleted;

        spreadSheetLength = trialDataSpreadSheet.RowCount;

        //Set Trial Timer variables
        stopWatch = 0.0f;
        isTimerActive = false;
        timerText.SetText(stopWatch.ToString("F2"));

        beginNextTrial = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTimerActive)
        {
            stopWatch += Time.deltaTime;
            timerText.SetText(stopWatch.ToString("F2"));
        }
    }

    // A "Trial" is one complete word/sentence pair and corresponding answer from the player
    // Will run trials while there is remaining lines of data in the input spreadsheet
    IEnumerator Trial()
    {
        //Load Data from CSV file to Easy Save Spreadsheet
        var trialDataSpreadSheet = new ES3Spreadsheet();
        //trialDataSpreadSheet.Load("SpreadSheets/TrialDataInput.csv");
        trialDataSpreadSheet.Load(Path.Combine(Application.streamingAssetsPath, "SpreadSheets", "TrialDataInput.csv"));


        //List<int> rowsDone = new List<int>();

        var rowPicked = new bool();
        var row = new int();

        while (beginNextTrial)
        {
            timerText.SetText("0.00");

            Debug.Log("Columns: " + (trialDataSpreadSheet.RowCount - 1));
            Debug.Log("Columns: " + spreadSheetLength);

            rowPicked = false;

            //Select a word/sentence pair from input spreadsheet for the trial
            //Don't repeat rows
            // Pick random line from spreadsheet
            while (!rowPicked)
            {
                row = UnityEngine.Random.Range(rowsRelatable + 1, trialDataSpreadSheet.RowCount - 1);  //Range of rows in CSV for regular trials
                if (rowsDone.Contains(row))
                {
                    rowPicked = false;
                }
                else
                {
                    rowsDone.Add(row);
                    rowPicked = true;
                    spreadsheetManager.UpdateCurrentRow(row);  //update current row in SpreadsheetManager
                    Debug.Log("New row picked: " + row);
                }
            }

            //reset word values
            wordIsBenign = false;
            wordIsUnrelated = false;

            yield return new WaitForSeconds(1.0f);

            fixationCross.SetActive(true);  //Flash fixation cross
            yield return new WaitForSeconds(1.0f);
            fixationCross.SetActive(false);


            sentenceText.SetText(trialDataSpreadSheet.GetCell<string>(2, row));  //Show sentence and wait
            sentenceText.gameObject.SetActive(true);
            continueButton.gameObject.SetActive(true);

            yield return new WaitUntil(() => continueButtonPressed);  //wait for continueButton to be pressed
            continueButton.gameObject.SetActive(false);
            sentenceText.gameObject.SetActive(false);
            continueButtonPressed = false;

            wordText.SetText(trialDataSpreadSheet.GetCell<string>(1, row));  //Set Word

            wordText.gameObject.SetActive(true);  //  New: Wait for choice / Old: Flash word
            relatedButton.gameObject.SetActive(true);
            notRelatedButton.gameObject.SetActive(true);
            //yield return new WaitForSeconds(0.5f);
            //wordText.gameObject.SetActive(false);

            //Determine value of word
            if (trialDataSpreadSheet.GetCell<string>(0, row) == "positive" || trialDataSpreadSheet.GetCell<string>(0, row) == "neutral" )
            {
                wordIsBenign = true;
            }
            else if(trialDataSpreadSheet.GetCell<string>(0, row) == "negative")
            {
                wordIsBenign = false;
            }
            else if (trialDataSpreadSheet.GetCell<string>(0, row) == "Unrelated")
            {
                wordIsUnrelated = true;
            }

            

            isTimerActive = true;  //Start timer

            Debug.Log(trialDataSpreadSheet.GetCell<string>(0,row));  // Test if reading correctly
            Debug.Log(trialDataSpreadSheet.GetCell<string>(1, row));
            Debug.Log(trialDataSpreadSheet.GetCell<string>(2, row));

            beginNextTrial = false;
        }
    }

    //Show player result of their choice
    IEnumerator DisplayResult(string result)
    {
        if (result == "Correct")
        {
            correctText.SetActive(true);
        }
        else
        {
            incorrectText.SetActive(true);
        }
        yield return new WaitForSeconds(1.0f);
        
        correctText.SetActive(false);
        incorrectText.SetActive(false);
    }

    //Start a new trial if remaining word/sentence pairs or go to post session if finished and output the player's data to output spreadsheet
    public void StartTrial()
    {
        wordText.gameObject.SetActive(false);
        sentenceText.gameObject.SetActive(false);
        relatedButton.gameObject.SetActive(false);
        notRelatedButton.gameObject.SetActive(false);

        if (rowsDone.Count < (spreadSheetLength - 2 - rowsRelatable) && !shortTestMode)
        {
            beginNextTrial = true;
            StartCoroutine(Trial());
        }
        else if ((rowsDone.Count < shortTestModeNumTrials) && shortTestMode)
        {
            beginNextTrial = true;
            StartCoroutine(Trial());
        }
        else
        {
            //Post Session / End Session

            gameManager.isCBMIActive = false;
            CalculateHighscore();
            gameManager.switchScreen("PostSession");
            beginNextTrial = false;

            //Update sessionsCompleted

            //To Do: SpreadsheetManager export csv and upload to Playfab
            spreadsheetManager.OutputCSV();

        }

    }

    //If player clicked "Related Button", Determine if player's answer to trial is correct or not and record player's answer/data to spreadsheet
    public void CheckTrialAnswerRelated()
    {
        //Timer
        //To do: save trial time values,  create spreadsheet script to edit ES3Spreadsheet and save to csv
        isTimerActive = false;

        if (wordIsBenign)
        {
            interpretation = "Related - Endorsed Benign/Positive";

            audioManager.PlayCorrectSound();
            Instantiate(correctParticle, relatedButton.transform.position, Quaternion.identity);  //play correct anim  TO DO: add Correct or Good Job text
            StartCoroutine(DisplayResult("Correct"));

            spreadsheetManager.UpdateOutputSpreadsheet(timerColumn, interpretationColumn, sessionColumn, stopWatch, interpretation, sessionsCompleted);

            //Record trial performance to HighScore lists
            scoreTimes.Add(stopWatch);
            scoreInterp.Add(1);
        }
        else if (wordIsUnrelated)
        {
            interpretation = "Related - Endorsed Unrelated Word";

            audioManager.PlayUnrelatedSound();

            spreadsheetManager.UpdateOutputSpreadsheet(timerColumn, interpretationColumn, sessionColumn, stopWatch, interpretation, sessionsCompleted);

            //Record trial performance to HighScore lists
            scoreTimes.Add(stopWatch);
            scoreInterp.Add(0);
        }
        else
        {
            interpretation = "Related - Endorsed Negative";

            audioManager.PlayIncorrectSound();

            spreadsheetManager.UpdateOutputSpreadsheet(timerColumn, interpretationColumn, sessionColumn, stopWatch, interpretation, sessionsCompleted);
            StartCoroutine(DisplayResult("Incorrect"));

            //Record trial performance to HighScore lists
            scoreTimes.Add(stopWatch);
            scoreInterp.Add(0);
        }

        stopWatch = 0.0f; //Reset stopWatch for next trial
    }

    //If player clicked "Not Related" Button, Determine if player's answer to trial is correct or not and record player's answer/data to spreadsheet
    public void CheckTrialAnswerNotRelated()
    {
        //Timer
        //To do: save trial time values,  create spreadsheet script to edit ES3Spreadsheet and save to csv
        isTimerActive = false;
        //trialDataOutput.SetCell<float>(timerColumn, rowsDone.Last(), stopWatch);

        if (wordIsBenign)
        {
            interpretation = "Unrelated - Rejected Benign/Positive";

            audioManager.PlayIncorrectSound();

            spreadsheetManager.UpdateOutputSpreadsheet(timerColumn, interpretationColumn, sessionColumn, stopWatch, interpretation, sessionsCompleted);
            StartCoroutine(DisplayResult("Incorrect"));

            //Record trial performance to HighScore lists
            scoreTimes.Add(stopWatch);
            scoreInterp.Add(0);
        }
        else if (wordIsUnrelated)
        {
            interpretation = "Unrelated - Rejected Unrelated Word";

            audioManager.PlayUnrelatedSound();

            spreadsheetManager.UpdateOutputSpreadsheet(timerColumn, interpretationColumn, sessionColumn, stopWatch, interpretation, sessionsCompleted);

            //Record trial performance to HighScore lists
            scoreTimes.Add(stopWatch);
            scoreInterp.Add(0);
        }
        else
        {
            interpretation = "Unrelated - Rejected Negative";

            audioManager.PlayCorrectSound();
            Instantiate(correctParticle, notRelatedButton.transform.position, Quaternion.identity);  //play correct anim  TO DO: add Correct or Good Job text
            StartCoroutine(DisplayResult("Correct"));

            spreadsheetManager.UpdateOutputSpreadsheet(timerColumn, interpretationColumn, sessionColumn, stopWatch, interpretation, sessionsCompleted);

            //Record trial performance to HighScore lists
            scoreTimes.Add(stopWatch);
            scoreInterp.Add(1);
        }

        stopWatch = 0.0f;  //Reset stopWatch for next trial
    }

    public void CalculateHighscore()
    {
        highScore = PlayFabManager.PFM.highScore;
        sessionsCompleted = PlayFabManager.PFM.sessionsCompleted;

        totalTime = scoreTimes.Sum();
        totalInterp = scoreInterp.Sum();

        sessionScore = (Convert.ToInt32(Mathf.Round(totalTime)) + (totalInterp)) * 100;

        //update highscore if sessionScore is greater
        if (sessionScore > highScore)
        {
            PlayFabManager.PFM.highScore = sessionScore;
            highScore = sessionScore;

            //To Do: celebration popup
        }

        //Set Text with stats
        sessionStatsText.text = "Score: " + sessionScore + "\n" + "Average Time: " + scoreTimes.Average().ToString("F2") + "\n" + "High Score: " + highScore;  //To Do: add in highscore and other stats

       
        //update sessionsCompleted
        PlayFabManager.PFM.sessionsCompleted = sessionsCompleted + 1;

        //Upload stats to PlayFab Server
        PlayFabManager.PFM.SetStats();

        //PlayFab File Upload
        PlayFabManager.PFM.UploadFile("TrialOutputCSV.csv");
    }

    public void ContinueButtonPressed()
    {

        continueButtonPressed = true;
    }

    public void ResetSession()
    {
        rowsDone.Clear();
    }

    public void ShortTestModeOn()
    {
        shortTestMode = true;
    }
    public void ShortTestModeOff()
    {
        shortTestMode = false;
    }

    //For testing purposes, use shorter data set
    public void UpdateShortTrialNum(TMP_Text numTrialsText)
    {
        bool success = int.TryParse(numTrialsText.text, out int num);
        if (success)
        {
            shortTestModeNumTrials = num;
        }
        else
        {
            Debug.Log("Enter an integer for # of trails. Num: " + num); ;
        }

        //shortTestModeNumTrials = Int32.Parse(numTrialsText.GetComponent<TextMeshProUGUI>().text);

        //shortTestModeNumTrials = Convert.ToInt32();
    }
}
