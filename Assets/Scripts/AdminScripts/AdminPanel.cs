//This script handles the admin panel of adding/removing users.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine.SceneManagement;//Go back to main menu

using FirebaseWebGL.Examples.Utils;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;

public class AdminPanel : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_InputField removeField;
    public TextMeshProUGUI outputText; 
    //The deafult password for all users is test123
    private const string DefaultPassword = "test123";
    private string collectionPath = "users"; 
    private TextMeshProUGUI errorText; 
    public FireBase fireBase;

    [SerializeField]
    public Button SubmittButtonNew;

    //Struct for the username and password
    [Serializable]
    public struct UserData
    {
        public string username;
        public string password;

        public UserData(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }

    void Start()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
            DisplayError("The code is not running on a WebGL build; as such, the Javascript functions will not be recognized.");
    }

    //Attached function to add button on click in Admin Panel scene
    public void ProcessInput()
    {
        string userInput = inputField.text;
        //Split the input by semicolons into individual users
        string[] users = userInput.Split(';');
        List<UserData> validUsers = new List<UserData>();  //To store valid users

        foreach (string user in users)
        {
            string trimmedUser = user.Trim();  //Trim spaces from the user input
            //Debug.Log("User: " + trimmedUser);
            //Validate SIUE email format
            if (!IsValidSIUEEmail(trimmedUser))
            {
                Debug.LogError($"Invalid email: {trimmedUser}. Must be an SIUE email (@siue.edu).");
                continue;  //Skip invalid emails
            }

            //Goes through each user if the user does not have an @siue.edu email it will skip and continue through the loop
            string username = trimmedUser.Split('@')[0];
            UserData userData = new UserData(username + "@siue.edu", DefaultPassword); //User data
            validUsers.Add(userData);  //Adds users to list struct
        }

        foreach (UserData userData in validUsers)
        {
            string jsonData = JsonUtility.ToJson(userData);
            Debug.Log($"Valid SIUE User: {userData.username}");
            string username = userData.username.Replace("@siue.edu", "");
            FirebaseFirestore.SetDocument(collectionPath, username, jsonData, gameObject.name, "DisplayData", "DisplayErrorObject");
            PlayerSetUp(username);
        }

        if (validUsers.Count == 0)
        {
            Debug.LogError("No valid SIUE users were found.");
        }
    }

    //Handles the removal of the players (attached on click of remove button)
    public void ProcessRemoval(){
        string userInput = inputField.text;
        //Split the input by semicolons into individual users
        string[] users = userInput.Split(';');
        List<UserData> validUsers = new List<UserData>();  //To store valid users

        foreach (string user in users)
        {
            string trimmedUser = user.Trim();  //Trim spaces from the user input
            Debug.Log("User: " + trimmedUser);

            //Validate SIUE email format
            if (!IsValidSIUEEmail(trimmedUser))
            {
                Debug.LogError($"Invalid email: {trimmedUser}. Must be an SIUE email (@siue.edu).");
                continue;  //Skip invalid emails
            }

            //Stores valid users inputed in text field
            string username = trimmedUser.Split('@')[0];
            UserData userData = new UserData(username, DefaultPassword); //User data
            validUsers.Add(userData);  
        }

        //For each valid user goes through and deletes there user(username, password) and players(playerName,playerExp,characterCustomizationNum)
        foreach (UserData userData in validUsers)
        {
            Debug.Log($"Valid SIUE User: {userData.username}");
            //Calls firebase twice per user
            FirebaseFirestore.DeleteDocument("users", userData.username, gameObject.name,"DisplayInfo", "DisplayErrorObject");
            FirebaseFirestore.DeleteDocument("players", userData.username + "@siue.edu", gameObject.name,"DisplayInfo", "DisplayErrorObject");
        }

        if (validUsers.Count == 0)
        {
            Debug.LogError("No valid SIUE users were found.");
        }
    }


    bool IsValidSIUEEmail(string email)
    {
        string pattern = @"^[a-zA-Z0-9._%+-]+@siue\.edu$";  //Regex to validate SIUE email
        return Regex.IsMatch(email, pattern);
    }

    public void DisplayData(string data)
    {
        outputText.color = outputText.color == Color.green ? Color.blue : Color.green;
        outputText.text = data;
        Debug.Log(data);
    }

    public void DisplayErrorObject(string error)
    {
        var parsedError = StringSerializationAPI.Deserialize(typeof(FirebaseError), error) as FirebaseError;
        DisplayError(parsedError.message);
    }

    public void DisplayError(string error)
    {
        errorText.text = error;
        Debug.LogError(error);
    }

    //Set up a new player in the for loop above to add users one by one
    public void PlayerSetUp(string username)
    {
        //Hard coded level max to 20 however there is not level 0 so only level 1-19
        float[] newLevelExperience = new float[20];

        //Make sure the array is populated with a zero float
        for (int i = 0; i < newLevelExperience.Length; i++)
        {
            newLevelExperience[i] = 0.0f;
        }

        //Set the information provided and default information to the firebase struct (in Firebase script)
        FireBase.PlayerData newPlayerData = new FireBase.PlayerData
        {
            playerName = username + "@siue.edu",  //Use the user's username as the character name
            playerExperience = newLevelExperience,
            playerCustomization = 1110200
        };

        //Convert to json
        string jsonData = JsonUtility.ToJson(newPlayerData);
        //Debug.Log("Raw Data in Create Character: " + jsonData);

        //This is a function in firebase script that creates the player from scratch into the collection
        fireBase.SetCharacterDocument("players", username + "@siue.edu", jsonData);
    } 

    public void Return(){
        Application.ExternalCall("hideTextInput");
        SceneManager.LoadScene("Login");
    }

}
