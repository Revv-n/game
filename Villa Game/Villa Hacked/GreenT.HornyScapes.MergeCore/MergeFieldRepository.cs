using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Types;
using Merge;

namespace GreenT.HornyScapes.MergeCore;

public class MergeFieldRepository
{
	private readonly Dictionary<ContentType, HashSet<MergeField>> fields = new Dictionary<ContentType, HashSet<MergeField>>();

	public bool Contains(MergeField field)
	{
		if (field == null)
		{
			return false;
		}
		ContentType type = field.Data.Type;
		if (fields.ContainsKey(type))
		{
			return fields[type].Contains(field);
		}
		return false;
	}

	public bool ContainsOneOrMoreFields(ContentType type)
	{
		if (fields.ContainsKey(type))
		{
			return fields[type].Count > 0;
		}
		return false;
	}

	public bool TryGetMain(out MergeField mergeField)
	{
		mergeField = null;
		ContentType key = ContentType.Main;
		if (!fields.ContainsKey(key))
		{
			return false;
		}
		mergeField = fields[ContentType.Main].FirstOrDefault();
		return mergeField != null;
	}

	public bool TryAdd(MergeField mergeField)
	{
		ContentType type = mergeField.Data.Type;
		ConfigureDictionary(type);
		if (fields[type].Contains(mergeField))
		{
			return false;
		}
		fields[type].Add(mergeField);
		return true;
	}

	private void ConfigureDictionary(ContentType type)
	{
		if (!fields.ContainsKey(type))
		{
			fields.Add(type, new HashSet<MergeField>(1));
		}
	}

	public void SaveAllFields()
	{
		foreach (ContentType value in Enum.GetValues(typeof(ContentType)))
		{
			if (!fields.ContainsKey(value))
			{
				continue;
			}
			foreach (MergeField item in fields[value])
			{
				item.InvokeSaveAllItems();
			}
		}
	}

	public bool TryRemove(MergeField field)
	{
		if (!Contains(field))
		{
			return false;
		}
		ContentType type = field.Data.Type;
		fields[type].Remove(field);
		return true;
	}

	public bool TryGetFieldWithItem(GameItem findGameItem, out MergeField data)
	{
		data = null;
		foreach (ContentType value in Enum.GetValues(typeof(ContentType)))
		{
			if (fields.ContainsKey(value))
			{
				data = fields[value].FirstOrDefault((MergeField p) => p.Field.HashSetObjects.Contains(findGameItem));
				if (data != null)
				{
					break;
				}
			}
		}
		_ = data;
		return data != null;
	}

	public void Purge()
	{
		foreach (ContentType value in Enum.GetValues(typeof(ContentType)))
		{
			if (fields.ContainsKey(value))
			{
				fields[value].Clear();
			}
		}
		fields.Clear();
	}

	public void Preload(FieldMonoMediatorCase fieldMediators)
	{
		foreach (ContentType value in Enum.GetValues(typeof(ContentType)))
		{
			if (!fields.ContainsKey(value))
			{
				continue;
			}
			foreach (MergeField item in fields[value])
			{
				item.SetTransform(fieldMediators.Get(item.Type));
				item.ResetField();
			}
		}
	}
}
