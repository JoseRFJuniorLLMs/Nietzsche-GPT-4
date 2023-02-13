using UnityEngine;
using FrostweepGames.Plugins.Core;
using FrostweepGames.Plugins.Native;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public class VoiceDetectionManager : IService, IVoiceDetectionManager
    {
		private ISpeechRecognitionManager _speechRecognitionManager;

		private IMediaManager _mediaManager;

        private double _threshold;

        public void Init()
        {
			_speechRecognitionManager = ServiceLocator.Get<ISpeechRecognitionManager>();
			_mediaManager = ServiceLocator.Get<IMediaManager>();

			_threshold = _speechRecognitionManager.CurrentConfig.voiceDetectionThreshold;
		}

		public void Dispose()
        {
        }

        public void Update()
        {
        }

        public bool HasDetectedVoice(float[] data)
        {
            float average = 0f;
            return CustomMicrophone.IsVoiceDetected(data, ref average, _threshold);
        }

		public void DetectThreshold(int durationSec = 3)
		{
			GCSpeechRecognition.Instance.StartCoroutine(_mediaManager.OneTimeRecord(durationSec, (samples) =>
			{
                float accum = 0f;
                for (int i = 0; i < samples.Length; i++)
                {
                    accum += Mathf.Abs(samples[i]);
                }

                _threshold = System.Math.Round(accum / (float)samples.Length, 6) * 5;
                _speechRecognitionManager.CurrentConfig.voiceDetectionThreshold = _threshold;
            }));
		}
    }
}