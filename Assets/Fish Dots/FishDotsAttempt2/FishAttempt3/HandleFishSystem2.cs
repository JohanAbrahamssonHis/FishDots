using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct HandleFishSystem2 : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        NativeList<LocalTransform> transformsAllFishes = new NativeList<LocalTransform>(Allocator.TempJob);
        NativeList<float3> averagePositions = new NativeList<float3>(Allocator.TempJob);
        NativeList<float3> averageVelocities = new NativeList<float3>(Allocator.TempJob);
        NativeList<float3> separationForces = new NativeList<float3>(Allocator.TempJob);
        
        // Gather all fish positions into the list
        foreach ((RefRO<LocalTransform> localTransform, RefRO<FishComponent> fish) in
                 SystemAPI.Query<RefRO<LocalTransform>, RefRO<FishComponent>>())
        {
            transformsAllFishes.Add(localTransform.ValueRO);
            // Initialize with zero vectors for average positions, velocities, and separation forces.
            averagePositions.Add(float3.zero);
            averageVelocities.Add(float3.zero);
            separationForces.Add(float3.zero);
        }

        // Schedule Neighbor Detection Job
        var neighborJob = new NeighborDetectionJob3
        {
            AllFish = transformsAllFishes.AsDeferredJobArray(),
            neighborRadius = 5.0f,
            separationDistance = 2.0f,
            averagePositions = averagePositions,
            averageVelocities = averageVelocities,
            separationForces = separationForces,
        };
        JobHandle neighborJobHandle = neighborJob.Schedule(transformsAllFishes.Length, 64, state.Dependency);

        // Schedule Fish Update Job using IJobEntity after neighbors have been detected
        var fishJob = new FishJob3
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            averagePositions = averagePositions,
            averageVelocities = averageVelocities,
            separationForces = separationForces
        };
        JobHandle fishJobHandle = fishJob.ScheduleParallel(neighborJobHandle);

        // Set system state dependency to ensure all jobs complete before the next update
        state.Dependency = fishJobHandle;

        // Dispose arrays after all jobs are completed
        transformsAllFishes.Dispose(fishJobHandle);
        averagePositions.Dispose(fishJobHandle);
        averageVelocities.Dispose(fishJobHandle);
        separationForces.Dispose(fishJobHandle);
    }
}

// Neighbor detection job that writes average positions, velocities, and separation forces
[BurstCompile]
public struct NeighborDetectionJob3 : IJobParallelFor
{
    [ReadOnly] public NativeArray<LocalTransform> AllFish;
    public float neighborRadius;
    public float separationDistance;

    [NativeDisableParallelForRestriction]
    public NativeArray<float3> averagePositions;

    [NativeDisableParallelForRestriction]
    public NativeArray<float3> averageVelocities;

    [NativeDisableParallelForRestriction]
    public NativeArray<float3> separationForces;

    public void Execute(int index)
    {
        LocalTransform currentFish = AllFish[index];
        float3 cohesionPosition = float3.zero;
        float3 alignmentVelocity = float3.zero;
        float3 separationForce = float3.zero;
        int neighborCount = 0;

        for (int i = 0; i < AllFish.Length; i++)
        {
            if (i == index) continue;

            float distance = math.distance(currentFish.Position, AllFish[i].Position);
            if (distance <= neighborRadius)
            {
                cohesionPosition += AllFish[i].Position;
                alignmentVelocity += AllFish[i].Forward(); // Assuming Forward() is the velocity direction.
                neighborCount++;

                // Calculate separation force if neighbor is too close
                if (distance < separationDistance)
                {
                    float3 awayVector = currentFish.Position - AllFish[i].Position;
                    separationForce += math.normalize(awayVector) / distance; // Weighted by inverse distance
                }
            }
        }

        if (neighborCount > 0)
        {
            cohesionPosition /= neighborCount;
            alignmentVelocity /= neighborCount;
        }

        // Store the results in the corresponding arrays
        averagePositions[index] = cohesionPosition;
        averageVelocities[index] = alignmentVelocity;
        separationForces[index] = separationForce;
    }
}

// Fish behavior job that reads from the precomputed values and directly modifies entities
[BurstCompile]
public partial struct FishJob3 : IJobEntity
{
    public float deltaTime;

    [NativeDisableParallelForRestriction]
    [ReadOnly] public NativeArray<float3> averagePositions;

    [NativeDisableParallelForRestriction]
    [ReadOnly] public NativeArray<float3> averageVelocities;

    [NativeDisableParallelForRestriction]
    [ReadOnly] public NativeArray<float3> separationForces;

    public void Execute(ref LocalTransform localTransform, ref FishComponent fish, [EntityIndexInQuery] int index)
    {
        if (index >= averagePositions.Length) return;

        // Cohesion - move towards the average position of neighbors
        float3 cohesion = math.normalize(averagePositions[index] - localTransform.Position);
        //if (math.any(math.isnan(cohesion)))
       // {
        //    cohesion = float3.zero; // Fallback value if NaN is detected
        //}
        
        // Alignment - align velocity to match average neighbor velocity
        float3 alignment = math.normalize(averageVelocities[index]);
        if (math.any(math.isnan(alignment)))
        {
            alignment = float3.zero; // Fallback value if NaN is detected
        }

        // Separation - avoid crowding neighbors
        float3 separation = math.normalize(separationForces[index]);
        if (math.any(math.isnan(separation)))
        {
            separation = float3.zero; // Fallback value if NaN is detected
        }

        // Aggregate forces with weights
        float3 moveDirection = cohesion * fish.cohesionWeight + alignment * fish.alignmentWeight + separation * fish.separationWeight + SelectionPoint(ref localTransform, ref fish) * fish.selectionPointWeight;
        moveDirection = math.normalize(moveDirection);
        //if (math.any(math.isnan(moveDirection)))
        //{
        //    moveDirection = float3.zero; // Fallback value if NaN is detected in the final direction
        //}

        // Update velocity and apply movement
        fish.velocity += moveDirection * fish.speed * deltaTime;
        fish.velocity = Vector3.ClampMagnitude(fish.velocity, 4);

        // Update entity's transform
        localTransform.Position += fish.velocity * deltaTime;

        // Update rotation to face the movement direction
        if (math.lengthsq(fish.velocity) > 0)
        {
            quaternion targetRotation = quaternion.LookRotationSafe(math.normalize(fish.velocity), math.up());
            localTransform.Rotation = math.slerp(localTransform.Rotation, targetRotation, fish.turnSpeed * deltaTime);
        }
    }

    
    public static float3 SelectionPoint(ref LocalTransform localTransform, ref FishComponent fishComponent)
    {
        return Vector3.Distance(localTransform.Position, fishComponent.selectionPoint)>fishComponent.distanceFromSelectionPoint
            ? math.normalize(fishComponent.selectionPoint-localTransform.Position) : float3.zero;
    }
}
