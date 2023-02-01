using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class QuitApplication : MonoBehaviour
{
    private void OnApplicationQuit()
    {
        FileStream fcreate = File.Open(PreviewSettings.jsonFilePath, FileMode.Create);

        StreamWriter writer = new StreamWriter(fcreate);
        writer.Write(JsonConvert.SerializeObject(PreviewSettings.levelSettings));
        writer.Close();
    }
}
