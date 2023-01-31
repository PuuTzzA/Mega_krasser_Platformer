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

    [SerializeField]
    private TextAsset textasset;

    public static TextAsset jsonFile;
    public static string jsonFilePath = "Assets/JsonFiles/levelSettings.json";

    public LevelSettings settings;
    public float totalCollectables;

    [SerializeField]
    public string sceneToLoad;

    private void Awake()
    {
        if (PreviewSettings.jsonFile == null)
            PreviewSettings.jsonFile = textasset;
        List<LevelSettings> l = JsonConvert.DeserializeObject<List<LevelSettings>>(jsonFile.text);
        if (l == null)
        {
            l = new List<LevelSettings>();
        }
        try
        {
            foreach(var x in l)
            if(x.levelnumber == levelnumber)
                settings = x;
        }
        catch (ArgumentOutOfRangeException)
        {
            settings = new LevelSettings();
            settings.collectablesCollected = 0;
            settings.fastestTime = -1;
            settings.levelnumber = l.Count;
            l.Add(settings);
            FileStream fcreate = File.Open(jsonFilePath, FileMode.Create);

            StreamWriter writer = new StreamWriter(fcreate);
            writer.Write(JsonConvert.SerializeObject(l));
            writer.Close();
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
