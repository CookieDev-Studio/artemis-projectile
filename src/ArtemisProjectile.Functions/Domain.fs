namespace ArtemisProjectile

open UnityEngine

type HitResult =
| Ricochet of inVelocity : Vector3 * outVelocity : Vector3 * angle : single * hit : RaycastHit
| Penetration of velocity : Vector3 * entry : RaycastHit * exit : RaycastHit * thickness : single
| FailedPenetration of velocity : Vector3 * hit : RaycastHit * thickness : single
| NoContact

type ProjectileResult =
    { position : Vector3
      velocity : Vector3
      results : HitResult array }