using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class csvreadin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "data.csv");
        string fileContent = File.ReadAllText(path);

        List<string[]> csvData = new List<string[]>();
        foreach (string row in fileContent.Split('\n'))
        {
            csvData.Add(row.Split(','));
        }

        Debug.Log("Loaded " + csvData.Count + " rows from CSV.");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
