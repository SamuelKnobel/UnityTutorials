using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

/// <summary>
///  A Humming Bird Machine Learnign Agent
/// </summary>
public class HummingBirdAgent : Agent
{

    [Tooltip("Force to apply when moving")]
    public float moveForce = 2f;


    [Tooltip("speed to pitch up or down")]
    public float pitchSpeed = 100f;

    [Tooltip(" Speed to rotate around the up axis")]
    public float yawSpeed = 100f;

    [Tooltip("Transform at the Tip of the beak")]
    public Transform beakTip;

    [Tooltip("The Agent's Camera")]
    public Camera agentCamera;

    [Tooltip("Whetger this is training mode or gameplay mode")]
    public bool trainingMode;


    //the rigid body of the agend
    new private Rigidbody rigidbody;

    //the flower area that the agent is in
    private FlowerArea flowerArea;

    // The neares flower tp the agent
    private Flower nearestFlower;

    // allows for smootger pitch changes
    private float smoothPitchChange = 0f;

    // allows for smootger yaw changes
    private float smoothYawChange = 0f;


    // Maximum angle that the bird can pitch up or down
    private const float MaxPitchAngle = 80f;

    // Maximum distance from the beak  tip tip toaccept nectar collision
    private const float BeakTipRadius = 0.008f;

    // Whether the agend is frozen (intentionally not flying)
    private bool frozen = false;


    /// <summary>
    /// the amount of nectar the agent has obtained this episode
    /// </summary>
    public float NectarOptained { get; private set; }

    /// <summary>
    /// initialize the agent
    /// </summary>
    public override void Initialize()
    {
        rigidbody = GetComponent<Rigidbody>();
        flowerArea = GetComponentInParent<FlowerArea>();

        // if not training mode, no max step, play forever
        if (!trainingMode) MaxStep = 0;
    }

    /// <summary>
    /// Reset the agent when an episode begins
    /// </summary>
    public override void OnEpisodeBegin()
    {
        if (trainingMode)
        {
            //Only reset flowers in training when there is one agnet per area
            flowerArea.ResetFlower();
        }
        // Reset Nectar obtained
        NectarOptained = 0f;

        // Zero out velocitis, so that movement stops before a new episode begins
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        // Default to spawn in front of a flower
        bool inFrontOfFlower = true;
        if (trainingMode)
        {
            // Spawn in front of flower 50% of the time during training
            inFrontOfFlower = UnityEngine.Random.value > .5f;
        }
        //move the agent to a new random position
        MoveToSafeRandomPosition(inFrontOfFlower);

        // Recalculate to nearest flower now that the agent has moved
        UpdateNearestFlower();
    }

    /// <summary>
    /// Called when an action is recived from the player input or the neural network
    /// 
    /// vectorAction[i] represents:
    /// Index 0: move vector x (+1 = right, -1 = left)
    /// Index 1: move vector y (+1 = up, -1 = down)
    /// Index 2: move vector z (+1 = forward, -1 = backward)
    /// Index 3: pitch angle (+1 = pitch up, -1 = pitch down)
    /// Index 4: yaw angle (+1 = yaw up, -1 = yaw down)
    /// </summary>
    /// <param name="vectorAction">the Actions to take</param>
    public override void OnActionReceived(float[] vectorAction)
    {
        // dont take actions if frozen
        if (frozen) return;

        //calculate Movement Vector
        Vector3 move = new Vector3(vectorAction[0], vectorAction[1], vectorAction[2]);

        //add force in the direction of the move vector
        rigidbody.AddForce(move * moveForce);

        // Get the current Rotation
        Vector3 rotationVector = transform.rotation.eulerAngles;

        // Calcluate pitch and yaw rotation
        float pitchChange = vectorAction[3];
        float yawChange = vectorAction[4];

        // Calculate smooth rotation changes
        smoothPitchChange = Mathf.MoveTowards(smoothPitchChange, pitchChange, 2f * Time.fixedDeltaTime);
        smoothYawChange = Mathf.MoveTowards(smoothYawChange, yawChange, 2f * Time.fixedDeltaTime);

        // Calculate new Pitch and yaw based on smoothed values
        // Clamp pitch to avoiud flipping upside down
        float pitch = rotationVector.x + smoothPitchChange * Time.fixedDeltaTime * pitchSpeed;
        if (pitch > 180f) pitch -= 360f;
        pitch = Mathf.Clamp(pitch, -MaxPitchAngle, MaxPitchAngle);

        float yaw = rotationVector.y + smoothYawChange * Time.fixedDeltaTime * yawSpeed;

        // apply the new rotation
        transform.rotation = Quaternion.Euler(pitch, yaw, 0);
    }
    /// <summary>
    /// Collect vector observations from the environment
    /// </summary>
    /// <param name="sensor">The vector sensor</param>
    public override void CollectObservations(VectorSensor sensor)
    {
        // if nearestFlower is null, observe an empty array and return early
        if (nearestFlower == null)
        {
            sensor.AddObservation(new float[10]);
            return;
        }

        // Observe the agens local rotation (4 observations)
        sensor.AddObservation(transform.localRotation.normalized);

        // get a Vector from the beak tip to the nearest flower
        Vector3 toFlower = nearestFlower.FlowerCenterPosition - beakTip.position;

        // observe a normalized vector pointin to the nearest flower (3 Observations)
        sensor.AddObservation(toFlower.normalized);

        //observe a dot product that indicates whether the beak tip is in front of the flower  (1 Observations)
        // (+1 means that the beak tup is direct in front of th flower , -1 means directly behind)
        sensor.AddObservation(Vector3.Dot(toFlower.normalized, -nearestFlower.FlowerUpVector.normalized));

        //observe a dot product that indicates whether the beak tip is pointing toward the flower (1 Observations)
        //(+1 means tha the beak is pointing directli at the flwoer , -1 means directly away)
        sensor.AddObservation(Vector3.Dot(beakTip.forward.normalized, -nearestFlower.FlowerUpVector.normalized));

        // observe the relative distanse from the beak tip to the flower (1 Observations)
        sensor.AddObservation(toFlower.magnitude / FlowerArea.AreaDiameter);
        
        // 10 total observations
    }

