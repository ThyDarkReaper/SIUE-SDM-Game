using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.IO;
using TMPro;
using UnityEngine.UI;
using System.Text;
 

public class Importer : MonoBehaviour 
{

    public Button updateButton; // assign your button prefab in Inspector
    public string url = "http://103-89-14-161.cloud-xip.com/changePassword.php";

    public string username = "ymoto";
    public string password = "yami";
    void Start() {
        //StartCoroutine(GetInfo());
        updateButton.onClick.AddListener(UpdateInfo);
    }

    public void UpdateInfo() {
        StartCoroutine(GetInfo());
    }

     IEnumerator GetInfo() {

        Debug.Log("Starting web request...");

        UnityWebRequest source = UnityWebRequest.Get(url);
        source.timeout = 10; // Set a timeout for the request (in seconds)

        string auth = username + ":" + password;
        string authBase64 = System.Convert.ToBase64String(Encoding.ASCII.GetBytes(auth));
        source.SetRequestHeader("Authorization", "Basic " + authBase64); // [web:3][web:16][web:23]

        yield return source.SendWebRequest();
 
        if (source.result != UnityWebRequest.Result.Success) {
            Debug.Log(source.error);
            yield break; // Exit the coroutine if there's an error
        }
        else {
            // Show results as text
            Debug.Log("starting overwrite");
            //string path = Path.Combine(Application.persistentDataPath, "overwrite.txt");
            File.WriteAllText("Assets/Scripts/csv/overwrite.txt", "being recieved");

            //File.WriteAllText(path, "being recieved");
 
        }
    }
    // IEnumerator GetInfo() {
    //     UnityWebRequest source = UnityWebRequest.Get("http://103-89-14-161.cloud-xip.com");
    //     yield return source.SendWebRequest();
 
    //     if (source.result != UnityWebRequest.Result.Success) {
    //         Debug.Log(source.error);
    //     }
    //     else {
    //         // Show results as text
    //         //Debug.Log("being recieved");

    //         File.WriteAllText("Assests/Scripts/csv/overwrite.txt", "being recieved");
 
    //     }
    // }
}