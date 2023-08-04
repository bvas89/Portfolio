using System.Collections;
using UnityEngine;
using TMPro;
using static GameData;
using System.IO;

public class ScoreAndTimeManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    [SerializeField] private int Score = 0;
    [SerializeField] private float increaseAmount, increaseInterval;
    public float timer = 0f;

    private void Start()
    {
        StartCoroutine(PointsForTime());
    }

    private void OnEnable()
    {
        Actions.Game.GetPoint += AddToScore;
    }

    private void OnDisable()
    {
        Actions.Game.GetPoint -= AddToScore;
    }

    void AddToScore(int points)
    {
        Score += (int)(points * Data.Main.Multiplier);
    }

    // Update is called once per frame
    void Update()
    {
        //scoreText.text = Score.ToString();
        scoreText.text = timer.ToString();

        if (!Data.Main.IsPaused)
            timer += Time.deltaTime;
    }

    IEnumerator PointsForTime()
    {
        while (true)
        {
            while (!Data.Main.IsPaused)
            {
                //Add score for each second played
                AddToScore(Data.Points.Second);
                yield return new WaitForSeconds(1f);
            }
            yield return new WaitForChangedResult();
        }
    }
}
