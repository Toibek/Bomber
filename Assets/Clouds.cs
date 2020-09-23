using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clouds : MonoBehaviour
{
    [SerializeField] int cloudAmount = default;
    [SerializeField] Vector2 cloudHeight = default;
    [SerializeField] Vector2 cloudSpeed = default;
    [SerializeField] Sprite[] cloudSprites = default;
    List<float> speeds = new List<float>();
    List<GameObject> clouds = new List<GameObject>();
    private void Start()
    {
        for (int i = 0; i < cloudAmount; i++)
        {
            SpawnCloud(true);
        }
    }
    private void Update()
    {
        for (int i = clouds.Count - 1; i >= 0; i--)
        {
            clouds[i].transform.position -= new Vector3(speeds[i] * Time.deltaTime, 0, 0);
            if (clouds[i].transform.position.x <= Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x - 1)
            {
                GameObject go = clouds[i];
                clouds.Remove(go);
                speeds.RemoveAt(i);
                Destroy(go);
                SpawnCloud();
            }
        }
    }
    private void SpawnCloud(bool anywhere = false)
    {
        GameObject go = new GameObject("Cloud");
        go.transform.parent = transform;

        if (anywhere) 
        {
            float width = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.scaledPixelWidth, 0,0)).x+1;
            go.transform.position = new Vector3(Random.Range(-width, width), Random.Range(cloudHeight.x, cloudHeight.y), 0);
        }
        else
            go.transform.position = new Vector2(Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.scaledPixelWidth, 0, 0)).x + 1, Random.Range(cloudHeight.x, cloudHeight.y));
        
        SpriteRenderer rend = go.AddComponent<SpriteRenderer>();
        rend.sprite = cloudSprites[Random.Range(0, cloudSprites.Length)];
        rend.sortingOrder = -100;
        float speed = Random.Range(cloudSpeed.x, cloudSpeed.y);
        clouds.Add(go);
        speeds.Add(speed);
    }
}
