using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Net;
using UnityEngine.Networking;

public class ChangePassword : MonoBehaviour
{
    public TMP_InputField newPasswordInput;
    public TMP_InputField confirmPasswordInput;
    public TextMeshProUGUI errorText;
    private static string username;

    private void DisplayError(string message)
    {
        errorText.text = message;
        errorText.gameObject.SetActive(true);
    }
    public static void SetUsername(string user)
    {
        username = user;
        PlayerPrefs.SetString("username", username); // Store username for later use
    }

    public void CallSubmitNewPassword()
    {
        StartCoroutine(SubmitNewPassword());
    }

    IEnumerator SubmitNewPassword()
    {
        // Validate inputs first and stop if validation fails
        if (!VerifyInputs())
        {
            yield break;
        }

        Debug.Log("Submitting new password for user: " + PlayerPrefs.GetString("username"));
        WWWForm form = new WWWForm();
        form.AddField("username", PlayerPrefs.GetString("username"));
        form.AddField("newPassword", newPasswordInput.text);

        // Most likely URL based on your folder structure
        string url = "http://localhost/changePassword.php";
        Debug.Log("Attempting to connect to: " + url);
        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();
        
        if (www.result != UnityWebRequest.Result.Success)
        {
            DisplayError("Network Error: " + www.error);
            Debug.LogError("Network Error: " + www.error);
        }
        else
        {
            // Parse JSON response
            string responseText = www.downloadHandler.text;
            Debug.Log("Server Response: " + responseText);
            
            try
            {
                // Simple JSON parsing
                if (responseText.Contains("\"success\":true"))
                {
                    Debug.Log("Password changed successfully!");
                    SceneManager.LoadScene("Login");
                }
                else if (responseText.Contains("\"success\":false"))
                {
                    // Extract error message from JSON
                    string errorMessage = "Failed to change password";
                    int messageStart = responseText.IndexOf("\"message\":\"") + 11;
                    if (messageStart > 10)
                    {
                        int messageEnd = responseText.IndexOf("\"", messageStart);
                        if (messageEnd > messageStart)
                        {
                            errorMessage = responseText.Substring(messageStart, messageEnd - messageStart);
                        }
                    }
                    DisplayError(errorMessage);
                }
                else
                {
                    DisplayError("Unexpected server response: " + responseText);
                }
            }
            catch (System.Exception e)
            {
                DisplayError("Error parsing response: " + e.Message);
                Debug.LogError("JSON Parse Error: " + e.Message + " Response: " + responseText);
            }
        }
    }

    public bool VerifyInputs()
    {
        if (string.IsNullOrEmpty(newPasswordInput.text) || string.IsNullOrEmpty(confirmPasswordInput.text))
        {
            DisplayError("New Password and Confirm Password cannot be empty.");
            return false;
        }

        if (newPasswordInput.text != confirmPasswordInput.text)
        {
            DisplayError("Passwords do not match.");
            return false;
        }
        
        return true;
    }

    public void ReturnToLogin()
    {
        SceneManager.LoadScene("Login");
    }
}