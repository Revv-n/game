using System;
using Newtonsoft.Json;

namespace StripClub.Messenger.Data;

[Serializable]
public class MessageConfigMapper
{
	[JsonProperty]
	private int dialogue_id;

	[JsonProperty]
	private int serial_number;

	public int DialogueID => dialogue_id;

	public int Number => serial_number;
}
