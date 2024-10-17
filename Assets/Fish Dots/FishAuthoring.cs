using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishAuthoring : MonoBehaviour
{
    public class Baker : Baker<FishAuthoring>
    {
        public override void Bake(FishAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        
            AddComponent(entity, new FishComponent
            {
                movementVector = new float3(Random.Range(-1,1), 0,Random.Range(-1,1)),
                rotationValue = Random.Range(-5,5),
            });
        }
    }
}

public struct FishComponent : IComponentData
{
    public float3 movementVector;
    public float rotationValue;
}
