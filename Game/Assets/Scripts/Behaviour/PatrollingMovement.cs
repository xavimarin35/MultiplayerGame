using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrollingMovement : MonoBehaviour
{
    // Patrolling Movement Variables
    private NavMeshAgent agent;
    private int currentWaypoint;
    private bool isTravelling;
    private bool nextWaypoint = false;
    private Vector3 target;

    // Public
    public List<WayPoints> waypoints; // List of all waypoints in the game

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (waypoints != null && waypoints.Count >= 2)
        {
            currentWaypoint = 0;
            SetDestination();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isTravelling && agent.remainingDistance <= 0.5f)
        {
            isTravelling = false;

            ChangePatrolPoint();
            SetDestination();  
        }
    }

    private void SetDestination()
    {
        if (waypoints != null)
        {
            target = waypoints[currentWaypoint].transform.position;
            agent.SetDestination(target);
            isTravelling = true;
        }
    }

    private void ChangePatrolPoint()
    {
        if (nextWaypoint)
            currentWaypoint = (currentWaypoint + 1) % waypoints.Count;
        else
        {
            if (--currentWaypoint < 0)
                currentWaypoint = waypoints.Count - 1;
        }
    }
}
