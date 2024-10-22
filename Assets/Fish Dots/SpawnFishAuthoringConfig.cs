using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class SpawnFishAuthoringConfig : MonoBehaviour
{
    public GameObject fishPrefab;
    public int spawnAmount;

    public class Baker : Baker<SpawnFishAuthoringConfig>
    {
        public override void Bake(SpawnFishAuthoringConfig authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new SpawnFishConfig
            {
                fishPrefabEntity = GetEntity(authoring.fishPrefab, TransformUsageFlags.Dynamic),
                amountToSpawn = authoring.spawnAmount,
            });
            
            AddComponent(entity, new AllFishConfig
            {
                //Fishes = new NativeList<LocalTransform>(Allocator.Temp),
            });
        }
    }
}

public struct SpawnFishConfig : IComponentData
    {
        public Entity fishPrefabEntity;
        public int amountToSpawn;
    }

public struct AllFishConfig : IComponentData
{
    //public NativeList<LocalTransform> Fishes;

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
    */
    
    
    
}
