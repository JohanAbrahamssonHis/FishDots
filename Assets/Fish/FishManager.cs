using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    public static List<Fish> allFishes = new List<Fish>();

    private void OnDestroy()
    {
        allFishes.Clear();
    }
}
