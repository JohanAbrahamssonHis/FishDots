using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct HandleFishSystem : ISystem
{
    
    public void OnUpdate(ref SystemState state)
    {

        /*
        List<FishJob> fishJobs = new List<FishJob>();
       
        foreach (FishAspect fishAspect in
                 SystemAPI.Query<FishAspect>())
        {
            //fishAspect.FishLogic(SystemAPI.Time.DeltaTime);
            
            
        }
        */
        
        FishJob fishJob = new FishJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            neighbourFish = new NativeList<LocalTransform>(Allocator.Persistent),
            AllFish = new NativeList<LocalTransform>(Allocator.Persistent),
            //fish = fishAspect.fish.ValueRW,
            //localTransform = fishAspect.localTransform.ValueRW,
        };
        
        JobHandle job = fishJob.Schedule();
        
        fishJob.neighbourFish.Dispose(job);
        

       //CollectionHelper.CreateNativeArray<FishComponent>( SystemAPI.Query<FishComponent>, ref World.UpdateAllocator);

       /*
       FishJob fishJob = new FishJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            neighbourFish = new NativeList<LocalTransform>(Allocator.Persistent),
            AllFish = new NativeList<LocalTransform>(Allocator.Persistent),
            fish = fishAspect.First().fish.ValueRW,
            localTransform = fishAspect.First().localTransform.ValueRW,
        };
            
        JobHandle job = fishJob.Schedule();
            
        fishJob.neighbourFish.Dispose(job);
        */
        
        //job.Complete();

        //fishJob.neighbourFish.Dispose();
        //fishJob.AllFish.Dispose();
//AllFish = fish.GetFishes(),
        //neighbourFish = new NativeList<LocalTransform>(Allocator.TempJob),
    }

}
