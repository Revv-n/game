using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Booster;
using GreenT.HornyScapes.Characters.Skins.Content;
using GreenT.HornyScapes.Characters.Skins.UI;
using GreenT.HornyScapes.Characters.Skins.UI.Lootbox;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Meta.Decorations;
using GreenT.HornyScapes.Presents.Models;
using GreenT.HornyScapes.UI;
using StripClub.Model;
using StripClub.Rewards.UI;
using StripClub.UI.Rewards;
using UnityEngine;

namespace StripClub.UI;

public class RewardLineView : MonoBehaviour
{
	public int MaxInLine = 5;

	public RectTransform Root;

	[SerializeField]
	private CardViewPool cardViewPool;

	[SerializeField]
	private ResourceViewPool resourceViewPool;

	[SerializeField]
	private MergeCardViewPool mergeCardViewPool;

	[SerializeField]
	private SkinViewPool skinViewPool;

	[SerializeField]
	private DecorationViewPool decorationViewPool;

	[SerializeField]
	private BattlePassLevelViewPool battlePassLevelViewPool;

	[SerializeField]
	private BoosterViewPool _boosterViewPool;

	private readonly List<MonoBehaviour> viewList = new List<MonoBehaviour>();

	public bool IsFree => MaxInLine > viewList.Count;

	public MonoBehaviour DisplayContent<T>(T source) where T : LinkedContent
	{
		if (!(source is CurrencyLinkedContent currencyLinkedContent))
		{
			if (!(source is BattlePassLevelLinkedContent content))
			{
				if (!(source is CardLinkedContent cardLinkedContent))
				{
					if (!(source is MergeItemLinkedContent content2))
					{
						if (!(source is SkinLinkedContent skinLinkedContent))
						{
							if (!(source is DecorationLinkedContent source2))
							{
								if (!(source is BoosterLinkedContent boosterLinkedContent))
								{
									if (source is PresentLinkedContent presentLinkedContent)
									{
										ResourceView view = GetView(resourceViewPool);
										view.Set(presentLinkedContent.Present.CurrencyType, presentLinkedContent.Quantity, presentLinkedContent.CompositeIdentificator);
										return view;
									}
									throw new ArgumentOutOfRangeException("source", source.GetType(), "Reward line can't display this type of content");
								}
								CardBoosterView view2 = GetView(_boosterViewPool);
								view2.Set(boosterLinkedContent.BonusType);
								return view2;
							}
							RewardDecorationCardView view3 = GetView(decorationViewPool);
							view3.Set(source2);
							return view3;
						}
						SkinView view4 = GetView(skinViewPool);
						view4.Set(skinLinkedContent.Skin);
						return view4;
					}
					RewardMergeItemCardView view5 = GetView(mergeCardViewPool);
					view5.Set(content2);
					return view5;
				}
				RewardCardView view6 = GetView(cardViewPool);
				view6.Set(cardLinkedContent.Card);
				view6.SetQuantityText($"+{cardLinkedContent.Quantity}");
				return view6;
			}
			CardBattlePassLevelView view7 = GetView(battlePassLevelViewPool);
			view7.Set(content);
			return view7;
		}
		ResourceView view8 = GetView(resourceViewPool);
		view8.Set(currencyLinkedContent.Currency, currencyLinkedContent.Quantity, currencyLinkedContent.CompositeIdentificator);
		return view8;
	}

	private T GetView<T>(AbstractPool<T> pool) where T : MonoBehaviour
	{
		if (!IsFree)
		{
			return null;
		}
		T instance = pool.GetInstance();
		instance.gameObject.SetActive(value: true);
		viewList.Add(instance);
		return instance;
	}

	public void Clear()
	{
		foreach (MonoBehaviour view in viewList)
		{
			if (!(view is ResourceView obj))
			{
				if (!(view is RewardCardView obj2))
				{
					if (!(view is RewardMergeItemCardView obj3))
					{
						if (!(view is SkinView obj4))
						{
							if (!(view is RewardDecorationCardView obj5))
							{
								if (!(view is CardBattlePassLevelView obj6))
								{
									if (!(view is CardBoosterView obj7))
									{
										throw new ArgumentOutOfRangeException("view", view.GetType(), "There is no pool to put view in");
									}
									_boosterViewPool.Return(obj7);
								}
								else
								{
									battlePassLevelViewPool.Return(obj6);
								}
							}
							else
							{
								decorationViewPool.Return(obj5);
							}
						}
						else
						{
							skinViewPool.Return(obj4);
						}
					}
					else
					{
						mergeCardViewPool.Return(obj3);
					}
				}
				else
				{
					cardViewPool.Return(obj2);
				}
			}
			else
			{
				resourceViewPool.Return(obj);
			}
		}
		viewList.Clear();
	}
}
