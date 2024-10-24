using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct ApplyMovementJob : IJobChunk
{
    [ReadOnly] public ComponentTypeHandle<Velocity> VelocityHandle;
    public ComponentTypeHandle<LocalTransform> LocalTransformHandle;
    public float MaxSpeed;
    public float DeltaTime;

    public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
    {
        var localTransforms = chunk.GetNativeArray(LocalTransformHandle);
        var velocities = chunk.GetNativeArray(VelocityHandle);

        for (int i = 0; i < chunk.Count; i++)
        {
            float3 velocity = velocities[i].Value;
            velocity = math.normalize(velocity) * math.min(math.length(velocity), MaxSpeed);
            localTransforms[i] = new LocalTransform
            {
                Position = localTransforms[i].Position + velocity * DeltaTime,
                Rotation = localTransforms[i].Rotation,
                Scale = localTransforms[i].Scale
            };
        }
    }
}