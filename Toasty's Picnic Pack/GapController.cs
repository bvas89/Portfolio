using UnityEngine;
using static GameData;

public class GapController : MoveObject
{
    Transform _gap;
    [SerializeField] private float _gapHeight;
    [SerializeField] private GameObject[] Objects;
    Vector3 dimensions;
    bool isMalleableGap = false;

    [System.Serializable]
    public class Obstacle
    {
        public GameObject obstacle;
        public float height;
    }

    void Start()
    {

        _gap = transform.GetChild(0);

        // Get Gap Height - Not direct to dimensions; Can individualize heights.
        //_gapHeight = Data.Gaps.GapHeight;
        _gapHeight = Random.Range(1.5f, 3f);
        dimensions = new Vector3(1, _gapHeight, 1);
        _gap.transform.localScale = dimensions;
        GetComponent<BoxCollider2D>().size = dimensions;

        /*
        foreach (var v in Objects)
        {
            v.GetComponent<SpriteRenderer>().size = new Vector2(1, Data.Gaps.ObstacleHeight);
        }
        */

        RandomizeObjectSizes();

    }

    void RandomizeObjectSizes()
    {
        foreach (var v in Objects)
        {
            float height = Random.Range(0, 6);
            v.GetComponent<SpriteRenderer>().size = new Vector2(1, height);
        }
    }

    void FixedUpdate()
    {
        //Only constantly change size if gap is being changed.
        if (isMalleableGap) GetComponent<BoxCollider2D>().size = dimensions;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) GetPoints();
    }

    public void GetPoints()
    {
        Actions.Game.GetPoint(Data.Points.Gap); //GameData.Gaps.Points
    }
}