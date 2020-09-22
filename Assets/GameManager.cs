using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] int score;
    [SerializeField] int multiplier;
    [Header("Plane Settings:")]
    [SerializeField] GameObject prefab_Airplane = default;
    [SerializeField] GameObject prefab_Bomb = default;
    [SerializeField] float StartHeight;
    [SerializeField] float ScoreToSpeedMultiplier;
    [Header("House Settings:")]
    [SerializeField] float buildspeed = default;
    [SerializeField] int startY = default;
    [SerializeField] int Houses = default;
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
    [SerializeField] int BaseScore;
    [SerializeField] float multiplierMissEffect;
    [Header("UI:")]
    [SerializeField] RectTransform[] ScoreText;

    public static GameManager Instance;
    GameObject activeAirplane;
    bool skip;
    bool revived;
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
    // Start is called before the first frame update
    void Start()
    {
        PlaystateManager.Instance.GetState(EnumStates.PlayMode).StateEnter_Start += StartGame;
        PlaystateManager.Instance.GetState(EnumStates.PlayMode).StateExit_End += ClearGame;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !skip)
            skip = true;
        //else if (Input.GetKeyDown(KeyCode.Mouse0) && PlaystateManager.Instance.CurrentState == EnumStates.PlayMode)
        //{
        //    Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    Instantiate(prefab_Bomb,new Vector3(Mathf.Round(pos.x),pos.y) , Quaternion.identity);
        //}
    }
    public void StartGame()
    {
        StartCoroutine(StartGameRoutine());
    }
    IEnumerator StartGameRoutine()
    {
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
        for (int i = 0; i < 3; i++)
        {
            view.transform.GetChild(1).gameObject.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            view.transform.GetChild(1).gameObject.SetActive(false);
            yield return new WaitForSeconds(0.2f);
        }
        GameObject plane = Instantiate(prefab_Airplane, new Vector3(Camera.main.ScreenToWorldPoint(Vector3.zero).x-1, StartHeight), Quaternion.identity);
        plane.GetComponent<Airplane>().Speed = 2+(score * ScoreToSpeedMultiplier);

        plane.GetComponent<Airplane>().StartAirplane();
        activeAirplane = plane;
    }
    public void ClearGame()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        if (activeAirplane)
            Destroy(activeAirplane);

        StopAllCoroutines();
    }
    public void BombUsed(bool missed)
    {
        if (missed)
            multiplier = Mathf.RoundToInt((float)multiplier * (float)multiplierMissEffect);
        else
            multiplier++;
        updateScoreUI();
    }
    public void DestroyedPiece()
    {
        score += Mathf.RoundToInt((float)BaseScore * (float)multiplier);
        updateScoreUI();
        //calculate and add score
        if (transform.childCount == 1)
        {
            //other stuff about clearing level
            PlaystateManager.Instance.ChangeState(EnumStates.Pause);
        }
    }
    public void updateScoreUI()
    {
        for (int i = 0; i < ScoreText.Length; i++)
        {
        ScoreText[i].GetComponent<Text>().text = score.ToString("n0");
        ScoreText[i].GetChild(0).GetComponent<Text>().text = "X" + multiplier.ToString("n0");
        }
    }
    public void CrashPlane()
    {
        if (!revived)
        {
            revived = true;
            PlaystateManager.Instance.ChangeState(EnumStates.Reward);
        }
        else
        {
            PlaystateManager.Instance.ChangeState(EnumStates.Ad);
        }
    }
    public void HandleDeath()
    {
        LeaderboardEntry entry = LeaderboardManager.Instance.playerEntry;
        entry.Score = score;
        LeaderboardManager.Instance.WriteToLeaderboard(entry);
        score = 0;
    }


}
