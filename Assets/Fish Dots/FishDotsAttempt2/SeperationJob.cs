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
public partial struct SeparationJob : IJobChunk
{
    [ReadOnly] public float DesiredSeparation;
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
            float3 separationForce = float3.zero;
            int separationCount = 0;

            foreach (var neighbor in neighbors)
            {
                float3 neighborPosition = localTransforms[neighbor.Entity.Index].Position;
                float distance = math.distance(currentPosition, neighborPosition);

                if (distance > 0 && distance < DesiredSeparation)
                {
                    float3 repelDirection = math.normalize(currentPosition - neighborPosition);
                    separationForce += repelDirection / distance;
                    separationCount++;
                }
            }

            if (separationCount > 0)
            {
                separationForce /= separationCount;
            }

            velocities[i] = new Velocity { Value = velocities[i].Value + separationForce };
        }
    }
}
