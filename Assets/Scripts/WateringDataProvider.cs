using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Plant;
using UnityEngine;

public class WateringDataProvider : MonoBehaviour
{
    [SerializeField] private WaterNameString[] _waterNameString;

    public string GetName(WateringType type)
    {
        return _waterNameString.FirstOrDefault(d => d.Type == type)?.Name;
    }

    [Serializable]
    private class WaterNameString
    {
        public string Name;
        public WateringType Type;
    }
}