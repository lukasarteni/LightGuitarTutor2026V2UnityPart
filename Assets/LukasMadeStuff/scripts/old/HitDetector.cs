using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HitDetector : MonoBehaviour
{
    private List<GameObject> targetsInZone = new List<GameObject>();

    void Start()
    {
        Debug.Log("HitDetector started on: " + gameObject.name);
    }

    void Update()
    {
        // Debug: show what targets are currently inside zone
        if (targetsInZone.Count > 0)
        {
            Debug.Log("Targets currently in zone: " + targetsInZone.Count);
        }

        // G key
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            Debug.Log("G key pressed");
            HitTarget("G Target");
        }

        // E key
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("E key pressed");
            HitTarget("Em Target");
        }
    }

    void HitTarget(string targetName)
    {
        Debug.Log("Attempting to hit target: " + targetName);

        foreach (GameObject target in targetsInZone)
        {
            if (target == null)
                continue;

            Debug.Log("Checking target: " + target.name);

            if (target.name.Contains(targetName))
            {
                Debug.Log("TARGET HIT: " + target.name);

                targetsInZone.Remove(target);
                Destroy(target);
                return;
            }
        }

        Debug.Log("No matching target found for: " + targetName);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger ENTER detected: " + other.name);

        if (other.name.Contains("Target"))
        {
            Debug.Log("Target added to hit zone: " + other.name);
            targetsInZone.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger EXIT detected: " + other.name);

        if (targetsInZone.Contains(other.gameObject))
        {
            Debug.Log("Target removed from zone: " + other.name);
            targetsInZone.Remove(other.gameObject);
        }
    }
}