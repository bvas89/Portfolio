/* Tap.css
 * 
 * Controls user-input tapping.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(SpriteRenderer))]
public class Tap : MonoBehaviour
{
    //Presets
    Vector3 offset;
    Camera cam;
    RaycastHit hit;

    [Header("Tap Settings")]
    [Tooltip("The radius of the tap.")]
    [SerializeField] private float _radius = 1.0f;
    [Tooltip("The duration the tap lasts.")]
    [SerializeField] private float duration = 0.5f;
    [Tooltip("The damage the tap does.")]
    [SerializeField] private int damage = 5;

    //Components
    private AudioSource _source;
    private SpriteRenderer _sr;

    //Strings
    private string s_Bug = "Bug";


    private void Awake()
    {
        cam = Camera.main;
        _source = gameObject.GetComponent<AudioSource>();
        _sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _sr.enabled = false;
    }

    void Update()
    {
        offset = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);

        //On Player Tap (Currently MouseButton)
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit))
            {
                //If the raycast hits (anything), play sound and show Tap.
                transform.position = hit.point;
                _source.Play();
                StartCoroutine(ShowTap());

                //Cast a sphere.
                Collider[] col = Physics.OverlapSphere(hit.point, _radius);
                foreach(Collider c in col)
                {
                    //Deals damage to any Ants hit.
                    if (c.gameObject.CompareTag(s_Bug))
                    {
                        c.gameObject.GetComponent<Ant.AntController>()
                            .TakeDamage(damage);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Sets the tap radius
    /// </summary>
    /// <param name="newRadius">The radius to set the tap.</param>
    public void SetRadius(float newRadius)
    {
        _radius = newRadius;
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(hit.point, 1f);
    }

    IEnumerator ShowTap()
    {
        _sr.enabled = true;
        yield return new WaitForSeconds(duration);
        _sr.enabled = false;
    }
}
