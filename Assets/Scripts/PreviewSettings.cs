using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;



public class LevelSettings
{
    public int levelnumber;
    public float collectablesCollected = 0;

    public float fastestTime = -1;

}

public class PreviewSettings : MonoBehaviour
{

    [SerializeField]
    private int levelnumber;

    public static List<LevelSettings> levelSettings;

    public static string jsonFilePath;

    public LevelSettings settings;
    public float totalCollectables;

    public string sceneToLoad;

    private void Awake()
    {

        jsonFilePath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "LevelSettings.json";
        Debug.Log(jsonFilePath);
        if (PreviewSettings.levelSettings == null)
        {
            try
            {
                StreamReader reader = new StreamReader(jsonFilePath);
                levelSettings = JsonConvert.DeserializeObject<List<LevelSettings>>(reader.ReadToEnd());
                reader.Close();
            }
            catch (FileNotFoundException)
            {
                levelSettings = new List<LevelSettings>();
            }
        }

        foreach (var x in levelSettings)
            if (x.levelnumber == levelnumber)
                settings = x;
        if (settings == null)
        {
            settings = new LevelSettings();
            settings.collectablesCollected = 0;
            settings.fastestTime = -1;
            settings.levelnumber = levelSettings.Count;
            levelSettings.Add(settings);
        }
    }

    public string GetScene()
    {
        return sceneToLoad;
    }

    private void OnDisable()
    {

    }
}
