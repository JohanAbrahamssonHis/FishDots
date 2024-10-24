using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct BoidsSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float neighborRadius = 5.0f; // Example neighbor radius
        float desiredSeparation = 2.0f; // Example separation distance
        float maxSpeed = 10.0f; // Example max speed
        float deltaTime = SystemAPI.Time.DeltaTime;

        // Get type handles
        var localTransformHandle = state.GetComponentTypeHandle<LocalTransform>(true);
        var neighborBufferHandle = state.GetBufferTypeHandle<Neighbor>();
        var velocityHandle = state.GetComponentTypeHandle<Velocity>();

        // Get all transforms
        var allTransforms = new NativeArray<LocalTransform>(state.EntityManager.GetComponentCount<LocalTransform>(), Allocator.TempJob);
        state.EntityManager.GetAllUniqueSharedComponentData(allTransforms);

        // Schedule Neighbor Detection Job
        NeighborDetectionJob neighborJob = new NeighborDetectionJob
        {
            NeighborRadius = neighborRadius,
            LocalTransformHandle = localTransformHandle,
            NeighborBufferHandle = neighborBufferHandle,
            AllTransforms = allTransforms
        };
        var neighborHandle = neighborJob.ScheduleParallel(state.Dependency);

        // Schedule Separation Job
        SeparationJob separationJob = new SeparationJob
        {
            DesiredSeparation = desiredSeparation,
            NeighborBufferHandle = neighborBufferHandle,
            LocalTransformHandle = localTransformHandle,
            VelocityHandle = velocityHandle
        };
        var separationHandle = separationJob.ScheduleParallel(neighborHandle);

        // Schedule Cohesion Job
        CohesionJob cohesionJob = new CohesionJob
        {
            NeighborBufferHandle = neighborBufferHandle,
            LocalTransformHandle = localTransformHandle,
            VelocityHandle = velocityHandle
        };
        var cohesionHandle = cohesionJob.ScheduleParallel(separationHandle);

        // Schedule Alignment Job
        AlignmentJob alignmentJob = new AlignmentJob
        {
            NeighborBufferHandle = neighborBufferHandle,
            LocalTransformHandle = localTransformHandle,
            VelocityHandle = velocityHandle
        };
        var alignmentHandle = alignmentJob.ScheduleParallel(cohesionHandle);

        // Schedule Apply Movement Job
        ApplyMovementJob movementJob = new ApplyMovementJob
        {
            VelocityHandle = velocityHandle,
            LocalTransformHandle = localTransformHandle,
            MaxSpeed = maxSpeed,
            DeltaTime = deltaTime
        };
        var movementHandle = movementJob.ScheduleParallel(alignmentHandle);

        // Complete the final handle
        movementHandle.Complete();

        // Dispose temporary data
        allTransforms.Dispose();
    }
}

public struct Neighbor : IBufferElementData
{
    public Entity Entity;
}

public struct Velocity : IComponentData
{
    public float3 Value;
}
