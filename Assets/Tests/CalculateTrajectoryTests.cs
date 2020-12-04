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
        [UnityTest]
        public IEnumerator FixedUpdateVelocityChange()
        {
            yield return new MonoBehaviourTest<FixedUpdateTests>();
        }

        public class FixedUpdateTests : MonoBehaviour, IMonoBehaviourTest
        {
            private int frameCount;
            public bool IsTestFinished => frameCount >= 10;

            readonly Vector3 initialVelocity = new Vector3(1, 0.5f, 3) * 10;
            readonly Vector3 initialPosition = new Vector3(1, 25, 0.25f);
            Vector3 velocity;
            Vector3 position;

            void Start()
            {
                velocity = initialVelocity;
                position = initialPosition;
            }

            void FixedUpdate()
            {
                var expectedVelocity = initialVelocity + Physics.gravity * Time.fixedDeltaTime * frameCount;
                var expectedPosition = position + expectedVelocity;

                frameCount++;
                var result = Projectile.CalculateTrajectory(position, velocity, 60, 1, 15, ~0);

                position = result.position;
                velocity = result.velocity;

                var actualVelocity = result.velocity;
                Assert.That(actualVelocity == expectedVelocity, $"Velocity is not correct \nExpected: {expectedVelocity} \nActual: {actualVelocity}");

                var actualPosition = result.position;
                Assert.That(actualPosition == expectedPosition, $"Position is not correct \nExpected: {expectedPosition} \nActual: {actualPosition}");
            }
        }

    }
}
