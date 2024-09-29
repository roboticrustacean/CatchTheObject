using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropObject : MonoBehaviour
{
    [SerializeField] private GameObject objectPrefab; // Prefab to spawn
    [SerializeField] private GameObject agent; // Reference to the agent GameObject
    [SerializeField] private float spawnAreaWidth = 10f; // Width of the spawn area
    [SerializeField] private float spawnAreaDepth = 10f; // Depth of the spawn area
    [SerializeField] private float spawnHeight = 10f; // Height at which objects spawn
    [SerializeField] private float spawnInterval = 2f; // Interval between object spawns

    private AgentScript agentScript; // Reference to the AgentScript component

    void Start()
    {
        // Get the AgentScript component from the agent GameObject
        agentScript = agent.GetComponent<AgentScript>();

        // Start spawning objects at regular intervals
        StartCoroutine(SpawnObjects());
    }

    // Coroutine to spawn objects at intervals
    IEnumerator SpawnObjects()
    {
        while (true)
        {
            float randomX = Random.Range(-spawnAreaWidth / 2, spawnAreaWidth / 2);
            float randomZ = Random.Range(-spawnAreaDepth / 2, spawnAreaDepth / 2);
            Vector3 spawnPosition = new Vector3(randomX, spawnHeight, randomZ);

            // Instantiate the prefab
            GameObject newObject = Instantiate(objectPrefab, transform);

            // Set the local position of the newly instantiated object
            newObject.transform.localPosition = spawnPosition;

            // Attach the DropObject reference to the new object so it can detect when it hits the ground
            TargetBehavior targetBehavior = newObject.AddComponent<TargetBehavior>();
            targetBehavior.Initialize(agentScript); // Pass the agent script to the new object

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // Inner class for target behavior
    private class TargetBehavior : MonoBehaviour
    {
        private AgentScript agentScript;

        // Initialize method to pass the AgentScript reference
        public void Initialize(AgentScript agentScript)
        {
            this.agentScript = agentScript;
        }

        // Use OnTriggerEnter to detect when the target enters the trigger (GroundTrigger)
        private void OnTriggerEnter(Collider other)
        {
            // Check if the target hit the ground trigger
            if (other.gameObject.CompareTag("GroundTrigger"))
            {
                // Apply penalty before destroying the object
                if (agentScript != null)
                {
                    agentScript.ApplyFallingObjectPenalty();
                }

                // Destroy the target object
                Destroy(gameObject);
            }
        }
    }
}