    /// <summary>
    /// When behaviour Type is set to "Heuristic Only" on the agents's behaviour parameters, 
    /// this function will be called. Its retunr values will be fed into
    /// <see cref="OnActionReceived(float[])"/> insted of using the neural network
    /// </summary>
    /// <param name="actionsOut">An Output Action Array</param>
    public override void Heuristic(float[] actionsOut)
    {
        // Create Placeholders for all movements/Turnings

        Vector3 forward = Vector3.zero;
        Vector3 left = Vector3.zero;
        Vector3 up = Vector3.zero;
        float pitch = 0f;
        float yaw = 0f;


        // Convert Keyboard input to movement and turning, 
        // All values shousl be between -1 and +1

        // Forward/ backward
        if (Input.GetKey(KeyCode.W)) forward = transform.forward;
        else if (Input.GetKey(KeyCode.S)) forward = -transform.forward;    
        
        // left/ right
        if (Input.GetKey(KeyCode.A)) left = -transform.right;
        else if (Input.GetKey(KeyCode.D)) left = transform.right;

        // up/ down
        if (Input.GetKey(KeyCode.E)) up = transform.up;
        else if (Input.GetKey(KeyCode.C)) up = -transform.up;

        // pitch up/ down
        if (Input.GetKey(KeyCode.UpArrow)) pitch = 1f;
        else if (Input.GetKey(KeyCode.DownArrow)) pitch = -1f;

        // turn left/right
        if (Input.GetKey(KeyCode.LeftArrow)) yaw = -1f;
        else if (Input.GetKey(KeyCode.RightArrow)) yaw = 1f;

        // Combine the movement vector and normalize
        Vector3 combined = (forward + left + up).normalized;

        //add the 3 movement values, pitch and yaw to the actionsOut array
        actionsOut[0] = combined.x;
        actionsOut[1] = combined.y;
        actionsOut[2] = combined.z;
        actionsOut[3] = pitch;
        actionsOut[4] = yaw;
    }
    /// <summary>
    /// Prevent the Agent from moving and taking actions
    /// </summary>
    public void FreezeAgent()
    {
        Debug.Assert(trainingMode == false, "Freeze/Unfreeze not supported in training");
        frozen = true;
        rigidbody.Sleep();
    }
    /// <summary>
    /// Resume Agent movement and actions
    /// </summary>
    public void UnFreezeAgent()
    {
        Debug.Assert(trainingMode == false, "Freeze/Unfreeze not supported in training");
        frozen = false;
        rigidbody.WakeUp();
    }

    /// <summary>
    ///  Move the agent to a safe random position(i.e. does not collide with anything
    ///  If in frobt of flower, also point the beak at the flower
    /// </summary>
    /// <param name="inFrontOfFlower">Whether to chose a spot in front of the flower</param>
    void MoveToSafeRandomPosition(bool inFrontOfFlower)
    {
        bool safePositionFound = false;
        int attemptsRemaining = 100; // Prevents an infinite loop
        Vector3 potentialPosition = Vector3.zero;
        Quaternion potenialRotation = new Quaternion();

        // Loop untils safe position is found or round out of attempts
        while (!safePositionFound && attemptsRemaining>0)
        {
            attemptsRemaining--;
            if (inFrontOfFlower)
            {
                Flower randomFlower = flowerArea.Flowers[UnityEngine.Random.Range(0, flowerArea.Flowers.Count)];
                // position 10-20 cm in front of the flower
                float distanceFromFlower = UnityEngine.Random.Range(.1f, .2f);
                potentialPosition = randomFlower.transform.position + randomFlower.FlowerUpVector * distanceFromFlower;

                // Point beak at flower (birds head is center of transform)
                Vector3 toFLower = randomFlower.FlowerCenterPosition - potentialPosition;
                potenialRotation = Quaternion.LookRotation(toFLower, Vector3.up);
            }
            else
            {
                // pick random heigth from Ground
                float height = UnityEngine.Random.Range(1.2f, 2.5f);

                //pick a random radius from the center of the area
                float radius =  UnityEngine.Random.Range(2f, 7f);    
                
                //pick a random direction rotated around the y axis
                Quaternion direction  =  Quaternion.Euler(0f, UnityEngine.Random.Range(-180f, 180f),0f);

                //combine height, radius direction to pick a potential position
                potentialPosition = flowerArea.transform.position + Vector3.up * height + direction * Vector3.forward * radius;

                //Choose and set random starting pitch and yaw
                float pitch = UnityEngine.Random.Range(-60f,60f);
                float yaw = UnityEngine.Random.Range(-180f,180f);
                potenialRotation = Quaternion.Euler(pitch, yaw, 0);
            }

            //Check to see if the agent will collide with anything
            Collider[] colliders = Physics.OverlapSphere(potentialPosition, 0.05f);

            // Safe position has been found if no colliders are overlapping
            safePositionFound = colliders.Length == 0;
        }
        Debug.Assert(safePositionFound,"Could not find a safe position to spawn");

        // set the positon and rotation
        transform.position = potentialPosition;
        transform.rotation = potenialRotation;
    }

