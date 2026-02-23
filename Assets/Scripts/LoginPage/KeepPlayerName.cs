//This script is called in login and once the user logins it grabs the username of the player and keeps it alive across the game. 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepPlayerName : MonoBehaviour
{
    public static KeepPlayerName Instance;
    public string playerName;

    [SerializeField]
    private int volume = 5;
   
    void Awake()
    {
        if(Instance == null){
            Instance = this;
            //volume = 5;
            DontDestroyOnLoad(gameObject);
            Debug.Log("Not destroying");
        }
        else{
            Destroy(gameObject);
            Debug.Log("Destroyed for some reason");
        }
    }

    //Sets the player name
    public void SetCharacterName(string name){
        playerName = name;
        Debug.Log("In Keep playername its set to: " + playerName);
    }

    //Able to grab the character name (used in other scripts)
    public string GetCharacterName(){
        Debug.Log("In Keep playername get character name its set to: " + playerName);
        return playerName;
    }

    //Functions used for audio
    public void SetVolume(int v){
        volume = v;
        //Debug.Log("In Keep playername its set to: " + playerName);
    }

    public int GetVolume(){
        //Debug.Log("In Keep playername get character name its set to: " + playerName);
        return volume;
    }
}
