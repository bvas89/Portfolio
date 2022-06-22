/* Ability_Basketball : Ability
 * 
 * Sets the stats for the basketball (Navel Orange) to summon.
 * 
 * User must swipe to "toss" the basketball onto the field.
 * 
 */

using UnityEngine;

[CreateAssetMenu(fileName = "New Basketball", menuName = "Abilities/Basketball")]
public class Ability_Basketball : Ability
{
    [Tooltip("The damage the ball does on impact")]
    public int damage = 10;

    [Tooltip("The duration of the ability")]
    public float duration = 5.0f;

    [Tooltip("Lengthens/Shortens the swipe direction.")]
    [Range(0,2)] public float powerModifier = 0.5f;

    [Tooltip("The height in which the ball will be dropped from.")]
    public float dropHeight = 10.0f;

    [Tooltip("The added bounciness applied to the ball.")]
    public float bounciness = 100;

    [Tooltip("The reduction in bounciness after each bounce.")]
    public float bouncinessReduction = 0.1f;

    //The finger positions on screen
    private Vector3 startPos;
    private Vector3 endPos;

    //The times logged to determine power
    private float startTime;
    private float endTime;

    //The velocity of the swipe gesture
    private Vector3 velocity;

    public Vector3 posPos;

    //How to summon the ability
    public override void Gesture()
    {
        //Initialize basketball start settings on mouse down
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                startPos = new Vector3(hit.point.x, dropHeight, hit.point.z);
                startTime = Time.time;
            }
        }

        //SWIPE to determine direction/angle/power
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                endPos = new Vector3(hit.point.x, dropHeight, hit.point.z);
                endTime = Time.time;
            }

            float duration = endTime - startTime;
            Vector3 direction = endPos - startPos;
            float angle = Vector3.Angle(startPos, endPos);
            float distance = direction.magnitude;
            float power = distance / duration;

            //The velocity the ability will be instantiated with.
            velocity = (angle * direction).normalized * (power * powerModifier);

            isReady = true;
        }
    }

    public override void TriggerAbility()
    {
        SpawnBasketball();      
    }

    void SpawnBasketball()
    {
        GameObject ball = Instantiate(prefab, startPos, prefab.transform.rotation);
        ball.GetComponent<Rigidbody>().velocity = velocity;
        ball.GetComponent<BasketballController>().Initialize(damage, duration, bounciness, bouncinessReduction);
    }

    public override string SetEmoji()
    {
        return Emoji.orange;
    }
}
