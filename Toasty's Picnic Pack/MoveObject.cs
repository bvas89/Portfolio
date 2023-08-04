/// MoveObject.cs
/// The universal script to attach to objects that move across the screen


using UnityEngine;
using static GameData;

public class MoveObject : MonoBehaviour
{
    [SerializeField] private float Speed = 4.5f; // Default speed for objects
    Vector3 _direction => Data.Main.Direction switch
    {
        ObjectDirection.Left => Vector3.left,
        ObjectDirection.Right => Vector3.right,
        ObjectDirection.Up => Vector3.up,
        ObjectDirection.Down => Vector3.down,
        _ => Vector3.left
    };

    SpriteRenderer _spriteRenderer;
    float _destroyBoundary; // Where to destroy the object


    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        Move();
        if (transform.position.x <= _destroyBoundary) Destroy(gameObject);
    }

    /// <summary>
    /// Initialize the spawnable object for the FLAPJACK game.
    /// </summary>
    /// <param name="gameData">The FlapJack Game Data to pull from.</param>
    /// <param name="spawnable">The FlapJack object to spawn.</param>
    public void InitializeObject(gdSpawnable spawnable)
    {
        Speed = Data.Main.Speed;
        gameObject.transform.parent = Data.Main.SpawnablesParent;
        _spriteRenderer.sprite = spawnable.Sprite;
        _destroyBoundary = -Data.Main.ScreenWidth * 1.5f;
    }

    /// <summary>
    /// Change speed, if necessary to do manually
    /// </summary>
    /// <param name="speed">How fast to go</param>
    public void ChangeSpeed(float speed) { Speed = speed; }

    private void Move() { if (!Data.Main.IsPaused) transform.Translate(_direction * Speed * Data.Main.Multiplier * Time.deltaTime); }
}
