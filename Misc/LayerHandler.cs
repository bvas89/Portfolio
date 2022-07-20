/*   LayerHandler.cs
 *   
 *   This is a experimental script for a mini-project, Neo Moon System.
 *   The focus of the game is to be a 2d Sidescrolling Stealth game that takes place on 3 planes.
 *   
 *   The player (and enemies) can move between Back,Mid,Foreground, using Layers to do so.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(PlatformEffector2D), typeof(SpriteRenderer))]
public class LayerHandler : MonoBehaviour
{
    public enum Type { Player, Enemy, Object, Scenic };
    [SerializeField]
    public Type type;

    public enum Depth { Background, Midground, Foreground, All };
    [SerializeField] public Depth depth = new Depth();

    //Depth Lists
    private List<int> bg = new List<int>();
    private List<int> mg = new List<int>();
    private List<int> fg = new List<int>();

    private List<Collider2D> overlaps = new List<Collider2D>();

    [HideInInspector]
    public PlatformEffector2D eff;
    private SpriteRenderer sr;
    private Collider2D col;
    private Color color;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        eff = GetComponent<PlatformEffector2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        col.usedByEffector = true;
        GetDepthLists();
        sr.sortingOrder = -(int)type;
        HandleCharacterDepth(0);
        color = sr.color;
    }

    void Update()
    {
        //Set Object's children to same layer as it.
        foreach (Transform go in GetComponentsInChildren<Transform>())
        {
            go.gameObject.layer = gameObject.layer;
            if (go.GetComponent<SpriteRenderer>() != null)
                go.GetComponent<SpriteRenderer>().sortingLayerName = sr.sortingLayerName;
        }
    }

    //Determines if the Unit can move into the next Depth.
    public void HandleCharacterDepth(int val)
    {
        //Checks to see if Depth is blocked.
        if (!IsDepthBlocked(val))
            DepthIsNotBlocked(val);

        else if (IsDepthBlocked(val))
            DepthIsBlocked();
    }

    //Checks to see if changing depth is blocked.
    private bool IsDepthBlocked(int val)
    {
        int j = 0;
        bool g = false;
        int oc = Physics2D.OverlapCollider(col, new ContactFilter2D(), overlaps);

        // Are any of the colliders on the same Depth as the new Value?
        foreach (Collider2D c in overlaps)
        {
            if (c.GetComponent<LayerHandler>() != null)
                if (c.GetComponent<LayerHandler>().depth == depth + val)
                {
                    j++;
                }
        }

        if (j > 0) g = true;
        return g;
    }

    //Allows the Unit to move into their destinated Depth.
    private void DepthIsNotBlocked(int val)
    {
        // Sets the Depth
        if ((int)depth + val <= 2 && (int)depth + val >= 0)
            depth += val;

        // Changes attributes depending on Depth.
        switch (depth)
        {
            case (Depth.Background):
                DepthIsBackground();
                break;
            case (Depth.Midground):
                DepthIsMidground();
                break;
            case (Depth.Foreground):
                DepthIsForeground();
                break;
            case (Depth.All):
                DepthIsAll();
                break;
        }
    }

    //Prints to console that Depth is Blocked.
    private void DepthIsBlocked()
    {
        //TODO Create Feedback for the Player.
        print("Depth is Blocked!");
    }

    //What to do when the GameObject is in the Background Depth.
    private void DepthIsBackground()
    {
        //Change the Color to Black
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1);
        sr.material.EnableKeyword("DEPTH_BACKGROUND");
        sr.material.DisableKeyword("DEPTH_MIDGROUND");
        sr.material.DisableKeyword("DEPTH_FOREGROUND");

        //Set Collider Masks
        eff.colliderMask = 1 << bg[0] | 1 << bg[1] | 1 << bg[2] | 1 << bg[3] | 1 << bg[4];
        gameObject.layer = bg[(int)type];

        //Set Sorting Layer
        sr.sortingLayerName = "Background";
    }

    //What to do when the GameObject is in the Midground Depth.
    private void DepthIsMidground()
    {
        //Change the Color
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1);
        sr.material.DisableKeyword("DEPTH_BACKGROUND");
        sr.material.EnableKeyword("DEPTH_MIDGROUND");
        sr.material.DisableKeyword("DEPTH_FOREGROUND");

        //Set Collider Masks
        eff.colliderMask = 1 << mg[0] | 1 << mg[1] | 1 << mg[2] | 1 << mg[3] | 1 << mg[4];
        gameObject.layer = mg[(int)type];

        sr.sortingLayerName = "Midground";
    }

    //What to do when the GameObject is in the Foreground Depth.
    private void DepthIsForeground()
    {
        //Change the Color
        //Currently hardcoding Color due to Shader conflict.
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.6f);
        sr.material.DisableKeyword("DEPTH_BACKGROUND");
        sr.material.EnableKeyword("DEPTH_FOREGROUND");
        sr.material.DisableKeyword("DEPTH_MIDGROUND");


        //Set Collider Masks
        eff.colliderMask = 1 << fg[0] | 1 << fg[1] | 1 << fg[2] | 1 << fg[3] | 1 << fg[4];
        gameObject.layer = fg[(int)type];

        //transform.localScale = new Vector2(transform.localScale.x * 2.1f, transform.localScale.y * 2.1f);

        sr.sortingLayerName = "Foreground";
    }

    //What to do when Depth is All. (Primarily for Walls).
    private void DepthIsAll()
    {
       // sr.color = Color.white;
        eff.colliderMask = -1;
        gameObject.layer = LayerMask.NameToLayer("Ground");
        sr.sortingLayerName = "Midground";
    }

    private void GetDepthLists()
    {
        // Background Layers

        bg.Add(LayerMask.NameToLayer("BGPlayer"));
        bg.Add(LayerMask.NameToLayer("BGEnemy"));
        bg.Add(LayerMask.NameToLayer("BGObject"));
        bg.Add(LayerMask.NameToLayer("BGMisc"));
        bg.Add(LayerMask.NameToLayer("Ground"));

        // Midground Layers

        mg.Add(LayerMask.NameToLayer("MGPlayer"));
        mg.Add(LayerMask.NameToLayer("MGEnemy"));
        mg.Add(LayerMask.NameToLayer("MGObject"));
        mg.Add(LayerMask.NameToLayer("MGMisc"));
        mg.Add(LayerMask.NameToLayer("Ground"));

        // Foreground Layers

        fg.Add(LayerMask.NameToLayer("FGPlayer"));
        fg.Add(LayerMask.NameToLayer("FGEnemy"));
        fg.Add(LayerMask.NameToLayer("FGObject"));
        fg.Add(LayerMask.NameToLayer("FGMisc"));
        fg.Add(LayerMask.NameToLayer("Ground"));
    }

    // Checks layers for Player Dash
    public int DashLayer()
    {
        int d = 0;
        if (depth == Depth.Background)
        {
            d = 1 << bg[2] | 1 << bg[3] | 1 << bg[4];
        }
        if (depth == Depth.Midground)
        {
            d = 1 << mg[2] | 1 << mg[3] | 1 << mg[4];
        }
        if (depth == Depth.Foreground)
        {
            d = 1 << fg[2] | 1 << fg[3] | 1 << fg[4];
        }
        return d;
    }
}
