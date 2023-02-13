using UnityEngine;
using System;
using FrostweepGames.Plugins.Core;
using FrostweepGames.Plugins.Networking;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
	public class SpeechRecognitionManager : IService, IDisposable, ISpeechRecognitionManager
	{
		public event Action<RecognitionResponse> RecognizeSuccessEvent;
		public event Action<string> RecognizeFailedEvent;
		public event Action<Operation> LongRunningRecognizeSuccessEvent;
		public event Action<string> LongRunningRecognizeFailedEvent;
		public event Action<Operation> GetOperationSuccessEvent;
		public event Action<string> GetOperationFailedEvent;
		public event Action<ListOperationsResponse> ListOperationsSuccessEvent;
		public event Action<string> ListOperationsFailedEvent;

		private GCSpeechRecognition _gcSpeechRecognition;

		private NetworkingService _networkingService;

		public Config CurrentConfig { get; private set; }

		public void Init()
		{
			_gcSpeechRecognition = GCSpeechRecognition.Instance;

			_networkingService = new NetworkingService();
			_networkingService.NetworkResponseEvent += NetworkResponseEventHandler;

			CurrentConfig = Resources.Load<Config>("GCSpeechRecognitonConfig");
		}

		public void Update()
		{
			_networkingService?.Update();
		}

		public void Dispose()
		{
			_networkingService.NetworkResponseEvent -= NetworkResponseEventHandler;
			_networkingService.Dispose();
		}

		public void SetConfig(Config config)
		{
			CurrentConfig = config;
		}

		public bool CancelRequest(long id)
		{
			return _networkingService.CancelRequest(id);
		}

		public int CancelAllRequests()
		{
			return _networkingService.CancelAllRequests();
		}

		public long Recognize(GeneralRecognitionRequest request)
		{
			if (CurrentConfig == null)
				throw new NotImplementedException("Config isn't seted! Use SetConfig method!");

			if (request == null)
				throw new NullReferenceException("Recognition request is null");

			string postData = JsonConvert.SerializeObject(request);

			return _networkingService.SendRequest(
				GetAPiRouteEnd(Constants.POST_RECOGNIZE_REQUEST_URL),
				postData,
				NetworkEnumerators.RequestType.POST,
				GetHeaders(),
				new object[]
				{
					Enumerators.ApiType.RECOGNIZE
				});
		}

		public long LongRunningRecognize(GeneralRecognitionRequest request)
		{
			if (CurrentConfig == null)
				throw new NotImplementedException("Config isn't seted! Use SetConfig method!");

			if (request == null)
				throw new NullReferenceException("Recognition request is null");

			string postData = JsonConvert.SerializeObject(request);

			return _networkingService.SendRequest(
				GetAPiRouteEnd(Constants.POST_LONG_RUNNING_RECOGNIZE_REQUEST_URL),
				postData,
				NetworkEnumerators.RequestType.POST,
				GetHeaders(),
				new object[]
				{
					Enumerators.ApiType.LONG_RUNNING_RECOGNIZE
				});
		}

		public long GetOperation(string operation)
		{
			if (string.IsNullOrEmpty(operation))
				throw new NullReferenceException("operation id is null or empty");

			return _networkingService.SendRequest(
				GetAPiRouteEnd(Constants.GET_OPERATION_REQUEST_URL.Replace("{name}", operation)),
				string.Empty,
				NetworkEnumerators.RequestType.GET,
				GetHeaders(),
				new object[]
				{
					Enumerators.ApiType.OPERATION
				});
		}

		public long GetListOperations(string name = null, string filter = null, int pageSize = -1, string pageToken = null)
		{
			string uri = GetAPiRouteEnd(Constants.GET_LIST_OPERATIONS_REQUEST_URL);

			if (!string.IsNullOrEmpty(name))
			{
				uri += $"&name={name}";
			}

			if (!string.IsNullOrEmpty(filter))
			{
				uri += $"&filter={filter}";
			}

			if (pageSize != -1)
			{
				uri += $"&pageSize={pageSize}";
			}

			if (!string.IsNullOrEmpty(pageToken))
			{
				uri += $"&pageToken={pageToken}";
			}

			return _networkingService.SendRequest(
				uri,
				string.Empty,
				NetworkEnumerators.RequestType.GET,
				GetHeaders(),
				new object[]
				{
					Enumerators.ApiType.LIST_OPERATIONS
				});
		}

		private string GetAPiRouteEnd(string apiRoute)
		{
			return Constants.ROOT_REQUEST_URL + Constants.API_VERSION + apiRoute + Constants.API_KEY_PARAM + _gcSpeechRecognition.apiKey;
		}

		private Dictionary<string, string> GetHeaders()
		{
			return new Dictionary<string, string>()
			{
#if UNITY_ANDROID
				{ "X-Android-Package", CurrentConfig.packageName },
				{ "X-Android-Cert", CurrentConfig.keySignature }
#endif
			};
		}

		private void NetworkResponseEventHandler(NetworkResponse response)
		{
			if (response.HasError() && GCSpeechRecognition.Instance.isFullDebugLogIfError)
			{
				Debug.LogError(response.GetFullLog());
			}

			if (response.Parameters.Length > 0)
			{
				Enumerators.ApiType apiType = (Enumerators.ApiType)response.Parameters[0];

				switch (apiType)
				{
					case Enumerators.ApiType.RECOGNIZE:
						{
							if (response.HasError())
							{
								RecognizeFailedEvent?.Invoke(response.GetFullLog());
							}
							else
							{
								RecognizeSuccessEvent?.Invoke(JsonConvert.DeserializeObject<RecognitionResponse>(response.Response));
							}
						}
						break;
					case Enumerators.ApiType.LONG_RUNNING_RECOGNIZE:
						{
							if (response.HasError())
							{
								LongRunningRecognizeFailedEvent?.Invoke(response.GetFullLog());
							}
							else
							{
								LongRunningRecognizeSuccessEvent?.Invoke(JsonConvert.DeserializeObject<Operation>(response.Response));
							}
						}
						break;
					case Enumerators.ApiType.LIST_OPERATIONS:
						{
							if (response.HasError())
							{
								ListOperationsFailedEvent?.Invoke(response.GetFullLog());
							}
							else
							{
								ListOperationsSuccessEvent?.Invoke(JsonConvert.DeserializeObject<ListOperationsResponse>(response.Response));
							}
						}
						break;
					case Enumerators.ApiType.OPERATION:
						{
							if (response.HasError())
							{
								GetOperationFailedEvent?.Invoke(response.GetFullLog());
							}
							else
							{
								GetOperationSuccessEvent?.Invoke(JsonConvert.DeserializeObject<Operation>(response.Response));
							}
						}
						break;
					default: break;
				}
			}
		}
	}
}