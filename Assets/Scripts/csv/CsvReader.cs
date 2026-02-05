using System.Collections.Generic;
using UnityEngine;
using UniCSV; // Make sure to include the CSV library namespace

public class CsvReader : MonoBehaviour
{
    [SerializeField] private TextAsset csvFile; // drag your .csv file here in Inspector

    public CsvReader(TextAsset textAsset)
    {
        csvFile = textAsset;
    }

    public void onClick()
    {
        readingCSV();
    }

    public void readingCSV()
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
}
