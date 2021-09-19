using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

//Handles writing to spreadsheet files
public class SpreadsheetManager : MonoBehaviour
{
    public ES3Spreadsheet inputSpreadsheet;
    public ES3Spreadsheet outputSpreadsheet;

    public int currentRow;
    public string playerName;

    public GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        //var trialDataSpreadSheet = new ES3Spreadsheet();
        //trialDataSpreadSheet.Load("SpreadSheets/TrialDataInput.csv");

        //inputSpreadsheet.Load("SpreadSheets/TrialDataInput.csv");
        //outputSpreadsheet.Load("SpreadSheets/TrialDataOutput.csv");

        Directory.CreateDirectory(Application.streamingAssetsPath + "/Spreadsheets/");

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCurrentRow(int row)
    {
        currentRow = row;
    }

    public int GetCurrentRow()
    {
        return currentRow;
    }

    public ES3Spreadsheet GetInputSpreadsheet()
    {
        return inputSpreadsheet;
    }

    public ES3Spreadsheet GetOutputSpreadsheet()
    {
        return outputSpreadsheet;
    }

    public void UpdateInputSpreadsheet(ES3Spreadsheet input)
    {
        inputSpreadsheet = input;
    }

    public ES3Spreadsheet UpdateOutputSpreadsheet(int timerCol, int interpCol, int sessionCol, float time, string interpretation, int session)
    {

        //Set Trial time
        outputSpreadsheet.SetCell<float>(timerCol, currentRow, time);

        //set Trial interpretation
        outputSpreadsheet.SetCell<string>(interpCol, currentRow, interpretation);

        //set Trial session number
        outputSpreadsheet.SetCell<int>(sessionCol, currentRow, session);

        return outputSpreadsheet;
    }

    public void SendInputSpreadSheet(ES3Spreadsheet input)
    {
        inputSpreadsheet = input;
    }

    public void SendOutputSpreadsheet(ES3Spreadsheet output)
    {
        outputSpreadsheet = output;
    }

    public void OutputCSV()
    {
        //outputSpreadsheet.Save(Application.persistentDataPath + "TrialDataOutputTest.csv.bytes");
        //AssetDatabase.Refresh();

        //outputSpreadsheet.Save(Path.Combine(Application.persistentDataPath, "SpreadSheets", "TrialDataOutput.csv.bytes"), true);
        // AssetDatabase.Refresh();


        //Add player name string to file name?
        //playerName = gameManager.playerName;

        string csvFileName = Application.streamingAssetsPath + "/Spreadsheets/" + "TrialOutputCSV" + ".csv";

        if (!File.Exists(csvFileName))
        {
            File.WriteAllText(csvFileName, "Testing file creation.");
            //AssetDatabase.Refresh();
            outputSpreadsheet.Save(csvFileName, true);
            Debug.Log("CSV File created and written at: " + csvFileName);
        }
        else
        {
            outputSpreadsheet.Save(csvFileName, true);
            Debug.Log("CSV File written at: " + csvFileName);
        }
    }
}
