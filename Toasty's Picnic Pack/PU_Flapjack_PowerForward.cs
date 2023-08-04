/////Flapjack Power Forward
///This ability moves the Hero to the right and walks through obstacles.
///
using UnityEngine;
using static GameData;

public class PU_Flapjack_PowerForward : AbilityEvent
{
    public float speed = 1.5f; //How fast to move forward
    Rigidbody2D rb; //The Hero's rigidbody
    Collider2D col; //The Hero's collider

    protected override void Start()
    {
        base.Start();
        rb = Data.Main.Hero.GetComponent<Rigidbody2D>();
        col = Data.Main.Hero.GetComponentInChildren<Collider2D>();
        col.isTrigger = true; // Turn off collisions
    }

    private void Update()
    {
        if (!Data.Main.IsPaused)
        {
            // Move the hero to the right
            rb.velocity = transform.right.normalized * speed;
        }

        //If the Hero collides with the right boundary, stop the PowerUp
        if (col.IsTouching(GameObject.Find("Right Bar").GetComponent<Collider2D>()))
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        rb.velocity = transform.right.normalized * 0; // Stop forward movement
        col.isTrigger = false; //Turn collisions back on
    }
}