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

    Outline current;
    System.Random rnd = new System.Random();
    List<Effect> currentEffects = new List<Effect>();
    List<Effect> effectsList = new List<Effect>();

    void Start()
    {
        var ctrl = GetComponent<PlayerCharacterController>();
        buffs = new List<Action> {
            () => ctrl.MaxSpeedOnGround = 300,
            () => ctrl.MaxSpeedOnGround = 5,
            () => ctrl.JumpForce = 15,
            () => ctrl.JumpForce = 2,
            () => GetComponent<Transform>().localScale = new Vector3(1,0.3f,1),
            () => GetComponent<Transform>().localScale = new Vector3(5, 5, 5)
        };
        foreach (var e in buffs)
        {
            effectsList.Add(new Effect(5, e));
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit hit;
        var cam = GetComponentInChildren<Camera>().transform;
        Ray ray = new Ray(cam.position, cam.forward);
        int mask = 1 << 6;
        if (Physics.Raycast(ray, out hit, 100, mask))
        {
            //current = hit.collider.GetComponent<Selectable>();
            //current.Select();
            current = hit.collider.gameObject.GetComponent<Outline>();
            current.enabled = true;
            if (Input.GetKeyDown(KeyCode.E)) {
                UseBottle(effectsList[0], current);
            }
        }
        else
        {
            if (current != null)
                current.enabled = false;
        }
        pointer.position = hit.point;
        RefreshEffects();
        
    }

    void UseBottle(Effect effect, Outline selectedItem)
    {
        effect.StartTime = Time.time;
        effect.Action();
        currentEffects.Add(effect);
        Destroy(selectedItem.gameObject);
    }

    void RefreshEffects()
    {
        for (int i = 0; i< currentEffects.Count; i++)
        {
            var e = currentEffects[i];
            if (Time.time >= e.StartTime + e.Duration)
            {
                currentEffects.Remove(e);
            }
        }
    }
}
