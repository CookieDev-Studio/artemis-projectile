using NUnit.Framework;
using UnityEngine;
using ArtemisProjectile;

namespace Tests
{
    [TestFixture]
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

        private static GameObject testCube;

        [SetUp]
        public static void SetUp()
        {
            testCube = Object.Instantiate(new GameObject());
            testCube.AddComponent<BoxCollider>().size = new Vector3(1, 1, 0.05f);
            testCube.transform.position = Vector3.forward * 5;
            testCube.transform.rotation = Quaternion.Euler(Vector3.zero);
        }

        [TearDown]
        public static void TearDown()
        {
            Object.DestroyImmediate(testCube);
        }

        [Theory]
        public void VelocityChange(Vector3 velocity)
        {
            var timeStep = Time.fixedDeltaTime;

            var result = Projectile.CalculateTrajectory(Vector3.zero, velocity, 0, 1, 0, 0, timeStep);
            var actualVelocity = result.velocity;

            var expectedVelocity = velocity + Physics.gravity * timeStep;
            Assert.AreEqual(expectedVelocity, actualVelocity);
        }

        [Theory]
        public void PositionChange(Vector3 position)
        {
            var timeStep = Time.fixedDeltaTime;

            var result = Projectile.CalculateTrajectory(position, new Vector3(1, 0.5f, 3), 0, 1, 0, 0, timeStep);
            var actualPosition = result.position;

            var expectedPosition = position + result.velocity * timeStep;
            Assert.AreEqual(expectedPosition, actualPosition);
        }

        [Test]
        public void Penetrates()
        {

            var result = Projectile.CalculateTrajectory(Vector3.zero, Vector3.forward * 100, 60, 0, 0, ~0, 1);
            Debug.Log(testCube.transform.rotation.eulerAngles);
            TestContext.WriteLine(result);

            Assume.That(result.results.Length == 1, "No hit results returned");
            Assert.IsInstanceOf<HitResult.Penetration>(result.results[0]);
        }

        [Test]
        public void FailsToPenetrate()
        {
            testCube.transform.rotation = Quaternion.Euler(new Vector3(0, 45));

            var result = Projectile.CalculateTrajectory(Vector3.zero, Vector3.forward * 100, 60, 0, 0, ~0, 1);
            TestContext.WriteLine(result);

            Assume.That(result.results.Length == 1, "No hit results returned");
            Assert.IsInstanceOf<HitResult.FailedPenetration>(result.results[0]);
        }

        [Test]
        public void Ricochets()
        {
            testCube.transform.rotation = Quaternion.Euler(new Vector3(0, 75));

            var result = Projectile.CalculateTrajectory(Vector3.zero, Vector3.forward * 100, 0, 0, 15, ~0, 1);
            TestContext.WriteLine(result);

            Assume.That(result.results.Length == 1, "No hit results returned");
            Assert.IsInstanceOf<HitResult.Ricochet>(result.results[0]);
        }

        [Test]
        public void FailsToRicochet()
        {
            testCube.transform.rotation = Quaternion.Euler(new Vector3(0, 70));

            var result = Projectile.CalculateTrajectory(Vector3.zero, Vector3.forward * 100, 0, 0, 15, ~0, 1);
            TestContext.WriteLine(result);

            Assume.That(result.results.Length == 1, "No hit results returned");
            Assert.IsInstanceOf<HitResult.FailedPenetration>(result.results[0]);
        }
    }
}
