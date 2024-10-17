using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpawnCubeAuthoringConfig : MonoBehaviour
{
    public GameObject cubePrefab;
    public int spawnAmount;

    public class Baker : Baker<SpawnCubeAuthoringConfig>
    {
        public override void Bake(SpawnCubeAuthoringConfig authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            
            AddComponent(entity, new SpawnCubeConfig
            {
                cubePrefabEntity = GetEntity(authoring.cubePrefab, TransformUsageFlags.Dynamic),
                amountToSpawn = authoring.spawnAmount,
            });
        }
    }
}

public struct SpawnCubeConfig : IComponentData
{
    public Entity cubePrefabEntity;
    public int amountToSpawn;
}
