using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial struct HandleCubeSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (RotatingMovingCubeAspect rotatingMovingCubeAspect in
                 SystemAPI.Query<RotatingMovingCubeAspect>().WithAll<RotatingCube>() )
        {
            rotatingMovingCubeAspect.MoveAndRotate(SystemAPI.Time.DeltaTime);
        }
    }
}
