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
        
        // Gather all fish positions into the list
        foreach ((RefRO<LocalTransform> localTransform, RefRO<FishComponent> fish) in
                 SystemAPI.Query<RefRO<LocalTransform>, RefRO<FishComponent>>())
        {
            transformsAllFishes.Add(localTransform.ValueRO);
        }

        // Allocate a NativeArray for neighbours which will be filled by NeighborDetectionJob
        NativeArray<LocalTransform> neighboursFishes = new NativeArray<LocalTransform>(transformsAllFishes.Length, Allocator.TempJob);

        // Schedule Neighbor Detection Job
        var neighborJob = new NeighborDetectionJob3
        {
            AllFish = transformsAllFishes.AsDeferredJobArray(),
            neighbourFishPosition = neighboursFishes,
            neighborRadius = 5.0f
        };
        JobHandle neighborJobHandle = neighborJob.Schedule(transformsAllFishes.Length, 64, state.Dependency);

        // Schedule Fish Update Job using IJobEntity after neighbors have been detected
        var fishJob = new FishJob3
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            neighbourFishPosition = neighboursFishes
        };
        JobHandle fishJobHandle = fishJob.ScheduleParallel(neighborJobHandle);

        // Set system state dependency to ensure all jobs complete before the next update
        state.Dependency = fishJobHandle;

        // Dispose arrays after all jobs are completed
        transformsAllFishes.Dispose(fishJobHandle);
        neighboursFishes.Dispose(fishJobHandle);
    }
}

// Neighbor detection job that writes to neighbourFishPosition
[BurstCompile]
public struct NeighborDetectionJob3 : IJobParallelFor
{
    [ReadOnly] public NativeArray<LocalTransform> AllFish;
    [NativeDisableParallelForRestriction]
    public NativeArray<LocalTransform> neighbourFishPosition;
    public float neighborRadius;

    public void Execute(int index)
    {
        LocalTransform currentFish = AllFish[index];
        NativeList<LocalTransform> neighbors = new NativeList<LocalTransform>(Allocator.Temp);

        for (int i = 0; i < AllFish.Length; i++)
        {
            if (i == index) continue;

            LocalTransform otherFish = AllFish[i];
            if (math.distance(currentFish.Position, otherFish.Position) <= neighborRadius)
            {
                neighbors.Add(otherFish);
            }
        }

        // Store the average neighbor position for simplicity in this example
        if (neighbors.Length > 0)
        {
            float3 averagePosition = float3.zero;
            foreach (var neighbor in neighbors)
            {
                averagePosition += neighbor.Position;
            }
            averagePosition /= neighbors.Length;

            neighbourFishPosition[index] = new LocalTransform { Position = averagePosition }; // Modify as per your logic
        }

        neighbors.Dispose();
    }
}

// Fish behavior job that reads from neighbourFishPosition and directly modifies entities
[BurstCompile]
public partial struct FishJob3 : IJobEntity
{
    public float deltaTime;
    [NativeDisableParallelForRestriction]
    [ReadOnly] public NativeArray<LocalTransform> neighbourFishPosition;

    public void Execute(ref LocalTransform localTransform, ref FishComponent fish, [EntityIndexInQuery] int index)
    {
        if (index >= neighbourFishPosition.Length) return;

        // Read from neighboursFishes to perform boid logic
        LocalTransform neighbourTransform = neighbourFishPosition[index];

        // Cohesion - move towards the center of mass of neighbors
        float3 cohesion = math.normalize(neighbourTransform.Position - localTransform.Position);
        
        // Alignment - match velocity with neighbors (in this case using neighbor direction)
        float3 alignment = cohesion; // Simplification for demonstration purposes

        // Separation - avoid crowding neighbors
        float3 separation = math.normalize(localTransform.Position - neighbourTransform.Position);

        // Aggregate forces
        float3 moveDirection = cohesion * fish.cohesionWeight + alignment * fish.alignmentWeight + separation * fish.separationWeight + SelectionPoint(ref localTransform, ref fish) * fish.selectionPointWeight;
        moveDirection = math.normalize(moveDirection);

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
