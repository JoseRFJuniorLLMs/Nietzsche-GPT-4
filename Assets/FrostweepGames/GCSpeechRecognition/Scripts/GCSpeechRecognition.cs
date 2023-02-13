using FrostweepGames.Plugins.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
	public class GCSpeechRecognition : MonoBehaviour
	{
		private static GCSpeechRecognition _Instance;
		public static GCSpeechRecognition Instance
		{
			get
			{
				if (_Instance == null)
				{
					_Instance = new GameObject("[Singleton]GCSpeechRecognition").AddComponent<GCSpeechRecognition>();
					_Instance.configs = new List<Config>();
					_Instance.configs.Add(Resources.Load<Config>("GCSpeechRecognitonConfig"));
					_Instance.apiKey = string.Empty; // there could be a default api key
				}

				return _Instance;
			}
		}

		public event Action<RecognitionResponse> RecognizeSuccessEvent;
		public event Action<string> RecognizeFailedEvent;
		public event Action<Operation> LongRunningRecognizeSuccessEvent;
		public event Action<string> LongRunningRecognizeFailedEvent;
		public event Action<Operation> GetOperationSuccessEvent;
		public event Action<string> GetOperationFailedEvent;
		public event Action<ListOperationsResponse> ListOperationsSuccessEvent;
		public event Action<string> ListOperationsFailedEvent;

		public event Action StartedRecordEvent;
		public event Action<AudioClip, float[]> FinishedRecordEvent;
		public event Action RecordFailedEvent;

		public event Action BeginTalkigEvent;
		public event Action<AudioClip, float[]> EndTalkigEvent;

		private ISpeechRecognitionManager _speechRecognitionManager;

		private IMediaManager _mediaManager;

		private IVoiceDetectionManager _voiceDetectionManager;

		private bool _IsCurrentInstance { get { return Instance == this; } }

		[Header("----------Prefab Object Settings----------")]
		public bool isDontDestroyOnLoad = false;

		[Space]
		[Header("----------Recognition Configs----------")]
		public int currentConfigIndex;
		public List<Config> configs;

		[Space]
		[Header("----------Plugin Settings----------")]
		public bool isFullDebugLogIfError = false;

		[PasswordField]
		public string apiKey;

		public AudioClip LastRecordedClip => _mediaManager.LastRecordedClip;

		public bool IsRecording => _mediaManager.IsRecording;

		public float[] LastRecordedRaw => _mediaManager.LastRecordedRaw;

		private void Awake()
		{
			if (_Instance != null)
			{
				Destroy(gameObject);
				return;
			}

			if (isDontDestroyOnLoad)
			{
				DontDestroyOnLoad(gameObject);
			}

			_Instance = this;

			if (configs == null || configs.Count == 0)
			{
				throw new MissingFieldException("NO CONFIG FOUND!");
			}

			ServiceLocator.Register<ISpeechRecognitionManager>(new SpeechRecognitionManager());
			ServiceLocator.Register<IVoiceDetectionManager>(new VoiceDetectionManager());
			ServiceLocator.Register<IMediaManager>(new MediaManager());
			ServiceLocator.InitServices();

			_mediaManager = ServiceLocator.Get<IMediaManager>();
			_speechRecognitionManager = ServiceLocator.Get<ISpeechRecognitionManager>();
			_voiceDetectionManager = ServiceLocator.Get<IVoiceDetectionManager>();

			_mediaManager.RecordStartedEvent += RecordStartedEventHandler;
			_mediaManager.RecordEndedEvent += RecordEndedEventHandler;
			_mediaManager.RecordFailedEvent += RecordFailedEventHandler;
			_mediaManager.TalkBeganEvent += TalkBeganEventHandler;
			_mediaManager.TalkEndedEvent += TalkEndedEventHandler;

			_speechRecognitionManager.RecognizeSuccessEvent += RecognizeSuccessEventHandler;
			_speechRecognitionManager.RecognizeFailedEvent += RecognizeFailedEventHandler;
			_speechRecognitionManager.LongRunningRecognizeSuccessEvent += LongRunningRecognizeSuccessEventHandler;
			_speechRecognitionManager.LongRunningRecognizeFailedEvent += LongRunningRecognizeFailedEventHandler;
			_speechRecognitionManager.GetOperationSuccessEvent += GetOperationSuccessEventHandler;
			_speechRecognitionManager.GetOperationFailedEvent += GetOperationFailedEventHandler;
			_speechRecognitionManager.ListOperationsSuccessEvent += ListOperationsSuccessEventHandler;
			_speechRecognitionManager.ListOperationsFailedEvent += ListOperationsFailedEventHandler;

			_speechRecognitionManager.SetConfig(configs[Mathf.Clamp(currentConfigIndex, 0, configs.Count - 1)]);
		}

		private void Update()
		{
			if (!_IsCurrentInstance)
				return;

			ServiceLocator.Instance.Update();
		}

		private void OnDestroy()
		{
			if (!_IsCurrentInstance)
				return;

			_mediaManager.RecordStartedEvent -= RecordStartedEventHandler;
			_mediaManager.RecordEndedEvent -= RecordEndedEventHandler;
			_mediaManager.RecordFailedEvent -= RecordFailedEventHandler;
			_mediaManager.TalkBeganEvent -= TalkBeganEventHandler;
			_mediaManager.TalkEndedEvent -= TalkEndedEventHandler;

			_speechRecognitionManager.RecognizeSuccessEvent -= RecognizeSuccessEventHandler;
			_speechRecognitionManager.RecognizeFailedEvent -= RecognizeFailedEventHandler;
			_speechRecognitionManager.LongRunningRecognizeSuccessEvent -= LongRunningRecognizeSuccessEventHandler;
			_speechRecognitionManager.LongRunningRecognizeFailedEvent -= LongRunningRecognizeFailedEventHandler;
			_speechRecognitionManager.GetOperationSuccessEvent -= GetOperationSuccessEventHandler;
			_speechRecognitionManager.GetOperationFailedEvent -= GetOperationFailedEventHandler;
			_speechRecognitionManager.ListOperationsSuccessEvent -= ListOperationsSuccessEventHandler;
			_speechRecognitionManager.ListOperationsFailedEvent -= ListOperationsFailedEventHandler;

			ServiceLocator.Instance.Dispose();

			_Instance = null;
		}

		public float GetLastFrame()
		{
			return _mediaManager.GetLastFrame();
		}

		public float GetMaxFrame()
		{
			return _mediaManager.GetMaxFrame();
		}

		public void StartRecord(bool withVoiceDetection)
		{
			if (!_mediaManager.HasMicrophonePermission())
				_mediaManager.RequestMicrophonePermission(null);

			_mediaManager.StartRecord(withVoiceDetection);
		}

		public void StopRecord()
		{
			_mediaManager.StopRecord();
		}

		public void DetectThreshold()
		{
			_voiceDetectionManager.DetectThreshold();
		}

		public bool ReadyToRecord()
		{
			return _mediaManager.ReadyToRecord();
		}

		public string[] GetMicrophoneDevices()
		{
			return _mediaManager.GetMicrophoneDevices();
		}

		public bool HasConnectedMicrophoneDevices()
		{
			return _mediaManager.HasConnectedMicrophoneDevices();
		}

		public void SetMicrophoneDevice(string device)
		{
			_mediaManager.SetMicrophoneDevice(device);
		}

		public void SaveLastRecordedAudioClip(string path)
		{
			_mediaManager.SaveLastRecordedAudioClip(path);
		}

		public long Recognize(GeneralRecognitionRequest recognitionRequest)
		{
			return _speechRecognitionManager.Recognize(recognitionRequest);
		}

		public long LongRunningRecognize(GeneralRecognitionRequest recognitionRequest)
		{
			return _speechRecognitionManager.LongRunningRecognize(recognitionRequest);
		}

		public long GetOperation(string operation)
		{
			return _speechRecognitionManager.GetOperation(operation);
		}

		public long GetListOperations(string name = null, string filter = null, int pageSize = -1, string pageToken = null)
		{
			return _speechRecognitionManager.GetListOperations(name, filter, pageSize, pageToken);
		}

		public bool CancelRequest(long id)
		{
			return _speechRecognitionManager.CancelRequest(id);
		}

		public int CancelAllRequests()
		{
			return _speechRecognitionManager.CancelAllRequests();
		}

		public bool HasMicrophonePermission()
		{
			return _mediaManager.HasMicrophonePermission();
		}

		/// <summary>
		/// Currently works as synchronous function with callback when app unpauses
		/// could not work properly if has enabled checkbox regarding additional frame in pause
		/// </summary>
		/// <param name="callback"></param>
		public void RequestMicrophonePermission(Action<bool> callback)
		{
			_mediaManager.RequestMicrophonePermission(callback);
		}

		#region handlers
		private void RecognizeSuccessEventHandler(RecognitionResponse response)
		{
			RecognizeSuccessEvent?.Invoke(response);
		}

		private void LongRunningRecognizeSuccessEventHandler(Operation operation)
		{
			LongRunningRecognizeSuccessEvent?.Invoke(operation);
		}

		private void RecognizeFailedEventHandler(string error)
		{
			RecognizeFailedEvent?.Invoke(error);
		}

		private void LongRunningRecognizeFailedEventHandler(string error)
		{
			LongRunningRecognizeFailedEvent?.Invoke(error);
		}

		private void RecordFailedEventHandler()
		{
			RecordFailedEvent?.Invoke();
		}

		private void TalkBeganEventHandler()
		{
			BeginTalkigEvent?.Invoke();
		}

		private void TalkEndedEventHandler(AudioClip clip, float[] raw)
		{
			EndTalkigEvent?.Invoke(clip, raw);
		}

		private void RecordStartedEventHandler()
		{
			StartedRecordEvent?.Invoke();
		}

		private void RecordEndedEventHandler(AudioClip clip, float[] raw)
		{
			FinishedRecordEvent?.Invoke(clip, raw);
		}

		private void GetOperationSuccessEventHandler(Operation operation)
		{
			GetOperationSuccessEvent?.Invoke(operation);
		}

		private void GetOperationFailedEventHandler(string error)
		{
			GetOperationFailedEvent?.Invoke(error);
		}

		private void ListOperationsSuccessEventHandler(ListOperationsResponse operationsResponse)
		{
			ListOperationsSuccessEvent?.Invoke(operationsResponse);
		}

		private void ListOperationsFailedEventHandler(string error)
		{
			ListOperationsFailedEvent?.Invoke(error);
		}

		#endregion
	}
}