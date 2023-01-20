using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentaclePlant : MonoBehaviour
{
    [SerializeField] private GameObject tentacle;
    [SerializeField] private int tentacleAmount;
    [SerializeField] private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < tentacleAmount; i++)
        {
            GameObject temp = Instantiate(tentacle, transform.position, Quaternion.identity);
            temp.GetComponent<TentaclePlantTentacle>().player = player;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}