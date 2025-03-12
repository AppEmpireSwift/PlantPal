using System;
using System.Collections.Generic;
using UnityEngine;

namespace Plant
{
    [Serializable]
    public class PlantData
    {
        public string Name;
        public PlantType PlantType;
        public int CustomFrequency;
        public WateringType WateringType;
        public string Note;
        public List<PlantCareData> PlantCareDatas = new();
        public byte[] Photo;
        public int WateringProgress;
        public DateTime LastWateringDate;
        public DateTime LastWateringResetDate;

        public PlantData(string name, PlantType plantType, WateringType wateringType, string note, byte[] photo)
        {
            Name = name;
            PlantType = plantType;
            WateringType = wateringType;
            Note = note;
            Photo = photo;
            LastWateringDate = DateTime.Now;
            LastWateringResetDate = DateTime.Now;
        }
    }

    public enum PlantType
    {
        Leaves,
        Flower,
        Succulent,
        Palm,
        Bamboo,
        Fruit,
        Tree,
        None
    }

    public enum WateringType
    {
        Daily,
        EveryTwoDays,
        TwiceAWeek,
        OnceAWeek,
        EveryTwoWeeks,
        OnceAMonth,
        Custom,
        None
    }
}