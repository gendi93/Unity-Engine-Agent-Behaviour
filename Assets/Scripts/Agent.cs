using UnityEngine;
using UnityEngine.AI;

public class Agent : MonoBehaviour
{
    private bool hasArrived = false;
    private bool repelAllAgents;
    private bool repelAgents;
    private bool fixedSpeed = false;
    private bool fixedRadius = false;
    private float randomness;
    private float walkingDistance = 15f;
    private float interactionStrength;
    private float personalSpace;
    private float radius;
    private float mass;
    private float turnSpeed = 5;
    private float B = 1;
    private float initialRadius;
    private float initialMass;
    private float initialSpeed;
    private Quaternion initialDirection;
    private Vector3 initialPosition;
    private Vector3 endPoint;
    private GameObject[] reds;
    private GameObject[] blues;
    private GameObject[] agents;
    private NavMeshAgent agent;

    private void Start()
    {
        Initiate();
    }

    private void Update()
    {
        SeekEndpoint();
    }

    private void FixedUpdate()
    {
        ApplyForces();
    }

    private void Initiate()
    {
        initialDirection = transform.rotation;
        initialPosition = transform.position;
        agent = GetComponent<NavMeshAgent>();
        endPoint = tag == "Red" ? transform.position - new Vector3(0f, 0f, walkingDistance) : transform.position + new Vector3(0f, 0f, walkingDistance);
        initialRadius = Mathf.Min(Random.Range(0f, 1f) * 0.1f + 0.25f, 0.35f);
        radius = initialRadius;
        agent.radius = radius;
        initialMass = radius * 244.8f;
        mass = initialMass;
        GetComponent<Rigidbody>().mass = mass;
        transform.localScale = new Vector3(2 * radius, 1f, 2 * radius);
        initialSpeed = Mathf.Min(Random.Range(0f, 1f) * 0.6f + 1.2f, 1.8f);
        agent.speed = initialSpeed;
        SetupAgents();
    }

    public void SetupAgents()
    {
        blues = GameObject.FindGameObjectsWithTag("Blue");
        reds = GameObject.FindGameObjectsWithTag("Red");
        if (repelAgents)
        {
            if (repelAllAgents)
            {
                agents = new GameObject[blues.Length + reds.Length];
                for (int i = 0; i < blues.Length; i++)
                {
                    agents[i] = blues[i];
                }
                for (int i = blues.Length; i < agents.Length; i++)
                {
                    agents[i] = reds[i - blues.Length];
                }
            }
            else
            {
                if (tag == "Red")
                {
                    agents = new GameObject[blues.Length];
                    for (int i = 0; i < blues.Length; i++)
                    {
                        agents[i] = blues[i];
                    }
                }
                else
                {
                    agents = new GameObject[reds.Length];
                    for (int i = 0; i < reds.Length; i++)
                    {
                        agents[i] = reds[i];
                    }
                }
            }
        }
    }

    private void SeekEndpoint()
    {
        if (Vector3.Distance(transform.position, endPoint) <= 0.1 && !hasArrived)
        {
            hasArrived = true;
            interactionStrength = interactionStrength * 0.2f;
        }
        else
        {
            agent.SetDestination(endPoint);
            transform.rotation = Quaternion.Lerp(transform.rotation, initialDirection, turnSpeed * Time.deltaTime);
        }
    }

    void ApplyForces()
    {
        if (repelAgents)
        {
            for (int i = 0; i < agents.Length; i++)
            {
                float distance = Vector3.Distance(transform.position, agents[i].transform.position);
                if (distance != 0)
                {
                    if (distance < (radius + personalSpace))
                    {
                        Vector3 repulsion = (transform.position - agents[i].transform.position).normalized * (interactionStrength * Mathf.Exp(((radius + (agents[i].transform.localScale.x)) - distance)) / B);
                        agents[i].GetComponent<Rigidbody>().AddForce(-repulsion);
                    }
                }
            }
        }
        if (Vector3.Distance(transform.position, endPoint) > 0.5)
        {
            Vector3 dir = (endPoint - transform.position).normalized;
            float mult = Mathf.Pow(-1f, Mathf.Round(Random.Range(0f, 1f)));
            GetComponent<Rigidbody>().AddForce(new Vector3(mult * dir.z, 0f, mult * -1f * dir.x) * Random.Range(0f, randomness));
        }
    }

    public void Restart()
    {
        if (hasArrived == true)
        {
            hasArrived = false;
            interactionStrength = interactionStrength / 0.2f;
        }
        transform.position = initialPosition;
    }

    public float GetInitialSpeed()
    {
        return initialSpeed;
    }

    public float GetInitialRadius()
    {
        return initialRadius;
    }

    public float GetInitialMass()
    {
        return initialMass;
    }

    public float GetSpeed()
    {
        return (agent.speed - 0.4f) * 10;
    }

    public float GetRadius()
    {
        return (radius - 0.15f) * 20;
    }

    public float GetRandomness()
    {
        return randomness / 500;
    }

    public float GetRepel()
    {
        return repelAgents ? repelAllAgents ? 2f : 3f : 1f;
    }

    public bool FixedSpeed()
    {
        return fixedSpeed;
    }

    public bool FixedRadius()
    {
        return fixedRadius;
    }

    public bool HasArrived()
    {
        return hasArrived;
    }

    public float GetPersonalSpace()
    {
        return personalSpace * 10;
    }

    public float GetInteractionStrength()
    {
        return interactionStrength / 100;
    }

    public void SetSpeed(float speed)
    {
        agent.speed = speed;
    }

    public void SetRadius(float radius)
    {
        this.radius = radius;
        agent.radius = this.radius;
        transform.localScale = new Vector3(2 * this.radius, 1f, 2 * this.radius);
    }

    public void SetMass(float mass)
    {
        this.mass = mass;
    }

    public void SetInteractionStrength(float interactionStrength)
    {
        this.interactionStrength = interactionStrength;
    }

    public void SetPersonalSpace(float personalSpace)
    {
        this.personalSpace = personalSpace;
    }

    public void SetRandomness(float randomness)
    {
        this.randomness = randomness;
    }

    public void SetRepelAgents(bool repelAgents)
    {
        this.repelAgents = repelAgents;
    }

    public void SetRepelAllAgents(bool repelAllAgents)
    {
        this.repelAllAgents = repelAllAgents;
    }

    public void SetFixedSpeed(bool fixedSpeed)
    {
        this.fixedSpeed = fixedSpeed;
    }

    public void SetFixedRadius(bool fixedRadius)
    {
        this.fixedRadius = fixedRadius;
    }
}