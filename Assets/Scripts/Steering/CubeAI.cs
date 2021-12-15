using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAI.Scripts.Steering
{
    public class CubeAI : MonoBehaviour
    {
        private Vector3 _target;
        private Vector3 _randomPos;

        [SerializeField]
        private float _maxVelocity = 10;
        [Range(0f, 1f), SerializeField]
        private float _maxForce = 0.25f;
        [SerializeField]
        private float _approachRadius = 2.5f;
        private float _approachDistance;

        private Vector3 _currentVelocity;
        private Vector3 _desiredVelocity;
        private Vector3 _steeringVelocity;

        private Rigidbody _rigidbody;

        private bool _canMove = false;

        private void Start()
        {
            if (TryGetComponent(out Rigidbody rigidbody))
            {
                _rigidbody = rigidbody;
            }

            _target = GenerateRandomTarget();
        }

        private void Update()
        {
            DrawPath();

        }

        private void FixedUpdate()
        {
            //transform.forward += ArriveAtTarget();

            ObjectSeek(_target);
        }

        private void ObjectSeek(Vector3 target)
        {
            if (Vector3.Distance(transform.position, _target) < 1)
            {
                _target = GenerateRandomTarget();
                //_target = GameObject.Find("Capsule").transform.position;
            }

            _desiredVelocity = (target - transform.position).normalized;
            _desiredVelocity *= _maxVelocity;

            _steeringVelocity = _desiredVelocity - _currentVelocity;
            _steeringVelocity = Vector3.ClampMagnitude(_steeringVelocity, _maxForce);
            _steeringVelocity /= _rigidbody.mass;

            _currentVelocity += _steeringVelocity;
            _currentVelocity = Vector3.ClampMagnitude(_currentVelocity, _maxVelocity);

            transform.position += _currentVelocity * Time.deltaTime;
            transform.forward += _currentVelocity * Time.deltaTime;
        }

        private Vector3 ArriveAtTarget()
        {
            if (Vector3.Distance(transform.position, _target) < 1)
            {
                _canMove = false;
                _target = GenerateRandomTarget();

                StartCoroutine(MoveRoutine());
            }

            _desiredVelocity = (_target - transform.position).normalized;
            _desiredVelocity *= _maxVelocity;

            _steeringVelocity = _desiredVelocity - _currentVelocity;
            _steeringVelocity = Vector3.ClampMagnitude(_steeringVelocity, _maxForce);

            if (TryGetComponent(out Rigidbody rigidbody))
            {
                _steeringVelocity /= rigidbody.mass;
            }

            _currentVelocity += _steeringVelocity;
            _currentVelocity = Vector3.ClampMagnitude(_currentVelocity, _maxVelocity);

            transform.position += _currentVelocity * Time.deltaTime;

            return _currentVelocity;

            /*

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
            */

            //return _velocity.normalized;
        }

        private IEnumerator MoveRoutine()
        {
            yield return new WaitForSeconds(1.5f);

            _canMove = true;
        }

        private Vector3 GenerateRandomTarget()
        {
            _randomPos = CircleFollowing.CircleCenter + new Vector3(Random.insideUnitSphere.x, 0, Random.insideUnitSphere.y);
            _randomPos *= CircleFollowing.CircleRadius;// / 1.5f;

            return _randomPos;
        }

        private void DrawPath()
        {
            Debug.DrawLine(transform.position, _target, Color.cyan);
        }
    }
}

