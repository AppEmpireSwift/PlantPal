using System;
using System.Collections.Generic;
using System.Linq;
using Plant;
using UnityEngine;

public class CareTypeDataProvider : MonoBehaviour
{
    [SerializeField] private List<PlantCareTypeSpriteData> _datas;

    public PlantCareTypeSpriteData GetDataByType(CareType type)
    {
        return _datas.FirstOrDefault(d => d.Type == type);
    }

    [Serializable]
    public class PlantCareTypeSpriteData
    {
        public CareType Type;
        public Sprite Sprite;
        public string Name;
    }
}