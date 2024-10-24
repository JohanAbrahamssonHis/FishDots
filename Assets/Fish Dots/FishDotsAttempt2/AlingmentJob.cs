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
public partial struct AlignmentJob : IJobChunk
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
            var neighbors = neighborBuffers[i];
            float3 alignmentForce = float3.zero;
            int alignmentCount = 0;

            foreach (var neighbor in neighbors)
            {
                float3 neighborForward = localTransforms[neighbor.Entity.Index].Forward();
                alignmentForce += neighborForward;
                alignmentCount++;
            }

            if (alignmentCount > 0)
            {
                alignmentForce = math.normalize(alignmentForce / alignmentCount);
            }

            velocities[i] = new Velocity { Value = velocities[i].Value + alignmentForce };
        }
    }
}
