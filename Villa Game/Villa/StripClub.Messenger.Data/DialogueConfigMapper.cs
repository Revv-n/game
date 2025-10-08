using System;
using GreenT.HornyScapes.Data;
using GreenT.Model.Collections;
using Newtonsoft.Json;
using StripClub.Model;

namespace StripClub.Messenger.Data;

[Serializable]
[Mapper]
public class DialogueConfigMapper
{
	public class Manager : SimpleManager<DialogueConfigMapper>
	{
	}

	[JsonProperty]
	private int id;

	[JsonProperty]
	private int chat_id;

	[JsonProperty]
	private int total_messages;

	[JsonProperty]
	private int total_player_messages;

	[JsonProperty]
	private int total_media;

	[JsonProperty]
	private UnlockType[] unlock_type;

	[JsonProperty]
	private string[] unlock_value;

	public int ID => id;

	public int ConversationID => chat_id;

	public int TotalMessages => total_messages;

	public int TotalPlayerMessages => total_player_messages;

	public int TotalMedia => total_media;

	public UnlockType[] UnlockTypes => unlock_type;

	public string[] UnlockValues => unlock_value;
}
