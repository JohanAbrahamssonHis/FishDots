using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct RotatingMovingCubeAspect : IAspect
{
    //public readonly RefRO<RotatingCube> rotatingCube;
    public readonly RefRW<LocalTransform> localTransform;
    public readonly RefRO<RotateSpeed> rotateSpeed;
    public readonly RefRO<Movement> movement;
    
    public void MoveAndRotate(float deltaTime)
    {
        localTransform.ValueRW = localTransform.ValueRO.RotateY(rotateSpeed.ValueRO.value * deltaTime);
        localTransform.ValueRW = localTransform.ValueRO.Translate(movement.ValueRO.movementVector * deltaTime);

    }
    
    [BurstCompile]
    [WithAll(typeof(RotatingCube))]
    public partial struct RotatingMovingCubeJob : IJobEntity
    {
        public float deltaTime;
        //RefRW = ref & RefRO = in
        public void Execute(ref LocalTransform localTransform, in RotateSpeed rotateSpeed, in Movement movement)
        {
            localTransform = localTransform.RotateY(rotateSpeed.value * deltaTime);
            localTransform= localTransform.Translate(movement.movementVector * deltaTime);
        }
    }
}
