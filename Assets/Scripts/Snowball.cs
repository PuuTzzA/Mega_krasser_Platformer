using System.Collections;
using UnityEngine;

public class Snowball : MonoBehaviour
{
   // private float delay = 0.1f;
    [SerializeField] private GameObject snow;

    private void Start()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
       
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        
        if (other.gameObject.tag == "enemy")
        {
            //Destroy(gameObject, delay);
            Destroy(gameObject);
            GameObject _snow = Instantiate(snow, transform.position,transform.rotation);
            Destroy(_snow, 0.5f);

            //Destroy enemy
            //Destroy(other.gameObject,delay);

        }
        else
        {
            //StartCoroutine(DestroySnowball());
            Destroy(gameObject);
            GameObject _snow = Instantiate(snow, transform.position,transform.rotation);
            Destroy(_snow, 0.5f);
        }
    }

    IEnumerator DestroySnowball()
    {
        yield return new WaitForSeconds(0.1f); Destroy(gameObject);
    }
    

    
}
