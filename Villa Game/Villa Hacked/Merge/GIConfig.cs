using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Types;
using UnityEngine;

namespace Merge;

[Serializable]
public class GIConfig
{
	[SerializeField]
	private int uniqId;

	[SerializeField]
	private GIKey key;

	[SerializeField]
	private string gameItemName;

	[SerializeField]
	private ContentType contentType;

	[SerializeField]
	private string bundle;

	[SerializeField]
	private string gameItemDescription;

	[SerializeField]
	private List<ModuleConfigs.Base> modules = new List<ModuleConfigs.Base>();

	public GameItemType Type;

	public HowToGetType[] HowToGetTypes;

	public int UniqId => uniqId;

	public ContentType ContentType => contentType;

	public string Bundle => bundle;

	public GIKey Key => key;

	public string GameItemName => $"item.{key}";

	public string GameItemDescription => gameItemDescription;

	public bool NotAffectedAll { get; protected set; }

	public List<ModuleConfigs.Base> Modules => modules;

	public int Level => key.ID;

	public GIConfig()
	{
	}

	public GIConfig(int uniqId, GameItemType gameItemType, GIKey key, string gameItemName, string gameItemDescription, bool notAffectedAll, HowToGetType[] howToGetTypeAdditionalWay, ContentType contentType, string bundle, List<ModuleConfigs.Base> modules)
	{
		this.uniqId = uniqId;
		Type = gameItemType;
		this.key = key;
		this.contentType = contentType;
		this.bundle = bundle;
		this.gameItemName = gameItemName;
		this.gameItemDescription = gameItemDescription;
		NotAffectedAll = notAffectedAll;
		this.modules = modules;
		HowToGetTypes = howToGetTypeAdditionalWay;
	}

	public bool HasModule<T>() where T : ModuleConfigs.Base
	{
		return modules.Any((ModuleConfigs.Base x) => x is T);
	}

	public bool HasModule(GIModuleType type)
	{
		return modules.Any((ModuleConfigs.Base x) => x.ModuleType == type);
	}

	public T GetModule<T>() where T : ModuleConfigs.Base
	{
		return modules.FirstOrDefault((ModuleConfigs.Base x) => x is T) as T;
	}

	public ModuleConfigs.Base GetModule(GIModuleType type)
	{
		return modules.FirstOrDefault((ModuleConfigs.Base x) => x.ModuleType == type);
	}

	public bool TryGetModule<T>(out T result) where T : ModuleConfigs.Base
	{
		result = GetModule<T>();
		return result != null;
	}

	public bool TryGetModule(GIModuleType type, out ModuleConfigs.Base result)
	{
		result = GetModule(type);
		return result != null;
	}

	public bool EqualsOrEvent(string targetBundle)
	{
		if (Bundle.Equals(targetBundle))
		{
			return true;
		}
		if (targetBundle.Equals("Main") || Bundle.Equals("Main"))
		{
			return false;
		}
		return Bundle.Equals("Event");
	}
}
