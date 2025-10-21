using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Login : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI errorText;

    void Start()
    {
        usernameInput = GameObject.Find("Username").GetComponent<TMP_InputField>();
        passwordInput = GameObject.Find("Password").GetComponent<TMP_InputField>();
        errorText = GameObject.Find("Error").GetComponent<TextMeshProUGUI>();
    }

    public void CallSubmitLogin()
    {
        StartCoroutine(SubmitLogin());
    }

    IEnumerator SubmitLogin()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            DisplayError("Username and Password cannot be empty.");
            yield break;
        }
        
        // Check special cases first and stop execution if matched
        if (username == "admin@siue.edu" && password == "admin123")
        {
            PlayerPrefs.SetString("username", username);
            SceneManager.LoadScene("AdminLogin");
            yield break; // Stop execution here
        }
        if (password == "test123")
        {
            ChangePassword.SetUsername(username);
            SceneManager.LoadScene("ChangePassword");
            yield break; // Stop execution here
        }
        
        // Only proceed with web request if not a special case
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        string url = "http://localhost/login.php";
        Debug.Log("Attempting to connect to: " + url);
        
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
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
                        Debug.Log("User logged in successfully!");
                        PlayerPrefs.SetString("username", username);
                        SceneManager.LoadScene("WelcomeScene");
                    }
                    else if (responseText.Contains("\"success\":false"))
                    {
                        // Extract error message from JSON
                        string errorMessage = "Login failed";
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
    }

    public void DisplayError(string error)
    {
        errorText.text = error;
        Debug.LogError(error);
    }

    
}