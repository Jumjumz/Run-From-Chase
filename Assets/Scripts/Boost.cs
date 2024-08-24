using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Boost : MonoBehaviour
{
    public GameObject boostSpawner;
    public float spawnAreaSize;
    public float spawnHeight;
    public float spawnInterval;
    public float spawnTime;
    // Start is called before the first frame update
    void Start()
    {
		 InvokeRepeating("SpawnBoost",  spawnTime * Time.deltaTime, spawnInterval); // spawn at start
	}

    // Update is called once per frame
    void Update()
    {
		// InvokeRepeating("SpawnBoost", 5f, spawnInterval); // spawn every frame
	}

    void SpawnBoost()
    {
        Vector3 spwanPosition = new Vector3(Random.Range(-spawnAreaSize, spawnHeight), spawnHeight, Random.Range(-spawnAreaSize, spawnAreaSize));

        Instantiate(boostSpawner, spwanPosition, Quaternion.identity);
    }
}
