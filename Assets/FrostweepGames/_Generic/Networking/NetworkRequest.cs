using UnityEngine.Networking;
using System.Text;
using System.Collections.Generic;

namespace FrostweepGames.Plugins.Networking
{
    public class NetworkRequest
    {
        public long RequestId { get; private set; }
        public NetworkEnumerators.RequestType RequestType { get; private set; }
        public object[] Parameters { get; private set; }

        public NetworkMethod Request { get; private set; }

        public NetworkRequest(string uri, string data, long index, NetworkEnumerators.RequestType type, Dictionary<string, string> headers = null, object[] param = null)
        {
            RequestType = type;
            RequestId = index;
            Parameters = param;

            Request = new NetworkMethod(uri, data, headers, RequestType, NetworkConstants.NetworkMethod);
        }

        public void Send()
        {
            Request.Send();
        }
    }


    public class NetworkMethod
    {
        private string _uri,
                       _data;

        private Dictionary<string, string> _headers;

        private NetworkEnumerators.RequestType _requestType;

        private UnityWebRequest _webRequest;

        private NetworkEnumerators.NetworkMethod _method;

        public bool isDone
        {
            get
            {
                switch(_method)
                {
                    case NetworkEnumerators.NetworkMethod.WEB_REQUEST:
                        return _webRequest.isDone;
                    default: break;
                }

                return false;
            }
        }

        public string text
        {
            get
            {
                switch (_method)
                {
                    case NetworkEnumerators.NetworkMethod.WEB_REQUEST:
                        return _webRequest.downloadHandler.text;
                    default: break;
                }

                return string.Empty;
            }
        }

        public string error
        {
            get
            {
                switch (_method)
                {
                    case NetworkEnumerators.NetworkMethod.WEB_REQUEST:
                        return _webRequest.error;
                    default: break;
                }

                return string.Empty;
            }
        }

		public long responseCode
		{
			get
			{
				switch (_method)
				{
					case NetworkEnumerators.NetworkMethod.WEB_REQUEST:
						return _webRequest.responseCode;
					default: break;
				}

				return 0;
			}
		}

		public NetworkMethod(string uri, string data, Dictionary<string, string> headers, NetworkEnumerators.RequestType type, NetworkEnumerators.NetworkMethod method)
        {
            _uri = uri;
            _data = data;
            _headers = headers;
            _requestType = type;
            _method = method;

            switch (method)
            {
                case NetworkEnumerators.NetworkMethod.WEB_REQUEST:
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(_data);
                        
                        switch(_requestType)
                        {
                            case NetworkEnumerators.RequestType.GET:
                                _webRequest = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbGET);
                                break;
                            case NetworkEnumerators.RequestType.DELETE:
                                _webRequest = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbDELETE);
                                break;
                            case NetworkEnumerators.RequestType.POST:
                                _webRequest = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPOST);
                                break;
                            case NetworkEnumerators.RequestType.CREATE:
                                _webRequest = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbCREATE);
                                break;
                            case NetworkEnumerators.RequestType.PATCH:
                                _webRequest = new UnityWebRequest(uri, "PATCH");
                                break;
                            case NetworkEnumerators.RequestType.PUT:
                                _webRequest = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPUT);
                                break;
                        }

                        if (!string.IsNullOrEmpty(data))
                            _webRequest.uploadHandler = new UploadHandlerRaw(bytes);

                        _webRequest.downloadHandler = new DownloadHandlerBuffer();
                        _webRequest.SetRequestHeader("Content-Type", "application/json");

                        if (_headers != null)
                        {
                            foreach(var header in _headers)
                            {
                                _webRequest.SetRequestHeader(header.Key, header.Value);
                            }
						}
					}
					break;
                default: break;
            }
        }

        public void Send()
        {
            switch (_method)
            {
                case NetworkEnumerators.NetworkMethod.WEB_REQUEST:
                    _webRequest.SendWebRequest();
                    break;
                default: break;
            }
        }

        public void Cancel()
        {
            switch (_method)
            {
                case NetworkEnumerators.NetworkMethod.WEB_REQUEST:
                    _webRequest.Abort();
                    Dispose();
                    break;
                default: break;
            }
        }
		
		public void Dispose()
        {
			if(_webRequest != null)
			{
				_webRequest.Dispose();
				_webRequest = null;
			}
        }
    }
}