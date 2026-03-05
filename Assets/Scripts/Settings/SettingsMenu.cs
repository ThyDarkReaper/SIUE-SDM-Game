using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;




public class SettingsMenu : MonoBehaviour
{
    public GameObject HelpPanel;
    public void OnPlayButton(){ // Play button returns user to level selector
        SceneManager.LoadScene("LevelSelectorMainFloor");
    }
    public void OnReturnToStartButton(){ // start button returns user to wlecome screen
        SceneManager.LoadScene("WelcomeScene");
    }
    public void OnHelpButton(){ // help button activate the help panel
        HelpPanel.SetActive(true);
    }
    public void OnLeaderboardButton(){ // opens leaderboard
        SceneManager.LoadScene("Leaderboard");
    }
    public void OnHelpBackButton(){ // allows user to rehide the help panel and see the settings screen again
        HelpPanel.SetActive(false);
    }
    public void OnExitButton(){ // just returns user to login screen, more for player's comfort
        //SceneManager.LoadScene("Login");
        Application.Quit(); //temp quits the game instead of sending to scene that does not function
    }
}
