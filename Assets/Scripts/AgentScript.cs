using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;



public class AgentScript : Agent
{
    Rigidbody rBody;
    public float speed = 10f;
    //public float jumpForce = 5f;
    private bool isGrounded;
    private int consecutiveCatches = 0;
    private float consecutiveCatchReward = 1.0f;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;

    // find ObjectSpawner object and get DropObject script
    DropObject dropObject;

    private void Start()
    {
        dropObject = FindObjectOfType<DropObject>();
    }

    public override void Initialize()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(rBody.velocity);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        float moveX = actionBuffers.ContinuousActions[0];
        float moveZ = actionBuffers.ContinuousActions[1];
        float jump = actionBuffers.ContinuousActions[2];

        Vector3 move = new Vector3(moveX, 0, moveZ) * speed * Time.deltaTime;
        rBody.AddForce(move, ForceMode.VelocityChange);

        //if (isGrounded && jump > 0.5f)
        //{
        //    rBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        //}
    }


    public override void OnEpisodeBegin()
    {
        this.transform.localPosition = new Vector3(0, 0.5f, 0);
        rBody.velocity = Vector3.zero;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
        //continuousActions[2] = Input.GetKey(KeyCode.Space) ? 1f : 0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            isGrounded = true;
        }


        if (collision.gameObject.CompareTag("target"))
        {

            AddReward(consecutiveCatchReward);
            meshRenderer.material = winMaterial;
            consecutiveCatches++;

            // Bir sonraki yakalama ödülünü 0.2 artýr, maksimum ödül 3.0 olabilir
            consecutiveCatchReward = Mathf.Min(consecutiveCatchReward + 0.2f, 3.0f);


            Destroy(collision.gameObject);
            //dropObject.SpawnNewObject();
        }


        if (collision.gameObject.CompareTag("wall"))
        {
            meshRenderer.material = loseMaterial;
            AddReward(-0.5f);

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            isGrounded = false;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("target"))
        {

            AddReward(-1.0f);


            consecutiveCatches = 0;
            consecutiveCatchReward = 1.0f;

            meshRenderer.material = loseMaterial;


            Destroy(other.gameObject);
            //dropObject.SpawnNewObject();
        }
    }


}
