using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropObject : MonoBehaviour
{

    public GameObject objectPrefab;


    public float spawnAreaWidth = 10f;
    public float spawnAreaDepth = 10f;


    public float spawnHeight = 10f;


    public float spawnInterval = 2f;


    void Start()
    {

        StartCoroutine(SpawnObjects());
    }


    IEnumerator SpawnObjects()
    {
        while (true)
        {

            float randomX = Random.Range(-spawnAreaWidth / 2, spawnAreaWidth / 2);
            float randomZ = Random.Range(-spawnAreaDepth / 2, spawnAreaDepth / 2);
            Vector3 spawnPosition = new Vector3(randomX, spawnHeight, randomZ);


            Instantiate(objectPrefab, spawnPosition, Quaternion.identity);


            yield return new WaitForSeconds(spawnInterval);
        }
    }
}