using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAI.Scripts.Fleeing
{
    public class Fleeing : MonoBehaviour
    {
        [SerializeField]
        private GameObject _targetObj;
        private Vector3 _target;

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

        [SerializeField]
        private float _maxDistance = 5f;

        private Rigidbody _rigidbody;

        private bool _canMove = false;

        Vector3 flee = new Vector3();

        private void Start()
        {
            if (TryGetComponent(out Rigidbody rigidbody))
            {
                _rigidbody = rigidbody;
            }

            //_target = _targetObj.transform.position;
        }

        private void Update()
        {
            DrawPath();
        }

        private void FixedUpdate()
        {
            //transform.forward += ArriveAtTarget();

            ObjectFlee(_targetObj.transform.position);
        }

        private void ObjectFlee(Vector3 seeker)
        {
            // If the object's distance to the target/seeker is less than the max distance it's allowing
            // Then it should start moving
            if (Vector3.Distance(transform.position, seeker) < _maxDistance)
            {
                // Check and apply velocities to move here!
            }


            if (Vector3.Distance(transform.position, seeker) < _maxDistance)
            {

                // The object's position minus the target/seeker's position is 
                // where it should be going
                _desiredVelocity = (transform.position - seeker).normalized;
                // The desired velocity multiplied by its max speed 
                // is how fast it should be going to get there
                _desiredVelocity *= _maxVelocity;

                // The desired velocity minus curent velocity is the vector
                // steering the object where it should be going
                _steeringVelocity = _desiredVelocity - _currentVelocity;
                // Clamping that steering vector provides a max turning force to adhere to
                _steeringVelocity = Vector3.ClampMagnitude(_steeringVelocity, _maxForce);
                // Dividing the steering vector by the object's mass creates variation between
                // larger and smaller objects
                _steeringVelocity /= _rigidbody.mass;

                // Adding steering velocity to current velocity creates the flee path
                // for the object to follow
                _currentVelocity += _steeringVelocity;
                // Clamping the current velocity by the object's max speed prevents it from
                // moving too fast
                _currentVelocity = Vector3.ClampMagnitude(_currentVelocity, _maxVelocity);

                // Multiplying the current velocity by the time between frames and the adding it
                // to the object's position creates smooth movement along the flee path
                transform.position += _currentVelocity * Time.deltaTime;
                // Doing the same to the object's forward rotation creates smooth rotation along
                // the flee path as well
                transform.forward += _currentVelocity * Time.deltaTime;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            
        }

        private void DrawPath()
        {
            Debug.DrawRay(transform.position, transform.forward, Color.red);
        }
    }
}

