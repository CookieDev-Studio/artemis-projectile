using System.Collections.Generic;
using UnityEngine;

namespace ArtemisProjectile
{
    public enum UpdateLoop { Update, FixedUpdate };

    /// <summary>
    /// Base class For Artemis Projectile
    /// </summary>
    public abstract class ProjectileController : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        [Tooltip("The value by which Physics.gravity is multiplied.")]
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
        [Tooltip("The speed of the projectle in m/s.")]
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
        [Tooltip("The layer mask that will be applied to the projectile's collisions.")]
        private LayerMask _layerMask = new LayerMask() { value = -1 };
        /// <summary>
        /// The layer mask that is applied to the projectile's collisions.
        /// </summary>
        public LayerMask LayerMask
        {
            get => _layerMask;
            protected set => _layerMask = value;
        }

        [SerializeField]
        [HideInInspector]
        [Tooltip("Enable penetration")]
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
        [Tooltip("The maximum distance the projectile can travel through an object in milimeters. (inclusive)")]
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
        [Tooltip("Enable ricochet.")]
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
        [Tooltip("The maximum angle at which a ricochet can occur. (inclusive)")]
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
        [Tooltip("Enable debbuging tools.")]
        private bool _debugEnabled;
        /// <summary>
        /// Enable debbuging tools.
        /// </summary>
        public bool DebugEnabled
        {
            get => Debug.isDebugBuild && _debugEnabled;
            protected set => _debugEnabled = value;
        }

        [SerializeField]
        [HideInInspector]
        [Tooltip("Debug lines will keep rendering after the proectile is destroyed.")]
        private bool _ignoreDestroy;
        /// <summary>
        /// Debug lines will keep rendering after the proectile is destroyed.
        /// </summary>
        public bool IgnoreDestroy
        {
            get => _ignoreDestroy;
            protected set => _ignoreDestroy = value;
        }

        [SerializeField]
        [HideInInspector]
        [Tooltip("The color the projectile path will be drawn.")]
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
        [Tooltip("The color the normals will be drawn.")]
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
        [Tooltip("The color the path through an object is drawn.")]
        private Color _penetrationColor = Color.magenta;
        /// <summary>
        /// The color the path through an object is drawn
        /// </summary>
        public Color PenetrationColor
        {
            get => _penetrationColor;
            protected set => _penetrationColor = value;
        }

        [SerializeField]
        [HideInInspector]
        [Tooltip("Specify the update loop the projectile will run in. FixedUpdate is recomended; Use Update if you need the projectile to travel smoothly.")]
        private UpdateLoop _updateLoop = UpdateLoop.FixedUpdate;
        /// <summary>
        /// The update loop that the projectile is running in.
        /// </summary>
        public UpdateLoop UpdateLoop { get => _updateLoop; }

        /// <summary>
        /// The velocity vector of the projectile.
        /// </summary>
        public Vector3 Velocity { get; private set; }

        /// <summary>
        /// Called when the projectile sucssesfully penetrates an object.
        /// </summary>
        /// <param name="entry">The RaycastHit of when the projectile entered the object.</param>
        /// <param name="velocity">The velocity at which the projectile struck the object.</param>
        /// <param name="thickness">The reletive thickness of the object.</param>
        protected virtual void OnPenetrationEnter(RaycastHit entry, Vector3 velocity, float thickness) { }

        /// <summary>
        /// Called when the projectile exits a penetrated object
        /// </summary>
        /// <param name="exit">The RacastHit of when the projectile exited the object.</param>
        /// <param name="velocity">The velocity at which the projectile exited the object.</param>
        /// <param name="thickness">The reletive thickness of the object.</param>
        protected virtual void OnPenetrationExit(RaycastHit exit, Vector3 velocity, float thickness) { }

