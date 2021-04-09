using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeAI : MonoBehaviour
{
    private Vector3 _target;
    private Vector3 _randomPos;

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

    private void Start()
    {
        _target = GenerateRandomTarget();
    }

    private void Update()
    {
        DrawPath();

        transform.forward += ArriveAtTarget();
    }

    private Vector3 ArriveAtTarget()
    {
        if (Vector3.Distance(transform.position, _target) < 1)
        {
            _target = GenerateRandomTarget();
        }

        _desiredVelocity = _target - transform.position;
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

        _velocity += _steering;
        _velocity = Vector3.ClampMagnitude(_velocity, _maxVelocity);

        transform.position += _velocity * Time.deltaTime;

        return _velocity.normalized;
    }

    private Vector3 GenerateRandomTarget()
    {
        _randomPos = CircleFollowing.CircleCenter + new Vector3(Random.insideUnitSphere.x, 0, Random.insideUnitSphere.y);
        _randomPos *= CircleFollowing.CircleRadius / 1.5f;

        return _randomPos;
    }

    private void DrawPath()
    {
        Debug.DrawLine(transform.position, _target, Color.cyan);
    }
}
