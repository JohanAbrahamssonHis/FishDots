using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class FishAuthoring2 : MonoBehaviour
{
    public float3 SwimDirection;
    public float SwimSpeed;

    public class Baker : Baker<FishAuthoring2>
    {
        public override void Bake(FishAuthoring2 authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new FishConfig2
            {
                SwimDirection = authoring.SwimSpeed,
                SwimSpeed = authoring.SwimSpeed
            });
        }
    }
}

public struct FishConfig2 : IComponentData
{
    public float3 SwimDirection;
    public float SwimSpeed;
}
