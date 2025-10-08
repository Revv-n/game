using System;
using GreenT.Model.Collections;
using Newtonsoft.Json;

namespace StripClub.Messenger.Data;

[Serializable]
public class PlayerMessageConfigMapper : MessageConfigMapper
{
	[Serializable]
	public class ItemMapper
	{
		[JsonProperty]
		private string id;

		[JsonProperty]
		private int count;

		public string ID => id;

		public int Count => count;
	}

	public class Manager : SimpleManager<PlayerMessageConfigMapper>
	{
	}

	[JsonProperty]
	private ItemMapper[][] item_mappers;

	public ItemMapper[][] ItemMappers => item_mappers;
}
