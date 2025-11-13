using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class FileToTextboxes : MonoBehaviour
{
    public TextMeshProUGUI[] textboxes; // Assign your textboxes in the Inspector
    public Button next;
    public string filePath = "Assets/Resources/data.txt"; // Example file path
    private string[] lines;            // Stores all lines read from file
    private int currentIndex = 0;      // Tracks current page start index
    private int pageSize;              // Number of lines per "page"

    void Start()
    {

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            next.onClick.AddListener(OnButtonClick(lines,currentIndex));
        }
        else
        {
            Debug.LogError("File not found at: " + filePath);
        }
        
        
    }

    public void OnButtonClick(string[] lines, int currentIndex)
    {
        
        for (int i = currentIndex; i < 5 && i < lines.Length; i++)
        {
            textboxes[i%5].text = lines[i];
            currentIndex = i;
        }
    }
        
}
