using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousePiece : MonoBehaviour
{
    [SerializeField] Sprite[] BrokenPiece = default;
    internal bool bottomFloor;
    public void BreakPiece()
    {
        if (bottomFloor)
            DestroyPiece();
        else
        {
            GetComponent<SpriteRenderer>().sprite = BrokenPiece[Random.Range(0, BrokenPiece.Length)];
            GetComponent<BoxCollider2D>().offset = new Vector2(0, -0.4f);
            GetComponent<BoxCollider2D>().size = new Vector2(0.94f, 0.2f);
        }
    }
    public void DestroyPiece()
    {
        GetComponentInParent<GameManager>().DestroyedPiece();
        Destroy(gameObject);
    }

    public static int Compare(HousePiece x, HousePiece y) => x.transform.position.y < y.transform.position.y ? 1 : 0;
}
