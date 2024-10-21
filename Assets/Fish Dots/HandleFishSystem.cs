using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
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
            deltaTime = SystemAPI.Time.DeltaTime,
        };
        fishJob.Run();
    }

}
