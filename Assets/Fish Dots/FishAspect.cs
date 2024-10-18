using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct FishAspect : IAspect
{
    //public readonly RefRO<RotatingCube> rotatingCube;
    public readonly RefRW<LocalTransform> localTransform;
    public readonly RefRO<FishComponent> fish;

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
//[WithAll(typeof(RotatingCube))]
public partial struct FishJob : IJobEntity
{
    public float deltaTime;
    public List<Transform> neighborFish;
    //RefRW = ref & RefRO = in
    public void Execute(ref LocalTransform localTransform, ref FishComponent fish)
    {
        /*
        localTransform = localTransform.RotateY(fish.rotationValue * deltaTime);
        localTransform = localTransform.Translate(fish.movementVector * deltaTime);
        */
        
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
    }
}
