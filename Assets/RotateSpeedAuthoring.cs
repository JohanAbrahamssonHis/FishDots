using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class RotateSpeedAuthoring : MonoBehaviour
{
    public float value;
    
    private class Baker : Baker<RotateSpeedAuthoring>
    {
        public override void Bake(RotateSpeedAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new RotateSpeed
            {
                value = authoring.value
            });
        }
    }
}
