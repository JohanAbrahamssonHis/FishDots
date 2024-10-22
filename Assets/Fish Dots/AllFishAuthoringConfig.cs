using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class AllFishAuthoringConfig : MonoBehaviour
{
    /*

    public class Baker : Baker<AllFishAuthoringConfig>
    {
        public override void Bake(AllFishAuthoringConfig authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new AllFishConfig
            {
                //Fishes = new NativeList<LocalTransform>(Allocator.Persistent),
            });
        }
    }
    */
}
/*
public struct AllFishConfig : IComponentData
{
    public NativeList<LocalTransform> Fishes;

    /*
    public void Initialize()
    {
        Fishes = new NativeList<LocalTransform>(Allocator.Persistent);
    }
    
    public void AddEntity(LocalTransform localTransform)
    {
        Fishes.Add(localTransform);
    }

    public NativeList<LocalTransform> GetFishes()
    {
        return Fishes;
    }
    
    
    
    
}
*/


//Collider[] nearbyObjects = Physics.OverlapSphere(localTransform.Position, fish.neighborDistance, LayerMask.GetMask("Fish"));