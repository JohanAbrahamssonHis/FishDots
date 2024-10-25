using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public readonly partial struct FishAspect : IAspect
{
    //public readonly RefRO<RotatingCube> rotatingCube;
    public readonly RefRW<LocalTransform> localTransform;
    public readonly RefRW<FishComponent> fish;

    public void FishLogic(float deltaTime)
    {
        /*
        localTransform.ValueRW = localTransform.ValueRO.RotateY(fish.ValueRO.rotationValue * deltaTime);
        localTransform.ValueRW = localTransform.ValueRO.Translate(fish.ValueRO.movementVector * deltaTime);
        */
        
        /*
        neighborFish = HandleFishSystem.GetNeighbors(ref localTransform, ref fish);
        
        float3 cohesion = HandleFishSystem.Cohesion(ref localTransform, ref fish, ref neighborFish) * fish.cohesionWeight;
        float3 alignment = HandleFishSystem.Alignment(ref fish, ref neighborFish) * fish.alignmentWeight;
        float3 separation = HandleFishSystem.Separation(ref localTransform, ref fish, ref neighborFish) * fish.separationWeight;
        float3 selectionPoint = HandleFishSystem.SelectionPoint(ref localTransform, ref fish) * fish.selectionPointWeight;
        
        fish.direction = cohesion + alignment + separation  + selectionPoint;
        fish.velocity += Time.deltaTime * fish.direction;
        fish.velocity += Time.deltaTime * fish.speed * HandleFishSystem.Normalize(fish.velocity);
        fish.velocity = Vector3.ClampMagnitude(fish.velocity, fish.speed);
        
        Quaternion targetRotation = Quaternion.LookRotation(HandleFishSystem.Normalize(fish.velocity));
        localTransform.Rotation = Quaternion.Slerp(localTransform.Rotation, targetRotation, fish.turnSpeed * Time.deltaTime);

        localTransform.Position += fish.velocity * Time.deltaTime;
        */

    }
}

[BurstCompile]
[WithAll(typeof(FishAspect))]
public partial struct FishJob : IJobEntity
{
    public float deltaTime;
    public NativeArray<LocalTransform> AllFish;
    public NativeArray<LocalTransform> neighbourFishPosition;
    //public LocalTransform localTransform;
    //public FishComponent fish;
    
    //RefRW = ref & RefRO = in
    public void Execute(ref LocalTransform localTransform, ref FishComponent fish)
    {
        /*
        localTransform = localTransform.RotateY(fish.rotationValue * deltaTime);
        localTransform = localTransform.Translate(fish.movementVector * deltaTime);
        */
        
        
        neighbourFishPosition = GetNeighbors(ref localTransform, ref fish, ref AllFish, ref neighbourFishPosition);
        
        
        float3 cohesion = Cohesion(ref localTransform, ref fish, ref neighbourFishPosition) * fish.cohesionWeight;
        
        float3 alignment = Alignment(ref fish, ref neighbourFishPosition) * fish.alignmentWeight;
        
        float3 separation = Separation(ref localTransform, ref fish, ref neighbourFishPosition) * fish.separationWeight;
        
        float3 selectionPoint = SelectionPoint(ref localTransform, ref fish) * fish.selectionPointWeight;
        

        //float3 cohesion = new float3(0.5f, 0.5f, 0.5f);
        //float3 alignment = new float3(1, 1, 1);
        //float3 separation = new float3(1, 1, 1);
        //float3 selectionPoint =new float3(1, 1, 1);
        
        
        fish.direction = cohesion + alignment + separation  + selectionPoint;

        fish.velocity += deltaTime * fish.direction;
        fish.velocity += deltaTime * fish.speed * Normalize(fish.velocity);
        fish.velocity = Vector3.ClampMagnitude(fish.velocity, fish.speed);

        Quaternion targetRotation = Quaternion.LookRotation(Normalize(fish.velocity));
        localTransform.Rotation = Quaternion.Slerp(localTransform.Rotation, targetRotation, fish.turnSpeed * deltaTime);

        localTransform.Position += fish.velocity * deltaTime; 
        
    }
    
    public static float3 Cohesion(ref LocalTransform localTransform, ref FishComponent fishComponent, ref NativeArray<LocalTransform> neighborFish)
    {
        float3 centerOfMass = float3.zero;
        float3 position = localTransform.Position;

        foreach (LocalTransform fish in neighborFish)
        {
            centerOfMass += fish.Position;
        }

        if (neighborFish.Length == 0) return float3.zero;

        centerOfMass /= neighborFish.Length;
        return Normalize(centerOfMass - position);
    }

    public static float3 Alignment(ref FishComponent fishComponent, ref NativeArray<LocalTransform> neighborFish)
    {
        float3 averageHeading = float3.zero;

        foreach (LocalTransform fish in neighborFish)
        {
            averageHeading += fish.Forward();
        }

        if (neighborFish.Length == 0) return float3.zero;

        averageHeading /= neighborFish.Length;
        return Normalize(averageHeading);
    }

    public static float3 Separation(ref LocalTransform localTransform, ref FishComponent fishComponent, ref NativeArray<LocalTransform> neighborFish)
    {
        float3 separationForce = float3.zero;
        float3 position = localTransform.Position;

        foreach (LocalTransform fish in neighborFish)
        {
            if (Vector3.Distance(position, fish.Position) < fishComponent.separationDistance)
            {
                separationForce += position - fish.Position;
            }
        }

        if (neighborFish.Length == 0) return float3.zero;
        
        return Normalize(separationForce);
    }
    
    public static float3 SelectionPoint(ref LocalTransform localTransform, ref FishComponent fishComponent)
    {
        return Vector3.Distance(localTransform.Position, fishComponent.selectionPoint)>fishComponent.distanceFromSelectionPoint
            ? Normalize(fishComponent.selectionPoint-localTransform.Position) : float3.zero;
    }
    
    public static NativeArray<LocalTransform> GetNeighbors(ref LocalTransform localTransform, ref FishComponent fish, ref NativeArray<LocalTransform> allFishes, ref NativeArray<LocalTransform> nav)
    {
        NativeList<LocalTransform> jo = new NativeList<LocalTransform>(Allocator.TempJob);
        
        foreach (var obj in allFishes)
        {
            if(Vector3.Distance(obj.Position, localTransform.Position)<fish.neighborDistance) jo.Add(obj);
        }

        nav = jo;

        jo.Dispose();
        
        return nav;
    }

    public static float3 Normalize(float3 vector)
    {
        return vector / (Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z));
    }
}
