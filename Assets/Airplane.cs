using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airplane : MonoBehaviour
{
    public float Speed;
    public float dropSpeed;
    public GameObject prefab_Bomb;
    bool started = false;
    bool readyToBomb = true;
    float edge;
    private void Start()
    {
        edge = Mathf.Round(Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelWidth, 0)).x + 1);
    }
    public void StartAirplane() => started = true;
    private void Update()
    {
        if (started)
        {
            transform.position += new Vector3(Speed*Time.deltaTime,- dropSpeed*Time.deltaTime);
            if (transform.position.x > edge)
            {
                transform.position = new Vector3(-edge, transform.position.y);
                readyToBomb = true;
            }
            if (Input.GetKeyDown(KeyCode.Mouse0) && readyToBomb)
            {
                Instantiate(prefab_Bomb, v3Round(transform.position), Quaternion.identity);
                readyToBomb = false;
            }
        }
    }
    public static Vector3 v3Round(Vector3 roundMe) => new Vector3(Mathf.Round(roundMe.x), Mathf.Round(roundMe.y), Mathf.Round(roundMe.z));
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<HousePiece>() || collision.GetComponent<DestroyableGround>())
        {
            GameManager.Instance.CrashPlane();
            Destroy(gameObject);
        }
    }
}
