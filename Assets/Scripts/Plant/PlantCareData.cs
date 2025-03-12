using System;
using UnityEngine;

namespace Plant
{
    [Serializable]
    public class PlantCareData
    {
        public string Name;
        public CareType CareType;

        public PlantCareData(string name, CareType careType)
        {
            Name = name;
            CareType = careType;
        }
    }

    public enum CareType
    {
        Watering,
        Spray,
        ChemicalCare,
        Fertilizer,
        Pruning,
        Transfer,
        None
    }
}
