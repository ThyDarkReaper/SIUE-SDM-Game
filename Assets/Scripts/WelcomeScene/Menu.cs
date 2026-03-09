using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    
    public void OnPlayButton(){
        SceneManager.LoadScene("LevelSelectorMainFloor");
    }
    public void OnSettingsButton(){
        SceneManager.LoadScene("SettingsScene");//CHANGE BACK TO SETTINGS!
    }
    
    public void OnCharacterButton(){
        SceneManager.LoadScene("CharacterCreator");
    }

    
}