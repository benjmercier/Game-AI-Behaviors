using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleFollowing : MonoBehaviour
{
    private static Vector3 _circleCenter = Vector3.zero;
    public static Vector3 CircleCenter { get { return _circleCenter; } }
    private static float _circleRadius = 10f;
    public static float CircleRadius { get { return _circleRadius; } }

    private GameObject _cylinder;

    #region Waypoints
    [Header("Waypoints")]
    [SerializeField]
    private int _totalWaypoints = 10;
    [SerializeField]
    private List<Vector3> _waypointList = new List<Vector3>();
    private Vector3 _waypoint;
    private int _waypointIndex = 0;
    private int _previousIndex;

    private float _deltaTheta;
    private float _theta;
    private float _xPos;
    private float _zPos;
    #endregion

    #region Seek Steering Behavior
    [Header("Steering")]
    [SerializeField]
    private float _maxVelocity = 10;
    [Range(0f, 1f), SerializeField]
    private float _maxForce = 0.5f;
    [SerializeField]
    private float _approachRadius = 2.5f;
    private float _approachDistance;

    private Vector3 _desiredVelocity;
    private Vector3 _velocity;
    private Vector3 _steering;
    #endregion

    private void Start()
    {
        _velocity = Vector3.zero;

        CreatePath(_totalWaypoints, _circleRadius);
    }

    private void Update()
    {
        DrawPath();        
    }

    private void FixedUpdate()
    {
        transform.forward = FollowPath();
    }

    private void CreatePath(int points, float radius)
    {
        _deltaTheta = (2f * Mathf.PI) / points;
        _theta = 0f;

        for (int i = 0; i < points + 1; i++)
        {
            _xPos = radius * Mathf.Cos(_theta);
            _zPos = radius * Mathf.Sin(_theta);

            // aligns to center of circle
            _xPos += _circleCenter.x; 
            _zPos += _circleCenter.z;

            _waypoint = new Vector3(_xPos, 0, _zPos);

            _waypointList.Add(_waypoint);
            
            _cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            _cylinder.transform.position = _waypoint;
            if (_cylinder.TryGetComponent(out Collider collider))
            {
                collider.isTrigger = true;
            }
            
            _theta += _deltaTheta;
        }
    }

    private void DrawPath()
    {
        Debug.DrawLine(transform.position, _waypointList[_waypointIndex], Color.green);

        for (int i = 0; i < _waypointList.Count - 1; i++)
        {
            Debug.DrawLine(_waypointList[i], _waypointList[i + 1], Color.blue);
        }
        
    }

    private Vector3 Seek(Vector3 point)
    {
        _desiredVelocity = point - transform.position;
        _approachDistance = _desiredVelocity.magnitude;
        _desiredVelocity.Normalize();

        if (_approachDistance < _approachRadius)
        {
            _desiredVelocity *= _approachDistance / _approachRadius * _maxVelocity;
        }
        else
        {
            _desiredVelocity *= _maxVelocity;
        }

        _steering = _desiredVelocity - _velocity;
        _steering = Vector3.ClampMagnitude(_steering, _maxForce);
        
        return _steering;
    }

    private Vector3 FollowPath()
    {
        if (Vector3.Distance(transform.position, _waypointList[_waypointIndex]) <= 1)
        {
            if (_waypointList.IndexOf(_waypointList[_waypointIndex]) < _waypointList.Count - 1)
            {
                _previousIndex = _waypointIndex;
                _waypointIndex++;
            }
            else
            {
                _previousIndex = _waypointIndex;
                _waypointIndex = 0;
            }
        }

        _velocity += Seek(_waypointList[_waypointIndex]);
        _velocity = Vector3.ClampMagnitude(_velocity, _maxVelocity);
        
        transform.position += _velocity * Time.deltaTime;

        return _velocity.normalized;
    }
}
