using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpHandler : MonoBehaviour
{
    //public List<PowerUpEvent> _powerUpEvents;
    public AbilityEvent powerUpEvent;

    private void OnEnable()
    {
        Actions.PowerUps.PowerUpEvent += AttachPowerUp;
    }

    private void OnDisable()
    {
        Actions.PowerUps.PowerUpEvent -= AttachPowerUp;
    }

    void AttachPowerUp(AbilityEvent pue)
    {
        powerUpEvent = gameObject.AddComponent(pue.GetType()) as AbilityEvent;
    }
}
