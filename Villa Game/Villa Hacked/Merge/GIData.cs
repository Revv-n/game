using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Merge;

[Serializable]
public class GIData : IHasCoordinates
{
	[SerializeField]
	private GIKey key;

	[SerializeField]
	private Point coordinates;

	[SerializeField]
	private List<ModuleDatas.Base> modules = new List<ModuleDatas.Base>();

	public GIKey Key
	{
		get
		{
			return key;
		}
		set
		{
			key = value;
		}
	}

	public List<ModuleDatas.Base> Modules
	{
		get
		{
			return modules;
		}
		set
		{
			modules = value;
		}
	}

	public bool IsOverrided => Modules.Count > 0;

	public Point Coordinates
	{
		get
		{
			return coordinates;
		}
		set
		{
			coordinates = value;
		}
	}

	public bool Empty
	{
		get
		{
			if (key.Collection != null && !(key.Collection == ""))
			{
				return key.ID == 0;
			}
			return true;
		}
	}

	public GIData()
	{
	}

	public GIData(GIKey key)
		: this()
	{
		this.key = key;
	}

	public GIData(GIKey key, Point coordinates)
		: this(key)
	{
		this.coordinates = coordinates;
	}

	public GIData Copy()
	{
		GIData gIData = new GIData(key);
		gIData.coordinates = coordinates;
		for (int i = 0; i < modules.Count; i++)
		{
			gIData.modules.Add(modules[i].Copy());
		}
		return gIData;
	}

	public GIData SetCoordinates(Point value)
	{
		coordinates = value;
		return this;
	}

	public static GIData GetEmpty()
	{
		return new GIData();
	}

	public bool HasModule<T>() where T : ModuleDatas.Base
	{
		return modules.Any((ModuleDatas.Base x) => x is T);
	}

	public bool HasModule(GIModuleType type)
	{
		return modules.Any((ModuleDatas.Base x) => x.ModuleType == type);
	}

	public T GetModule<T>() where T : ModuleDatas.Base
	{
		return modules.FirstOrDefault((ModuleDatas.Base x) => x is T) as T;
	}

	public ModuleDatas.Base GetModule(GIModuleType type)
	{
		return modules.FirstOrDefault((ModuleDatas.Base x) => x.ModuleType == type);
	}

	public bool TryGetModule<T>(out T result) where T : ModuleDatas.Base
	{
		result = GetModule<T>();
		return result != null;
	}

	public bool TryGetModule(GIModuleType type, out ModuleDatas.Base result)
	{
		result = GetModule(type);
		return result != null;
	}
}
