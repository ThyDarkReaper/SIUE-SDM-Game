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
        if (username == "admin@siue.edu" && password == "admin123")
        {
            SceneManager.LoadScene("AdminLogin");
        }
        if (password == "test123")
        {
            ChangePassword.SetUsername(username);
            SceneManager.LoadScene("ChangePassword");
        }
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);
        UnityWebRequest www = UnityWebRequest.Post("https://localhost/login./php", form);
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
            Debug.Log("Response Text: " + responseText); // Log the raw response for debugging

            // Attempt to parse the JSON response
            try
            {
                var response = JsonUtility.FromJson<LoginResponse>(responseText);
                if (response.success)
                {
                    Debug.Log("Login successful for user: " + username);
                    SceneManager.LoadScene("MainMenu");
                }
                else
                {
                    DisplayError(response.message);
                    Debug.LogError("Login failed: " + response.message);
                }
            }
            catch (System.Exception e)
            {
                DisplayError("Failed to parse server response.");
                Debug.LogError("JSON Parsing Error: " + e.Message);
            }
        }
    }

    public void DisplayError(string error)
    {
        errorText.text = error;
        Debug.LogError(error);
    }

    
}