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
        var ctrl = GetComponent<PlayerCharacterController>();
        buffs = new List<Action> {
            () => ctrl.MaxSpeedOnGround = 30,
            () => ctrl.MaxSpeedOnGround = 5,
            () => ctrl.JumpForce = 15,
            () => ctrl.JumpForce = 2,
            () => GetComponent<Transform>().localScale = new Vector3(1,0.3f,1),
            () => GetComponent<Transform>().localScale = new Vector3(5, 5, 5)
        };
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit hit;
        var cam = GetComponentInChildren<Camera>().transform;
        Ray ray = new Ray(cam.position, cam.forward);
        int a = (1 << 2) ^ 31;
        if (Physics.Raycast(ray, out hit, 100, a) && hit.collider.GetComponent<Selectable>())
        {
            current = hit.collider.GetComponent<Selectable>();
            current.Select(material.color);
            if (Input.GetKeyDown(KeyCode.E)) {
                current.Use(buffs[5]);
                //current.Use(buffs[rnd.Next(3, buffs.Count-1)]);
                //Destroy(current.gameObject);
            }
        }
        else
        {
            if (current != null)
                current.Deselect();
        }
        pointer.position = hit.point;
    }
}
