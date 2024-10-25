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
    //public float3 movementVectorAuthoring;
    //public float rotationValueAuthoring;
    
    //[Header("Weights")]
    public float cohesionWeightAuthoring;
    public float alignmentWeightAuthoring;
    public float separationWeightAuthoring;
    public float selectionPointWeightAuthoring;
    //[Header("Neighbours")]
    //public NativeList<LocalTransform> neighbourFish;
    public float neighborDistanceAuthoring;
    //[Header("Seperation")]
    public float separationDistanceAuthoring;
    //[Header("Selection Point")]
    public float distanceFromSelectionPointAuthoring;
    public float3 selectionPointAuthoring;
    //[Header("Other")]
    /*
    public float3 velocityAuthoring;

    public float3 directionAuthoring;
    
    public float speedAuthoring;
    public float turnSpeedAuthoring;
    */
    
    public class Baker : Baker<FishAuthoring>
    {
        public override void Bake(FishAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        
            AddComponent(entity, new FishComponent
            {
                //movementVector = new float3(Random.Range(-1,1), 0,Random.Range(-1,1)),
                //rotationValue = Random.Range(-5,5),
                
                cohesionWeight = authoring.cohesionWeightAuthoring,
                alignmentWeight = authoring.alignmentWeightAuthoring,
                separationWeight = authoring.separationWeightAuthoring,
                selectionPointWeight = authoring.selectionPointWeightAuthoring,
                //neighbourFish = new NativeList<LocalTransform>(),
                neighborDistance = authoring.neighborDistanceAuthoring,
                separationDistance = authoring.separationDistanceAuthoring,
                distanceFromSelectionPoint = authoring.distanceFromSelectionPointAuthoring,
                selectionPoint = authoring.selectionPointAuthoring,
                speed = 3.5f,
                turnSpeed = 5.0f,
            });
        }
    }
}

public struct FishComponent : IComponentData
{
    //public float3 movementVector;
    //public float rotationValue;
    
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
