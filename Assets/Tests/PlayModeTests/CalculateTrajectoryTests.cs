using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using ArtemisProjectile;
using UnityEngine.TestTools;

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

        GameObject testCube;

        [SetUp]
        public void SetUp()
        {
            testCube = Object.Instantiate(new GameObject());
            testCube.AddComponent<BoxCollider>().size = new Vector3(1, 1, 0.05f);
            testCube.transform.position = Vector3.forward * 5;
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(testCube);
        }

        [Theory]
        public void VelocityChange(Vector3 velocity)
        {
            var timeStep = Time.fixedDeltaTime;

            var result = Projectile.CalculateTrajectory(Vector3.zero, velocity, 60, 1, 15, 0, timeStep);
            var actualVelocity = result.velocity;

            var expectedVelocity = velocity + Physics.gravity * timeStep;
            Assert.AreEqual(expectedVelocity, actualVelocity);
        }

        [Theory]
        public void PositionChange(Vector3 position)
        {
            var timeStep = Time.fixedDeltaTime;

            var result = Projectile.CalculateTrajectory(position, new Vector3(1, 0.5f, 3), 60, 1, 15, 0, timeStep);
            var actualPosition = result.position;

            var expectedPosition = position + result.velocity * timeStep;
            Assert.AreEqual(expectedPosition, actualPosition);
        }

        [Test]
        public void SuccessfullyPenetrates()
        {
            var result = Projectile.CalculateTrajectory(Vector3.zero, Vector3.forward * 100, 60, 1, 15, ~0, 1);

            Assert.That(result.results.Length == 1);
            Assert.IsInstanceOf<HitResult.Penetration>(result.results[0]);
        }

        [Test]
        public void FailsToPenetrate()
        {
            var result = Projectile.CalculateTrajectory(Vector3.zero, Vector3.forward * 100, 50, 1, 15, ~0, 1);

            Assert.That(result.results.Length == 1);
            Assert.IsInstanceOf<HitResult.FailedPenetration>(result.results[0]);
        }
    }
}
