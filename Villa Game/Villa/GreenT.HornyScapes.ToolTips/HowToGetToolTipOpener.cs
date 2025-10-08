using System;
using GreenT.HornyScapes.MergeCore;
using GreenT.HornyScapes.MergeCore.Collection;
using Merge;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace GreenT.HornyScapes.ToolTips;

public class HowToGetToolTipOpener : ToolTipUIOpener<HowToGetToolTipSettings, HowToGetToolTipView>
{
	[Serializable]
	public class DictionaryHowToGetType : SerializableDictionary<HowToGetType, Sprite>
	{
	}

	[SerializeField]
	private CollectionWindow collectionWindow;

	[SerializeField]
	private DictionaryHowToGetType additionalSprites = new DictionaryHowToGetType();

	private InfoGetItem infoGetItem;

	[Inject]
	private void Init(InfoGetItem infoGetItem)
	{
		this.infoGetItem = infoGetItem;
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		if (!infoGetItem.HowToGetDict.TryGetValue(collectionWindow.Source.Key, out var value) && collectionWindow.Source.HowToGetTypes[0] == HowToGetType.None)
		{
			return;
		}
		base.Settings.HowToGet = value;
		base.Settings.AdditionalWays.Clear();
		Sprite value2 = null;
		HowToGetType[] howToGetTypes = collectionWindow.Source.HowToGetTypes;
		foreach (HowToGetType howToGetType in howToGetTypes)
		{
			if (howToGetType != 0 && additionalSprites.TryGetValue(howToGetType, out value2))
			{
				base.Settings.AdditionalWays.Add(value2);
			}
		}
		base.OnPointerClick(eventData);
	}
}
