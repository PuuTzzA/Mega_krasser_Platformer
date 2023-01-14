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
    float sensibility = 1f;
    // Start is called before the first frame update
    void Start()
    {
        tmp = new GameObject();
        _rotationPoint = new GameObject("RotationPoint");
        _rotationPoint.transform.localScale = new Vector3(1, 1, 1);
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
        this.transform.position = new Vector3(0, 0, 20);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (player != null)
        {
            
            tmp.transform.position = _rotationPoint.transform.position;
            tmp.transform.localScale = _rotationPoint.transform.localScale;
            tmp.transform.rotation = _rotationPoint.transform.rotation;
            tmp.transform.LookAt(new Vector3(player.transform.position.x - player.transform.localScale.x / 4, player.transform.position.y, player.transform.position.z - player.transform.localScale.z / 4));
            _rotationPoint.transform.rotation = Quaternion.RotateTowards(_rotationPoint.transform.rotation, tmp.transform.rotation,Time.deltaTime * sensibility * Mathf.Abs(_rotationPoint.transform.eulerAngles.y - tmp.transform.eulerAngles.y));
            Debug.Log(_rotationPoint.transform.eulerAngles.ToString() + " " +  tmp.transform.eulerAngles.ToString());

        }
    }
}
