using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ArtemisProjectile;
namespace Tests
{
    public class CalculateTrajectoryTests
    {
        [DatapointSource]
        Vector3[] VectorSource = new Vector3[]
        {
            Vector3.zero,
            new Vector3(2.53f, 5.0f, 0.00003f),
            new Vector3(100, 5.0f, 45346),
            new Vector3(250000, 100000, 200000),
        };

        [Theory]
        public void FixedUpdateVelocityChange(Vector3 velocity)
        {
            var timeStep = Time.fixedDeltaTime;
            var expectedVelocity = velocity + Physics.gravity * timeStep;

            var result = Projectile.CalculateTrajectory(Vector3.zero, velocity, 60, 1, 15, ~0, timeStep);
            var actualVelocity = result.velocity;

            Assert.AreEqual(expectedVelocity, actualVelocity);
        }

        [Theory]
        public void FixedUpdatePositionChange(Vector3 position)
        {
            var timeStep = Time.fixedDeltaTime;

            var result = Projectile.CalculateTrajectory(position, new Vector3(1, 0.5f, 3), 60, 1, 15, ~0, timeStep);

            var expectedPosition = position + result.velocity * timeStep;
            var actualPosition = result.position;
            Assert.AreEqual(expectedPosition, actualPosition);
        }
    }
}
