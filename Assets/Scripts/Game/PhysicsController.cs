using UnityEngine;

public class PhysicsController : MonoBehaviour
{
    [SerializeField]
    private bool simulate = true;

    public bool Simulate
    {
        get { return simulate; }
        set { simulate = value; }
    }

    private void Start()
    {
        Physics2D.simulationMode = SimulationMode2D.Script;
    }

    void FixedUpdate()
    {
        if (simulate) Physics2D.Simulate(0.02f);
    }

    private void OnDisable()
    {
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate; //default back to FixedUpdate simulation
    }
}