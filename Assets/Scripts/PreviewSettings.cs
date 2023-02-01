using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;



public class LevelSettings
{
    public int levelnumber;
    public float collectablesCollected;

    public float fastestTime;
    public float rotationSpeed = 10f;

}

public class PreviewSettings : MonoBehaviour
{

    [SerializeField]
    private int levelnumber;

    public static List<LevelSettings> levelSettings;

    public static string jsonFilePath;

    public LevelSettings settings;
    public float totalCollectables;

    [SerializeField]
    public string sceneToLoad;

    private void Awake()
    {

        jsonFilePath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "LevelSettings.json";
        Debug.Log(jsonFilePath);
        if (PreviewSettings.levelSettings == null)
        {
            StreamReader reader = new StreamReader(jsonFilePath);
            levelSettings = JsonConvert.DeserializeObject<List<LevelSettings>>(reader.ReadToEnd());
            reader.Close();
        }
        if (PreviewSettings.levelSettings == null)
        {
            levelSettings = new List<LevelSettings>();
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

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.RotateAround(transform.position, Vector3.up, Time.deltaTime * settings.rotationSpeed);
    }

    public string GetScene()
    {
        return sceneToLoad;
    }

    private void OnDisable()
    {

    }
}
