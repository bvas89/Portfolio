using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static GameData;

public class GloudGenerator : MonoBehaviour
{
    private float speed = 0.5f;
    public float multiplier = 1f;
    public GameObject prefab;
    public List<GameObject> clouds = new List<GameObject>();
    public Sprite[] sprites;
    public BoxCollider2D spawner;
    [System.Serializable]
    public class RandomWaitTime
    {
        public float minTime = 2f;
        public float maxTime = 5f;
    }
    public RandomWaitTime randomWaitTime;
    float RandomTime() { return Random.Range(randomWaitTime.minTime, randomWaitTime.maxTime); }


    // Start is called before the first frame update
    void Start()
    {
        speed = Data.Main.Speed / multiplier;
        StartCoroutine(SpawnClouds());
    }

    IEnumerator SpawnClouds()
    {
        while (true)
        {
            if (!Data.Main.IsPaused)
            {
                var randomX = Random.Range(spawner.bounds.min.x, spawner.bounds.max.x);
                var randomY = Random.Range(spawner.bounds.min.y, spawner.bounds.max.y);
                Vector3 startPos = new Vector3(randomX, randomY, 0);
                GameObject go = Instantiate(prefab, startPos, Quaternion.identity);
                go.transform.parent = transform;
                go.GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Count())];
                clouds.Add(go);
                go.transform.localScale *= Random.Range(0f, 2f);
                yield return new WaitForSeconds(RandomTime());
            }
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        speed = Data.Main.Speed / multiplier;
    }

    void Move()
    {
        if (!Data.Main.IsPaused)
        {
            for (int i = clouds.Count - 1; i >= 0; i--)
            {
                clouds[i].transform.Translate(Vector3.left * speed * Time.deltaTime);

                // Check if the sprite is completely off-screen
                if (clouds[i].transform.position.x < -Data.Main.ScreenWidth * 1.5f)
                {
                    Destroy(clouds[i]);
                    clouds.RemoveAt(i);
                }
            }
        }
    }
}
