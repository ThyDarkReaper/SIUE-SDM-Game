using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangePassword : MonoBehavior
{
    private TMP_InputField newPasswordInput;
    private TMP_InputField confirmPasswordInput;
    private TextMeshProUGUI errorText;

    void start()
    {
        newPasswordInput = GameObject.Find("NewPasswordInputField").GetComponent<TMP_InputField>();
        confirmPasswordInput = GameObject.Find("ConfirmPasswordInputField").GetComponent<TMP_InputField>();
        errorText = GameObject.Find("ErrorText").GetComponent<TextMeshProUGUI>();

        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            DisplayError("The code is not running on a WebGL build; as such, the Javascript functions will not be recognized.");
        }
    }

    private void DisplayError(string message)
    {
        errorText.text = message;
        errorText.gameObject.SetActive(true);
    }

    public void SubmitNewPassword()
    {
        string newPassword = newPasswordInput.text;
        string confirmPassword = confirmPasswordInput.text;

        if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
        {
            DisplayError("New Password and Confirm Password cannot be empty.");
            return;
        }

        if (newPassword != confirmPassword)
        {
            DisplayError("Passwords do not match.");
            return;
        }



        Debug.Log("Password changed successfully!");
        SceneManager.LoadScene("LoginScene"); // Redirect to login scene after password change
    }
}