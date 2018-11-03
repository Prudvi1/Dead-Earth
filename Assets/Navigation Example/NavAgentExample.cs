using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavAgentExample : MonoBehaviour {

    public AIWaypointNetwork waypointNetwork = null;
    public int CurrentIndex = 0;
    public bool HasPath = false;
    public bool pathPending = false;
    public bool pathStale = false;
    public NavMeshPathStatus PathStatus;

    private NavMeshAgent _navAgent = null;
	// Use this for initialization
	void Start () {
        _navAgent = GetComponent<NavMeshAgent>();

       /* _navAgent.updatePosition = false;
        _navAgent.updateRotation = false; */

        if (waypointNetwork == null) return ;
        if (waypointNetwork.Waypoints[CurrentIndex] != null)

        setNextDestination(false);
    }
	void setNextDestination(bool increment)
    {
        if (!waypointNetwork) return;

        int incStep = increment ? 1 : 0;
        Transform nextWayPointTransform = null;

        int nextWayPoint = (CurrentIndex + incStep >= waypointNetwork.Waypoints.Count) ? 0 : CurrentIndex + incStep;
        nextWayPointTransform = waypointNetwork.Waypoints[nextWayPoint];

        if (nextWayPointTransform != null)
        {
            CurrentIndex = nextWayPoint;
            _navAgent.destination = nextWayPointTransform.position;
            return;
        }
        CurrentIndex++;
    }

	// Update is called once per frame
	void Update () {
        HasPath = _navAgent.hasPath;
        pathPending = _navAgent.pathPending;
        pathStale = _navAgent.isPathStale;
        PathStatus = _navAgent.pathStatus;
        if (_navAgent.isOnOffMeshLink)
        {
            StartCoroutine(Jump(2.0f));
            return;
        }

        if((_navAgent.remainingDistance<=_navAgent.stoppingDistance && !pathPending)|| PathStatus==NavMeshPathStatus.PathInvalid /* || PathStatus == NavMeshPathStatus.PathPartial */)
        {
            setNextDestination(true);
        }
        else if (_navAgent.isPathStale)
        {
            setNextDestination(false);
        }
	}

    IEnumerator Jump(float duration)
    {
        OffMeshLinkData data = _navAgent.currentOffMeshLinkData;
        Vector3 startPos = _navAgent.transform.position;
        Vector3 endPos = data.endPos + (_navAgent.baseOffset*Vector3.up);
        float time = 0.0f;

        while (time <= duration)
        {
            float t = time / duration;
            _navAgent.transform.position = Vector3.Lerp(startPos, endPos, t);
            time += Time.deltaTime;
            yield return null;
        }
        _navAgent.CompleteOffMeshLink();
    }
}
