using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class UIController : MonoBehaviour
{
    public Toggle speedToggle;
    public Canvas speedCanvas;
    public Slider speedSlider;
    public Text speed;
    public Toggle radiusToggle;
    public Canvas radiusCanvas;
    public Slider radiusSlider;
    public Text radius;
    public Slider personalSpaceSlider;
    public Text personalSpace;
    public Slider interactionSlider;
    public Text interaction;
    public Text timer;
    public Button restartButton;
    public Text restart;
    public Button defaultSettings;
    public Slider agentTypeSlider;
    public Text agentType;
    public Slider repelSlider;
    public Text repel;
    public Slider gapSlider;
    public Text gapWidth;
    public Slider randomSlider;
    public Text random;
    public Slider agentSlider;
    public Text agent;
    public NavMeshSurface navMesh;
    public GameObject red;
    public GameObject blue;

    private GameObject[] reds;
    private GameObject[] blues;
    private GameObject[] agents;
    private GameObject[] walls;
    private bool hasStarted = false;
    private bool fixedSpeed;
    private bool fixedRadius;
    private bool agentSet;
    private bool agentTypeChanged;
    private float time;
    private int count = 0;
    private float previousSpeed;
    private float previousRadius;
    private float previousPersonalSpace;
    private float previousInteraction;
    private float previousAgentType;
    private float previousRepel;
    private float previousGap;
    private float previousRandomness;
    private float previousAgent;

    private void Start()
    {
        blues = GameObject.FindGameObjectsWithTag("Blue");
        reds = GameObject.FindGameObjectsWithTag("Red");
        agents = new GameObject[blues.Length + reds.Length];
        walls = GameObject.FindGameObjectsWithTag("Wall");
    }

    private void FixedUpdate()
    {
        if (hasStarted)
        {
            time += Time.deltaTime;
            timer.text = Mathf.Floor(time / 60).ToString("00") + ":" + (time % 60).ToString("00");
        }
        UpdateParameters();
    }

    private void UpdateParameters()
    {
        UpdateAgentType(false);
        UpdateSpeed();
        UpdateAgentSpeeds();
        UpdateRadius();
        UpdateAgentRadii();
        UpdateRandomness();
        UpdateAgent();
        UpdateRepel();
        UpdateInteraction();
        UpdateGapWidth();
        UpdatePersonalSpace();
        agentTypeChanged = false;

    }

    private void UpdateAgentType(bool restarted)
    {
        if (agentTypeSlider.value != previousAgentType || restarted)
        {
            if (agentTypeSlider.value == 1f)
            {
                agentType.text = "ALL";
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
            else if (agentTypeSlider.value == 2f)
            {
                agentType.text = "RED";
                agents = new GameObject[reds.Length];
                for (int i = 0; i < reds.Length; i++)
                {
                    agents[i] = reds[i];
                }
            }
            else
            {
                agentType.text = "BLUE";
                agents = new GameObject[blues.Length];
                for (int i = 0; i < blues.Length; i++)
                {
                    agents[i] = blues[i];
                }
            }
            agentTypeChanged = true;
            previousAgentType = agentTypeSlider.value;
        }
    }

    private void UpdateSpeed()
    {
        if (agentTypeChanged)
        {
            if (agentTypeSlider.value != 1f)
            {
                if (agents[1].GetComponent<Agent>().FixedSpeed())
                {
                    speedToggle.isOn = true;
                    speedCanvas.GetComponent<CanvasGroup>().alpha = 1f;
                    speedSlider.value = agents[1].GetComponent<Agent>().GetSpeed();
                    speed.text = (speedSlider.value / 10 + 0.4f).ToString();
                }
                else
                {
                    speedSlider.value = 10f;
                    speed.text = (speedSlider.value / 10 + 0.4f).ToString();
                    speedToggle.isOn = false;
                    speedCanvas.GetComponent<CanvasGroup>().alpha = 0f;
                    fixedSpeed = false;
                    previousSpeed = -1f;
                }
            }
            else
            {
                RevertToDefaultOptions();
            }
        }
        else if (speedToggle.isOn && speedSlider.value != previousSpeed)
        {
            speed.text = (speedSlider.value / 10 + 0.4f).ToString();
            speedCanvas.GetComponent<CanvasGroup>().alpha = 1f;
            for (int i = 0; i < agents.Length; i++)
            {
                Agent script = agents[i].GetComponent<Agent>();
                script.SetSpeed(speedSlider.value / 10 + 0.4f);
                script.SetFixedSpeed(true);
            }
            fixedSpeed = true;
            previousSpeed = speedSlider.value;
        }
        else if (!speedToggle.isOn && fixedSpeed)
        {
            speedCanvas.GetComponent<CanvasGroup>().alpha = 0f;
            for (int i = 0; i < agents.Length; i++)
            {
                Agent script = agents[i].GetComponent<Agent>();
                script.SetSpeed(script.GetInitialSpeed());
                script.SetFixedSpeed(false);
            }
            previousSpeed = -1f;
            fixedSpeed = false;
        }
    }

    private void UpdateAgentSpeeds()
    {
        if (count < 2)
        {
            if (speedToggle.isOn)
            {
                for (int i = 0; i < agents.Length; i++)
                {
                    Agent script = agents[i].GetComponent<Agent>();
                    script.SetSpeed(speedSlider.value / 10 + 0.4f);
                    script.SetFixedSpeed(true);
                }
            }
        }
    }

    private void UpdateRadius()
    {
        if (agentTypeChanged && hasStarted)
        {
            if (agentTypeSlider.value != 1f)
            {
                if (agents[1].GetComponent<Agent>().FixedRadius())
                {
                    radiusToggle.isOn = true;
                    radiusCanvas.GetComponent<CanvasGroup>().alpha = 1f;
                    radiusSlider.value = agents[1].GetComponent<Agent>().GetRadius();
                    radius.text = (radiusSlider.value / 20 + 0.15f).ToString();
                }
                else
                {
                    radiusSlider.value = 2f;
                    radius.text = (radiusSlider.value / 20 + 0.15f).ToString();
                    radiusToggle.isOn = false;
                    radiusCanvas.GetComponent<CanvasGroup>().alpha = 0f;
                    fixedRadius = false;
                    previousRadius = -1f;
                }
            }
            else
            {
                RevertToDefaultOptions();
            }
        }
        else if (radiusToggle.isOn && radiusSlider.value != previousRadius)
        {
            radius.text = (radiusSlider.value / 20 + 0.15f).ToString();
            radiusCanvas.GetComponent<CanvasGroup>().alpha = 1f;
            for (int i = 0; i < agents.Length; i++)
            {
                Agent script = agents[i].GetComponent<Agent>();
                script.SetRadius(radiusSlider.value / 20 + 0.15f);
                script.SetMass(script.GetRadius() * 244.8f);
                script.SetFixedRadius(true);
            }
            fixedRadius = true;
            previousRadius = radiusSlider.value;
        }
        else if (!radiusToggle.isOn && fixedRadius)
        {
            radiusCanvas.GetComponent<CanvasGroup>().alpha = 0f;
            for (int i = 0; i < agents.Length; i++)
            {
                Agent script = agents[i].GetComponent<Agent>();
                script.SetRadius(script.GetInitialRadius());
                script.SetMass(script.GetInitialMass());
                script.SetFixedRadius(false);
            }
            previousRadius = -1f;
            fixedRadius = false;
        }
    }

    private void UpdateAgentRadii()
    {
        if (count < 2)
        {
            if (radiusToggle.isOn)
            {
                for (int i = 0; i < agents.Length; i++)
                {
                    Agent script = agents[i].GetComponent<Agent>();
                    script.SetRadius(radiusSlider.value / 20 + 0.15f);
                    script.SetMass(script.GetRadius() * 244.8f);
                    script.SetFixedRadius(true);
                }
            }
            count++;
        }
    }

    private void UpdatePersonalSpace()
    {
        if (personalSpaceSlider.value != previousPersonalSpace)
        {
            personalSpace.text = (personalSpaceSlider.value / 10).ToString();
            for (int i = 0; i < agents.Length; i++)
            {
                Agent script = agents[i].GetComponent<Agent>();
                script.SetPersonalSpace(personalSpaceSlider.value / 10);
            }
            previousPersonalSpace = personalSpaceSlider.value;
        }
        if (agentTypeChanged && hasStarted)
        {
            if (agentTypeSlider.value != 1f)
            {
                personalSpaceSlider.value = agents[1].GetComponent<Agent>().GetPersonalSpace();
                personalSpace.text = (personalSpaceSlider.value / 10).ToString();
            }
            else
            {
                RevertToDefaultOptions();
            }
        }
    }

    private void UpdateInteraction()
    {
        if (interactionSlider.value != previousInteraction)
        {
            interaction.text = (interactionSlider.value * 100).ToString();
            for (int i = 0; i < agents.Length; i++)
            {
                Agent script = agents[i].GetComponent<Agent>();
                if (script.HasArrived())
                {
                    script.SetInteractionStrength(interactionSlider.value * 20);
                }
                else
                {
                    script.SetInteractionStrength(interactionSlider.value * 100);
                }
            }
            previousInteraction = interactionSlider.value;
        }
        if (agentTypeChanged && hasStarted)
        {
            if (agentTypeSlider.value != 1f)
            {
                if (!agents[1].GetComponent<Agent>().HasArrived())
                {
                    interactionSlider.value = agents[1].GetComponent<Agent>().GetInteractionStrength();
                    interaction.text = (interactionSlider.value * 100).ToString();
                }
                else
                {
                    interactionSlider.value = agents[1].GetComponent<Agent>().GetInteractionStrength() / 0.2f;
                    interaction.text = (interactionSlider.value * 100).ToString();
                }
            }
            else
            {
                RevertToDefaultOptions();
            }
        }
    }

    private void UpdateRepel()
    {
        if (repelSlider.value != previousRepel)
        {
            if (repelSlider.value == 1f)
            {
                repel.text = "None";
                for (int i = 0; i < agents.Length; i++)
                {
                    Agent script = agents[i].GetComponent<Agent>();
                    script.SetRepelAgents(false);
                    script.SetRepelAllAgents(false);
                    script.SetupAgents();
                }
            }
            else if (repelSlider.value == 2f)
            {
                repel.text = "All";
                for (int i = 0; i < agents.Length; i++)
                {
                    Agent script = agents[i].GetComponent<Agent>();
                    script.SetRepelAgents(true);
                    script.SetRepelAllAgents(true);
                    script.SetupAgents();
                }
            }
            else if (repelSlider.value == 3f)
            {
                repel.text = "Opposite";
                for (int i = 0; i < agents.Length; i++)
                {
                    Agent script = agents[i].GetComponent<Agent>();
                    script.SetRepelAgents(true);
                    script.SetRepelAllAgents(false);
                    script.SetupAgents();
                }
            }
            previousRepel = repelSlider.value;
        }
        if (agentTypeChanged && hasStarted)
        {
            if (agentTypeSlider.value != 1f)
            {
                repelSlider.value = agents[1].GetComponent<Agent>().GetRepel();
                if (repelSlider.value == 1f)
                {
                    repel.text = "None";
                }
                else if (repelSlider.value == 2f)
                {
                    repel.text = "All";
                }
                else
                {
                    repel.text = "Opposite";
                }
            }
            else
            {
                RevertToDefaultOptions();
            }
        }
    }

    private void UpdateGapWidth()
    {
        if (gapSlider.value != previousGap)
        {
            gapWidth.text = (gapSlider.value / 4).ToString();
            previousGap = gapSlider.value;
            
        }
    }

    private void UpdateRandomness()
    {
        if (randomSlider.value != previousRandomness)
        {
            random.text = (randomSlider.value * 500).ToString();
            for (int i = 0; i < agents.Length; i++)
            {
                Agent script = agents[i].GetComponent<Agent>();
                script.SetRandomness(randomSlider.value * 500);
            }
            previousRandomness = randomSlider.value;
        }
        if (agentTypeChanged && hasStarted)
        {
            if (agentTypeSlider.value != 1f)
            {
                randomSlider.value = agents[1].GetComponent<Agent>().GetRandomness();
                random.text = (randomSlider.value * 500).ToString();
            }
            else
            {
                RevertToDefaultOptions();
            }
        }
    }

    private void UpdateAgent()
    {
        if (agentSlider.value != previousAgent)
        {
            agent.text = (agentSlider.value).ToString();
            previousAgent = agentSlider.value;
        }
    }

    private void CreateAgents(int numAgents)
    {
        for (int i = 0; i < numAgents; i++)
        {
            if (i % 2 == 0)
            {
                Instantiate(red, new Vector3(0.5f + (i / 2) % 5, 1.2f, 9.5f - Mathf.Floor(i / 10)), Quaternion.Euler(0f, 180f, 0f));
                Instantiate(blue, new Vector3(0.5f + (i / 2) % 5, 1.2f, -9.5f + Mathf.Floor(i / 10)), Quaternion.Euler(0f, 0f, 0f));
            }
            else
            {
                Instantiate(red, new Vector3(-0.5f - Mathf.Floor(i / 2) % 5, 1.2f, 9.5f - Mathf.Floor(i / 10)), Quaternion.Euler(0f, 180f, 0f));
                Instantiate(blue, new Vector3(-0.5f - Mathf.Floor(i / 2) % 5, 1.2f, -9.5f + Mathf.Floor(i / 10)), Quaternion.Euler(0f, 0f, 0f));
            }
        }
        blues = GameObject.FindGameObjectsWithTag("Blue");
        reds = GameObject.FindGameObjectsWithTag("Red");
        agentTypeSlider.value = 1f;
        agents = new GameObject[blues.Length + reds.Length];
        for (int i = 0; i < blues.Length; i++)
        {
            agents[i] = blues[i];
        }
        for (int i = blues.Length; i < agents.Length; i++)
        {
            agents[i] = reds[i - blues.Length];
        }
        for (int i = 0; i < agents.Length; i++)
        {
            Agent script = agents[i].GetComponent<Agent>();
            script.SetInteractionStrength(interactionSlider.value * 100);
            script.SetPersonalSpace(personalSpaceSlider.value / 10);
            script.SetRandomness(randomSlider.value * 500);
            if (repelSlider.value == 1f)
            {
                script.SetRepelAgents(false);
                script.SetRepelAllAgents(false);
            }
            if (repelSlider.value == 2f)
            {
                script.SetRepelAgents(true);
                script.SetRepelAllAgents(true);
            }
            if (repelSlider.value == 3f)
            {
                script.SetRepelAgents(true);
                script.SetRepelAllAgents(false);
            }
        }
        count = 0;
    }

    public void RestartSimulation()
    {
        if (hasStarted)
        {
            time = 0f;
            agents = new GameObject[blues.Length + reds.Length];
            for (int i = 0; i < blues.Length; i++)
            {
                agents[i] = blues[i];
            }
            for (int i = blues.Length; i < agents.Length; i++)
            {
                agents[i] = reds[i - blues.Length];
            }
            if (agentSlider.value != agents.Length / 2)
            {
                for (int i = 0; i < agents.Length; i++)
                {
                    DestroyImmediate(agents[i]);
                }
                CreateAgents((int)agentSlider.value);
            }
            else
            {
                for (int i = 0; i < agents.Length; i++)
                {
                    Agent script = agents[i].GetComponent<Agent>();
                    script.Restart();
                }
                if (agentType.text != "ALL")
                {
                    UpdateAgentType(true);
                }
            }
            float gap = gapSlider.value / 4;
            for (int i = 0; i < walls.Length; i++)
            {
                walls[i].transform.localScale = new Vector3((10 - gap) / 2 * 0.25f, 1f, 1f);
            }
            navMesh.BuildNavMesh();
        }
        else
        {
            restart.text = "Restart";
            time = 0f;
            CreateAgents((int)agentSlider.value);
            float gap = gapSlider.value / 4;
            for (int i = 0; i < walls.Length; i++)
            {
                walls[i].transform.localScale = new Vector3((10 - gap) / 2 * 0.25f, 1f, 1f);
            }
            navMesh.BuildNavMesh();
            hasStarted = true;
        }
    }

    public void RevertToDefaultOptions()
    {
        speedSlider.value = 10f;
        speedToggle.isOn = false;
        radiusSlider.value = 2f;
        radiusToggle.isOn = false;
        personalSpaceSlider.value = 3f;
        interactionSlider.value = 15f;
        agentTypeSlider.value = 1f;
        repelSlider.value = 2f;
        gapSlider.value = 6f;
        randomSlider.value = 1f;
    }
}
