using System;
using System.Collections.Generic;
using System.Linq;
using Plant;
using UnityEngine;

public class PlantTypeDataProvider : MonoBehaviour
{
    [SerializeField] private List<PlantTypeSpriteData> _datas;

    public PlantTypeSpriteData GetDataByType(PlantType type)
    {
        return _datas.FirstOrDefault(d => d.Type == type);
    }

    [Serializable]
    public class PlantTypeSpriteData
    {
        public PlantType Type;
        public Sprite Sprite;
        public string TypeName;
    }
}