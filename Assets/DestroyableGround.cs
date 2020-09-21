using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableGround : MonoBehaviour
{
    [SerializeField] Sprite[] Stages = default;
    int damage = 0;
    SpriteRenderer render;
    private void Start()
    {
        render = GetComponent<SpriteRenderer>();
        render.sprite = Stages[damage];
    }
    public void DealDamage()
    {
        damage = Mathf.Clamp(damage + 1, 0, Stages.Length-1);
        render.sprite = Stages[damage];
    }
    public void ResetDamage()
    {
        damage = 0;
        render.sprite = Stages[damage];
    }
}
