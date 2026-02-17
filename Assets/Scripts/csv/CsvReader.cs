using System.Collections.Generic;
using UnityEngine;
using UniCSV; // Make sure to include the CSV library namespace
using TMPro;
using UnityEngine.UI;

public class CsvReader : MonoBehaviour
{
    public TextAsset csvFile; // drag your .csv file here in Inspector
    public Button loadingButton; // assign your button prefab in Inspector
    public TextMeshProUGUI[] textboxes;   // Assign your textboxes in Inspector
    public int currentIndex = 0;         // Tracks current starting index
    public readonly int rowIndex = 5;             // Number of lines per "page"

    public CsvReader(TextAsset textAsset)
    {
        csvFile = textAsset;
    }

    public void OnClick()
    {
        loadingButton.onClick.AddListener(OnClick);
        CSVToButtons();
    }

    public void ReadingCSV()
    {
        if (csvFile == null)
        {
            Debug.LogError("NO CSV file.");
            return;
        }

        string csvString = csvFile.text;

        // CsvParser is from the CSV library (e.g., Uni-CSV)
        var sheet = CsvParser.ParseFromString(csvString, hasHeader: true);

        foreach (var row in sheet)
        {
            Debug.Log(string.Join(", ", row));
        }
    }

    public void CSVToButtons()
    {
        if (csvFile == null)
        {
            Debug.LogError("NO CSV file.");
            return;
        }

        string csvString = csvFile.text;

        // CsvParser is from the CSV library (e.g., Uni-CSV)
        var sheet = CsvParser.ParseFromString(csvString, hasHeader: true);

        foreach (var row in sheet)
        {
            for (int i = 0; i < rowIndex; i++)
            {
                int lineIndex = currentIndex + i;
                if (lineIndex < row.Count)
                {
                    textboxes[i].text = row[lineIndex];
                }
                else
                {
                    textboxes[i].text = ""; // Blank if no data
                }
            }
        }
    }
}
