using System.Collections.Generic;
using UnityEngine;

namespace ArtemisProjectile
{
    public abstract class ProjectileController : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        private float _gravityMultiplier = 1;
        public float GravityMultiplier
        {
            get => _gravityMultiplier;
            protected set => _gravityMultiplier = value;
        }

        [SerializeField]
        [HideInInspector]
        private float _speed = 300;
        public float Speed
        {
            get => _speed;
            protected set => _speed = value;
        }
        [SerializeField]
        [HideInInspector]
        private LayerMask _layerMask = new LayerMask() {value = -1 };
        public LayerMask LayerMask
        {
            get => _layerMask;
            protected set => _layerMask = value;
        }

        [SerializeField]
        [HideInInspector]
        private bool _penetrationEnabled = true;
        public bool PenetrationEnabled
        {
            get => _penetrationEnabled;
            protected set => _penetrationEnabled = value;
        }

        [SerializeField]
        [HideInInspector]
        private float _penetration = 50;
        public float Penetration
        {
            get => PenetrationEnabled ? _penetration : 0;
            protected set => _penetration = value;
        }

        [SerializeField]
        [HideInInspector]
        private bool _ricochetEnabled = true;
        public bool RicochetEnabled
        {
            get => _ricochetEnabled;
            protected set => _ricochetEnabled = value;
        }

        [SerializeField]
        [HideInInspector]
        private float _ricochetAngle = 15;
        public float RicochetAngle
        {
            get => RicochetEnabled ? _ricochetAngle : 0;
            protected set => _ricochetAngle = value; 
        }

        [SerializeField]
        [HideInInspector]
        private bool _debugEnabled;
        public bool DebugEnabled
        {
            get => Debug.isDebugBuild && _debugEnabled;
            protected set => _debugEnabled = value;
        }

        [SerializeField]
        [HideInInspector]
        private bool _debugLinesSurviveDestroy;
        public bool DebugLinesSurviveDestroy
        {
            get => _debugLinesSurviveDestroy;
            protected set => _debugLinesSurviveDestroy = value;
        }

        [SerializeField]
        [HideInInspector]
        private Color _pathColor = Color.white;
        public Color PathColor
        {
            get => _pathColor;
            protected set => _pathColor = value;
        }

        [SerializeField]
        [HideInInspector]
        private Color _normalColor = Color.yellow;
        public Color NormalColor
        {
            get => _normalColor;
            protected set => _normalColor = value;
        }

        [SerializeField]
        [HideInInspector]
        private Color _penetrationColor = Color.magenta;
        public Color PenetrationColor
        {
            get => _penetrationColor;
            protected set => _penetrationColor = value;
        }

        public Vector3 Velocity { get; private set; }

        protected virtual void OnPenetrationEnter(RaycastHit entry, Vector3 velocity, float thickness) { }
        protected virtual void OnPenetrationExit(RaycastHit exit, Vector3 velocity) { }
        protected virtual void OnPenetrationFailed(RaycastHit hit, Vector3 velocity) { }
        protected virtual void OnRicochet(float inAngle, Vector3 entryDirection, Vector3 exitDirection, RaycastHit hit) { }

        private ProjectileResult result;
        private List<DebugLine> debugLines = new List<DebugLine>();

        protected virtual void FixedUpdate()
        {
            result = Projectile.CalculateTrajectory(
                transform.position,
                result?.velocity.normalized ?? transform.forward,
                result?.velocity.magnitude ?? Speed,
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

        protected virtual void Update()
        {
            if (DebugEnabled)
                ProjectileControllerExtentions.RenderLines(debugLines);
        }

        protected virtual void OnDestroy()
        {
            if (DebugEnabled && DebugLinesSurviveDestroy)
                ProjectileControllerExtentions.RenderLines(debugLines, float.PositiveInfinity);
        }
    }
}
