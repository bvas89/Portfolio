using UnityEngine;

public abstract class Collectible : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") Collect();
    }

    /// <summary>
    /// What happens when the player collects this object
    /// </summary>
    public abstract void Collect();
}
