using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

using FrostweepGames.Plugins.Core;
using FrostweepGames.Plugins.GoogleCloud.TextToSpeech;
using OpenAI;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition.Examples
{
	public class GCSR_DoCommandsExample : MonoBehaviour
	{

		private OpenAIApi openai = new OpenAIApi();
		private string userInput;
		private string Instruction = "Always answer in English, with long responses.\nQ: ";

		private GCSpeechRecognition _speechRecognition;

		private Image _speechRecognitionState;

		private Button _startRecordButton,
					   _stopRecordButton;

		private InputField _commandsInputField;

		private Text _resultText;

		private RectTransform _objectForCommand;

		//TextToSpeech
		private GCTextToSpeech _gcTextToSpeech;

		private void Start()
		{


			_speechRecognition = GCSpeechRecognition.Instance;
			_speechRecognition.RecognizeSuccessEvent += RecognizeSuccessEventHandler;
			_speechRecognition.RecognizeFailedEvent += RecognizeFailedEventHandler;
			_speechRecognition.FinishedRecordEvent += FinishedRecordEventHandler;
			_speechRecognition.StartedRecordEvent += StartedRecordEventHandler;
			_speechRecognition.RecordFailedEvent += RecordFailedEventHandler;
			_speechRecognition.EndTalkigEvent += EndTalkigEventHandler;

			_startRecordButton = transform.Find("Canvas/Button_StartRecord").GetComponent<Button>();
			_stopRecordButton = transform.Find("Canvas/Button_StopRecord").GetComponent<Button>();
			_speechRecognitionState = transform.Find("Canvas/Image_RecordState").GetComponent<Image>();
			_resultText = transform.Find("Canvas/Text_Result").GetComponent<Text>();
	
			// select first microphone device
			if (_speechRecognition.HasConnectedMicrophoneDevices())
			{
				_speechRecognition.SetMicrophoneDevice(_speechRecognition.GetMicrophoneDevices()[0]);
			}
			// start recording automatically
			_speechRecognition.StartRecord(withVoiceDetection: true);

			_startRecordButton.onClick.AddListener(StartRecordButtonOnClickHandler);
			_stopRecordButton.onClick.AddListener(StopRecordButtonOnClickHandler);

			_startRecordButton.interactable = true;
			_stopRecordButton.interactable = false;
			_speechRecognitionState.color = Color.yellow;

			_speechRecognition.RequestMicrophonePermission(null);

			//TextToSpeech

		}

		private void OnDestroy()
		{
			_speechRecognition.RecognizeSuccessEvent -= RecognizeSuccessEventHandler;
			_speechRecognition.RecognizeFailedEvent -= RecognizeFailedEventHandler;

			_speechRecognition.FinishedRecordEvent -= FinishedRecordEventHandler;
			_speechRecognition.StartedRecordEvent -= StartedRecordEventHandler;
			_speechRecognition.RecordFailedEvent -= RecordFailedEventHandler;

			_speechRecognition.EndTalkigEvent -= EndTalkigEventHandler;
		}

		private void StartRecordButtonOnClickHandler()
		{
			_startRecordButton.interactable = false;
			_stopRecordButton.interactable = true;
			_resultText.text = string.Empty;

			_speechRecognition.StartRecord(false);
		}

		private void StopRecordButtonOnClickHandler()
		{
			_stopRecordButton.interactable = false;
			_startRecordButton.interactable = true;

			_speechRecognition.StopRecord();
		}

		private void StartedRecordEventHandler()
		{
			_speechRecognitionState.color = Color.red;
		}

		private void RecordFailedEventHandler()
		{
			_speechRecognitionState.color = Color.yellow;
			_resultText.text = "<color=red>Start record Failed. Please check microphone device and try again.</color>";

			_stopRecordButton.interactable = false;
			_startRecordButton.interactable = true;
		}

		private void EndTalkigEventHandler(AudioClip clip, float[] raw)
		{
			FinishedRecordEventHandler(clip, raw);
		}

		private void FinishedRecordEventHandler(AudioClip clip, float[] raw)
		{
			if (_startRecordButton.interactable)
			{
				_speechRecognitionState.color = Color.yellow;
			}

			if (clip == null)
				return;

			RecognitionConfig config = RecognitionConfig.GetDefault();
			//config.languageCode = Enumerators.LanguageCode.en_US.Parse();
			config.languageCode = Enumerators.LanguageCode.pt_BR.Parse();
			//config.languageCode = Enumerators.LanguageCode.es_ES.Parse();

			config.audioChannelCount = clip.channels;
			// configure other parameters of the config if need

			GeneralRecognitionRequest recognitionRequest = new GeneralRecognitionRequest()
			{
				audio = new RecognitionAudioContent()
				{
					content = raw.ToBase64()
				},
				//audio = new RecognitionAudioUri() // for Google Cloud Storage object
				//{
				//    uri = "gs://bucketName/object_name"
				//},
				config = config
			};
			_speechRecognition.Recognize(recognitionRequest);
		}


		private void RecognizeFailedEventHandler(string error)
		{
			_resultText.text = "Recognize Failed: " + error;
		}



		private async void RecognizeSuccessEventHandler(RecognitionResponse recognitionResponse)
		{
			string resultText = "Detected: ";

			foreach (var result in recognitionResponse.results)
			{
				foreach (var alternative in result.alternatives)
				{
					resultText += alternative.transcript;
				}
			}

			//_resultText.text = resultText;
			Debug.Log(resultText);

			Instruction += $"{resultText}\nA: ";

			var completionResponse = await openai.CreateCompletion(new CreateCompletionRequest()
			{
				Prompt = Instruction,
				Model = "text-davinci-003",
				MaxTokens = 4097 - Instruction.Length - 1
			});

			var resposta = completionResponse.Choices[0].Text;
			Instruction += $"{resposta}\nQ: ";

			Debug.Log("Resposta gpt3: " + resposta);
			_resultText.text = resposta;
		}



	}
}