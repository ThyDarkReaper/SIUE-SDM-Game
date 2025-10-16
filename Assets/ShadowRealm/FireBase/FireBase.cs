//For this script it handles the Firebase connection of grabing and sending and is used by other scripts to send the information.
//There are a lot of Debug.Logs you may comment them back in to see when they are beginng called. Mainly was used for error checking and to see what was going on after a build.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FirebaseWebGL.Examples.Utils;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;
using System;


public class FireBase : MonoBehaviour
{
    public PlayerData currentPlayerData;
    //public KeepPlayerPOS pos;
    public LevelButtonManager LBM;

    [System.Serializable] 
    //STRUCT FOR PLAYERDATA THAT IS USED FOR EACH PLAYER
    public struct PlayerData
    {
        public string playerName;
        public float[] playerExperience;//An array to store the percents based on the Level (num in array == level num)
        public int playerCustomization; //Store character customization from character creator
    }

    void Start()
    {
        //If not a WEBGL build do not connect to Firebase
        if (Application.platform != RuntimePlatform.WebGLPlayer){
            Debug.Log("The code is not running on a WebGL build; as such, the Javascript functions will not be recognized.");
            FireBase tmp = GetComponent<FireBase>();
            tmp.enabled = false;
            return;
        }
        
        //Checking for Gameobject and Character name to verify 
        //Debug.Log("GameObject name: " + gameObject.name);
        //Debug.Log("Character name: " + KeepPlayerName.Instance.GetCharacterName());
        //Gets the players name that is stored in KeepPlayerName script at the login page and does not destroy
        GetPlayerData(KeepPlayerName.Instance.GetCharacterName());
    }

    //Gets the player data from Firebase
    public void GetPlayerData(string character)
    {
        //Debug.Log("I am in the GetPlayerData function" + character);
        FirebaseFirestore.GetDocument("players", character, "Character", "DisplayData", "DisplayErrorObject");
    }

    //Handles the player data by updating the local playerdata to what was sent to Firebase to keep correct tracking of player data locally and on firebase
    //This avoids making to many calls to Firebase to grab information!
    public void HandlePlayerData(PlayerData playerData)
    {
        //Debug.Log("Name: " + playerData.playerName);
        //Debug.Log("Player Level: " + playerData.playerLevel);
        //Debug.Log("Player Exp: " + playerData.playerExperience);
        PlayerSaveData.Instance.SetPlayerData(playerData);//Saves the PlayerData to a Gameobject and does not delete it
    }

    //IMPORTANT FUNCTION TO CALL TO SEND DATA TO FIREBASE
    //Call this function in another script and pass in the fieldName (what you are updating) and the value of that object
    public void UpdateCharacterField(string fieldName, object value)
    {
        //Debug.Log("In FB: "+ LBM.GetLevelNumber());

        //Gets the current player data from the PlayerSaveData script that has the players current information grabbed on login and does not destroy
        PlayerData currentPlayerData = PlayerSaveData.Instance.GetPlayerData();

        //Debug.Log("Name: " + currentPlayerData.playerName);
        //Debug.Log("Player Level: " + currentPlayerData.playerLevel);
        //Debug.Log("Player Exp: " + currentPlayerData.playerExperience);
        
        //Now proceed with updating the data
        Debug.Log("Now updating character info");
        switch (fieldName)
        {
            case "playerName":
                currentPlayerData.playerName = (string)value;
                break;
            case "playerExperience":
                //The array of the Level number has to be equal to zero to show it has never been played if it has been played will not be written to firebase
                if (currentPlayerData.playerExperience[LBM.GetLevelNumber()] == 0f)
                {
                    currentPlayerData.playerExperience[LBM.GetLevelNumber()] = float.Parse((string)value);  //Only overwrite if the experience is 0 (first entry of progression)
                }
                break;
            case "playerCustomization":
                currentPlayerData.playerCustomization = (int)value;
                break;
        }

        //Convert the updated data to JSON and send to Firebase
        string jsonUpdate = JsonUtility.ToJson(currentPlayerData);
        Debug.Log("Json Update: " + jsonUpdate);
        FirebaseFirestore.UpdateDocument("players", KeepPlayerName.Instance.GetCharacterName(), jsonUpdate, "Character", "DisplayData", "DisplayErrorObject");

        //Reset the flag for next data update
        //isPlayerDataHandled = false;
    }

    //Called in the admin script to set up the character document to create new users!
    public void SetCharacterDocument(string collectionPath, string documentId, string jsonData)
    {
        //Debug.Log("Im in this character function");
        FirebaseFirestore.SetDocument(collectionPath, documentId, jsonData, "Character", "DisplayData", "DisplayErrorObject");
    }

    //Called when a FirebaseFirestore function is called
    public void DisplayData(string data)
    {
        if (!string.IsNullOrEmpty(data) && data != "null")
        {
            //Debug.Log("Raw Data: " + data);

            //The playerData is stored as a json file with the new passed data
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(data);
            HandlePlayerData(playerData);//Sends to Handle player data
            Debug.Log("Player Data Retrieved: " + data);
        }
        else{
            Debug.LogError("No data received or data is null.");
        }
    }

    public void DisplayErrorObject(string error)
    {
        Debug.LogError("Error occurred: " + error);
    }
}




