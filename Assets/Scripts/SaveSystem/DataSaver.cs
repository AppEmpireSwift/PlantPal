using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace SaveSystem
{
    /*public class DataSaver
    {
        private readonly string _savePath = Path.Combine(Application.persistentDataPath, "Datas");

        public void SaveData(List<Data.Data> datas)
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

        public List<Data.Data> LoadData()
        {
            try
            {
                if (!File.Exists(_savePath))
                {
                    return null;
                }

                string jsonData = File.ReadAllText(_savePath);

                DataWrapper wrapper = JsonConvert.DeserializeObject<DataWrapper>(jsonData);

                return wrapper?.Datas ?? new List<Data.Data>();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading item data: {e.Message}");
                return new List<Data.Data>();
            }
        }
    }

    [Serializable]
    public class DataWrapper
    {
        public List<Data.Data> Datas;

        public DataWrapper(List<Data.Data> datas)
        {
            Datas = datas;
        }
    }*/
}