using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAI.Scripts.Fleeing
{
    public class Chasing : MonoBehaviour
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

            _canMove = true;
        }

        private void Update()
        {
            DrawPath();
        }

        private void FixedUpdate()
        {
            //transform.forward += ArriveAtTarget();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _canMove = true;
            }

            if (_canMove)
            {
                ObjectSeek(_target);
            }            
        }

        private void ObjectSeek(Vector3 target)
        {
            if (Vector3.Distance(transform.position, _target) < 1)
            {
                StartCoroutine(MoveRoutine());

                return;
            }

            var horInput = Input.GetAxis("Horizontal");
            var verInput = Input.GetAxis("Vertical");

            var direction = new Vector3(horInput, 0, verInput);

            var td = transform.position + direction;


            _desiredVelocity = (td - transform.position).normalized;
            _desiredVelocity *= _maxVelocity;

            _steeringVelocity = _desiredVelocity - _currentVelocity;
            _steeringVelocity = Vector3.ClampMagnitude(_steeringVelocity, _maxForce);
            _steeringVelocity /= _rigidbody.mass;

            _currentVelocity += _steeringVelocity;
            _currentVelocity = Vector3.ClampMagnitude(_currentVelocity, _maxVelocity);

            transform.position += _currentVelocity * Time.deltaTime;
            transform.forward += _currentVelocity * Time.deltaTime;
        }

        private IEnumerator MoveRoutine()
        {
            _canMove = false;

            yield return new WaitForSeconds(1.5f);

            _target = GameObject.Find("Capsule").transform.position; // GenerateRandomTarget();

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
            Debug.DrawRay(transform.position, transform.forward, Color.blue);
        }
    }
}

