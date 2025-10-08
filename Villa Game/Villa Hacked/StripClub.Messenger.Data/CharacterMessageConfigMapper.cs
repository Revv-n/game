using System;
using GreenT.Model.Collections;
using Newtonsoft.Json;
using UnityEngine;

namespace StripClub.Messenger.Data;

[Serializable]
public class CharacterMessageConfigMapper : MessageConfigMapper
{
	public class Manager : SimpleManager<CharacterMessageConfigMapper>
	{
	}

	[SerializeField]
	[JsonProperty]
	private int character_id;

	[SerializeField]
	[JsonProperty]
	private int? media_id;

	public int CharacterID => character_id;

	public int? MediaID => media_id;
}
