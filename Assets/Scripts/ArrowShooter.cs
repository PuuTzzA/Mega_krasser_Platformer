using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowShooter : MonoBehaviour
{
    [SerializeField] private GameObject arrow;
    [SerializeField] private GameObject despawnZone;
    private GameObject player;
    [SerializeField] private float timeBetweenFiring;
    
    private float _timeSinceShooting;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        ShootArrow();
    }

    // Update is called once per frame
    void Update()
    {
        _timeSinceShooting += Time.deltaTime;
        if (_timeSinceShooting > timeBetweenFiring)
        {
            ShootArrow();
            _timeSinceShooting = 0;
        }
    }

    private void ShootArrow()
    {
        GameObject newArrow = Instantiate(arrow, transform.position, transform.rotation);
        newArrow.GetComponent<Arrow>().despawnZone = despawnZone;
        newArrow.GetComponent<Arrow>().player = player;
    }
}
