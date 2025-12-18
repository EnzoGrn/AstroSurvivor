using UnityEngine;
using System.IO;

namespace AstroSurvivor {
    
    public static class BuildSaveSystem {

        private readonly static string _SaveFileName = "player_build.json";

        public static void Save(PlayerBuildState buildState)
        {
            string json = JsonUtility.ToJson(buildState);
            string path = Path.Combine(Application.persistentDataPath, _SaveFileName);

            File.WriteAllText(path, json);

            Debug.Log($"Build saved to {path}");
        }

        public static PlayerBuildState Load()
        {
            string path = Path.Combine(Application.persistentDataPath, _SaveFileName);

            if (File.Exists(path)) {
                string json = File.ReadAllText(path);

                return JsonUtility.FromJson<PlayerBuildState>(json);
            }

            return new PlayerBuildState();
        }

        public static void Reset()
        {
            string path = Path.Combine(Application.persistentDataPath, _SaveFileName);

            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
