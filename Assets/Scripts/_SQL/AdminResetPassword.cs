using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

class AdminResetPassword : MonoBehaviour
{
    public TMP_InputField emailField;
    public TextMeshProUGUI feedbackText;
    private const string defaultPassword = "test123";

    public void CallResetPassword()
    {
        StartCoroutine(ResetPassword());
    }

    IEnumerator ResetPassword()
    {
        string email = emailField.text;

        if (string.IsNullOrEmpty(email))
        {
            feedbackText.text = "Email cannot be empty.";
            feedbackText.color = Color.yellow;
            yield break;
        }

        if (!IsValidSIUEEmail(email))
        {
            feedbackText.text = "Please enter a valid SIUE email address.";
            feedbackText.color = Color.yellow;
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("username", email);
        form.AddField("newPassword", defaultPassword);
        
        string url = "https://103-89-14-188.cloud-xip.com/changePassword.php";

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                feedbackText.text = "Error resetting password: " + www.error;
                feedbackText.color = Color.red;
            }
            else
            {
                feedbackText.text = "Password reset successful.";
                feedbackText.color = Color.green;
            }
        }
    }

    private bool IsValidSIUEEmail(string email)
    {
        return email.EndsWith("@siue.edu");
    }
}