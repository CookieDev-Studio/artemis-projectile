using System.Collections.Generic;
using UnityEngine;

namespace ArtemisProjectile
{
    /// <summary>
    /// Base class For Artemis Projectile
    /// </summary>
    public abstract class ProjectileController : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        private float _gravityMultiplier = 1;
        /// <summary>
        /// The value by which Physics.gravity is multiplied.
        /// </summary>
        public float GravityMultiplier
        {
            get => _gravityMultiplier;
            protected set => _gravityMultiplier = value;
        }

        [SerializeField]
        [HideInInspector]
        private float _speed = 300;
        /// <summary>
        /// The speed of the projectile in m/s.
        /// </summary>
        public float Speed
        {
            get => _speed;
            protected set => _speed = value;
        }
        [SerializeField]
        [HideInInspector]
        private LayerMask _layerMask = new LayerMask() { value = -1 };
        /// <summary>
        /// The layer mask the projectile uses to filter collisions.
        /// </summary>
        public LayerMask LayerMask
        {
            get => _layerMask;
            protected set => _layerMask = value;
        }

        [SerializeField]
        [HideInInspector]
        private bool _penetrationEnabled = true;
        /// <summary>
        /// Whether the projectile should be able to penetrate objects.
        /// </summary>
        public bool PenetrationEnabled
        {
            get => _penetrationEnabled;
            protected set => _penetrationEnabled = value;
        }

        [SerializeField]
        [HideInInspector]
        private float _penetration = 50;
        /// <summary>
        /// The maximum thickness the projectile can penetrate in mm. (inclusive)
        /// </summary>
        public float Penetration
        {
            get => PenetrationEnabled ? _penetration : 0;
            protected set => _penetration = value;
        }

        [SerializeField]
        [HideInInspector]
        private bool _ricochetEnabled = true;
        /// <summary>
        /// Whether the projectile should bounce off of surfaces.
        /// </summary>
        public bool RicochetEnabled
        {
            get => _ricochetEnabled;
            protected set => _ricochetEnabled = value;
        }

        [SerializeField]
        [HideInInspector]
        private float _ricochetAngle = 15;
        /// <summary>
        /// The maximum angle at which a ricochet can occur. (inclusive)
        /// </summary>
        public float RicochetAngle
        {
            get => RicochetEnabled ? _ricochetAngle : 0;
            protected set => _ricochetAngle = value;
        }

        [SerializeField]
        [HideInInspector]
        private bool _debugEnabled;
        /// <summary>
        /// Enable Debbuging tools
        /// </summary>
        public bool DebugEnabled
        {
            get => Debug.isDebugBuild && _debugEnabled;
            protected set => _debugEnabled = value;
        }

        [SerializeField]
        [HideInInspector]
        private bool _debugPathSurvivesDestroy;
        /// <summary>
        /// Lines drawn during debug will remain after the projectile is destroyed
        /// </summary>
        public bool DebugPathSurvivesDestroy
        {
            get => _debugPathSurvivesDestroy;
            protected set => _debugPathSurvivesDestroy = value;
        }

        [SerializeField]
        [HideInInspector]
        private Color _pathColor = Color.white;
        /// <summary>
        /// The color path lines will be drawn
        /// </summary>
        public Color PathColor
        {
            get => _pathColor;
            protected set => _pathColor = value;
        }

        [SerializeField]
        [HideInInspector]
        private Color _normalColor = Color.yellow;
        /// <summary>
        /// The color normals will be drawn
        /// </summary>
        public Color NormalColor
        {
            get => _normalColor;
            protected set => _normalColor = value;
        }

        [SerializeField]
        [HideInInspector]
        private Color _penetrationColor = Color.magenta;
        /// <summary>
        /// The color the path through an object is drawn
        /// </summary>
        public Color PenetrationColor
        {
            get => _penetrationColor;
            protected set => _penetrationColor = value;
        }

        /// <summary>
        /// The velocity vector of the projectile
        /// </summary>
        public Vector3 Velocity { get; private set; }

