using System.IO;
using UnityEngine;

namespace Kamenote.Save
{
    [System.Serializable]
    public abstract class SaveData<T> where T : SaveData<T>, new()
    {

        public static T Instance { get; protected set; } = new();

        protected enum Type
        {
            ItemDatas, OrderArchives, Store, Setting
        }

        protected void Save(Type type)
        {
            string json = JsonUtility.ToJson(Instance, true);

            StreamWriter streamWriter = new(Application.persistentDataPath + FileName(type));
            streamWriter.Write(json);
            streamWriter.Close();
        }

        protected void Load(Type type)
        {
            string path = Application.persistentDataPath + FileName(type);

            if (File.Exists(path))
            {
                StreamReader streamReader = new(path);
                string json = streamReader.ReadToEnd();
                streamReader.Close();

                Instance = JsonUtility.FromJson<T>(json);
            }
            else Instance = new();
        }

        protected void Delete(Type type)
        {
            string path = Application.persistentDataPath + FileName(type);
            
            if (File.Exists(path)) File.Delete(path);

            else throw new System.Exception(path + "を削除できませんでした。");
        }

        private static string FileName(Type type)
        {
            return type switch
            {
                Type.ItemDatas => "/ItemDatas.json",
                Type.OrderArchives => "/OrderArchives.json",
                Type.Store => "/Store.json",
                Type.Setting => "/Application.json",
                _ => null
            };
        }
    }
}
