namespace ArtemisProjectile

open UnityEngine
open System

///Core functions of Artemis Projectile
module Projectile =
    ///<summary>Calculates the path a projectile will travel in 1 time step.</summary>
    ///<param name="position">The current position of the projectile.</param>
    ///<param name="velocity">The current velocity of the projectile.</param>
    ///<param name="penetration">The maximum thickness the projectile can penetrate in mm. (inclusive)</param>
    ///<param name="gravityMultiplier">The value by which Physics.gravity is multiplied.</param>
    ///<param name="ricochetAngle">The maximum angle at which a ricochet can occur. (inclusive)</param>
    ///<param name="layerMask">The layer mask the projectile uses to filter collisions.</param>
    let CalculateTrajectory position (velocity : Vector3 ) (penetration : single) gravityMultiplier ricochetAngle layerMask = 
        let timeStep = 
            if Time.inFixedTimeStep 
                then Time.fixedDeltaTime 
                else Time.deltaTime
        let velocityThisStep = 
            Vector3
                ( velocity.x,
                  velocity.y + Physics.gravity.y * gravityMultiplier * timeStep,
                  velocity.z)

        let positionThisStep = position + velocityThisStep * timeStep

        let rec GetResults (startPoint : Vector3) (endPoint : Vector3) distanceLeft projectileResult =
            let mutable contact = RaycastHit()
            let hitObject = Physics.Linecast(startPoint, endPoint, &contact, layerMask)
            let contact = contact;

            let result =
                match hitObject with
                | false -> NoContact
                | true ->
                    let inDirection = projectileResult.velocity.normalized
                    let angle = -(90.0f - Vector3.Angle(contact.normal, inDirection))

                    if angle <= ricochetAngle then
                        let outVelocity = Vector3.Reflect(projectileResult.velocity, contact.normal)
                        Ricochet(projectileResult.velocity, outVelocity, angle, contact)
                    else
                        let hits =  
                            let backCastStartPoint = Vector3(contact.point.x + inDirection.x * 10f, contact.point.y + inDirection.y * 10f, contact.point.z + inDirection.z * 10f)
                            Physics.RaycastAll(
                                backCastStartPoint,
                                contact.point - backCastStartPoint,
                                Vector3.Distance(contact.point, backCastStartPoint),
                                layerMask)
                            |> Array.filter(fun x -> x.collider = contact.collider)
                            |> Array.sortBy(fun x -> x.distance)
                        
                        if hits.Length = 0 then
                            FailedPenetration(projectileResult.velocity, contact, Single.PositiveInfinity)
                        else
                            let exitHit = Array.last(hits)
                            let thickness = Vector3.Distance(contact.point, exitHit.point) * 1000f

                            if thickness < penetration
                                then Penetration(projectileResult.velocity, contact, exitHit, thickness)
                                else FailedPenetration(projectileResult.velocity, contact, thickness)

            match result with
            | Ricochet (_, outVelocity, _, hit) -> 
                let newDistanceLeft = distanceLeft - hit.distance

                let newEndPoint = 
                    let outDirection = outVelocity.normalized
                    Vector3(hit.point.x + outDirection.x * newDistanceLeft, hit.point.y + outDirection.y * newDistanceLeft, hit.point.z + outDirection.z * newDistanceLeft)

                let newProjectileResult = 
                    { position = hit.point
                      velocity = outVelocity
                      results = projectileResult.results |> Array.append [|result|] }

                GetResults hit.point newEndPoint newDistanceLeft newProjectileResult

            | Penetration (velocity, entry, exit, _) ->
                let newDistanceLeft = distanceLeft - entry.distance
                let direction = velocity.normalized
                GetResults 
                    (Vector3(entry.point.x + direction.x * 0.01f, entry.point.y + direction.y * 0.01f, entry.point.z + direction.z * 0.01f))
                    (Vector3(exit.point.x + direction.x * newDistanceLeft, exit.point.y + direction.y * newDistanceLeft, exit.point.z + direction.z * newDistanceLeft))
                    newDistanceLeft 
                    { position = entry.point
                      velocity = projectileResult.velocity
                      results = projectileResult.results |> Array.append [|result|] }

            | FailedPenetration (_,hit,_) ->
                { projectileResult with 
                    position = hit.point
                    results = projectileResult.results |> Array.append [|result|] }
            | NoContact -> 
                { projectileResult with 
                    position = endPoint }

        let projectileResult = GetResults position positionThisStep (Vector3.Distance(position, positionThisStep)) { position = position; velocity = velocityThisStep; results = [||]; }
        { projectileResult with 
            results = projectileResult.results |> Array.rev }