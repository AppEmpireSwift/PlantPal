using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Plant;
using UnityEngine;

namespace SaveSystem
{
    public class DataSaver
    {
        private readonly string _savePath = Path.Combine(Application.persistentDataPath, "PlantDatas");

        public void SaveData(List<PlantData> datas)
        {
            try
            {
                DataWrapper wrapper = new DataWrapper(datas);
                string jsonData = JsonConvert.SerializeObject(wrapper, Formatting.Indented);

                File.WriteAllText(_savePath, jsonData);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving item data: {e.Message}");
                throw;
            }
        }

        public List<PlantData> LoadData()
        {
            try
            {
                if (!File.Exists(_savePath))
                {
                    return null;
                }

                string jsonData = File.ReadAllText(_savePath);

                DataWrapper wrapper = JsonConvert.DeserializeObject<DataWrapper>(jsonData);

                return wrapper?.Datas ?? new List<PlantData>();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading item data: {e.Message}");
                return new List<PlantData>();
            }
        }
    }

    [Serializable]
    public class DataWrapper
    {
        public List<PlantData> Datas;

        public DataWrapper(List<PlantData> datas)
        {
            Datas = datas;
        }
    }
}