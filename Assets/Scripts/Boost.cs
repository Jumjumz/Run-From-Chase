using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// NOTE: DONT SET spawnInterval and spawnTime to zero if you dont want your pc lagging or hanging xD
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
        /*if(boostSpawner.IsDestroyed()) // check if game object is destroyed
        {
			InvokeRepeating("SpawnBoost", spawnTime * Time.deltaTime, spawnInterval); // spawn at start
		}*/
		 
	}

    // Update is called once per frame
    void Update()
    {
	
		InvokeRepeating("SpawnBoost", spawnTime, spawnInterval); // spawn at start
        print(boostSpawner.IsDestroyed());
		// InvokeRepeating("SpawnBoost", 5f, spawnInterval); // spawn every frame
	}

	private void OnTriggerEnter(Collider triggerBoost) // collider function. This is what it looks all the time except triggerBoost this shit is made up
	{
		if (triggerBoost.tag.StartsWith("Player")) // check the tag of the collider (make sure you create a tag first before doing this shit)
		{
            Destroy(boostSpawner);
		}
	}

	void SpawnBoost()
    {
        Vector3 spwanPosition = new Vector3(Random.Range(-spawnAreaSize, spawnHeight), spawnHeight, Random.Range(-spawnAreaSize, spawnAreaSize));

        Instantiate(boostSpawner, spwanPosition, Quaternion.identity);
    }
}
