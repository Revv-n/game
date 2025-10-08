using System.Collections.Generic;
using System.Linq;
using GreenT.Model.Collections;
using Merge;

namespace GreenT.HornyScapes.GameItems;

public class GameItemConfigManager : SimpleManager<GIConfig>
{
	private readonly Dictionary<string, List<GIConfig>> _collections = new Dictionary<string, List<GIConfig>>(1024);

	public GIConfig GetConfigOrNull(GIKey key)
	{
		GIConfig gIConfig = collection.FirstOrDefault((GIConfig entity) => entity.Key == key);
		if (key.Collection == null || key.ID == 0 || gIConfig == null)
		{
			return null;
		}
		return gIConfig;
	}

	public bool TryGetConfig(GIKey key, out GIConfig giConfig)
	{
		giConfig = collection.FirstOrDefault((GIConfig entity) => entity.Key == key);
		return giConfig != null;
	}

	public bool TryGetConfig(int uniqID, out GIConfig giConfig)
	{
		giConfig = collection.FirstOrDefault((GIConfig entity) => entity.UniqId == uniqID);
		return giConfig != null;
	}

	public List<GIConfig> GetCollection(string collectionId)
	{
		return GetCollections()[collectionId];
	}

	public Dictionary<string, List<GIConfig>> GetCollections()
	{
		if (_collections.Count != 0)
		{
			return _collections;
		}
		foreach (GIConfig item in collection)
		{
			string key = item.Key.Collection;
			if (_collections.ContainsKey(key))
			{
				_collections[key].Add(item);
				continue;
			}
			_collections.Add(key, new List<GIConfig>(16) { item });
		}
		return _collections;
	}
}
