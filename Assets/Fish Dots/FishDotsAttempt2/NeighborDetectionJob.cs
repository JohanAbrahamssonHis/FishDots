using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct NeighborDetectionJob : IJobChunk
{
    [ReadOnly] public float NeighborRadius;
    [ReadOnly] public ComponentTypeHandle<LocalTransform> LocalTransformHandle;
    public BufferTypeHandle<Neighbor> NeighborBufferHandle;
    public NativeArray<LocalTransform> AllTransforms;

    public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
    {
        var localTransforms = chunk.GetNativeArray(LocalTransformHandle);
        var neighborBuffers = chunk.GetBufferAccessor(NeighborBufferHandle);

        for (int i = 0; i < localTransforms.Length; i++)
        {
            var currentTransform = localTransforms[i];
            var currentPosition = currentTransform.Position;
            var neighbors = neighborBuffers[i];
            neighbors.Clear();

            for (int j = 0; j < AllTransforms.Length; j++)
            {
                if (i == j) continue;
                float3 otherPosition = AllTransforms[j].Position;
                float distance = math.distance(currentPosition, otherPosition);

                if (distance <= NeighborRadius)
                {
                    neighbors.Add(new Neighbor { Entity = new Entity() }); // Note: Properly set Entity if needed
                }
            }
        }
    }
}
