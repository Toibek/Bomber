using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] int Health = default;
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
            if(health == Health)
            {
                GameManager.Instance.MissBuildings();
            }
            Destroy(gameObject);
        }
    }
}
