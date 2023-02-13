using System;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public static class Enumerators
    {
		public enum ApiType : int
		{
			UNDEFINED = 0,

			RECOGNIZE,
			LONG_RUNNING_RECOGNIZE,
			OPERATION,
			LIST_OPERATIONS,
			LOCATION_OPERATION,
			LOCATION_LIST_OPERATIONS
		}

		public enum AudioEncoding
        {
            ENCODING_UNSPECIFIED,

            LINEAR16,
            FLAC,
            MULAW,
            AMR,
            AMR_WB,
            OGG_OPUS,
            SPEEX_WITH_HEADER_BYTE,

			/// <summary>
			/// BETA FIELD
			/// </summary>
			MP3
		}

		public enum InteractionType
		{
			INTERACTION_TYPE_UNSPECIFIED,

			DISCUSSION,
			PRESENTATION,
			PHONE_CALL,
			VOICEMAIL,
			PROFESSIONALLY_PRODUCED,
			VOICE_SEARCH,
			VOICE_COMMAND,
			DICTATION
		}

		public enum MicrophoneDistance
		{
			MICROPHONE_DISTANCE_UNSPECIFIED,

			NEARFIELD,
			MIDFIELD,
			FARFIELD
		}

		public enum OriginalMediaType
		{
			ORIGINAL_MEDIA_TYPE_UNSPECIFIED,

			AUDIO,
			VIDEO
		}

		public enum RecordingDeviceType
		{
			RECORDING_DEVICE_TYPE_UNSPECIFIED,

			SMARTPHONE,
			PC,
			PHONE_LINE,
			VEHICLE,
			OTHER_OUTDOOR_DEVICE,
			OTHER_INDOOR_DEVICE
		}

		public enum LanguageCode
        {
			af_ZA,
			sq_AL,
			am_ET,
			ar_DZ,
			ar_BH,
			ar_EG,
			ar_IQ,
			ar_IL,
			ar_JO,
			ar_KW,
			ar_LB,
			ar_MR,
			ar_MA,
			ar_OM,
			ar_QA,
			ar_SA,
			ar_PS,
			ar_TN,
			ar_AE,
			ar_YE,
			hy_AM,
			az_AZ,
			eu_ES,
			bn_BD,
			bn_IN,
			bs_BA,
			bg_BG,
			my_MM,
			ca_ES,
			yue_Hant_HK,
			cmn_Hans_CN,
			zh_TW,
			hr_HR,
			cs_CZ,
			da_DK,
			nl_BE,
			nl_NL,
			en_AU,
			en_CA,
			en_GH,
			en_HK,
			en_IN,
			en_IE,
			en_KE,
			en_NZ,
			en_NG,
			en_PK,
			en_PH,
			en_SG,
			en_ZA,
			en_TZ,
			en_GB,
			en_US,
			et_EE,
			fil_PH,
			fi_FI,
			fr_BE,
			fr_CA,
			fr_FR,
			fr_CH,
			gl_ES,
			ka_GE,
			de_AT,
			de_DE,
			de_CH,
			el_GR,
			gu_IN,
			iw_IL,
			hi_IN,
			hu_HU,
			is_IS,
			id_ID,
			it_IT,
			it_CH,
			ja_JP,
			jv_ID,
			kn_IN,
			kk_KZ,
			km_KH,
			ko_KR,
			lo_LA,
			lv_LV,
			lt_LT,
			mk_MK,
			ms_MY,
			ml_IN,
			mr_IN,
			mn_MN,
			ne_NP,
			no_NO,
			fa_IR,
			pl_PL,
			pt_BR,
			pt_PT,
			pa_Guru_IN,
			ro_RO,
			ru_RU,
			sr_RS,
			si_LK,
			sk_SK,
			sl_SI,
			es_AR,
			es_BO,
			es_CL,
			es_CO,
			es_CR,
			es_DO,
			es_EC,
			es_SV,
			es_GT,
			es_HN,
			es_MX,
			es_NI,
			es_PA,
			es_PY,
			es_PE,
			es_PR,
			es_ES,
			es_US,
			es_UY,
			es_VE,
			su_ID,
			sw_KE,
			sw_TZ,
			sv_SE,
			ta_IN,
			ta_MY,
			ta_SG,
			ta_LK,
			te_IN,
			th_TH,
			tr_TR,
			uk_UA,
			ur_IN,
			ur_PK,
			uz_UZ,
			vi_VN,
			zu_ZA
		}

		public static string Parse(this Enum value, char symbolFrom = '_', char symbolTo = '-')
		{
			return value.ToString().Replace(symbolFrom, symbolTo);
		}
    }
}