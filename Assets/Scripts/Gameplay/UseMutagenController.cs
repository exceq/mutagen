using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UseMutagenController : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform pointer;
    public Material material;

    Outline current;
    List<Effect> buffs;
    System.Random rnd = new System.Random();
    List<Effect> currentEffects = new List<Effect>();

    void Start()
    {
        //Time.timeScale = 2f;
        //Time.fixedDeltaTime = Time.timeScale;

        var ctrl = GetComponent<PlayerCharacterController>();
        float startSpeed = ctrl.MaxSpeedOnGround;
        float startJF = ctrl.JumpForce;
        buffs = new List<Effect> {
            new Effect(10, () => ctrl.MaxSpeedOnGround = 50f, () => ctrl.MaxSpeedOnGround = startSpeed),
            new Effect(10, () => ctrl.MaxSpeedOnGround = 4f, () => ctrl.MaxSpeedOnGround = startSpeed),
            new Effect(10, () => ctrl.JumpForce = 12, () => ctrl.JumpForce = startJF),
            new Effect(10, () => ctrl.JumpForce = 3, () => ctrl.JumpForce = startJF),
            new Effect(10, () => { ctrl.Scale = 1.3f; GetComponent<Transform>().localScale = Vector3.up * 3; },
                                () => {ctrl.Scale = 1;GetComponent<Transform>().localScale = Vector3.one * 1; }),
            new Effect(10, () => { ctrl.Scale = 0.8f; GetComponent<Transform>().localScale = Vector3.one * 0.3f; },
                                () => {ctrl.Scale = 1;GetComponent<Transform>().localScale = Vector3.one * 1; }),
            new Effect(10, () => GetComponent<Transform>().localScale = new Vector3(1,1,0), () => GetComponent<Transform>().localScale = Vector3.one )
        };
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit hit;
        var cam = GetComponentInChildren<Camera>().transform;
        Ray ray = new Ray(cam.position, cam.forward);
        int mask = ~(1 << 2);
        if (Physics.Raycast(ray, out hit, 2f, mask))
        {
            if (hit.collider.gameObject.layer == 6 && hit.distance <= 1f)
            {
                current = hit.collider.gameObject.GetComponent<Outline>();
                current.enabled = true;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    UseBottle(buffs[6], current);
                }
            }
            else if (current != null && hit.collider.gameObject != current.gameObject)
            {
                current.enabled = false;
            }
        }
        //else if (current != null)
        //{
        //    current.enabled = false;
        //}
        pointer.position = hit.point;
        //RefreshEffects();        
    }

    void UseBottle(Effect effect, Outline selectedItem)
    {
        currentEffects.Add(effect);
        StartCoroutine(UseEffect(effect));
        Destroy(selectedItem.gameObject);
    }

    void RefreshEffects()
    {
        for (int i = 0; i < currentEffects.Count; i++)
        {
            var e = currentEffects[i];
            if (Time.time >= e.StartTime + e.Duration)
            {
                currentEffects.Remove(e);
            }
        }
    }

    IEnumerator UseEffect(Effect effect)
    {
        effect.Action();
        yield return new WaitForSeconds(effect.Duration);
        effect.PostAction();
        RefreshEffects();
    }
}
