using System;
using System.IO;
using UnityEngine;

namespace Sources.Saves
{
    public class SaveLoadSystem
    {
        string RootPath =>
            _rootPath ??= Application.isEditor ? "Assets/Data/Saves/" : Application.persistentDataPath;

        private string _rootPath;

        private const string Format = ".json";

        public Snapshot Load()
        {
            var path = Path.Combine(RootPath, nameof(Snapshot) + Format);
            if (!File.Exists(path)) return null;
            
            try
            {
                var json = File.ReadAllText(Path.Combine(RootPath, nameof(Snapshot) + Format));
                var result = JsonUtility.FromJson<Snapshot>(json);
                return result;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public void Save(Snapshot snapshot)
        {
            if (snapshot == null) throw new NullReferenceException("save-object is null");
            
            try
            {
                var json = JsonUtility.ToJson(snapshot);
                File.WriteAllText(Path.Combine(RootPath, nameof(Snapshot) + Format), json);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}