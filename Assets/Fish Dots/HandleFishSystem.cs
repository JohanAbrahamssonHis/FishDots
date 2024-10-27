using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Unity.Burst;
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
        
        
        

        //fishes = SystemAPI.Query<FishAspect>();
        /*
        List<FishJob> fishJobs = new List<FishJob>();
       
        foreach (FishAspect fishAspect in
                 SystemAPI.Query<FishAspect>())
        {
            fishAspect.FishLogic(SystemAPI.Time.DeltaTime);
            
            
        }
        */
        
        /*
        int boidCount = 10/* Get boid count from entity manager;
        NativeArray<float3> boidPositions = new NativeArray<float3>(boidCount, Allocator.TempJob);
        NativeParallelMultiHashMap<int, int> neighbors = new NativeParallelMultiHashMap<int, int>(boidCount * 10, Allocator.TempJob); // Estimate capacity

        // Fill boidPositions with your current entity positions, e.g.:
        // for (int i = 0; i < boidCount; i++) { boidPositions[i] =  position of boid i; }

        NeighborDetectionJob neighborJob = new NeighborDetectionJob
        {
            boidPositions = boidPositions,
            neighbors = neighbors.AsParallelWriter(),
            neighborRadius = 5.0f // Example radius
        };

        JobHandle handle = neighborJob.Schedule(boidCount, 64);
        handle.Complete();

        // You can now read from `neighbors` NativeMultiHashMap to find neighbors for each boid.

        // Cleanup
        boidPositions.Dispose();
        neighbors.Dispose();
        */

        /*
        NativeList<LocalTransform> neighboursFishes = new NativeList<LocalTransform>(Allocator.TempJob);
        NativeList<LocalTransform> transformsAllFishes = new NativeList<LocalTransform>(Allocator.TempJob);
        
        
        foreach ((RefRO<LocalTransform> localTransform, RefRO<FishComponent> fish) in
                 SystemAPI.Query<RefRO<LocalTransform>, RefRO<FishComponent>>())
        {
            //neighboursFishes.Add(localTransform.ValueRO);
            transformsAllFishes.Add(localTransform.ValueRO);
        }
        */
        //Debug.Log($"{transformsAllFishes.Length} ya da first one");
        /*
        JobHandle jobH2 = new NeightbourJob
        {
            AllFish = transformsAllFishes.AsDeferredJobArray(),
            neighbourFishPosition = neighboursFishes.AsDeferredJobArray(),
            //fish = fishAspect.fish.ValueRW,
            //localTransform = fishAspect.localTransform.ValueRW,
        }.ScheduleParallel(state.Dependency);
        jobH2.Complete();
        */
        /*
        FishJob fishJob = new FishJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            AllFish = transformsAllFishes.AsDeferredJobArray(),
            neighbourFishPosition = neighboursFishes.AsDeferredJobArray(),
            //fish = fishAspect.fish.ValueRW,
            //localTransform = fishAspect.localTransform.ValueRW,
        };
        
        JobHandle jobH = fishJob.ScheduleParallel(state.Dependency);

        state.Dependency = jobH;
        
        neighboursFishes.Dispose(jobH);
        transformsAllFishes.Dispose(jobH);
        */
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
/*
[BurstCompile]
struct NeighborDetectionJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<float3> boidPositions;
    public NativeParallelMultiHashMap<int, int>.ParallelWriter neighbors; // Use ParallelWriter for concurrent writes.
    public float neighborRadius;

    public void Execute(int index)
    {
        float3 boidPosition = boidPositions[index];
        for (int i = 0; i < boidPositions.Length; i++)
        {
            if (i == index) continue; // Skip self

            float3 otherPosition = boidPositions[i];
            float distance = math.distance(boidPosition, otherPosition);

            if (distance <= neighborRadius)
            {
                neighbors.Add(index, i);
            }
        }
    }
}
*/

