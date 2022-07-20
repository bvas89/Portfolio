/* BasketballController
 * 
 * Allows the basketball (Navel Orange) to squash any bugs it touches.
 * 
 * Adds force to simulate bounciness.
 * 
 */

using UnityEngine;

public class BasketballController : MonoBehaviour
{
    private int damage;
    private float duration;
    private float bounciness;
    private float bounceReduction;

    //Sets the variables from the Ability SO
    public void Initialize(int dmg, float dur, float bnc, float reduction)
    {
        damage = dmg;
        duration = dur;
        bounciness = bnc;
        bounceReduction = reduction;
    }

    private void Start()
    {
        //Destroy it after its duration has elapsed.
        Destroy(gameObject, duration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Bug"))
        {
            collision.gameObject.GetComponent<Ant.AntController>().TakeDamage(damage);
        }

        if(collision.gameObject.CompareTag("Floor"))
        {
            //Adds an additional bouncy force to simulate a basketball
            Vector3 v = new Vector3(0, bounciness, 0);
            GetComponent<Rigidbody>().AddForce(v);

            //Reduce the added force after each bounce
            bounciness = bounciness - (bounciness * bounceReduction);
        }
    }
}
