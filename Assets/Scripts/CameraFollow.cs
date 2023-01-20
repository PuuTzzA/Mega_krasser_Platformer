using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private GameObject _pointOfReference;
    [SerializeField]
    private GameObject player;
    private GameObject _rotationPoint;
    private GameObject tmp;

    [SerializeField]
    private float rotationSensibility = 1f;
    [SerializeField]
    private float verticalSensibility = 1f;

    [SerializeField]
    private float radius;


    // Start is called before the first frame update
    void Start()
    {
        tmp = new GameObject();
        _rotationPoint = new GameObject("RotationPoint");
        if (_pointOfReference == null)
        {
            _rotationPoint.transform.position = new Vector3(0, 0, 0);
        }
        else
        {
            _rotationPoint.transform.position = _pointOfReference.transform.position;
        }
        this.transform.SetParent(_rotationPoint.transform);
        this.transform.eulerAngles = new Vector3(0, 180, 0);
        this.transform.position = new Vector3(0, 0, radius);

        tmp.transform.position = _rotationPoint.transform.position;
        tmp.transform.localScale = _rotationPoint.transform.localScale;
        tmp.transform.rotation = _rotationPoint.transform.rotation;

        tmp.transform.LookAt(new Vector3(player.transform.position.x - player.transform.localScale.x / 4, player.transform.position.y, player.transform.position.z - player.transform.localScale.z / 4));
        tmp.transform.rotation = new Quaternion(0, tmp.transform.rotation.y, 0, tmp.transform.rotation.w);

        _rotationPoint.transform.rotation = Quaternion.RotateTowards(_rotationPoint.transform.rotation, tmp.transform.rotation, 360);
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        Vector3 relativePos = player.transform.position - _rotationPoint.transform.position;
        relativePos = new Vector3(relativePos.x,0,relativePos.z);
        Quaternion rotation = Quaternion.LookRotation(relativePos);

        Quaternion current = _rotationPoint.transform.localRotation;
        Debug.Log(Time.deltaTime);
        //Debug.Log(Quaternion.Angle(current, rotation) * Time.deltaTime * rotationSensibility);
        _rotationPoint.transform.localRotation = Quaternion.Slerp(current, rotation, Time.deltaTime * rotationSensibility);


        /* tmp.transform.position = _rotationPoint.transform.position;
         tmp.transform.localScale = _rotationPoint.transform.localScale;
         tmp.transform.rotation = _rotationPoint.transform.rotation;
         tmp.transform.LookAt(new Vector3(player.transform.position.x - player.transform.localScale.x / 4, player.transform.position.y, player.transform.position.z - player.transform.localScale.z / 4));
         tmp.transform.rotation = new Quaternion(0, tmp.transform.rotation.y, 0, tmp.transform.rotation.w);
         _rotationPoint.transform.rotation = Quaternion.Slerp(_rotationPoint.transform.rotation, Quaternion.RotateTowards(_rotationPoint.transform.rotation, tmp.transform.rotation, 360), Time.deltaTime * rotationSensibility);
         */
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, player.transform.position.y, transform.position.z), Time.deltaTime * verticalSensibility);
    }
}
