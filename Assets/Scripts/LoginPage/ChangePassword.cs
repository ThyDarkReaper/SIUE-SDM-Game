// //This script is used to handle changing the players passwords in the ChangePassword scene.
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using TMPro;

// using FirebaseWebGL.Examples.Utils;
// using FirebaseWebGL.Scripts.FirebaseBridge;
// using FirebaseWebGL.Scripts.Objects;

// public class ChangePassword : MonoBehaviour
// {
//     //Attached in Unity Inspector
//     public TMP_InputField passwordField;
//     public TMP_InputField confirmPasswordField;
//     public TextMeshProUGUI feedbackText;

//     //HANDLERS UPDATING PASSWORD
//     public void UpdatePassword()
//     {
//         //Trims the new password and verify password
//         string newPassword = passwordField.text.Trim();
//         string confirmPassword = confirmPasswordField.text.Trim();

//         //Cannot submit when empty
//         if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
//         {
//             ShowFeedback("Password fields cannot be empty.", Color.red);
//             return;
//         }

//         //Both passwords need to be the same, to account for miss type
//         if (newPassword != confirmPassword)
//         {
//             ShowFeedback("Passwords do not match.", Color.red);
//             return;
//         }

//         //Make sure user does not use same default password (HAS TO CHANGE)
//         if (newPassword == "test123")
//         {
//             ShowFeedback("Please make a new password", Color.red);
//             return;
//         }

//         //Gets the players user name and splits it since thats how users are stored in firebase
//         string userEmail = KeepPlayerName.Instance.GetCharacterName();
//         string username = userEmail.Split('@')[0];

//         //Calls the struct in the Admin panel to grab the correct format and pass the users user name and new password
//         AdminPanel.UserData updatedUser = new AdminPanel.UserData(userEmail, newPassword);
//         string jsonData = JsonUtility.ToJson(updatedUser);

//         //Update the user with their user name and json file with new password
//         FirebaseFirestore.UpdateDocument("users", username , jsonData, gameObject.name, "DisplayData", "DisplayErrorObject");

//         ShowFeedback("Updating password", Color.yellow);
//     }

//     //Displays when the password is updated successfully
//     public void DisplayData(string result)
//     {
//         //Long feedback to notify player that they have to contact prof to change password again (Admin Panel)
//         ShowFeedback("Password updated successfully! Please make sure to store your new password in a safe place. If you forget it, you will need to contact your professors to request a password reset.", Color.green);
//         //Debug.Log("Password updated: " + result);
//         gameObject.SetActive(false);
//     }

//     //If error
//     public void DisplayErrorObject(string error)
//     {
//         var parsedError = StringSerializationAPI.Deserialize(typeof(FirebaseError), error) as FirebaseError;
//         ShowFeedback("Error: " + parsedError.message, Color.red);
//         Debug.LogError(parsedError.message);
//     }

//     //Show the feedback message in the passed in color
//     private void ShowFeedback(string message, Color color)
//     {
//         feedbackText.text = message;
//         feedbackText.color = color;
//     }

//     //Back button on Change Password Scene
//     public void BackButton(){
//         SceneManager.LoadScene("Login");
//     }
// }
