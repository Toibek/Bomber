using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airplane : MonoBehaviour
{
    public GameObject prefab_Bomb;
    [SerializeField] bool SpeedPerLap = default;
    [SerializeField] bool DropPerLap = default;

    internal float Speed;
    internal float dropSpeed;
    internal int lapsBeforeDrop;
    internal int BombsPerLap;

    bool started = false;
    float edge;
    Bomb[] bombs;
    private void Start()
    {
        edge = Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelWidth, 0)).x;
        transform.GetChild(0).transform.localPosition = new Vector2(-edge * 2, 0);
        transform.GetChild(1).transform.localPosition = new Vector2(edge * 2, 0);
    }
    public void StartAirplane() 
    {
        started = true;

        bombs = new Bomb[BombsPerLap];

        for (int i = 0; i < bombs.Length; i++)
        {
            if (bombs[i] == null)
                bombs[i] = Instantiate(prefab_Bomb, new Vector3(transform.position.x, transform.position.y - 0.3f), Quaternion.identity, transform).GetComponent<Bomb>();
        }
    }
    private void Update()
    {
        if (started)
        {
            if (lapsBeforeDrop > 0)
                transform.position += new Vector3(Speed * Time.deltaTime, 0);
            else
                transform.position += new Vector3(Speed * Time.deltaTime, -dropSpeed * Time.deltaTime);

            if (transform.position.x > edge)
            {
                transform.position = new Vector3(-edge, transform.position.y);
                GameManager.Instance.CheckForCompletion();

                if(SpeedPerLap)
                    Speed = GameManager.Instance.CalculateSpeed();
                if (DropPerLap)
                    dropSpeed = GameManager.Instance.CalculateDrop();

                lapsBeforeDrop--;
                for (int i = 0; i < bombs.Length; i++)
                {
                    if (bombs[i] == null)
                        bombs[i] = Instantiate(prefab_Bomb, new Vector3(transform.position.x, transform.position.y - 0.3f), Quaternion.identity, transform).GetComponent<Bomb>();
                }
            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (Mathf.Abs(Mathf.Round(transform.position.x)) <= Mathf.Floor(GameManager.Instance.Houses / 2))
                {
                    for (int i = 0; i < bombs.Length; i++)
                    {
                        if (bombs[i] != null)
                        {
                            bombs[i].transform.SetParent(null);
                            bombs[i].transform.position = new Vector3(Mathf.Round(bombs[i].transform.position.x), bombs[i].transform.position.y);
                            bombs[i].Drop();
                            bombs[i].Speed = Speed;
                            bombs[i] = null;
                            break;
                        }
                    }
                }
            }
        }
    }
    public static Vector3 v3Round(Vector3 roundMe) => new Vector3(Mathf.Round(roundMe.x), Mathf.Round(roundMe.y), Mathf.Round(roundMe.z));
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<HousePiece>() || collision.GetComponent<DestroyableGround>())
        {
            Crash();
        }
    }
    public void Crash()
    {
        if (started)
        {
            started = false;
            GameManager.Instance.CrashPlane();
            Destroy(gameObject);
        }
    }
}
