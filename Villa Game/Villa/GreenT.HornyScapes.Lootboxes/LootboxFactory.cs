using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Lootboxes.Data;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Lootboxes;

public class LootboxFactory : IFactory<LootboxMapper, Lootbox>, IFactory
{
	private DropFactory dropFactory;

	private readonly LootBoxSettings lootBoxSettings;

	public LootboxFactory(DropFactory dropFactory, LootBoxSettings lootBoxSettings)
	{
		this.dropFactory = dropFactory;
		this.lootBoxSettings = lootBoxSettings;
	}

	public Lootbox Create(LootboxMapper mapper)
	{
		List<RandomDropSettings> list = new List<RandomDropSettings>();
		List<DropSettings> list2 = new List<DropSettings>();
		try
		{
			ValidateLootboxMapper(mapper);
			for (int i = 0; i != mapper.rew_type.Length; i++)
			{
				Selector selector = SelectorTools.CreateSelector(mapper.rew_id[i]);
				RandomDropSettings item = dropFactory.Create(mapper.rew_type[i], selector, mapper.rew_qty[i], mapper.rew_delta[i], mapper.type, mapper.rew_chance[i]);
				list.Add(item);
			}
			for (int j = 0; j != mapper.guaranteed_type.Length; j++)
			{
				Selector selector2 = SelectorTools.CreateSelector(mapper.guaranteed_id[j]);
				DropSettings item2 = dropFactory.Create(mapper.guaranteed_type[j], selector2, mapper.guaranteed_qty[j], mapper.guaranteed_delta[j], mapper.type);
				list2.Add(item2);
			}
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("Can't parse lootbox config with id:" + mapper.chest_id + "\n");
		}
		string name = lootBoxSettings.GetName(mapper.rarity);
		Sprite icon = lootBoxSettings.GetIcon(mapper.rarity);
		Sprite alternativeIcon = lootBoxSettings.GetAlternativeIcon(mapper.rarity);
		return new Lootbox(mapper, list, list2, name, icon, alternativeIcon);
	}

	private void ValidateLootboxMapper(LootboxMapper mapper)
	{
		string text = string.Empty;
		if (mapper.rew_type.Length != mapper.rew_id.Length)
		{
			text += "Array size of rew_type must be equal to array size of rew_id\n";
		}
		if (mapper.rew_type.Length != mapper.rew_qty.Length)
		{
			text += "Array size of rew_type must be equal to array size of rew_qty\n";
		}
		if (mapper.rew_type.Length != mapper.rew_delta.Length)
		{
			text += "Array size of rew_type must be equal to array size of rew_delta\n";
		}
		if (mapper.rew_type.Length != mapper.rew_chance.Length)
		{
			text += "Array size of rew_type must be equal to array size of rew_chance\n";
		}
		if (mapper.guaranteed_type.Length != mapper.guaranteed_id.Length)
		{
			text += "Array size of guaranteed_type must be equal to array size of guaranteed_id\n";
		}
		if (mapper.guaranteed_type.Length != mapper.guaranteed_qty.Length)
		{
			text += "Array size of guaranteed_type must be equal to array size of guaranteed_qty\n";
		}
		if (mapper.guaranteed_type.Length != mapper.guaranteed_delta.Length)
		{
			text += "Array size of guaranteed_type must be equal to array size of guaranteed_delta\n";
		}
		if (!string.IsNullOrEmpty(text))
		{
			throw new ArgumentException(text);
		}
	}
}
