using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airplane : MonoBehaviour
{
    public GameObject prefab_Bomb;
    public int BombsPerLap = 2;

    internal float Speed;
    internal float dropSpeed;
    internal int lapsBeforeDrop;
    bool started = false;
    int bombs;
    float edge;
    private void Start()
    {
        edge = Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelWidth, 0)).x;
        transform.GetChild(0).transform.localPosition = new Vector2(-edge * 2, 0);
        transform.GetChild(1).transform.localPosition = new Vector2(edge * 2, 0);
        bombs = BombsPerLap;
    }
    public void StartAirplane() => started = true;
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
                lapsBeforeDrop--;
                bombs = BombsPerLap;
            }
            if (Input.GetKeyDown(KeyCode.Mouse0) && bombs > 0)
            {
                if (Mathf.Abs(Mathf.Round(transform.position.x)) <= Mathf.Floor(GameManager.Instance.Houses / 2))
                {
                    GameObject go = Instantiate(prefab_Bomb, new Vector3(Mathf.Round(transform.position.x), transform.position.y - 0.5f), Quaternion.identity);
                    go.GetComponent<Bomb>().Speed = Speed;
                    bombs--;
                }
            }
        }
    }
    public static Vector3 v3Round(Vector3 roundMe) => new Vector3(Mathf.Round(roundMe.x), Mathf.Round(roundMe.y), Mathf.Round(roundMe.z));
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<HousePiece>() || collision.GetComponent<DestroyableGround>())
        {
            if (started)
            {
                started = false;
                GameManager.Instance.CrashPlane();
                Destroy(gameObject);
            }
        }
    }
}