        /// <summary>
        /// Called when the Projectile fails to penetrate an object. if penetration is disabled, this will always be called upon collision.
        /// </summary>
        /// <param name="hit">The RaycastHit of the collision with the object.</param>
        /// <param name="velocity">The velocity at which the projectile struck the object.</param>
        /// <param name="thickness">The thickness of the struck object.</param>
        protected virtual void OnPenetrationFailed(RaycastHit hit, Vector3 velocity, float thickness) { }

        /// <summary>
        /// Called when the projectile ricochets off of an object
        /// </summary>
        /// <param name="hit">The RaycastHit of the contact with the surface</param>
        /// <param name="inAngle">The angle that the projectile hit the surface.</param>
        /// <param name="entryVelocity">The velocity the projectile hit the surface.</param>
        /// <param name="exitVelocity">The velocity of the projectile after reflection.</param>
        protected virtual void OnRicochet(RaycastHit hit, float inAngle, Vector3 entryVelocity, Vector3 exitVelocity) { }


        private ProjectileResult result;
        private readonly List<DebugLine> debugLines = new List<DebugLine>();

        ///<inheritdoc/>
        protected virtual void Update()
        {
            if (UpdateLoop == UpdateLoop.Update)
                MovePosition();

            if (DebugEnabled)
                ProjectileControllerExtentions.RenderLines(debugLines);
        }

        ///<inheritdoc/>
        protected virtual void FixedUpdate()
        {
            if (UpdateLoop == UpdateLoop.FixedUpdate)
                MovePosition();
        }

        private void MovePosition()
        {
            result = Projectile.CalculateTrajectory(
                transform.position,
                result?.velocity ?? transform.forward * Speed,
                Penetration,
                GravityMultiplier,
                RicochetAngle,
                LayerMask);

            var curPosition = transform.position;
            for (var i = 0; i < result.results.Length; i++)
            {
                switch (result.results[i])
                {
                    case HitResult.Ricochet ricochet:
                        OnRicochet(ricochet.hit, ricochet.angle, ricochet.inVelocity, ricochet.outVelocity);

                        if (DebugEnabled)
                        {
                            debugLines.Add(new DebugLine(curPosition, ricochet.hit.point, PathColor));

                            var distance = 0.1f;
                            debugLines.Add(
                                new DebugLine(
                                    ricochet.hit.point,
                                    new Vector3(ricochet.hit.point.x + ricochet.hit.normal.x * distance, ricochet.hit.point.y + ricochet.hit.normal.y * distance, ricochet.hit.point.z + ricochet.hit.normal.z * distance),
                                    NormalColor)
                                );
                            curPosition = ricochet.hit.point;
                        }
                        break;

                    case HitResult.Penetration penetration:
                        OnPenetrationEnter(penetration.entry, penetration.velocity, penetration.thickness);
                        OnPenetrationExit(penetration.exit, penetration.velocity, penetration.thickness);
                        if (DebugEnabled)
                        {
                            debugLines.Add(new DebugLine(curPosition, penetration.entry.point, PathColor));
                            debugLines.Add(new DebugLine(penetration.entry.point, penetration.exit.point, PenetrationColor));

                            curPosition = penetration.exit.point;
                        }
                        break;

                    case HitResult.FailedPenetration failedPen:
                        OnPenetrationFailed(failedPen.hit, failedPen.velocity, failedPen.thickness);

                        if (DebugEnabled && i != result.results.Length - 1)
                            debugLines.Add(new DebugLine(curPosition, failedPen.hit.point, PathColor));
                        break;

                }
            }
            debugLines.Add(new DebugLine(curPosition, result.position, PathColor));

            if (DebugEnabled && result.results.Length == 0)
                debugLines.Add(new DebugLine(transform.position, result.position, PathColor));

            transform.position = result.position;
            Velocity = result.velocity;
        }

        ///<inheritdoc/>
        protected virtual void OnDestroy()
        {
            if (DebugEnabled && IgnoreDestroy)
                ProjectileControllerExtentions.RenderLines(debugLines, float.PositiveInfinity);
        }
    }
}