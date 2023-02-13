using System;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
	[Serializable]
    public class RecognitionAudio
    {
    }

	[Serializable]
	public class RecognitionAudioContent : RecognitionAudio
	{
		public string content = string.Empty;
	}

	[Serializable]
	public class RecognitionAudioUri : RecognitionAudio
	{
		public string uri = string.Empty;
	}

	[Serializable]
    public class RecognitionConfig
    {
        public Enumerators.AudioEncoding encoding; 
        public double sampleRateHertz;
		public int audioChannelCount;
		public bool enableSeparateRecognitionPerChannel;
		public string languageCode;
		public string[] alternativeLanguageCodes = new string[0];
		public double maxAlternatives; 
        public bool profanityFilter;
		public SpeechAdaptation adaptation;
		public SpeechContext[] speechContexts = new SpeechContext[0];
		public bool enableWordTimeOffsets;
		public bool enableWordConfidence;
		public bool enableAutomaticPunctuation;
		public bool enableSpokenPunctuation;
		public bool enableSpokenEmojis;
		public SpeakerDiarizationConfig diarizationConfig;
		public RecognitionMetadata metadata;
		public string model;
		public bool useEnhanced;

		public static RecognitionConfig GetDefault()
		{
			return new RecognitionConfig()
			{
				encoding = Enumerators.AudioEncoding.LINEAR16,
				sampleRateHertz = 16000,
				audioChannelCount = 2,
				enableSeparateRecognitionPerChannel = false,
				maxAlternatives = 10,
				profanityFilter = false,
				enableWordTimeOffsets = true,
				enableAutomaticPunctuation = true,
				metadata = new RecognitionMetadata()
				{
					audioTopic = string.Empty,
					originalMediaType = Enumerators.OriginalMediaType.ORIGINAL_MEDIA_TYPE_UNSPECIFIED,
					industryNaicsCodeOfAudio = 0,
					interactionType = Enumerators.InteractionType.INTERACTION_TYPE_UNSPECIFIED,
					microphoneDistance = Enumerators.MicrophoneDistance.MICROPHONE_DISTANCE_UNSPECIFIED,
					originalMimeType = string.Empty,
					recordingDeviceName = string.Empty,
					recordingDeviceType = Enumerators.RecordingDeviceType.RECORDING_DEVICE_TYPE_UNSPECIFIED
				},
				model = "default",
				useEnhanced = true,
				languageCode = Enumerators.LanguageCode.en_GB.Parse(),
				diarizationConfig = new SpeakerDiarizationConfig()
				{
					enableSpeakerDiarization = false,
					maxSpeakerCount = 6,
					minSpeakerCount = 2
				},
				adaptation = new SpeechAdaptation()
                {
					customClasses = new CustomClass[0],
					phraseSetReferences = new string[0],
					phraseSets = new PhraseSet[0]
                }
			};
		}
	}

	[Serializable]
	public class SpeakerDiarizationConfig
	{
		public bool enableSpeakerDiarization;
		public double minSpeakerCount;
		public double maxSpeakerCount;
		// public int speakerTag; // deprecated
	}

	/// <summary>
	///  Deprecated
	/// </summary>
	[Serializable]
	public class RecognitionMetadata
	{
		public Enumerators.InteractionType interactionType;
		public uint industryNaicsCodeOfAudio;
		public Enumerators.MicrophoneDistance microphoneDistance;
		public Enumerators.OriginalMediaType originalMediaType;
		public Enumerators.RecordingDeviceType recordingDeviceType;
		public string recordingDeviceName;
		public string originalMimeType;
		//public string obfuscatedId; // deprecated
		public string audioTopic;
	}
	   
	[Serializable]
    public class SpeechContext
    {
		public string[] phrases = new string[0];
		public double boost;	
	}

	[Serializable]
	public class WordInfo
	{
		public string startTime;
		public string endTime;
		public string word;
		public double confidence;
		public int speakerTag;
	}

	[Serializable]
	public class SpeechAdaptation
	{
		public PhraseSet[] phraseSets;
		public string[] phraseSetReferences;
		public CustomClass[] customClasses;
	}

	[Serializable]
	public class PhraseSet
	{
		public string name;
		public Phrase[] phrases;
		public double boost;
	}

	[Serializable]
	public class Phrase
	{
		public string value;
		public double boost;
	}

	[Serializable]
	public class CustomClass
	{
		public string name;
		public string customClassId;
		public ClassItem[] items;
	}

	[Serializable]
	public class ClassItem
	{
		public string value;
	}

	[Serializable]
	public class Operation
	{
		public string name;
		public LongRunningRecognizeMetadata metadata;
		public bool done;
		public Status error;
		public RecognitionResponse response;
	}

	[Serializable]
	public class SpeechRecognitionAlternative
	{
		public string transcript;
		public double confidence;
		public WordInfo[] words;
	}

	[Serializable]
	public class Status
	{
		public double code;
		public string message;
		public object[] details;
	}

	[Serializable]
	public class LongRunningRecognizeMetadata
	{
		public double progressPercent;
		public string startTime;  // Timestamp format
		public string lastUpdateTime;  // Timestamp format
	}

	[Serializable]
	public class SpeechRecognitionResult
	{
		public SpeechRecognitionAlternative[] alternatives = new SpeechRecognitionAlternative[0];
		public double channelTag;
		public string resultEndTime;
		public string languageCode;
	}

	[Serializable]
	public class RecognitionResponse
	{
		public SpeechRecognitionResult[] results = new SpeechRecognitionResult[0];
		public string totalBilledTime;
	}

	[Serializable]
	public class ListOperationsResponse
	{
		public Operation[] operations;
		public string nextPageToken;
	}

	[Serializable]
    public class GeneralRecognitionRequest
    {
        public RecognitionConfig config = new RecognitionConfig();
        public RecognitionAudio audio = new RecognitionAudio();
    }
}