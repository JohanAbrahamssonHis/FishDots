using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct CohesionJob : IJobChunk
{
    [ReadOnly] public BufferTypeHandle<Neighbor> NeighborBufferHandle;
    [ReadOnly] public ComponentTypeHandle<LocalTransform> LocalTransformHandle;
    public ComponentTypeHandle<Velocity> VelocityHandle;

    public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
    {
        var localTransforms = chunk.GetNativeArray(LocalTransformHandle);
        var neighborBuffers = chunk.GetBufferAccessor(NeighborBufferHandle);
        var velocities = chunk.GetNativeArray(VelocityHandle);

        for (int i = 0; i < chunk.Count; i++)
        {
            var currentTransform = localTransforms[i];
            float3 currentPosition = currentTransform.Position;
            var neighbors = neighborBuffers[i];
            float3 cohesionForce = float3.zero;
            int cohesionCount = 0;

            foreach (var neighbor in neighbors)
            {
                float3 neighborPosition = localTransforms[neighbor.Entity.Index].Position;
                cohesionForce += neighborPosition;
                cohesionCount++;
            }

            if (cohesionCount > 0)
            {
                float3 averagePosition = cohesionForce / cohesionCount;
                cohesionForce = math.normalize(averagePosition - currentPosition);
            }

            velocities[i] = new Velocity { Value = velocities[i].Value + cohesionForce };
        }
    }
}
