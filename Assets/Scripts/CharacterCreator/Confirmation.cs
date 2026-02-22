using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Confirmation : MonoBehaviour
{
    public UpdateCharacterID UCID;
    //public FireBase firebase;
    public GlobalVariables gv;
    public Button submitButton;
    public Button backButton;
    //confirmation buttons
    public Button confirmButton;
    public Button returnButton;
    //canvas's
    public Canvas menu;
    public Canvas confMenu;

    void Start()
    {
        menu.enabled = true;
        confMenu.enabled = false;
        submitButton.onClick.AddListener(submitButtonOnClick);
        backButton.onClick.AddListener(backButtonOnClick);

    }

    public void submitButtonOnClick()
    {
        Debug.Log("" + gv.getCharacterID());

        menu.enabled = false;
        confMenu.enabled = true;
        confirmButton.onClick.AddListener(confirmButtonOnClick);
        returnButton.onClick.AddListener(returnButtonOnClick);
        
        int characterID = gv.getCharacterID();   //Gets new character ID
        Debug.Log("Character ID: " + characterID);

        //firebase.UpdateCharacterField("playerCustomization", characterID);

        UCID.CallUpdateCharID(KeepPlayerName.Instance.GetCharacterName(), characterID);

    }
    void backButtonOnClick()
    {
        confMenu.enabled = true;
        menu.enabled = false;
    }
    void confirmButtonOnClick()
    {

        
    }

    void returnButtonOnClick()
    {
        menu.enabled = true;
        confMenu.enabled = false;
    }

}
