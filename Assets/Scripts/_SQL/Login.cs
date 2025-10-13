using System.Collections;
using System.Collections.Generic;
using UnityEngiene;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    private TMP_InputField usernameInput;
    private TMP_InputField passwordInput;
    private TextMeshProUGUI errorText;

    void Start()
    {
        usernameInput = GameObject.Find("UsernameInputField").GetComponent<TMP_InputField>();
        passwordInput = GameObject.Find("PasswordInputField").GetComponent<TMP_InputField>();
        errorText = GameObject.Find("ErrorText").GetComponent<TextMeshProUGUI>();

        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            DisplayError("The code is not running on a WebGL build; as such, the Javascript functions will not be recognized.");
        }


    }

    public void SubmitLogin()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            DisplayeError("Username and Password cannot be empty.");
            return;
        }
        if (password == "test123")
        {
            SceneManager.LoadScene("ChangePassword");
            ChangePassword.Instance.SetUsername(username);
        }
    }

    
    
}