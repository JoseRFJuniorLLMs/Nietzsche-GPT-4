namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    [UnityEditor.InitializeOnLoad]
    public class DefineProcessing : Plugins.DefineProcessing
    {
        internal static readonly string[] _Defines = new string[]
        {
            "FG_GCSR"
        };

        static DefineProcessing()
        {
            AddOrRemoveDefines(true, true, _Defines);
        }
    }
}