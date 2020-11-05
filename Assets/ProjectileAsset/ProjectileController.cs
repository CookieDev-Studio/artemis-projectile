﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ProjectileAsset
{
    public class ProjectileController : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        private LayerMask _layerMask;
        public LayerMask LayerMask
        {
            get => _layerMask;
            protected set => _layerMask = value;
        }

        [SerializeField]
        [HideInInspector]
        private bool _penetrationEnabled;
        public bool PenetrationEnabled
        {
            get => _penetrationEnabled;
            protected set => _penetrationEnabled = value;
        }

        [SerializeField]
        [HideInInspector]
        private float _penetration;
        public float Penetration
        {
            get => _penetration;
            protected set => _penetration = value;
        }

        [SerializeField]
        [HideInInspector]
        private float _ricochetAngle = 5;
        public float RicochetAngle
        {
            get => _ricochetAngle;
        }

        [SerializeField]
        [HideInInspector]
        private float _gravityMultiplier = 1;
        public float GravityMultiplier
        {
            get => _gravityMultiplier;
        }

        [SerializeField]
        [HideInInspector]
        private float _speed = 300;
        public float Speed
        {
            get => _speed;
            protected set => _speed = value;
        }

        protected virtual void OnPenetrationFailed(RaycastHit hit, Vector3 dirrection) { }
        protected virtual void OnPenetrationEnter(RaycastHit entry, Vector3 dirrection) { }
        protected virtual void OnPenetrationExit(RaycastHit exit, Vector3 dirrection) { }
        protected virtual void OnRicochet(Vector3 entryDirection, Vector3 exitDirection, RaycastHit hit) { }


        private ProjectileStart projectileStart;
        private ProjectileResult result;
        protected virtual void Awake()
        {
            projectileStart = new ProjectileStart(transform.position, transform.forward, Time.time);
            _ricochetAngle = 45f;
            result = ProjectileNew.CalculateTrajectory(projectileStart, Speed, Penetration, GravityMultiplier, RicochetAngle, LayerMask);
            Time.timeScale = 0.1f;
        }

        protected void FixedUpdate()
        {
            /*var nextPosition = Projectile.CalculateTrajectory(Time.time - startTime + Time.fixedDeltaTime, startPosition, transform.forward, GravityMultiplier, Speed);
            var nextNextPosition = Projectile.CalculateTrajectory(Time.time - startTime + Time.fixedDeltaTime * 2, startPosition, transform.forward, GravityMultiplier, Speed);
            if (Physics.Linecast(transform.position, nextPosition, LayerMask.value))
                if (PenetrationEnabled)
                {
                    var results = Projectile.GetPenetrations(new Vector3[] { transform.position, nextPosition, nextNextPosition }, LayerMask.value);

                    foreach (var result in results)
                    {
                        //if penetration is successful
                        if (result.thickness <= Penetration)
                        {
                            OnPenetrationEnter(result.entryHit, result.dirrection);
                            if (result.exitHit != null)
                                OnPenetrationExit((RaycastHit)result.exitHit, result.dirrection);
                        }
                        else
                            OnPenetrationFailed(result.entryHit, result.dirrection);
                    }
                }*/

            //transform.position = nextPosition;

            result = ProjectileNew.CalculateTrajectory(result.startInfo, Speed, Penetration, GravityMultiplier, RicochetAngle, LayerMask);
            Debug.Log(result.results[result.results.Length - 1]);
            transform.position = result.position;
        }
    }
}