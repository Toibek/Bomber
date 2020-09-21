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
            GetComponent<SpriteRenderer>().sprite = BrokenPiece[Random.Range(0,BrokenPiece.Length)];
    }
    public void DestroyPiece()
    {
        GetComponentInParent<GameManager>().DestroyedPiece();
        Destroy(gameObject);
    }

}
