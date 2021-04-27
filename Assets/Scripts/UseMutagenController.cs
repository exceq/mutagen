using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UseMutagenController : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform pointer;
    public Material material;

    List<Action> buffs;

    Selectable current;
    System.Random rnd = new System.Random();
    void Start()
    {
        buffs = new List<Action> {
            () => gameObject.GetComponentInParent<CharacterMovement>().speed += 500,
            () => gameObject.GetComponentInParent<CharacterMovement>().speed = 50,
            () => gameObject.GetComponentInParent<CharacterMovement>().jumpForce += 500,
            () => gameObject.GetComponentInParent<CharacterMovement>().speed = 50,
            () => gameObject.GetComponentInParent<Transform>().localScale = new Vector3(0.5f,0.5f,0.5f),
            () => gameObject.GetComponentInParent<Transform>().localScale = new Vector3(5, 5, 5)
        };
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
            if (Input.GetKeyDown(KeyCode.E)) {
                current.Use(buffs[rnd.Next(4, buffs.Count-1)]);
                Destroy(current.gameObject);
            }
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
