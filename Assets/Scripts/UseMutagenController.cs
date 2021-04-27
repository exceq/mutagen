using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseMutagenController : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform pointer;
    public Material material;

    Selectable current;
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        print(ray);
        if (Physics.Raycast(ray, out hit, 100) && hit.collider.GetComponent<Selectable>())
        {
            current = hit.collider.GetComponent<Selectable>();
            current.Select(material.color);
            print("bulbbb!!!!!!");
        }
        else
        {
            if (current != null)
                current.Deselect();
        }
        pointer.position = hit.point;
        Debug.DrawLine(ray.origin, ray.direction, Color.red);
    }
}
