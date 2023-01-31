using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BridgeGenerate : MonoBehaviour
{
    [SerializeField] private GameObject start;

    [SerializeField] private GameObject end;
    [SerializeField] private GameObject plank;
    private Vector3 startposition;

    private Vector3 endposition;
    // Start is called before the first frame update
    void Start()
    {
        startposition = start.transform.position;
        endposition = end.transform.position;
        GeneratePlank();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int Numberofplanks (float distance)
    {
        /*
        Vector3 length = new Vector3(endposition.x - startposition.x, endposition.y - startposition.y,
            endposition.z - startposition.z);
        float _length = length.x * length.x + length.y * length.y + length.z * length.z;
        float distance = Mathf.Sqrt(_length);
        /**/
        float plankLength = plank.transform.localScale.z;
        int numb= (int)(distance / (plankLength*1.1))-1;
        return numb;
    }
    public void GeneratePlank()
    {
        Vector3 distance = end.transform.position - start.transform.position;
        float mag = distance.magnitude;
        int numberofplanks= this.Numberofplanks(mag);
        GameObject prev = start;
        for (int i=0; i < numberofplanks; i++)
        {
            GameObject newplank=Instantiate(plank, start.transform.position + distance * (i + 1) / (numberofplanks + 1), Quaternion.identity); 
            newplank.transform.SetParent(transform);
            newplank.GetComponent<HingeJoint>().connectedBody = prev.GetComponent<Rigidbody>();
            prev = newplank;
        } 
        end.GetComponent<HingeJoint>().connectedBody = prev.GetComponent<Rigidbody>(); 
        
    }
    
    
    
}
