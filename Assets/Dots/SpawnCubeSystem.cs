using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class SpawnCubeSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<SpawnCubeConfig>();
    }
    
    protected override void OnUpdate()
    {
        //Ensures it only runs once
        this.Enabled = false;

        SpawnCubeConfig spawnCubeConfig = SystemAPI.GetSingleton<SpawnCubeConfig>();

        for (int i = 0; i < spawnCubeConfig.amountToSpawn; i++)
        {
            Entity spawnedEntity = EntityManager.Instantiate(spawnCubeConfig.cubePrefabEntity);
            EntityManager.SetComponentData(spawnedEntity, new LocalTransform
            {
                Position = new float3(Random.Range(-1000,500), 0.6f, Random.Range(-400,700)),
                Rotation = quaternion.identity,
                Scale = 1f
            });
            EntityManager.SetComponentData(spawnedEntity, new Movement
            {
                movementVector = new float3(Random.Range(-1,1), 0,Random.Range(-1,1))
            });
        }
    }
}