        /// <summary>
        /// Called when the projectile sucssesfully penetrates an object
        /// </summary>
        /// <param name="entry">The RaycastHit of when the projectile entered the object</param>
        /// <param name="velocity">The velocity at which the projectile struck the object</param>
        /// <param name="thickness">The reletive thickness of the object</param>
        protected virtual void OnPenetrationEnter(RaycastHit entry, Vector3 velocity, float thickness) { }
        /// <summary>
        /// Called when the projectile exits the object
        /// </summary>
        /// <param name="exit">The RacastHit of when the projectile exited the object</param>
        /// <param name="velocity">The velocity at which the projectile exited the object</param>
        protected virtual void OnPenetrationExit(RaycastHit exit, Vector3 velocity) { }
        /// <summary>
        /// Called when the Projectile fails to penetrate an object. if penetration is disabled, this will always be called upon collision.
        /// </summary>
        /// <param name="hit">The RaycastHit of the collision with the object</param>
        /// <param name="velocity">The velocity at which the projectile struck the object</param>
        protected virtual void OnPenetrationFailed(RaycastHit hit, Vector3 velocity) { }
        /// <summary>
        /// Called when the projectile ricochets off of an object
        /// </summary>
        /// <param name="inAngle"></param>
        /// <param name="entryDirection"></param>
        /// <param name="exitDirection"></param>
        /// <param name="hit"></param>
        protected virtual void OnRicochet(float inAngle, Vector3 entryDirection, Vector3 exitDirection, RaycastHit hit) { }

        private ProjectileResult result;
        private List<DebugLine> debugLines = new List<DebugLine>();

        ///<inheritdoc/> 
        protected virtual void FixedUpdate()
        {
            result = Projectile.CalculateTrajectory(
                transform.position,
                result?.velocity ?? transform.forward * Speed,
                Penetration,
                GravityMultiplier,
                RicochetAngle,
                LayerMask);

            for (var i = 0; i < result.results.Length; i++)
            {
                switch (result.results[i])
                {
                    case HitResult.Ricochet ricochet:
                        OnRicochet(ricochet.angle, ricochet.inVelocity, ricochet.outVelocity, ricochet.hit);

                        if (DebugEnabled)
                        {
                            debugLines.Add(new DebugLine(transform.position, ricochet.hit.point, PathColor));
                            debugLines.Add(new DebugLine(ricochet.hit.point, result.position, PathColor));

                            var distance = 0.1f;
                            debugLines.Add(
                                new DebugLine(
                                    ricochet.hit.point,
                                    new Vector3(ricochet.hit.point.x + ricochet.hit.normal.x * distance, ricochet.hit.point.y + ricochet.hit.normal.y * distance, ricochet.hit.point.z + ricochet.hit.normal.z * distance),
                                    NormalColor)
                                );
                        }
                        break;

                    case HitResult.Penetration penetration:
                        OnPenetrationEnter(penetration.entry, penetration.velocity, penetration.thickness);
                        OnPenetrationExit(penetration.exit, penetration.velocity);
                        if (DebugEnabled)
                        {
                            if (i == 0)
                                debugLines.Add(new DebugLine(transform.position, penetration.entry.point, PathColor));
                            debugLines.Add(new DebugLine(penetration.entry.point, penetration.exit.point, PenetrationColor));
                            debugLines.Add(new DebugLine(penetration.exit.point, result.position, PathColor));
                        }
                        break;

                    case HitResult.FailedPenetration failedPen:
                        OnPenetrationFailed(failedPen.hit, failedPen.velocity);

                        if (DebugEnabled && i != result.results.Length - 1)
                            debugLines.Add(new DebugLine(transform.position, failedPen.hit.point, PathColor));
                        break;
                }
            }

            if (DebugEnabled && result.results.Length == 0)
                debugLines.Add(new DebugLine(transform.position, result.position, PathColor));

            transform.position = result.position;
            Velocity = result.velocity;
        }
        ///<inheritdoc/>
        protected virtual void Update()
        {
            if (DebugEnabled)
                ProjectileControllerExtentions.RenderLines(debugLines);
        }

        ///<inheritdoc/>
        protected virtual void OnDestroy()
        {
            if (DebugEnabled && DebugPathSurvivesDestroy)
                ProjectileControllerExtentions.RenderLines(debugLines, float.PositiveInfinity);
        }
    }
}