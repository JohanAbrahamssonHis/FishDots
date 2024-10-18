using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct HandleFishSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        /*
        foreach (FishAspect fishAspect in
                 SystemAPI.Query<FishAspect>())
        {
            fishAspect.FishLogic(SystemAPI.Time.DeltaTime);
        }
        */
        
        FishJob fishJob = new FishJob()
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            neighborFish = new List<Transform>(),
        };
        fishJob.ScheduleParallel();
        
    }
    
    public static float3 Cohesion(ref LocalTransform localTransform, ref FishComponent fishComponent, ref List<Transform> neighborFish)
    {
        Vector3 centerOfMass = Vector3.zero;
        Vector3 position = localTransform.Position;

        foreach (Transform fish in neighborFish)
        {
            centerOfMass += fish.position;
        }

        if (neighborFish.Count == 0) return Vector3.zero;

        centerOfMass /= neighborFish.Count;
        return (float3)(centerOfMass - position).normalized;
    }

    public static float3 Alignment(ref FishComponent fishComponent, ref List<Transform> neighborFish)
    {
        Vector3 averageHeading = float3.zero;

        foreach (Transform fish in neighborFish)
        {
            averageHeading += fish.forward;
        }

        if (neighborFish.Count == 0) return Vector3.zero;

        averageHeading /= neighborFish.Count;
        return (float3)averageHeading.normalized;
    }

    public static float3 Separation(ref LocalTransform localTransform, ref FishComponent fishComponent, ref List<Transform> neighborFish)
    {
        Vector3 separationForce = Vector3.zero;
        Vector3 position = localTransform.Position;

        foreach (Transform fish in neighborFish)
        {
            if (Vector3.Distance(position, fish.position) < fishComponent.separationDistance)
            {
                separationForce += position - fish.position;
            }
        }

        return separationForce.normalized;
    }
    
    public static float3 SelectionPoint(ref LocalTransform localTransform, ref FishComponent fishComponent)
    {
        return Vector3.Distance(localTransform.Position, fishComponent.selectionPoint)>fishComponent.distanceFromSelectionPoint
            ? Normalize(fishComponent.selectionPoint-localTransform.Position) : float3.zero;
    }
    
    public static List<Transform> GetNeighbors(ref LocalTransform localTransform, ref FishComponent fish)
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(localTransform.Position, fish.neighborDistance, LayerMask.GetMask("Fish"));
        
        return nearbyObjects.Select(obj => obj.transform).ToList();
    }

    public static float3 Normalize(float3 vector)
    {
        return vector / (Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z));
    }

}
