using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HeroControls : MonoBehaviour
{
    public float jumpForce = 5f; // The force applied when the hero jumps
    public float gravity = 2f; // The gravity applied to the hero

    private bool isJumping = false; // Indicates if the hero is currently jumping
    private Rigidbody2D rb; // Reference to the hero's Rigidbody2D component

    GameData GameData;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameData = FindObjectOfType<GameData>();
    }

    private void Update()
    {
        if (!GameData.Main.IsPaused)
        {
            //TODO Don't run in Update
            rb.simulated = true;
            Jump();
        }
        else
        {
            rb.simulated = false;
        }
    }
    private void FixedUpdate()
    {
        if (!GameData.Main.IsPaused) ApplyGravity();
    }

    private void Jump()
    {
        if (Input.GetMouseButtonDown(0) && !isJumping)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJumping = true;
            isJumping = false;
        }
    }

    private void ApplyGravity()
    {
        rb.AddForce(Vector2.down * gravity);
    }
}
