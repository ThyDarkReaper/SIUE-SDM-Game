using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FullSerializer;
using FullSerializer.Internal.DirectConverters;


using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TalkingHandler : MonoBehaviour
{

    //public variables here
    public GlobalVariables GV;
    public LevelButtonManager LBM;
    public Texture[] enterImages;
    public Button[] responseButtons;
    public GameObject buttonsEmpty;
    public TextMeshProUGUI text;
    public GameObject enterImageObj;
    //private ints here
    private bool done = false;
    private TextAsset textScript;
    private int lineNumber = 0;
    private bool skipSignal = false;
    private bool nextSignal = false;
    private bool isTalking = false;
    public bool gtg = false;
    private string[] lines = new string[4];
    private string[] response = new string[4];
    private int[] optionsTF = new int[4];
    private bool enterAnimation = false;
    public bool holdForResponse = false;
    private string finalResponse = "";
    private bool clearButtons = false;
    private bool finished = false;

    //For score tracking
    public int questions = 0;
    public int questionsCorrect = 0;

    public void GetDialogueScore(){
        Debug.Log("Get Dialogue Score");
        
        int questionsAsked = GetNumQuestions();
        Debug.Log("Questions asked: " + questionsAsked);
        int correctQuestions = GetScore();
        Debug.Log("Correct: " + correctQuestions);

        float baseScore = 0.5f; // 50%
        float accuracy = (float)correctQuestions / questionsAsked;
        Debug.Log("Dialogue Score " + accuracy);
        LBM.score += baseScore * accuracy * 100f;
        Debug.Log("Final Score: " + LBM.score);
    }

    public int GetNumQuestions()
    {
        return questions;
    }

    public void AddScore(int points)
    {
        questionsCorrect += points;
    }

    public int GetScore()
    {
        return questionsCorrect;
    }

    private int CheckIfDialogueFinished()
    {
        #if UNITY_EDITOR
        string fullPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", UnityEditor.AssetDatabase.GetAssetPath(textScript)));
        int lineCount = File.ReadAllText(fullPath).Replace("\r\n", "\n").Replace("\r", "\n").Split('\n').Length;
        #else
        int lineCount = textScript.text.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n').Length;
        #endif
        Debug.Log("Line count: " + lineCount);
        return lineCount;
    }

    void Start()
    {
        GV = Camera.main.GetComponent<GlobalVariables>();
        LBM = GameObject.Find("LevelButtonManager").GetComponent<LevelButtonManager>();
        done = false;
        textScript = GV.getScript();
        lineNumber = 0;
        skipSignal = false;
        nextSignal = false;
        gtg = false;
        lines = new string[4];
        response = new string[4];
        optionsTF = new int[4];
        enterAnimation = false;
        holdForResponse = false;
        finalResponse = "";
        buttonsEmpty.SetActive(false);
        #if UNITY_EDITOR
        string scriptPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", UnityEditor.AssetDatabase.GetAssetPath(textScript)));
        lines = File.ReadAllText(scriptPath).Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
        #else
        lines = textScript.text.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
        #endif
        startTalking();
    }
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(clearButtons)
            {
                buttonColorReset();
                buttonsEmpty.SetActive(false);
                clearButtons = false;
            }
            if(!done && !finished)
            {
                //Debug.Log(holdForResponse);
                if(!holdForResponse)
                {
                    if(!isTalking)
                    {
                        enterAnimation = false;
                        startTalking();
                    }else{
                        skipSignal = true;

                    }
                }
            }else{
                GV.clearScript();
                GV.swapTalking();
            }
        }
        GV.setTalkingSound(isTalking);
        //Debug.Log(isTalking);

    }
    private IEnumerator enterAnimationF()
    {
        while(enterAnimation)
        {
            enterImageObj.GetComponent<RawImage>().texture = enterImages[0];
            yield return new WaitForSeconds(0.5f);
            enterImageObj.GetComponent<RawImage>().texture = enterImages[1];
            yield return new WaitForSeconds(0.5f);
        }
    }
    private void startTalking()
    {
        //If the player has not checked the computer beforehand, this is added to the dialog and end the dialogue
        if(!GV.getCheckedAxium()) {
            done = true;
            GV.setAlreadyTalkedTo(false);
            StartCoroutine(AppendCharacters("Please check your computer machine for the details first, I don't like repeating myself."));
        }
        else if(!GV.getAlreadyTalkedTo())
        {
            if (lineNumber >= lines.Length)
            {
                done = true;
                return;
            }
            int result = 0;
            bool canConvert = lineNumber + 1 < lines.Length && Int32.TryParse(lines[lineNumber+1].Trim(), out result);
            if(canConvert)
            {
                if(result == 1101588)
                {
                    gtg = true;
                }
            }
            StopAllCoroutines();
            StartCoroutine(AppendCharacters(lines[lineNumber]));
            if(gtg)
            {
                holdForResponse = true;
                Debug.Log("Here");
                responseSetup();
            }else{
                StartCoroutine(enterAnimationF());
            }
            lineNumber++;
            Debug.Log("Line Number: " + lineNumber);
        }
        else{
            //if the player has already spoken to the NPC, this is said and ends dialogue
            done = true;
            StartCoroutine(AppendCharacters("Sorry, I'm afraid you have already talked with me."));
        }
    }


    private IEnumerator AppendCharacters(string line)
    {
        text.text = "";
        foreach(char character in line)
        {
            
            if(!skipSignal)
            {
                isTalking = true;
                text.text += character;
                yield return new WaitForSeconds(0.01f);
            }else{
                text.text = line;
                isTalking = false;
                resumeAnswering();
                yield return null;
            }
        }
        isTalking = false;
        resumeAnswering();
        skipSignal = false;
        enterAnimation = true;
        if(gtg)
        {
            holdForResponse = true;
            Debug.Log("Here");
            responseSetup();
        }else{
            StartCoroutine(enterAnimationF());
        }
    }
    private void responseSetup()
    {
        buttonsEmpty.SetActive(true);
        lineNumber++;
        for(int i = 0; i < 4; i++)
        {
            lineNumber++;
            buttonSplitter(lines[lineNumber], i);
        }
        for(int i = 0; i < 4; i++)
        {
            lineNumber++;
            responseSetter(lines[lineNumber], i);
        }
        lineNumber++;
        int result;
        bool canConvert = Int32.TryParse(lines[lineNumber], out result);
        if(canConvert)
        {
            if(result == 8675309)
            {
                gtg = false;
            }
        }
            
    }
    private void responseSetter(string rLine, int value)
    {
        response[value] = rLine;
    }
    private void buttonSplitter(string rLine, int button)
    {
        bool firstChar = true;
        foreach(char character in rLine)
        {
            if(firstChar)
            {
                optionsTF[button] = Int32.Parse(new string(character, 1));
                responseButtons[button].GetComponentInChildren<TextMeshProUGUI>().text = "";
                firstChar = false;
            }else{
                responseButtons[button].GetComponentInChildren<TextMeshProUGUI>().text += character;
            }
        }
    }
    public void Response1OnClick()
    {
        if(GV.getButtonValue() == 1)
        {
            pauseAnswering();
            StopAllCoroutines();
            buttonColorChanger();
            finalResponse = response[0];
            StartCoroutine(appendResponse());
            holdForResponse = false;
            clearButtons = true;

            if (optionsTF[0] == 1) //is correct
            {
                AddScore(1);
                //Debug.Log("Correct Answer! Score: " + GetScore());
            }
            questions++;
            //Debug.Log("Questions Answered: " + questions);
        }
    }
    public void Response2OnClick()
    {
        if(GV.getButtonValue() == 2)
        {
            pauseAnswering();
            StopAllCoroutines();
            buttonColorChanger();
            finalResponse = response[1];
            StartCoroutine(appendResponse());
            holdForResponse = false;
            clearButtons = true;

            if (optionsTF[1] == 1) //is correct
            {
                AddScore(1);
                //Debug.Log("Correct Answer! Score: " + GetScore());
            }
            questions++;
            //Debug.Log("Questions Answered: " + questions);
        }
    }
    public void Response3OnClick()
    {
        if(GV.getButtonValue() == 3)
        {
            pauseAnswering();
            StopAllCoroutines();
            //Debug.Log("Now Here wtf");
            buttonColorChanger();
            finalResponse = response[2];
            StartCoroutine(appendResponse());

            holdForResponse = false;
            clearButtons = true;

            if (optionsTF[2] == 1) //is correct
            {
                AddScore(1);
                //Debug.Log("Correct Answer! Score: " + GetScore());
            }
            questions++;
            //Debug.Log("Questions Answered: " + questions);
        }
    }
    public void Response4OnClick()
    {
        if(GV.getButtonValue() == 4)
        {
            pauseAnswering();
            StopAllCoroutines();
            buttonColorChanger();
            finalResponse = response[3];
            StartCoroutine(appendResponse());

            holdForResponse = false;
            clearButtons = true;
            if (optionsTF[3] == 1) //is correct
            {
                AddScore(1);
                //Debug.Log("Correct Answer! Score: " + GetScore());
            }
            questions++;
            //Debug.Log("Questions Answered: " + questions);
        }
    }
    private void buttonColorChanger()
    {
        for(int i = 0; i < 4; i++)
        {
            if(optionsTF[i] == 0)
            {
                responseButtons[i].GetComponent<Image>().color = Color.red;
            }else{
                responseButtons[i].GetComponent<Image>().color = Color.green;
            }
        }
    }
    private void buttonColorReset()
    {
        for(int i = 0; i < 4; i++)
        {
            responseButtons[i].GetComponent<Image>().color = Color.white;
        }
    }
    
    private IEnumerator appendResponse()
    {
        text.text = "";
        foreach(char character in finalResponse)
        {
            
            if(!skipSignal)
            {
                isTalking = true;
                text.text += character;
                yield return new WaitForSeconds(0.01f);
            }else{
                text.text = finalResponse;
                isTalking = false;
                yield return null;
            }
        }
        isTalking = false;
        skipSignal = false;
        enterAnimation = true;
        if(lineNumber + 1 >= lines.Length)
            {
                done = true;
                GetDialogueScore();
                LBM.diagnosisButton.SetActive(true);
                GV.setAlreadyTalkedTo(true);
            }
    }

    private void pauseAnswering() {
        for(int i = 0; i < 4; i++)
        {
            responseButtons[i].interactable = false;
        }
    }

    private void resumeAnswering() {
        for(int i = 0; i < 4; i++)
        {
            responseButtons[i].interactable = true;
        }
    }

}
