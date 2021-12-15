using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAI.Scripts.Boids
{
    [CreateAssetMenu]
    public class BoidSettings : ScriptableObject
    {
        public float minSpeed = 2f;
        public float maxSpeed = 5f;
        public float perceptionRaidus = 2.5f;
        public float avoidanceRadius = 1f;
        public float maxSteerForce = 3f;

        public float alignWeight = 1f;
        public float cohesionWeight = 1f;
        public float separateWeight = 1f;

        public float targetWeight = 1f;

        [Header("Collisions")]
        public LayerMask obstacleMask;
        public float boundsRadius = 0.27f;
        public float avoidCollisionWeight = 1f;
        public float collisionAvoidDst = 5f;
    }
}
