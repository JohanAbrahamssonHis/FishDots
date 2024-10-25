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

//[BurstCompile]
public partial struct BoidsSystem : ISystem
{
    /*
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
        var FishConfig2Handle = state.GetComponentTypeHandle<FishConfig2>();

        // Create EntityQuery to select appropriate entities with FishAspect
        EntityQuery query = state.GetEntityQuery(typeof(FishConfig2), typeof(LocalTransform), typeof(Velocity));

        // Get all transforms (temporary data for neighbor detection)
        var allTransforms = query.ToComponentDataArray<LocalTransform>(Allocator.TempJob);

        // Schedule Neighbor Detection Job
        NeighborDetectionJob neighborJob = new NeighborDetectionJob
        {
            NeighborRadius = neighborRadius,
            LocalTransformHandle = localTransformHandle,
            NeighborBufferHandle = neighborBufferHandle,
            AllTransforms = allTransforms
        };
        var neighborHandle = neighborJob.ScheduleParallel(query, state.Dependency);
        state.Dependency = neighborHandle; // Update dependency

        // Schedule Separation Job
        SeparationJob separationJob = new SeparationJob
        {
            DesiredSeparation = desiredSeparation,
            NeighborBufferHandle = neighborBufferHandle,
            LocalTransformHandle = localTransformHandle,
            VelocityHandle = velocityHandle
        };
        var separationHandle = separationJob.ScheduleParallel(query, state.Dependency);
        state.Dependency = separationHandle; // Update dependency

        // Schedule Cohesion Job
        CohesionJob cohesionJob = new CohesionJob
        {
            NeighborBufferHandle = neighborBufferHandle,
            LocalTransformHandle = localTransformHandle,
            VelocityHandle = velocityHandle
        };
        var cohesionHandle = cohesionJob.ScheduleParallel(query, state.Dependency);
        state.Dependency = cohesionHandle; // Update dependency

        // Schedule Alignment Job
        AlignmentJob alignmentJob = new AlignmentJob
        {
            NeighborBufferHandle = neighborBufferHandle,
            LocalTransformHandle = localTransformHandle,
            VelocityHandle = velocityHandle
        };
        var alignmentHandle = alignmentJob.ScheduleParallel(query, state.Dependency);
        state.Dependency = alignmentHandle; // Update dependency

        // Schedule Apply Movement Job
        ApplyMovementJob movementJob = new ApplyMovementJob
        {
            VelocityHandle = velocityHandle,
            LocalTransformHandle = localTransformHandle,
            MaxSpeed = maxSpeed,
            DeltaTime = deltaTime
        };
        var movementHandle = movementJob.ScheduleParallel(query, state.Dependency);
        state.Dependency = movementHandle; // Update dependency

        // Complete the final handle
        state.Dependency.Complete();

        // Dispose temporary data
        allTransforms.Dispose();
        
    }
    */
}

public struct Neighbor : IBufferElementData
{
    public Entity Entity;
}

public struct Velocity : IComponentData
{
    public float3 Value;
}
