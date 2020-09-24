using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float Speed;
    [SerializeField] int Health = default;
    [SerializeField] GameObject prefab_Particles = default;
    [SerializeField] AudioClip explosionSound = default;
    [SerializeField] List<HousePiece> ToDestroy = new List<HousePiece>();
    int health;
    private void Start()
    {
        health = Health;
    }
    private void Update()
    {
        transform.position += new Vector3(0, -Speed * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<HousePiece>())
        {
            if (health == 0)
            {
                ToDestroy.Add(collision.GetComponent<HousePiece>());
                if (collision.GetComponent<HousePiece>().bottomFloor)
                    Instantiate(prefab_Particles, transform.position - new Vector3(0, 0), Quaternion.identity);
                else
                    Instantiate(prefab_Particles, transform.position - new Vector3(0,0.5f), Quaternion.identity);
                GameManager.Instance.BombUsed(false);
                SoundManager.Play(explosionSound);
                for (int i = 0; i < ToDestroy.Count; i++)
                {
                    if(i == ToDestroy.Count -1)
                        ToDestroy[i].BreakPiece();
                    else
                        ToDestroy[i].DestroyPiece();
                }
                Destroy(gameObject);
            }
            else
            {
                ToDestroy.Add(collision.GetComponent<HousePiece>());
                health--;
            }
        }
        if (collision.GetComponent<DestroyableGround>())
        {
            collision.GetComponent<DestroyableGround>().DealDamage();
            GameManager.Instance.BombUsed(health == Health);
            Instantiate(prefab_Particles, transform.position - new Vector3(0, 0), Quaternion.identity);
            SoundManager.Play(explosionSound);
            Destroy(gameObject);
        }
    }
}
