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
        
        string username = PlayerPrefs.GetString("username");
        string newPassword = newPasswordInput.text;
        
        Debug.Log($"Sending - Username: '{username}', Password Length: {newPassword.Length}");
        
        // Try using string-based POST data instead of WWWForm
        string postData = $"username={UnityWebRequest.EscapeURL(username)}&newPassword={UnityWebRequest.EscapeURL(newPassword)}";
        Debug.Log($"POST Data: {postData}");
        
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(postData);

        string url = "http://103-89-14-161.cloud-xip.com/changePassword_test.php";
        Debug.Log("Attempting to connect to: " + url);
        
        UnityWebRequest www = new UnityWebRequest(url, "POST");
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        www.timeout = 30;
        yield return www.SendWebRequest();
        
        if (www.result != UnityWebRequest.Result.Success)
        {
            string errorDetails = $"Status Code: {www.responseCode}, Error: {www.error}";
            if (www.downloadHandler != null && !string.IsNullOrEmpty(www.downloadHandler.text))
            {
                errorDetails += $", Response: {www.downloadHandler.text}";
            }
            DisplayError("Network Error: " + errorDetails);
            Debug.LogError("Network Error: " + errorDetails);
        }
        else
        {
            string responseText = www.downloadHandler.text;
            Debug.Log("Server Response: " + responseText);
            
            try
            {
                // Change password successful
                if (responseText.Contains("\"success\":true"))
                {
                    Debug.Log("Password changed successfully!");
                    SceneManager.LoadScene("Login");
                }
                // Change password failed
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