using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Boost : MonoBehaviour
{
    public GameObject boostSpawner;
    public float spawnAreaSize;
    public float spawnHeight;
    public float spawnInterval;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnBoost", Time.deltaTime, spawnInterval);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawBoost()
    {
        Vector3 spwanPosition = new Vector3(Random.Range(-spawnAreaSize, spawnHeight), spawnHeight, Random.Range(-spawnAreaSize, spawnAreaSize));

        Instantiate(boostSpawner, spwanPosition, Quaternion.identity);
    }
}
