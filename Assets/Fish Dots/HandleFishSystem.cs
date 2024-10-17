using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public partial struct HandleFishSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        /*
        foreach (FishAspect fishAspect in
                 SystemAPI.Query<FishAspect>())
        {
            fishAspect.FishLogic(SystemAPI.Time.DeltaTime);
        }
        */
        
    
        
        FishJob fishJob = new FishJob()
        {
            deltaTime = SystemAPI.Time.DeltaTime
        };
        fishJob.ScheduleParallel();
        
    }

}
