using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct FishAspect : IAspect
{
    //public readonly RefRO<RotatingCube> rotatingCube;
    public readonly RefRW<LocalTransform> localTransform;
    public readonly RefRO<FishComponent> fish;

    public void FishLogic(float deltaTime)
    {
        localTransform.ValueRW = localTransform.ValueRO.RotateY(fish.ValueRO.rotationValue * deltaTime);
        localTransform.ValueRW = localTransform.ValueRO.Translate(fish.ValueRO.movementVector * deltaTime);

    }
}

[BurstCompile]
//[WithAll(typeof(RotatingCube))]
public partial struct FishJob : IJobEntity
{
    public float deltaTime;

    //RefRW = ref & RefRO = in
    public void Execute(ref LocalTransform localTransform, in FishComponent fish)
    {
        localTransform = localTransform.RotateY(fish.rotationValue * deltaTime);
        localTransform = localTransform.Translate(fish.movementVector * deltaTime);
    }
}
