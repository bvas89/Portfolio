/***
 * Magnifying Glass Controller
 * 
 * Creates a trail of fire according to the player's dragged finger input.
 * Each point is destroyed after its duration has elapsed..
 * 
 */

using System.Collections.Generic;
using UnityEngine;

public class MagnifyingGlassController : MonoBehaviour
{
    [Tooltip("The MagGlassLineRenderer prefab. This will be the actual lines drawn.")]
    public GameObject MagGlassLineRenderer;

    //A linerenderer in which to create a trail of fire
    LineRenderer line;
    List<LineRenderer> lines;

    //A list of lists of points.
    //Each KVP contains the (v3)point position and its (f)duration.
    //Each new LineRenderer is assigned a new List.
    List<List<KeyValuePair<Vector3, float>>> points;

    //The index of the next instantiated LineRenderer
    int _index;

    //Location of the input.
    Vector3 _thisPos;

    //The duration (lifetime) of each point.
    private float duration = 2.5f;

    //Is this able to create new points?
    bool _canCreateNewPoints = true;

    //When this stops creating new points.
    float _endTime;

    //How much damage this does per tick.
    float _damage;

    GameObject magGlassSprite;
    //---------------------

    private void Awake()
    {
        _index = 0;
        lines = new List<LineRenderer>();
        points = new List<List<KeyValuePair<Vector3, float>>>();
        magGlassSprite = transform.GetChild(0).gameObject;
        magGlassSprite.SetActive(false);
    }

    public void Initialize(float pointDuration, float abilityDuration, float damage)
    {
        duration = pointDuration;
        _endTime = Time.time + duration;
        _damage = damage;

        //Destroy this after its time has elapsed.
        Destroy(gameObject, abilityDuration + pointDuration);
    }

    private void Update()
    {
        //QUICK FIX: Prevent line being drawn while paused.
        if (Time.timeScale > 0)
            DrawLine();
        EraseLine();
        CreateSphereCast();
    }

    //Erases each point after Duration.
    void EraseLine()
    {
        //For each List of points
        for (int i = 0; i < points.Count; i++)
        {
            //...and each point within
            for (int j = 0; j < points[i].Count; j++)
            {
                //If duration has elapsed
                if (Time.time > points[i][j].Value)
                {
                    //Remove this point
                    points[i].RemoveAt(j);

                    //Reset line position count.
                    lines[i].positionCount = points[i].Count;

                    //Create a new list of points (without the removed one).
                    List<Vector3> pos = new List<Vector3>();
                    foreach(KeyValuePair<Vector3,float> kvp in points[i])
                        pos.Add(kvp.Key);

                    //Set the line's new positions.
                    lines[i].SetPositions(pos.ToArray());
                }
            }
        }
    }

    //Adds the points to corresponding lists to draw the line
    void DrawLine()
    {
        if (_canCreateNewPoints)
        {
            //Upon input...
            if (Input.GetMouseButtonDown(0))
            {
                //...creates a list of points and their lifetimes.
                points.Add(new List<KeyValuePair<Vector3, float>>());

                //Create a new MGLR and assign it to the list of Lines.
                GameObject g = Instantiate(MagGlassLineRenderer, transform);
                line = g.GetComponent<LineRenderer>();
                lines.Add(line);
                lines[_index].positionCount = 0;
            }

            //Upon mouse movement...
            if (Input.GetMouseButton(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    //Point of the input.
                    _thisPos = new Vector3(hit.point.x, hit.point.y + 1, hit.point.z);

                    magGlassSprite.SetActive(true);
                    //magGlassSprite.transform.SetPositionAndRotation(_thisPos, Quaternion.identity);
                    magGlassSprite.transform.position = _thisPos;

                    //...Creates a KVP of Position and Lifetime end time.
                    KeyValuePair<Vector3, float> pair
                        = new KeyValuePair<Vector3, float>(_thisPos, Time.time + duration);

                    //If this Line's Points does not contan this Point...
                    if (!points[_index].Contains(pair))
                    {
                        //...add it to the list..
                        points[_index].Add(pair);

                        //...and apply the new points to the line.
                        line.positionCount = points[_index].Count;
                        line.SetPosition(points[_index].Count - 1, points[_index][points[_index].Count - 1].Key);
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                //Ascertain current line has ended.
                line = null;

                magGlassSprite.SetActive(false);

                //Prepare index for next line creation.
                _index++;
            }
        }

        //Stops this from creating new points after the duration has elapsed.
        if (Time.time > _endTime)
            _canCreateNewPoints = false;

        if (_canCreateNewPoints == false) magGlassSprite.SetActive(false);
    }

    //Has checks for Ant interaction.
    void CreateSphereCast()
    {
        for(int i = 0; i < points.Count; i++)
        {
            for(int j = 0; j < points[i].Count; j++)
            {
                Vector3 nPoint = points[i][j].Key;
                float r = MagGlassLineRenderer.GetComponent<LineRenderer>().startWidth;

                var col = Physics.OverlapSphere(nPoint, r);
                
                foreach(Collider c in col)
                {
                    if (c.gameObject.CompareTag("Bug"))
                    {
                        c.gameObject.GetComponent<Ant.AntController>().TakeDamage(_damage);
                    }
                }
            }
        }
    }

    private void OnDisable()
    {
        foreach(var v in lines)
        {
           // Destroy(v);
        }
    }
}
