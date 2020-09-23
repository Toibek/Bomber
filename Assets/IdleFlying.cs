using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleFlying : MonoBehaviour
{
    float edge;
    private void Start()
    {
        edge = Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelWidth, 0)).x;
        transform.GetChild(0).transform.localPosition = new Vector2(-edge * 2, 0);
        transform.GetChild(1).transform.localPosition = new Vector2(edge * 2, 0);
    }
    private void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            transform.position += new Vector3(1 * Time.deltaTime,0);
            if(transform.position.x > edge)
            {
                transform.position = new Vector3(-edge, transform.position.y);
            }
        }
    }
}
