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
    bool dropped;
    private void Start()
    {
        health = Health;
    }
    public void Drop() => dropped = true;
    private void Update()
    {
        if (dropped)
            transform.position += new Vector3(0, -Speed * Time.deltaTime);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (dropped)
        {
            if (collision.GetComponent<HousePiece>())
            {
                if (health == 0)
                {
                    if (!ToDestroy.Contains(collision.GetComponent<HousePiece>()))
                    {
                        ToDestroy.Add(collision.GetComponent<HousePiece>());

                        if (collision.GetComponent<HousePiece>().bottomFloor)
                            Instantiate(prefab_Particles, transform.position - new Vector3(0, 0), Quaternion.identity);
                        else
                            Instantiate(prefab_Particles, transform.position - new Vector3(0, 0.5f), Quaternion.identity);

                        GameManager.Instance.BombUsed(false);
                        SoundManager.Play(explosionSound);

                        ToDestroy.Sort(HousePiece.Compare);

                        for (int i = 0; i < ToDestroy.Count; i++)
                        {
                            if (i == ToDestroy.Count - 1)
                                ToDestroy[i].BreakPiece();
                            else
                                ToDestroy[i].DestroyPiece();
                        }
                        Destroy(gameObject);
                    }
                }
                else
                {
                    if (!ToDestroy.Contains(collision.GetComponent<HousePiece>()))
                    {
                        ToDestroy.Add(collision.GetComponent<HousePiece>());
                        health--;
                    }
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
        else if(collision.GetComponent<HousePiece>() || collision.GetComponent<DestroyableGround>())
        {
            Instantiate(prefab_Particles, transform.position - new Vector3(0, 0), Quaternion.identity);
            SoundManager.Play(explosionSound);

            Bomb[] bombs = transform.parent.GetComponentsInChildren<Bomb>();

            transform.parent.GetComponent<Airplane>().Crash();

            //for (int i = 0; i < bombs.Length; i++)
            //    if(bombs[i] != this)
            //        Destroy(bombs[i]);
            //Destroy(gameObject);
        }
    }
}
