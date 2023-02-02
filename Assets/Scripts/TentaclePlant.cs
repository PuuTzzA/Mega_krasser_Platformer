using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Random = UnityEngine.Random;

public class TentaclePlant : MonoBehaviour
{
    [SerializeField] private GameObject tentacle;
    private GameObject player;

    [Header("Tentacle Settings, get passed on to individual Tentacles")]
    [SerializeField] private int tentacleAmount;
    [SerializeField] private float detectionRadius;
    [SerializeField] private float grappleStrength;
    [SerializeField] private float breakFreeDistance;
    [SerializeField] private float maxLength;
    [SerializeField] private float lineRendererMinWidth = 1.2f;
    [SerializeField] private float lineRendererMaxWidth = 1.6f;
    [SerializeField] private Color lineRendererColor1;
    [SerializeField] private Color lineRendererColor2;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        for (int i = 0; i < tentacleAmount; i++)
        {
            GameObject temp = Instantiate(tentacle, transform.position, Quaternion.identity);
            temp.gameObject.transform.parent = transform;
            // Line Renderer
            LineRenderer lR = temp.GetComponent<LineRenderer>();
            lR.widthMultiplier = Random.Range(lineRendererMinWidth, lineRendererMaxWidth);
            Gradient gradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[2];
            colorKeys[0].time = 0.0f;
            colorKeys[0].color = Color.black;
            colorKeys[1].time = 1f;
            colorKeys[1].color = Color.Lerp(lineRendererColor1, lineRendererColor2, Random.Range(0.0f, 1.0f));
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0].time = 0.0f;
            alphaKeys[0].alpha = 255;
            alphaKeys[1].time = 1f;
            alphaKeys[1].alpha = 255;
            gradient.SetKeys(colorKeys, alphaKeys);
            lR.colorGradient = gradient;
            // Script Sachen
            TentaclePlantTentacle tempScript = temp.GetComponent<TentaclePlantTentacle>();
            tempScript.player = player;
            tempScript.detectionRadius = detectionRadius;
            tempScript.grappleStrength = grappleStrength;
            tempScript.breakFreeDistance = breakFreeDistance;
            tempScript.maxLength = maxLength;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == player)
        {
            player.GetComponent<Player>().damage();
        }
    }
}