using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
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
        }
    }
}

public struct SpawnFishConfig : IComponentData
    {
        public Entity fishPrefabEntity;
        public int amountToSpawn;
    }
