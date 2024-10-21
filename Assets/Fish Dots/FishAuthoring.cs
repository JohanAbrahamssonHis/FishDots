using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
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
                
                cohesionWeight = 1.0f,
                alignmentWeight = 1.0f,
                separationWeight = 1.5f,
                selectionPointWeight = 10f,
                //neighbourFish = new NativeList<LocalTransform>(),
                neighborDistance = 3.0f,
                separationDistance = 1.0f,
                distanceFromSelectionPoint = 10f,
                speed = 3.5f,
                turnSpeed = 5.0f,
            });
        }
    }
}

public struct FishComponent : IComponentData
{
    public float3 movementVector;
    public float rotationValue;
    
    //[Header("Weights")]
    public float cohesionWeight;
    public float alignmentWeight;
    public float separationWeight;
    public float selectionPointWeight;
    //[Header("Neighbours")]
    //public NativeList<LocalTransform> neighbourFish;
    public float neighborDistance;
    //[Header("Seperation")]
    public float separationDistance;
    //[Header("Selection Point")]
    public float distanceFromSelectionPoint;
    public float3 selectionPoint;
    //[Header("Other")]
    
    public float3 velocity;

    public float3 direction;
    
    public float speed;
    public float turnSpeed;
    
    //public List<Transform> neighborFish;
    //[Header("Other")]
    //public Collider collider;
    //public Renderer renderer;
}
