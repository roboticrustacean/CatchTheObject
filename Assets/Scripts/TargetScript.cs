using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour
{
    [SerializeField] GameObject agent; // Reference to the agent GameObject
    private AgentScript agentScript; // Reference to the agent script


    // Start is called before the first frame update
    void Start()
    {
        // Get the AgentScript component from the agent GameObject
        agentScript = agent.GetComponent<AgentScript>();
    }

    // Use OnTriggerEnter to detect when the target enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        // Check if the target hit the ground trigger
        if (other.gameObject.CompareTag("GroundTrigger"))  // The trigger just above the ground
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
