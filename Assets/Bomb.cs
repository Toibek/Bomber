using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] int Health = default;
    [SerializeField] GameObject prefab_Particles;
    int health;
    private void Start()
    {
        health = Health;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<HousePiece>())
        {
            if (health == 0)
            {
                collision.GetComponent<HousePiece>().BreakPiece();
                if(collision.GetComponent<HousePiece>().bottomFloor)
                    Instantiate(prefab_Particles, transform.position - new Vector3(0, 0), Quaternion.identity);
                else
                    Instantiate(prefab_Particles, transform.position - new Vector3(0,1), Quaternion.identity);
                GameManager.Instance.BombUsed(false);
                Destroy(gameObject);
            }
            else
            {
                collision.GetComponent<HousePiece>().DestroyPiece();
                health--;
            }
        }
        if (collision.GetComponent<DestroyableGround>())
        {
            collision.GetComponent<DestroyableGround>().DealDamage();
            GameManager.Instance.BombUsed(health == Health);
            Instantiate(prefab_Particles, transform.position - new Vector3(0, 0), Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
