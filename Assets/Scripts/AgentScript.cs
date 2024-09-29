using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;

public class AgentScript : Agent
{
    // private Rigidbody rBody; // Commented out to stop using Rigidbody for movement
    public float speed = 10f;
    private bool isGrounded;
    private int consecutiveCatches = 0;
    private float consecutiveCatchReward = 1.0f;
    private int objectsFallen = 0;
    public int maxObjectsPerEpisode = 5;

    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;

    private void Start()
    {
        // rBody = GetComponent<Rigidbody>();
    }

    public override void Initialize()
    {
        // rBody = GetComponent<Rigidbody>(); 
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition);
        // sensor.AddObservation(rBody.velocity); // Commented out since we aren't using Rigidbody
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        float moveX = actionBuffers.ContinuousActions[0];
        float moveZ = actionBuffers.ContinuousActions[1];

        // Use localPosition to move the agent
        Vector3 move = new Vector3(moveX, 0, moveZ) * speed * Time.deltaTime;
        transform.localPosition += move; // Update the agent's position

        // Uncomment the following if you want to apply some form of damping or friction
        // if (rBody) // Check if we are still using Rigidbody
        // {
        //     rBody.AddForce(move, ForceMode.VelocityChange);
        // }
    }

    public override void OnEpisodeBegin()
    {
        this.transform.localPosition = new Vector3(0, 0.5f, 0);
        // rBody.velocity = Vector3.zero; // Commented out since we aren't using Rigidbody

        objectsFallen = 0;
        consecutiveCatches = 0;
        consecutiveCatchReward = 1.0f;

        // Reset time scale
        Time.timeScale = 2f;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Slow down simulation for heuristics
        Time.timeScale = 0.5f;  // Set time scale to half speed

        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }

    public void ApplyFallingObjectPenalty()
    {
        Console.WriteLine("Falling object penalty applied");
        objectsFallen++;
        AddReward(-1.0f);
        meshRenderer.material = loseMaterial;

        if (objectsFallen >= maxObjectsPerEpisode)
        {
            EndEpisode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("wall"))
        {
            AddReward(-0.5f);
            meshRenderer.material = loseMaterial;
            // rBody.velocity = Vector3.zero; // Commented out
            EndEpisode();
        }
        if (other.gameObject.CompareTag("target"))
        {
            AddReward(consecutiveCatchReward);
            meshRenderer.material = winMaterial;

            consecutiveCatches++;
            consecutiveCatchReward = Mathf.Min(consecutiveCatchReward + 0.2f, 3.0f);

            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            isGrounded = true;
        }


    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            isGrounded = false;
        }
    }
}