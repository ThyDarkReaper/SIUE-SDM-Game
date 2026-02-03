//This script overall is used for managing the UI of the Levels and grading to send to Firebase
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;
using System.Text.RegularExpressions;

public class LevelButtonManager : MonoBehaviour
{
    //Used functions from other scripts
    public GlobalVariables GV;
    public chatboxHandler cbh;
    public TalkingHandler TH;
    public FireBase FB;
    public KeepPlayerPOS pos;//Used for the player POS when exit the scene

    //To keep track of a score of 100
    public float score = 0f;
    public int attemptsMed = 0;
    public int attemptsDia = 0;
    public float maxScore = 50f;

    public GameObject MainCamera;
    public GameObject MedCamera;

    public GameObject axiumUIPanel;
    public GameObject diagnosisButton;
    public GameObject medicineButton;
    public GameObject backButtonFromAxium;
    public GameObject axiumButton;
    public GameObject diagnosisAns;
    public GameObject medicineAns;
    public GameObject backButtonFromDiaAndMed;
    public GameObject completeLevel;
    public GameObject npc;

    //For the Diagnosis and Medicine Anwsers
    public Button[] medButtons;
    public Button[] diaButtons;
    public TextAsset diaFile; 
    public TextAsset medFile;

    //For Score
    public TMP_Text scoreText;

    //For if wrong or right button for Med and Diagnosis
    public int correctDiaAnswerIndex;
    public int correctMedAnswerIndex;
    public bool isDiaCorrect =  false;
    public bool isMedCorrect = false;
    public int correctAnswerDia = 0;
    public int correctAnswerMed = 0;
    public int onlyCountOnceDia = 0;
    public int onlyCountOnceMed = 0;
    private bool[] diaButtonClicked;
    private bool[] medButtonClicked;

    //Used for calculation
    public int onlyUpdateOnce = 0;
    public int correctAnswerDialogue;
    public int numOfQuestions;

    //To disable NPC click
    public GameObject npcHoverDetector;

    //To grab scene name for score
    public string currentSceneName;


    void Start(){
        //Load the Diagnosis
        LoadTextFromFile(diaFile, diaButtons);
        string[] diaLines = diaFile.text.Split('\n');
        correctDiaAnswerIndex = int.Parse(diaLines[0].Trim());//Find the correct anwser in DIA
        //Load the med file
        LoadTextFromFile(medFile, medButtons);
        string[] medLines = medFile.text.Split('\n');
        correctMedAnswerIndex = int.Parse(medLines[0].Trim());//Find correct button anwser in MED
        diagnosisButton.SetActive(false);//Diagnosis button false till talk to NPC

        diaButtonClicked = new bool[diaButtons.Length];
        medButtonClicked = new bool[medButtons.Length];

        //Handles the dia buttons make sure each button is only clicked once
        for (int i = 0; i < diaButtons.Length; i++)
        {
            int buttonIndex = i; //Capture index
            diaButtons[i].onClick.AddListener(() =>
            {
                if (!diaButtonClicked[buttonIndex])
                {
                    diaButtonClicked[buttonIndex] = true;
                    OnClickColorsTheButtonsDia(diaButtons[buttonIndex], diaButtons, correctDiaAnswerIndex);
                }
            });
        }
        
        //Handles the med buttons make sure each button is only clicked once
        for (int i = 0; i < medButtons.Length; i++)
        {
            int buttonIndex = i; //Capture index
            medButtons[i].onClick.AddListener(() =>
            {
                if (!medButtonClicked[buttonIndex])
                {
                    medButtonClicked[buttonIndex] = true;
                    OnClickColorsTheButtonsMed(medButtons[buttonIndex], medButtons, correctMedAnswerIndex);
                }
            });
        }
        //StartCoroutine(UpdateToGrabTheDiaAfterTalk());
    }

    //Gets the number at the end of the current scene (ex: Level1 extracts that 1) for the array in firebase to store the score there
    public int GetLevelNumber()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log("Current Scene Name: " + currentSceneName);

