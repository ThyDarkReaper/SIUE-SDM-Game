using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    //Character ID: 
    //first digit = sexID: 1=male 2=female
    //second digit = hairID: 0=none, 1=short, 2=ponytail
    //third digit = hairColorID: 0=black, 1=blonde, 2=brown, 3=orange, 4=red
    //fourth digit = skinID
    //fifth digit = clothesColorID: 0=cyan, 1=black, 2=grey, 3=blue, 4=orange, 5=purple, 6=pink,7=red, 8=white, 9=yellow.
    //sixth digit = labcoat bool: 0=no 1=yes
    //seventh digit = glasses bool: 0=no 1=yes
    int volume = 5;
    int characterID = 1110200;
    bool talking = false;
    bool paused = false;
    bool talkSignal = false;
    string objectTalking = "";
    TextAsset talkingScript;
    public bool alreadyTalkedTo = false;

    bool checkedAxium = false;
    int buttonValue = 0;
    bool playTalkingSound = false;
    //this is for talking audio
    void OnDestroy()
    {
        KeepPlayerName.Instance.SetVolume(volume);
    }
    void Awake()
    {
        //KeepPlayerName kpn = GetComponent<KeepPlayerName>();
        volume = KeepPlayerName.Instance.GetVolume();
    }
    //Signal for the talking audio
    public void setTalkingSound(bool sound)
    {
        playTalkingSound = sound;
    }
    public bool getTalkingSound()
    {
        return playTalkingSound;
    }


    public void setCheckedAxium(bool val)
    {
        checkedAxium = val;
    }
    public bool getCheckedAxium()
    {
        return checkedAxium;
    }


    //this is for mouse hover over buttons
    public void setButtonValue(int button)
    {
        buttonValue = button;
    }
    public int getButtonValue()
    {
        return buttonValue;
    }
    public void setAudioVolume(int vol)
    {
        volume = vol;
    }
    public int getAudioVolume()
    {
        return volume;
    }
    public void setAlreadyTalkedTo(bool talked)
    {
        alreadyTalkedTo = talked;
    }
    public bool getAlreadyTalkedTo()
    {
        return alreadyTalkedTo;
    }
    public void swapTalkSignal()
    {
        if(talkSignal)
        {
            talkSignal = false;
        }else{
            talkSignal = true;
        }
    }
    public bool getTalkSignal()
    {
        return talkSignal;
    }
    public void clearScript()
    {
        talkingScript = null;
    }
    public TextAsset getScript()
    {
        return talkingScript;
    }
    public void setScript(TextAsset scr)
    {
        talkingScript = scr;
    }
    public int getCharacterID()
    {
        return characterID;
    }
    public void setCharacterID(int newID)
    {
        characterID = newID;
    }
    public bool isTalking()
    {
        if(talking)
        {
            return true;
        }else{
            return false;
        }
    }
    public void swapTalking()
    {
        if(talking)
        {
            talking=false;
        }else{
            talking=true;
        }
    }
    public bool isPaused()
    {
        if(paused)
        {
            return true;
        }else{
            return false;
        }
    }
    public void pause()
    {
        if(paused)
        {
            paused = false;
        }else{
            paused = true;
        }
    }
    public void setTalkingCharacter(string talkID)
    {
        objectTalking = talkID;
    }
    public void clearTalkingCharacter()
    {
        objectTalking = "";
    }
    public string getTalkingCharacter()
    {
        return objectTalking;
    }


}
