using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavAgentNoRootMotion : MonoBehaviour {
    
    public AIWaypointNetwork waypointNetwork = null;
    public int CurrentIndex = 0;
    public bool HasPath = false;
    public bool pathPending = false;
    public bool pathStale = false;
    public NavMeshPathStatus PathStatus = NavMeshPathStatus.PathInvalid;

    public AnimationCurve jumpCurve = new AnimationCurve();

    private NavMeshAgent _navAgent = null;
    private Animator _animator = null;
    private float _originalMaxSpeed = 0;

	// Use this for initialization
	void Start () {
        _navAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        if (_navAgent)
            _originalMaxSpeed = _navAgent.speed;

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
        int turnOnSpot;

        HasPath = _navAgent.hasPath;
        pathPending = _navAgent.pathPending;
        pathStale = _navAgent.isPathStale;
        PathStatus = _navAgent.pathStatus;

        Vector3 crosss = Vector3.Cross(transform.forward, _navAgent.desiredVelocity.normalized);
        float horizontal = (crosss.y < 0) ? -crosss.magnitude : crosss.magnitude;
        horizontal = Mathf.Clamp(horizontal * 2.32f,-2.32f, 2.32f);

        if(_navAgent.desiredVelocity.magnitude <1.0f && Vector3.Angle(transform.forward,_navAgent.desiredVelocity)>10.0f)
        {
            _navAgent.speed = 0.1f;
            turnOnSpot = (int)Mathf.Sign(horizontal);

        }
        else       {
           // print("hello");
            _navAgent.speed = _originalMaxSpeed;
            turnOnSpot = 0;
        }

        _animator.SetFloat("Horizontal", horizontal, 0.1f, Time.deltaTime);
        _animator.SetFloat("Vertical", _navAgent.desiredVelocity.magnitude, 0.1f, Time.deltaTime);
        _animator.SetInteger("TurnOnSpot", turnOnSpot);
        
        /* if (_navAgent.isOnOffMeshLink)
         {
             StartCoroutine(Jump(1.0f));
             return;
         } */

        if ((_navAgent.remainingDistance<=_navAgent.stoppingDistance && !pathPending)|| PathStatus==NavMeshPathStatus.PathInvalid /* || PathStatus == NavMeshPathStatus.PathPartial */)
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
            _navAgent.transform.position = Vector3.Lerp(startPos, endPos, t) + (jumpCurve.Evaluate(t)*Vector3.up);
            time += Time.deltaTime;
            yield return null;
        }
        _navAgent.CompleteOffMeshLink();
    }
}
