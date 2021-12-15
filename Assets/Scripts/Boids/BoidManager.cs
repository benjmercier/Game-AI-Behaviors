using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAI.Scripts.Boids
{
    public class BoidManager : MonoBehaviour
    {
        const int threadGroupSize = 1024;

        public BoidSettings settings;
        public ComputeShader compute;
        private Boid[] _boids;

        private void Start()
        {
            _boids = FindObjectsOfType<Boid>();

            foreach (Boid b in _boids)
            {
                b.Initialize(settings, this.transform);
            }
        }

        private void Update()
        {
            if (_boids != null)
            {
                int numBoids = _boids.Length;
                var boidData = new BoidData[numBoids];

                for (int i = 0; i < _boids.Length; i++)
                {
                    boidData[i].position = _boids[i].position;
                    boidData[i].direction = _boids[i].forward;
                }

                var boidBuffer = new ComputeBuffer(numBoids, BoidData.Size);
                boidBuffer.SetData(boidData);

                compute.SetBuffer(0, "boids", boidBuffer);
                compute.SetInt("numBoids", _boids.Length);
                compute.SetFloat("viewRadius", settings.perceptionRaidus);
                compute.SetFloat("avoidRadius", settings.avoidanceRadius);

                int threadGroups = Mathf.CeilToInt(numBoids / (float)threadGroupSize);
                compute.Dispatch(0, threadGroups, 1, 1);

                boidBuffer.GetData(boidData);

                for (int i = 0; i < _boids.Length; i++)
                {
                    _boids[i].avgFlockHeading = boidData[i].flockHeading;
                    _boids[i].centerOfFlockmates = boidData[i].flockCenter;
                    _boids[i].avgAvoidanceHeading = boidData[i].avoidanceHeading;
                    _boids[i].numPerceivedFlockmates = boidData[i].numFlockmates;

                    _boids[i].UpdateBoid();
                }

                boidBuffer.Release();
            }
        }

        public struct BoidData
        {
            public Vector3 position;
            public Vector3 direction;

            public Vector3 flockHeading;
            public Vector3 flockCenter;
            public Vector3 avoidanceHeading;
            public int numFlockmates;

            public static int Size { get { return sizeof(float) * 3 * 5 + sizeof(int); } }
        }
    }
}
