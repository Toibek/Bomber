﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float drop;
    [Space]
    [SerializeField] int score = default;
    [SerializeField] int multiplier = default;
    [SerializeField] int screens = default;
    [Header("Plane Settings:")]
    [SerializeField] GameObject prefab_Airplane = default;
    //[SerializeField] GameObject prefab_Bomb = default;
    [SerializeField] float StartHeight = default;
    [SerializeField] float baseSpeed = default;
    [SerializeField] float ScoreToSpeedMultiplier = default;
    [SerializeField] float ScreenToSpeedMultiplier = default;
    [SerializeField] float maxSpeed = default;
    [Space]
    [SerializeField] float baseDrop = default;
    [SerializeField] float ScoreToDropMultiplier = default;
    [SerializeField] float ScreenToDropMultiplier = default;
    [SerializeField] int LapsBeforeDrop = default;
    [SerializeField] float maxDrop = default;
    [Space]
    [SerializeField] int bombsPerLap = default;
    [Header("House Settings:")]
    [SerializeField] float buildspeed = default;
    [SerializeField] int startY = default;
    public int Houses = default;
    [SerializeField] Vector2Int HouseHeight = default;
    [SerializeField] Color[] Colors = default;
    [SerializeField] GameObject Prefab_HousePart = default;
    [SerializeField] Sprite BottomSprite = default;
    [SerializeField] Sprite[] HouseSprites = default;
    [SerializeField] Sprite[] RoofSprites = default;
    [SerializeField] GameObject ground = default;
    List<GameObject> HouseParts = new List<GameObject>();
    List<int> HouseHeights = new List<int>();
    [Header("Score Settings:")]
    [SerializeField] int BaseScore = default;
    [SerializeField] float multiplierMissEffect = default;
    [Header("UI:")]
    [SerializeField] RectTransform[] ScoreText = default;
    [SerializeField] float scoreIncrements;
    [SerializeField] Text DeathText = default;
    [SerializeField] string[] DeathMessage;
    [Header("Sounds:")]
    [SerializeField] AudioClip StartSound;
    [SerializeField] AudioClip DeathSound;
    Coroutine routine;
    GameObject activeAirplane;
    Coroutine ScoreCounter;
    bool skip;
    bool revived;
    int shownScore = 0;
    int scoreDiff;

    #region Singleton
    public static GameManager Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        if (LeaderboardManager.Instance.playerEntry.Name == "")
            PlaystateManager.Instance.SetState(EnumStates.Name);
        else
            PlaystateManager.Instance.SetState(EnumStates.MainMenu);
        PlaystateManager.Instance.GetState(EnumStates.PlayMode).StateEnter_Start += StartGame;
        PlaystateManager.Instance.GetState(EnumStates.PlayMode).StateExit_End += ClearGame;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !skip)
            skip = true;
        if (Input.GetKeyDown(KeyCode.F1))
            PlayerPrefs.DeleteAll();
    }
    public void StartGame()
    {
        screens++;
        multiplier++;
        routine =StartCoroutine(StartGameRoutine());
    }
    IEnumerator StartGameRoutine()
    {
        if(ScoreCounter == null)
            ScoreCounter = StartCoroutine(WriteScore());
        updateScoreUI();
        skip = false;
        for (int i = 0; i < ground.transform.childCount; i++)
            ground.transform.GetChild(i).GetComponent<DestroyableGround>()?.ResetDamage();

        List<Color> colLis = new List<Color>();
        colLis.AddRange(Colors);

        int startPos = -Mathf.FloorToInt(Houses / 2);
        Vector2Int pos;
        List<int> positions = new List<int>();
        for (int i = 0; i < Houses; i++)
            positions.Add(startPos + i);
        int lastHeight = 2;
        for (int i = 0; i < Houses; i++)
        {
            int x = positions[Random.Range(0, positions.Count)];
            positions.Remove(x);

            int height = Random.Range(HouseHeight.x, HouseHeight.y);
            if (i != 0)
            {
                while (lastHeight == height)
                {
                    height = Random.Range(HouseHeight.x, HouseHeight.y);
                }
            }
            lastHeight = height;

            if (colLis.Count == 0)
                colLis.AddRange(Colors);

            Color color = colLis[Random.Range(0, colLis.Count)];
            colLis.Remove(color);
            Sprite houseSprite = HouseSprites[Random.Range(0, HouseSprites.Length)];
            for (int k = 0; k < height; k++)
            {
                pos = new Vector2Int(x, k + startY);

                GameObject go = Instantiate(Prefab_HousePart, new Vector3(pos.x, pos.y), Quaternion.identity, transform);
                go.GetComponent<SpriteRenderer>().color = color;

                if (k == 0)
                {
                    go.GetComponent<SpriteRenderer>().sprite = BottomSprite;
                    go.GetComponent<HousePiece>().bottomFloor = true;
                }
                else if (k == height - 1)
                {
                    go.GetComponent<SpriteRenderer>().sprite = RoofSprites[Random.Range(0, RoofSprites.Length)];
                    go.GetComponent<BoxCollider2D>().offset = new Vector2(0, -0.4f);
                    go.GetComponent<BoxCollider2D>().size = new Vector2(0.94f, 0.2f);
                }
                else
                    go.GetComponent<SpriteRenderer>().sprite = houseSprite;
                if (!skip)
                    yield return new WaitForSeconds(buildspeed);
            }

        }
        skip = true;

        GameObject view = PlaystateManager.Instance.GetState(EnumStates.PlayMode).View;
        SoundManager.PlaySolo(StartSound);
        GameObject plane = Instantiate(prefab_Airplane, new Vector3(0, StartHeight), Quaternion.identity);
        for (int i = 0; i < 3; i++)
        {
            view.transform.GetChild(1).gameObject.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            view.transform.GetChild(1).gameObject.SetActive(false);
            yield return new WaitForSeconds(0.2f);
        }

        plane.GetComponent<Airplane>().Speed = CalculateSpeed();
        plane.GetComponent<Airplane>().dropSpeed = CalculateDrop();
        plane.GetComponent<Airplane>().lapsBeforeDrop = LapsBeforeDrop;
        plane.GetComponent<Airplane>().BombsPerLap = bombsPerLap;

        plane.GetComponent<Airplane>().StartAirplane();
        activeAirplane = plane;
    }
    public float CalculateSpeed()
    {
        speed = Mathf.Clamp(baseSpeed + (score * ScoreToSpeedMultiplier) + (screens * ScreenToSpeedMultiplier), 0, maxSpeed);
        return speed;
    }
    public float CalculateDrop()
    {
        drop = Mathf.Clamp(baseDrop + (score * ScoreToDropMultiplier) + (screens * ScreenToDropMultiplier), 0, maxDrop);
        return drop;
    }
    public void ClearGame()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        if (activeAirplane)
            Destroy(activeAirplane);
        StopCoroutine(routine);
    }
    public void BombUsed(bool missed)
    {
        if (missed)
            multiplier = Mathf.RoundToInt( Mathf.Clamp(((float)multiplier * (float)multiplierMissEffect),screens,Mathf.Infinity));
        else
            multiplier++;
        updateScoreUI();
    }
    public void DestroyedPiece()
    {
        score += Mathf.RoundToInt((float)BaseScore * (float)multiplier);
        updateScoreUI();
        CheckForCompletion();
    }
    public void CheckForCompletion()
    {
        if (transform.childCount <= 1)
        {
            GameObject[] bombs = GameObject.FindGameObjectsWithTag("Bomb");
            for (int i = 0; i < bombs.Length; i++)
                Destroy(bombs[i]);
            Destroy(activeAirplane);

            PlaystateManager.Instance.ChangeState(EnumStates.Pause);
        }
    }
    public void updateScoreUI()
    {
        scoreDiff = score - shownScore;
        for (int i = 0; i < ScoreText.Length; i++)
        {
            if(ScoreText[i].GetChild(0) != null)
                ScoreText[i].GetChild(0).GetComponent<Text>().text = "X " + multiplier.ToString("n0");
        }
    }
    IEnumerator WriteScore()
    {
        while (true)
        {
            shownScore = Mathf.Clamp(shownScore + Mathf.RoundToInt(scoreDiff*scoreIncrements) , 0, score);
            for (int i = 0; i < ScoreText.Length; i++)
                ScoreText[i].GetComponent<Text>().text = shownScore.ToString("n0");
            yield return new WaitForSeconds(0.05f);
        }
    }
    public void CrashPlane()
    {
        GameObject[] bombs = GameObject.FindGameObjectsWithTag("Bomb");
        for (int i = 0; i < bombs.Length; i++)
            Destroy(bombs[i]);

        SoundManager.PlaySolo(DeathSound);
        if (!revived)
        {
            PlaystateManager.Instance.ChangeState(EnumStates.Reward);
            revived = true;
        }
        else
        {
            HandleDeath();
            //PlaystateManager.Instance.GetState(EnumStates.Ad).View.GetComponent<Ad>().SetState(EnumStates.Death);
            PlaystateManager.Instance.ChangeState(EnumStates.Death);
        }
    }
    public void HandleDeath()
    {
        StopCoroutine(ScoreCounter);
        ScoreCounter = null;
        updateScoreUI();
        SetDeathText();

        LeaderboardEntry entry = LeaderboardManager.Instance.playerEntry;
        revived = false;
        if (score > PlayerPrefs.GetInt("Highscore"))
            PlayerPrefs.SetInt("Highscore",score);
        entry.Score = score;
        LeaderboardManager.Instance.WriteToLeaderboard(entry);
        score = 0;
        shownScore = 0;
        screens = 0;
        multiplier = 0;
    }
    void SetDeathText()
    {
        if(score >= PlayerPrefs.GetInt("Highscore"))
        {
            DeathText.text = "New high score!";
            DeathText.color = Color.yellow;
        }
        else
        {
            DeathText.text = DeathMessage[Random.Range(0, DeathMessage.Length)];
            DeathText.color = Color.white;
        }
    }


}
