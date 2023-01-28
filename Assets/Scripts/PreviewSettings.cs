using System;
using System.Collections;
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
    public TextAsset jsonFile;

    private LevelSettings _settings;
    public float totalCollectables;

    [SerializeField]
    public string sceneToLoad;

    private void Awake()
    {

        List<LevelSettings> l =  JsonConvert.DeserializeObject<List<LevelSettings>>(jsonFile.text);
        if (l == null)
        {
            l = new List<LevelSettings>();
        }
        try
        {
            _settings = l[levelnumber];
        }
        catch (ArgumentOutOfRangeException)
        {
            _settings = new LevelSettings();
            _settings.collectablesCollected = 0;
            _settings.fastestTime = -1;
            _settings.levelnumber = l.Count;
            l.Add(_settings);
            FileStream fcreate = File.Open(AssetDatabase.GetAssetPath(jsonFile), FileMode.Create);
            StreamWriter writer = new StreamWriter(fcreate);
            writer.Write(JsonConvert.SerializeObject(l));
            writer.Close();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.RotateAround(transform.position, Vector3.up, Time.deltaTime * _settings.rotationSpeed);
    }

    public string GetScene()
    {
        return sceneToLoad;
    }

    private void OnDisable()
    {

    }
}
