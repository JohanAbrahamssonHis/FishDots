using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class MovementAuthoring : MonoBehaviour
{
    public class Baker : Baker<MovementAuthoring>
    {
        public override void Bake(MovementAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new Movement
            {
                movementVector = new float3(Random.Range(-1,1), 0,Random.Range(-1,1))
            });
        }
    }
}

public struct Movement : IComponentData
{
    public float3 movementVector;
}
