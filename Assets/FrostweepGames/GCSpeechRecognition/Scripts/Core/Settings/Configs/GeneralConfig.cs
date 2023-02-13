using UnityEngine;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    //[CreateAssetMenu(fileName = "GeneralConfig", menuName = "FrostweepGames/GoogleCloud/SpeechRecognition/GeneralConfig", order = 3)]
    public class GeneralConfig : ScriptableObject
    {
        private static GeneralConfig _Config;
        public static GeneralConfig Config
        {
            get
            {
                if (_Config == null)
                    _Config = GetConfig();
                return _Config;
            }
        }

        public bool showWelcomeDialogAtStartup = true;

        private static GeneralConfig GetConfig()
        {
            string path = "GCSpeechRecognition/GeneralConfig";
            var config = Resources.Load<GeneralConfig>(path);

            if(config == null)
            {
                Debug.LogError($"Google Cloud Speech Recognition General Config not found in {path} Resources folder. Will use default.");

                config = (GeneralConfig)CreateInstance("GeneralConfig");

#if UNITY_EDITOR
                string pathToFolder = "Assets/Frostweepgames/GCSpeechRecognition/Resources/GCSpeechRecognition";
                string filename = "GeneralConfig.asset";

                if (!System.IO.Directory.Exists(Application.dataPath + "/../" + pathToFolder))
                {
                    System.IO.Directory.CreateDirectory(pathToFolder);
                    UnityEditor.AssetDatabase.ImportAsset(pathToFolder);
                }

                if (!System.IO.File.Exists(Application.dataPath + "/../" + pathToFolder + "/" + filename))
                {
                    UnityEditor.AssetDatabase.CreateAsset(config, pathToFolder + "/" + filename);
                }
                UnityEditor.AssetDatabase.SaveAssets();
#endif
            }

            return config;
        }
    }
}
