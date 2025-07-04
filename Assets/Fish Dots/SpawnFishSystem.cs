using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class SpawnFishSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<SpawnFishConfig>();
        RequireForUpdate<AllFishConfig>();
    }
    
    protected override void OnUpdate()
    {
        //Ensures it only runs once
        this.Enabled = false;
        
        SpawnFishConfig spawnFishConfig = SystemAPI.GetSingleton<SpawnFishConfig>();

        //AllFishConfig allFish = SystemAPI.GetSingleton<AllFishConfig>();
        //allFish.Initialize();
        
        for (int i = 0; i < spawnFishConfig.amountToSpawn; i++)
        {
            Entity spawnedEntity = EntityManager.Instantiate(spawnFishConfig.fishPrefabEntity);
            EntityManager.SetComponentData(spawnedEntity, new LocalTransform
            {
                Position = new float3(Random.Range(-100f,100f), Random.Range(-10f,10f), Random.Range(-100f,100f)),
                Rotation = quaternion.identity,
                Scale = 1f
            });
            /*
            EntityManager.SetComponentData(spawnedEntity, new Movement
            {
                movementVector = new float3(Random.Range(-1,1), 0,Random.Range(-1,1))
            });
            */
            
            //allFish.AddEntity(EntityManager.GetComponentData<LocalTransform>(spawnedEntity));
            
        }
    }
}
