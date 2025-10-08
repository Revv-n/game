using System;
using System.Collections.Generic;
using System.Linq;
using StripClub.Model.Cards;
using UnityEngine;

namespace GreenT.HornyScapes.Lootboxes;

[CreateAssetMenu(menuName = "GreenT/LootBox", fileName = "LootBoxSettings", order = 0)]
public class LootBoxSettings : ScriptableObject
{
	[Serializable]
	private class SubCase
	{
		[HideInInspector]
		public string Name;

		public Rarity Rarity;

		public Sprite Image;

		public Sprite AlternativeImage;

		public string GlobalName;

		public void UpdateName()
		{
			Name = Rarity.ToString();
		}
	}

	[SerializeField]
	private List<SubCase> settings;

	public Sprite GetIcon(Rarity rarity)
	{
		return settings.FirstOrDefault((SubCase p) => p.Rarity == rarity)?.Image;
	}

	public Sprite GetAlternativeIcon(Rarity rarity)
	{
		return settings.FirstOrDefault((SubCase p) => p.Rarity == rarity)?.AlternativeImage;
	}

	public string GetName(Rarity rarity)
	{
		return settings.FirstOrDefault((SubCase p) => p.Rarity == rarity)?.GlobalName;
	}

	private void OnValidate()
	{
		settings = settings.OrderBy((SubCase p) => p.Rarity).ToList();
		foreach (SubCase setting in settings)
		{
			setting.UpdateName();
		}
	}
}
