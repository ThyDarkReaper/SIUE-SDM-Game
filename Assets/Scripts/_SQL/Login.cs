using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Login : MonoBehaviour
{
    [System.Serializable]
    private class CharacterIdResponse
    {
        public bool success;
        public int charID;
        public string message;
    }

    public GlobalVariables GV;
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI errorText;

    void Start()
    {
        GV = GameObject.Find("GlobalVariables").GetComponent<GlobalVariables>();
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

        // Check if user is an admin
        if (username == "admin@siue.edu" && password == "admin123")
        {
            PlayerPrefs.SetString("username", username);
            SceneManager.LoadScene("AdminLogin");
            yield break; // Stop execution here
        }
        // Check if password needs to be changed
        if (password == "test123")
        {
            ChangePassword.SetUsername(username);
            SceneManager.LoadScene("ChangePassword");
            yield break; // Stop execution here
        }
        
        // Use raw URL-encoded POST data so the request format matches the other working backend calls.
        string postData = $"username={UnityWebRequest.EscapeURL(username)}&password={UnityWebRequest.EscapeURL(password)}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(postData);

        string url = "https://103-89-14-188.cloud-xip.com/login.php";
        Debug.Log("Attempting to connect to: " + url);
        
        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
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

                bool loadStudentCharacter = false;
                bool goToAdminPanel = false;
                
                try
                {
                    // User login success
                    if (responseText.Contains("\"success\":true"))
                    {
                        Debug.Log("User logged in successfully!");
                        PlayerPrefs.SetString("username", username);

                        KeepPlayerName.Instance.SetCharacterName(username);
                        if (responseText.Contains("\"type\":\"admin\"")) // Check if user logged in as an admin
                        {
                            Debug.Log("Admin user detected.");
                            goToAdminPanel = true;
                        }
                        else // Student user logged in
                        {
                            Debug.Log("Regular user detected.");
                            loadStudentCharacter = true;
                        }
                    }
                    // User login failed
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
                    // Server error
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

                if (goToAdminPanel)
                {
                    SceneManager.LoadScene("AdminPanel");
                    yield break;
                }

                if (loadStudentCharacter)
                {
                    yield return loadCharacterID(username);
                    SceneManager.LoadScene("WelcomeScene");
                    yield break;
                }
            }
        }
    }

    IEnumerator loadCharacterID(string username)
    {
        string postData = $"username={UnityWebRequest.EscapeURL(username)}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(postData);

        string url = "https://103-89-14-188.cloud-xip.com/loadCharacterID.php";
        Debug.Log("Attempting to connect to: " + url);

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
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
                    CharacterIdResponse response = JsonUtility.FromJson<CharacterIdResponse>(responseText);
                    if (response != null && response.success)
                    {
                        GV.setCharacterID(response.charID);
                        Debug.Log("Character ID loaded successfully: " + response.charID);
                    }
                    else
                    {
                        string message = response != null && !string.IsNullOrEmpty(response.message)
                            ? response.message
                            : "Character ID not found in response.";
                        DisplayError(message);
                        Debug.LogError("Character ID load failed. Response: " + responseText);
                    }
                }
                catch (System.Exception e)
                {
                    DisplayError("Error parsing character ID: " + e.Message);
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