using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public struct AllFish : IComponentData
{
    private NativeList<LocalTransform> Fishes;

    public void Initialize()
    {
        Fishes = new NativeList<LocalTransform>();
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


//Collider[] nearbyObjects = Physics.OverlapSphere(localTransform.Position, fish.neighborDistance, LayerMask.GetMask("Fish"));