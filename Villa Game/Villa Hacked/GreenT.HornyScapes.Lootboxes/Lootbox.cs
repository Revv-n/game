using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Lootboxes.Data;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Cards;
using UnityEngine;

namespace GreenT.HornyScapes.Lootboxes;

public class Lootbox
{
	private LinkedContent _savedContent;

	private LinkedContent _preopenedContent;

	public int ID { get; }

	public Rarity Rarity { get; }

	public int DropCount { get; }

	public string Name { get; }

	public Sprite Icon { get; }

	public Sprite AlternativeIcon { get; }

	public ContentType ContentType { get; }

	public List<RandomDropSettings> DropOptions { get; }

	public List<DropSettings> GuarantedDrop { get; }

	public IEnumerable<int> CharacterIdPossibleDrops { get; private set; }

	public IEnumerable<int> SkinIdPossibleDrops { get; private set; }

	public CurrencyAmplitudeAnalytic.SourceType SourceType { get; private set; }

	public bool HasPreopenedContent => _preopenedContent != null;

	public Lootbox(LootboxMapper mapper, IEnumerable<RandomDropSettings> dropOptions, IEnumerable<DropSettings> guarantedOption, string name, Sprite icon, Sprite alternativeIcon)
		: this(mapper.chest_id, mapper.rarity, mapper.drop_count.GetValueOrDefault(), mapper.type, dropOptions, guarantedOption, name, icon, alternativeIcon)
	{
	}

	public Lootbox(int id, Rarity rarity, int count, ContentType contentType, IEnumerable<RandomDropSettings> dropOptions, IEnumerable<DropSettings> guarantedOption, string name, Sprite icon, Sprite alternativeIcon)
	{
		ID = id;
		Name = name;
		Icon = icon;
		AlternativeIcon = alternativeIcon;
		Rarity = rarity;
		DropCount = count;
		ContentType = contentType;
		DropOptions = new List<RandomDropSettings>(dropOptions);
		GuarantedDrop = new List<DropSettings>(guarantedOption);
		CharacterIdPossibleDrops = FillPossibleDrops(dropOptions, guarantedOption, RewType.Characters);
		SkinIdPossibleDrops = FillPossibleDrops(dropOptions, guarantedOption, RewType.Skin);
	}

	public Lootbox Clone(LinkedContent savedContent)
	{
		return new Lootbox(-1, Rarity, DropCount, ContentType, new List<RandomDropSettings>(), new List<DropSettings>(), Name, Icon, AlternativeIcon)
		{
			_savedContent = savedContent
		};
	}

	private IEnumerable<int> FillPossibleDrops(IEnumerable<RandomDropSettings> dropOptions, IEnumerable<DropSettings> guarantedOption, RewType byRule)
	{
		List<int> list = new List<int>();
		AddAllIdByRule(dropOptions, list, byRule);
		AddAllIdByRule(guarantedOption, list, byRule);
		return list;
	}

	private static void AddAllIdByRule(IEnumerable<DropSettings> dropOptions, List<int> temp, RewType rewTypeRule)
	{
		foreach (DropSettings dropOption in dropOptions)
		{
			if (dropOption.Type == rewTypeRule && dropOption.Selector is SelectorByID selectorByID)
			{
				temp.Add(selectorByID.ID);
			}
		}
	}

	public LinkedContent GetPreopenedContent()
	{
		if (_preopenedContent == null)
		{
			return Open();
		}
		LinkedContent preopenedContent = _preopenedContent;
		_preopenedContent = null;
		return preopenedContent;
	}

	public LinkedContent PrepareContent()
	{
		if (_preopenedContent != null)
		{
			return _preopenedContent;
		}
		_preopenedContent = Open();
		for (LootboxLinkedContent next = _preopenedContent.GetNext<LootboxLinkedContent>(checkThis: true); next != null; next = next.GetNext<LootboxLinkedContent>())
		{
			next.Lootbox.PrepareContent();
		}
		return _preopenedContent;
	}

	internal LinkedContent Open()
	{
		LinkedContent guarantedDropPackage;
		try
		{
			guarantedDropPackage = GetGuarantedDropPackage();
			guarantedDropPackage = GetRandomDropPackage(guarantedDropPackage);
			guarantedDropPackage = AddSavedContent(guarantedDropPackage);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException($"Error when try to open lootbox with ID: {ID}");
		}
		if (guarantedDropPackage == null)
		{
			throw new Exception().SendException($"Error when try to open lootbox with ID: {ID}, empty rewards ");
		}
		return guarantedDropPackage;
	}

	private LinkedContent AddSavedContent(LinkedContent reward)
	{
		if (_savedContent == null)
		{
			return reward;
		}
		if (reward == null)
		{
			reward = _savedContent;
		}
		else
		{
			reward.Insert(_savedContent);
		}
		return reward;
	}

	public void SetSource(CurrencyAmplitudeAnalytic.SourceType sourceType)
	{
		SourceType = sourceType;
		if (_preopenedContent != null)
		{
			_preopenedContent.AnalyticData.SourceType = sourceType;
		}
	}

	private LinkedContent GetRandomDropPackage(LinkedContent reward)
	{
		RandomDropSettings[] array = DropOptions.Where((RandomDropSettings _drop) => _drop.IsAvailable()).ToArray();
		int totalWeight = array.Sum((RandomDropSettings _option) => _option.Weight);
		if (array.Length == 0)
		{
			return reward;
		}
		int num = DropCount;
		if (reward == null && num > 0)
		{
			while (reward == null)
			{
				reward = GetRandomDrop(totalWeight, array);
				if (reward != null)
				{
					num--;
				}
			}
		}
		for (int i = 0; i != num; i++)
		{
			LinkedContent randomDrop = GetRandomDrop(totalWeight, array);
			reward.Insert(randomDrop);
		}
		return reward;
	}

	private LinkedContent GetGuarantedDropPackage()
	{
		LinkedContent linkedContent = null;
		foreach (DropSettings item in GuarantedDrop.Where((DropSettings _drop) => _drop.IsAvailable()))
		{
			LinkedContent drop = item.GetDrop(SourceType);
			if (linkedContent == null)
			{
				linkedContent = drop;
			}
			else
			{
				linkedContent.Insert(drop);
			}
		}
		return linkedContent;
	}

	private LinkedContent GetRandomDrop(int totalWeight, RandomDropSettings[] available)
	{
		int num = UnityEngine.Random.Range(0, totalWeight);
		int num2 = 0;
		for (int i = 0; i < available.Length; i++)
		{
			num2 += available[i].Weight;
			if (num2 > num)
			{
				this.SendLogCollection($"id = {i}, random = {num}, weights = ", available.Select((RandomDropSettings _temp) => _temp.Weight), LogType.Lootbox);
				return available[i].GetDrop(SourceType);
			}
		}
		throw new Exception().LogException();
	}
}