        //Grab Number of Level
        string numberString = Regex.Match(currentSceneName, @"\d+").Value;
        if (int.TryParse(numberString, out int levelNumber))
        {
            Debug.Log("Extracted Level Number: " + levelNumber);//Check output
            return levelNumber;
        }
        else
        {
            Debug.Log("No valid level number found in the scene name.");
            return 0;
        }
    } 

    //To activate the Diagnosis button in the scene
    public void ActivateDia(){
        diagnosisButton.SetActive(true);
    }

    //Reads the buttons
    void LoadTextFromFile(TextAsset textFile, Button[] buttons){
        //Go through the textfile that is connected to the diaFile or medFile and pulls the info into the 4 buttons
        string[] lines = textFile.text.Split('\n');

        for(int i = 0; i < buttons.Length && i < lines.Length; i++){
            TMP_Text buttonText = buttons[i].GetComponentInChildren<TMP_Text>();
                if (buttonText != null)
                {
                    buttonText.text = lines[i + 1].Trim(); //Set button text from file
                }

        //Color buttonColor = (i == correctAnswerIndex) ? Color.green : Color.red;
        //buttons[i].GetComponent<Image>().color = buttonColor;
        }
    }

    //Track buttons for DIA
    public void OnClickColorsTheButtonsDia(Button clickedButton, Button[] buttons, int correctAnswerIndex){
        int buttonIndex = System.Array.IndexOf(buttons, clickedButton);

        if ((buttonIndex == correctAnswerIndex) && (onlyCountOnceDia == 0))
        {
            //Calculate the score. 25% highest and decreases evenly from there
            onlyCountOnceDia++;//Make sure after correct user can get no more wrong 
            clickedButton.GetComponent<Image>().color = Color.green; //Correct
            isDiaCorrect = true;
            float rewardMultiplier = attemptsDia == 0 ? 1f :      //100% of DIA section on first try
                                 attemptsDia == 1 ? 0.5f :
                                 attemptsDia == 2 ? 0.25f :
                                 0.125f;

            float diaWeight = 0.25f; //DIA is worth 25% of total score
            float percentEarned = diaWeight * rewardMultiplier;

            score += (percentEarned * 100f); //Turn to percent

            Debug.Log("Percent earned from DIA: " + (percentEarned * 100f) + "%");
        }
        else if(onlyCountOnceDia == 1){
            return;
        }
        else if(attemptsDia <= 4)
        {
            attemptsDia++;//Each attempt wrong attempt is tracked for calculation above
            clickedButton.GetComponent<Image>().color = Color.red; //Wrong
            Debug.Log("Wrong answer!");
        }
        else{
            return;
        }
    }

    //Track the buttons for MED (same as DIA)
    public void OnClickColorsTheButtonsMed(Button clickedButton, Button[] buttons, int correctAnswerIndex){
        int buttonIndex = System.Array.IndexOf(buttons, clickedButton);

        if ((buttonIndex == correctAnswerIndex) && (onlyCountOnceMed == 0))
        {
            onlyCountOnceMed++;
            clickedButton.GetComponent<Image>().color = Color.green; //Correct
            isMedCorrect = true;
            float rewardMultiplier = attemptsMed == 0 ? 1f :      // 100% of MED section on first try
                                 attemptsMed == 1 ? 0.5f :
                                 attemptsMed == 2 ? 0.25f :
                                 0.125f;

            float medWeight = 0.25f; //Med is worth 25% of total score
            float percentEarned = medWeight * rewardMultiplier;

            score += (percentEarned*100f); 

            Debug.Log("Percent earned from Med: " + (percentEarned * 100f) + "%");
        }
        else if(onlyCountOnceMed == 1){
            return;
        }
        else if(attemptsMed <= 4)
        {
            attemptsMed++;//Each attempt wrong attempt is tracked
            clickedButton.GetComponent<Image>().color = Color.red; //Wrong
            Debug.Log("Wrong answer!");
        }
        else{
            return;
        }
    }

    //Output the score (after the Player completes the DIA and MED questions completeLevel is made active and everything is diabled)
    public void CalculationOfDiaAndMed(){
        axiumUIPanel.SetActive(false);
        diagnosisButton.SetActive(false);
        medicineButton.SetActive(false);
        backButtonFromAxium.SetActive(false);
        axiumButton.SetActive(false);
        diagnosisAns.SetActive(false);
        medicineAns.SetActive(false);
        backButtonFromDiaAndMed.SetActive(false);
        diagnosisButton.SetActive(false);
        medicineButton.SetActive(false);
        npc.SetActive(false);
        completeLevel.SetActive(true);

        scoreText.text = score.ToString("F2") + "%";
        Debug.Log("In LBM: "+ GetLevelNumber());

        //Send the information the Firebase script to send the player score as a percent to specific level completed and make sure 2 decimal places
        //FB.UpdateCharacterField("playerExperience", score.ToString("F2"));

        Debug.Log("Correct answer!");
        //Not really needed but reset everything
        attemptsMed = 0;//Reset for next button set
        attemptsDia = 0;
        isDiaCorrect = false;
        isMedCorrect = false;
    }

    //If user clicks on Noxium in scene moves camera to display computer prefab with screen properly and make sure everything is disabled
    public void OnClickOfAxium(){
        GV.setCheckedAxium(true);
        cbh.canFLip = true;
        axiumUIPanel.SetActive(true);
        backButtonFromAxium.SetActive(true); 
        diagnosisButton.SetActive(false);
        medicineButton.SetActive(false);
        axiumButton.SetActive(false);
        axiumButton.GetComponent<HoveringText>().hoverText.SetActive(false);
        //Move camera to this position, no rotation
        Camera.main.transform.position = new Vector3(515f, 267f, -435f);
        Camera.main.transform.rotation = Quaternion.Euler(0f, 0f, 0f); 
    }

    //If user clicks on back button in the axium display moves camera back to level scene and actives items in scene
    public void BackButtonInAxium(){
        backButtonFromAxium.SetActive(false); 
        axiumUIPanel.SetActive(false); 
        diagnosisButton.SetActive(GV.getAlreadyTalkedTo());//Need to talk to NPC first before active
        medicineButton.SetActive(HandleMedShown());
        axiumButton.SetActive(true);
        //Move camera back to level scene
        Camera.main.transform.position = new Vector3(0f, 3f, -11f);
        Camera.main.transform.rotation = Quaternion.Euler(5f, 0f, 0f); 
    }

    //The back button for the DIA and MED buttons on the right hand corner of the screen
    public void OnClickGoBackDiaAndMed(){
        //medicineAns.SetActive(false);
        diagnosisAns.SetActive(false);
        backButtonFromDiaAndMed.SetActive(false);
        diagnosisButton.SetActive(true);
        npcHoverDetector.SetActive(true);
        MainCamera.SetActive(true);
        MedCamera.SetActive(false);
        medicineButton.SetActive(HandleMedShown());

        //Outputs the final grade screen if both are anwsered correct
        if(isDiaCorrect == true && isMedCorrect == true){
            CalculationOfDiaAndMed();
        }

    }

    //Set other object to inactive while on this UI
    public void OnClickOfDiagnosis(){
        diagnosisButton.SetActive(false);
        medicineButton.SetActive(false);
        diagnosisAns.SetActive(true);
        backButtonFromDiaAndMed.SetActive(true);
        npcHoverDetector.SetActive(false);
    }

    //Set other object to inactive while on this UI
    public void OnClickOfMedicine(){
        diagnosisButton.SetActive(false);
        medicineButton.SetActive(false);
        axiumButton.SetActive(false);
        //medicineAns.SetActive(true);
        MainCamera.SetActive(false);
        MedCamera.SetActive(true);
        backButtonFromDiaAndMed.SetActive(true);
        npcHoverDetector.SetActive(false);
    }

    //Will display Med if isDiaCorrect meaning the user anwsered the question correctly and now shows medicine
    public bool HandleMedShown(){
        if(isDiaCorrect == false){
            return false;
        }
        else{
            return true;
        }
    }
}
