using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTest : MonoBehaviour
{
    public GameObject Player;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        { 
            other.gameObject.transform.position += new Vector3(0, 30, 0);
            other.GetComponent<CharacterMovement>().speed += 50;
        }

        //Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
    }
}
