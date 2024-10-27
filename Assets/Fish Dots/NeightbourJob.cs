using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;


//[BurstCompile]
[WithAll(typeof(FishAspect))]
public partial struct NeightbourJob : IJobEntity
{
    [ReadOnly] public NativeArray<LocalTransform> AllFish;

    public NativeArray<LocalTransform> neighbourFishPosition;
    //public LocalTransform localTransform;
    //public FishComponent fish;

    //RefRW = ref & RefRO = in
    public void Execute([EntityIndexInQuery] int entityIndexInQuery, ref LocalTransform localTransform, ref FishComponent fish)
    {
        neighbourFishPosition = GetNeighbors(ref localTransform, ref fish, in AllFish, ref neighbourFishPosition);
    }
    
    public static NativeArray<LocalTransform> GetNeighbors(ref LocalTransform localTransform, ref FishComponent fish, in NativeArray<LocalTransform> allFishes, ref NativeArray<LocalTransform> nav)
    {
        NativeList<LocalTransform> jo = new NativeList<LocalTransform>(Allocator.TempJob);
        
        foreach (var obj in allFishes)
        {
            if(Vector3.Distance(obj.Position, localTransform.Position)<fish.neighborDistance) jo.Add(obj);
        }
        
        nav = jo;

        jo.Dispose();
        
        return nav;
    }
}

