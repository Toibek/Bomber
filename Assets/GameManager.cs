using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject prefab_Bomb = default;
    [Header("Plane Settings:")]

    [Header("House Settings:")]
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
    // Start is called before the first frame update
    void Start()
    {
        PlaystateManager.Instance.GetState(EnumStates.PlayMode).StateEnter_Start += StartGame;
        PlaystateManager.Instance.GetState(EnumStates.PlayMode).StateExit_End += ClearGame;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && PlaystateManager.Instance.CurrentState == EnumStates.PlayMode)
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Instantiate(prefab_Bomb,new Vector3(Mathf.Round(pos.x),pos.y) , Quaternion.identity);
        }
    }
    public void StartGame()
    {
        StartCoroutine(StartGameRoutine());
    }
    IEnumerator StartGameRoutine()
    {
        List<Color> colLis = new List<Color>();
        colLis.AddRange(Colors);

        int startPos = -Mathf.FloorToInt(Houses / 2);
        Vector2Int pos;
        List<int> positions = new List<int>();
        for (int i = 0; i < Houses; i++)
            positions.Add(startPos + i);
        for (int i = 0; i < Houses; i++)
        {
            int x = positions[Random.Range(0, positions.Count)];
            positions.Remove(x);

            int Height = Random.Range(HouseHeight.x, HouseHeight.y);

            if (colLis.Count == 0)
                colLis.AddRange(Colors);
            
            Color color = colLis[Random.Range(0, colLis.Count)];
            colLis.Remove(color);
            Sprite houseSprite = HouseSprites[Random.Range(0, HouseSprites.Length)];
            for (int k = 0; k < Height; k++)
            {
                pos = new Vector2Int(x, k + startY);

                GameObject go = Instantiate(Prefab_HousePart,new Vector3(pos.x,pos.y),Quaternion.identity,transform);
                go.GetComponent<SpriteRenderer>().color = color;

                if (k == 0)
                {
                    go.GetComponent<SpriteRenderer>().sprite = BottomSprite;
                    go.GetComponent<HousePiece>().bottomFloor = true;
                }
                else if (k == Height - 1)
                    go.GetComponent<SpriteRenderer>().sprite = RoofSprites[Random.Range(0, RoofSprites.Length)];
                else
                    go.GetComponent<SpriteRenderer>().sprite = houseSprite;

                yield return new WaitForSeconds(0.15f);
            }
        }
    }
    public void ClearGame()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);
        for (int i = 0; i < ground.transform.childCount; i++)
            ground.transform.GetChild(i).GetComponent<DestroyableGround>()?.ResetDamage();
        StopAllCoroutines();
    }
    public void DestroyedPiece()
    {
        //calculate and add score
        if (transform.childCount == 1)
        {
            //other stuff about clearing level
            PlaystateManager.Instance.ChangeState(EnumStates.Pause);
        }
    }
    public void MissBuildings()
    {
        //Decrease multiplier
    }


}
