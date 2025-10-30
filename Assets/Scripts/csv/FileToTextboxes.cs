using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class FileToTextboxes : MonoBehaviour
{
    public TextMeshProUGUI[] textboxes; // Assign your textboxes in the Inspector

    public string filePath = "Assets/Resources/data.txt"; // Example file path

    void Start()
    {
        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            for (int i = 0; i < textboxes.Length && i < lines.Length; i++)
            {
                textboxes[i].text = lines[i];
            }
        }
        else
        {
            Debug.LogError("File not found at: " + filePath);
        }
    }
}
