/*
 * PheramoneHandler.cs
 * 
 * Pheramones are released while an Ant returns a crumb to its AntHill.
 * Other Ants that detect the Pheramone will follow it.
 * 
 * This script causes the Pheramone strength to dissipate before being destroyed.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PheramoneHandler : MonoBehaviour
{
    public float duration = 2f;
    public float weight = 50f;

    void Start()
    {
        Destroy(gameObject, duration);
        StartCoroutine(Dissipate());
    }

    IEnumerator Dissipate()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.1f);
            weight -= 1f;
        }
    }
}
