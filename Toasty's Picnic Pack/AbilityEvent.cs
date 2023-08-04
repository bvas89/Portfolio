using System;
using System.Collections;
using UnityEngine;
using static GameData;

[Serializable]
public abstract class AbilityEvent : MonoBehaviour
{
    public float duration;
    float remainingTime;

    protected virtual void Start()
    {
        remainingTime = duration;
        StartCoroutine(CountDown());
    }

    /// <summary>
    /// Destroy the ability after its duration.
    /// </summary>
    /// <returns></returns>
    IEnumerator CountDown()
    {
        while (remainingTime > 0f)
        {
            if (!Data.Main.IsPaused)
            {
                remainingTime -= Time.deltaTime;
            }
            yield return null;
        }
        Destroy(gameObject);
    }
}
