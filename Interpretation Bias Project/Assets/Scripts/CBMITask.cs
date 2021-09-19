using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class CBMITask : MonoBehaviour
{
    public TextMeshProUGUI wordText;
    public TextMeshProUGUI sentenceText;
    public GameObject fixationCross;
    public Button relatedButton;
    public Button notRelatedButton;

    string readPathBenign;
    string readPathNegative;
    string readPathSentence;

    public List<string> benignWordList = new List<string>();
    public List<string> negativeWordList = new List<string>();
    public List<string> sentenceList = new List<string>();


    private bool beginNextTrial;
    private GameManager gameManager;
    private AudioManager audioManager;
    private bool wordIsBenign;

    private TextAsset benignTextFile;
    private TextAsset negativeTextFile;
    private TextAsset sentenceTextFile;



    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        beginNextTrial = true;

        //   readPathBenign = Path.Combine(Application.streamingAssetsPath, "BenignWordList.txt");
        //   readPathNegative = Path.Combine(Application.streamingAssetsPath, "NegativeWordList.txt");
        //   readPathSentence = Path.Combine(Application.streamingAssetsPath, "SentenceList.txt");

        benignTextFile = Resources.Load<TextAsset>("Lists/BenignWordList");
        negativeTextFile = Resources.Load<TextAsset>("Lists/NegativeWordList");
        sentenceTextFile = Resources.Load<TextAsset>("Lists/SentenceList");

        benignWordList = new List<string>(benignTextFile.text.Split('\n'));
        negativeWordList = new List<string>(negativeTextFile.text.Split('\n'));
        sentenceList = new List<string>(sentenceTextFile.text.Split('\n'));

        //  ReadListFiles(readPathBenign, readPathNegative, readPathSentence);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Trial()
    {
        while (beginNextTrial)
        {

            int listLine = Random.Range(0, sentenceList.Count); // Pick random line from lists
            float benignOrNegative = Random.value;  // Randomly pick benign or negative pair word


            fixationCross.SetActive(true);  //Flash fixation cross
            yield return new WaitForSeconds(1.0f);
            fixationCross.SetActive(false);

            if (benignOrNegative > 0.5f)  //Set wordText to benign or negative
            {
                wordText.SetText(benignWordList[listLine]);
                wordIsBenign = true;
            }
            else
            {
                wordText.SetText(negativeWordList[listLine]);
                wordIsBenign = false;
            }

            
            wordText.gameObject.SetActive(true);  //Flash word
            yield return new WaitForSeconds(0.5f);
            wordText.gameObject.SetActive(false);

            sentenceText.SetText(sentenceList[listLine]);  //Show sentence and wait
            sentenceText.gameObject.SetActive(true);
            relatedButton.gameObject.SetActive(true);
            notRelatedButton.gameObject.SetActive(true);

            benignWordList.RemoveAt(listLine);
            negativeWordList.RemoveAt(listLine);
            sentenceList.RemoveAt(listLine);

            beginNextTrial = false;
        }
        

    }

    public void StartTrial()
    {
        sentenceText.gameObject.SetActive(false);
        relatedButton.gameObject.SetActive(false);
        notRelatedButton.gameObject.SetActive(false);

        if(sentenceList.Count > 0)
        {
            beginNextTrial = true;
            StartCoroutine(Trial());
        }
        else
        {
            //ReadListFiles(readPathBenign, readPathNegative, readPathSentence);

            //Populate lists from text files
            benignWordList = new List<string>(benignTextFile.text.Split('\n'));
            negativeWordList = new List<string>(negativeTextFile.text.Split('\n'));
            sentenceList = new List<string>(sentenceTextFile.text.Split('\n'));

            gameManager.isCBMIActive = false;
            gameManager.switchScreen("Trial");
            beginNextTrial = false;

            //Temporary save profile info - To Do: determine score based on timings/correct answers
            gameManager.highScore = Random.Range(0, 100);  //temp highscore test
            
        }

        

    }

    public void CheckTrialAnswerRelated()
    {
        if (wordIsBenign)
        {
            audioManager.PlayCorrectSound();
        }
        else
        {
            audioManager.PlayIncorrectSound();
        }
    }

    public void CheckTrialAnswerNotRelated()
    {
        if (wordIsBenign)
        {
            audioManager.PlayIncorrectSound();
        }
        else
        {
            audioManager.PlayCorrectSound();
        }
    }

    /*  OLD READER
    void ReadListFiles(string readPathBenign, string readPathNegative, string readPathSentence)
    {
        StreamReader sReaderB = new StreamReader(readPathBenign);
        StreamReader sReaderN = new StreamReader(readPathNegative);
        StreamReader sReaderS = new StreamReader(readPathSentence);

        while (!sReaderB.EndOfStream)
        {
            string line = sReaderB.ReadLine();
            benignWordList.Add(line);
        }

        sReaderB.Close();

        while (!sReaderN.EndOfStream)
        {
            string line = sReaderN.ReadLine();
            negativeWordList.Add(line);
        }

        sReaderN.Close();

        while (!sReaderS.EndOfStream)
        {
            string line = sReaderS.ReadLine();
            sentenceList.Add(line);
        }

        sReaderS.Close();
    }
    */
}
