using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Logger : MonoBehaviour
{
    private const string LOG_FOLDER = "Assets/Logs/";
    public string currentVersionFolder = "01-FirstCreatures";
    private string currentDirectory = "";

    public CreatureManager creatureManager;
    public FoodGenerator foodGenerator;

    void Start()
    {
        currentDirectory = LOG_FOLDER + currentVersionFolder;
        if (!Directory.Exists(currentDirectory))
        {
            Directory.CreateDirectory(currentDirectory);
            Debug.Log("Created log folder with path: " + currentDirectory);
            
        }

        currentDirectory += "/";
        uint logNum = 1;
        string newLogFolder = "run" + logNum;
        while (Directory.Exists(currentDirectory + newLogFolder))
        {
            logNum++;
            newLogFolder = "run" + logNum;
        }
        currentDirectory += newLogFolder;

        Directory.CreateDirectory(currentDirectory);
        Debug.Log("Created new log for current execution: " + newLogFolder);
    }

    void Update()
    {
        
    }
}
