using System;
using GreenT.Model.Collections;
using Newtonsoft.Json;

namespace StripClub.Messenger.Data;

[Serializable]
public class ConversationConfigMapper
{
	public class Manager : SimpleManager<ConversationConfigMapper>
	{
	}

	[JsonProperty]
	private int chat_id;

	[JsonProperty]
	private int[] character_id;

	[JsonProperty]
	private string custom_bundle;

	[JsonProperty]
	private int dialogues_count;

	[JsonProperty]
	private int total_media;

	[JsonProperty]
	private bool names_visibility;

	public int ID => chat_id;

	public int[] ParticipantIDCollection => character_id;

	public int DialoguesCount => dialogues_count;

	public int TotalMedia => total_media;

	public string CustomBundleName => custom_bundle;

	public bool NamesVisibility => names_visibility;
}