    /// <summary>
    /// update the nearest flower tho the agent
    /// </summary>
    private void UpdateNearestFlower()
    {
        foreach (Flower flower in flowerArea.Flowers)
        {
            if (nearestFlower == null && flower.HasNectar)
            {
                //no current nearest flower and this flower has nectar, so set to this flower
                nearestFlower = flower;
            }
            else if(flower.HasNectar)
            {
                //Calculate Distance to this flower and distance to the current neares flower
                float distanceToFlower = Vector3.Distance(flower.transform.position, beakTip.position);
                float distanceToCurrentFlower = Vector3.Distance(nearestFlower.transform.position, beakTip.position);

                // if current nearest flower is empty or this flower is closer, update the nearest flower
                if (!nearestFlower.HasNectar|| distanceToFlower  < distanceToCurrentFlower)
                {
                    nearestFlower = flower;
                }
            }
        }
    }

    /// <summary>
    /// called when the agents collider enters a trigger collider
    /// </summary>
    /// <param name="other">The Trigger collider</param>
    private void OnTriggerEnter(Collider other)
    {
        TriggerEnterOrStay(other);
    }
    /// <summary>
    /// called when the agents collider stays in a trigger collider
    /// </summary>
    /// <param name="other">The Trigger collider</param>
    private void OnTriggerStay(Collider other)
    {
        TriggerEnterOrStay(other);
    }
    /// <summary>
    ///  Handerls when the agent's collider enters or stays in a trigger collider
    /// </summary>
    /// <param name="collider">The Trigger collider</param>
    private void TriggerEnterOrStay(Collider collider)
    {
        //Check if agent is colliding with nectar

        if (collider.CompareTag("nectar"))
        {
            Vector3 closestPointToBeakTip = collider.ClosestPoint(beakTip.position);

            // Check if th closest collision point is close to the beak tip
            // Note : a collition with anythin but the beak tip should not count
            if (Vector3.Distance(beakTip.position, closestPointToBeakTip)< BeakTipRadius)
            {
                // Look up the flower for this collider
                Flower flower = flowerArea.GetFlowerFromNectar(collider);

                // Attempt to take 0.01 nectar
                // Note: this i sper fixed timestep, meaning it happens every 0.02 seconds, or 50x per sec
                float nectarReceived = flower.Feed(0.01f);

                // Keep Track of Nectar obtained
                NectarOptained += nectarReceived;

                if (trainingMode)
                {
                    //calculate Rewaed for getting nectar
                    float bonus = 0.02f * Mathf.Clamp01(Vector3.Dot(transform.forward.normalized, -nearestFlower.FlowerUpVector.normalized));

                    AddReward(0.01f+bonus);
                }
                // if flower is empty update neares flower
                if (!flower.HasNectar)
                {
                    UpdateNearestFlower();
                }
            }
        }
    }

    /// <summary>
    /// called when the agent collides with something solid
    /// </summary>
    /// <param name="collision">Collision Info</param>
    private void OnCollisionEnter(Collision collision)
    {
        if (trainingMode&& collision.collider.CompareTag("boundary"))
        {
            // collided with the area boundary, give a negative reward
            AddReward(-.5f);
        }   
    }

    /// <summary>
    /// Called every frame
    /// </summary>
    private void Update()
    {
        // Draw a line from the beaktip to the neares flower
        if (nearestFlower != null)
        {
            Debug.DrawLine(beakTip.position, nearestFlower.FlowerCenterPosition, Color.green);
        }
    }/// <summary>
    /// called  every .02 sec
    /// </summary>
    private void FixedUpdate()
    {
        // avoid scenariop where neares flower is stolen by opponent and not updated
        if (nearestFlower != null && ! nearestFlower.HasNectar)
        {
            UpdateNearestFlower();
        }
    }

}
