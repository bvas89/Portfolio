/////Flapjack Magnet
///This Ability draws Coins in for a few seconds.
//

using UnityEngine;
using static GameData;

public class PU_Flapjack_Magnet : AbilityEvent
{
    public float pullSpeed = 2.0f;
    public float stoppingDistance = 0.1f;

    private void Update()
    {
        // Find all objects with the "Coin" tag
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");

        // Pull each coin towards the target point
        foreach (GameObject coin in coins)
        {
            Vector3 directionToTarget = Data.Main.Hero.transform.position - coin.transform.position;
            float distanceToTarget = directionToTarget.magnitude;

            if (distanceToTarget > stoppingDistance)
            {
                Vector3 pullDirection = directionToTarget.normalized;
                float pullDistance = pullSpeed * Data.Main.Multiplier * Time.deltaTime;
                float distanceMoved = Mathf.Min(pullDistance, distanceToTarget);

                coin.transform.position += pullDirection * distanceMoved;
            }
        }
    }
}
