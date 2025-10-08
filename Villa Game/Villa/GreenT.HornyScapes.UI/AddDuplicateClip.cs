using System;
using DG.Tweening;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Characters.Skins.Content;
using GreenT.HornyScapes.Characters.Skins.UI;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Meta.Decorations;
using StripClub.Model;
using StripClub.Rewards.UI;
using StripClub.UI;
using StripClub.UI.Rewards;
using UnityEngine;

namespace GreenT.HornyScapes.UI;

public class AddDuplicateClip : Clip
{
	[Serializable]
	public class RewardDisplaySettings<T> : RewardDisplaySettings where T : MonoBehaviour
	{
		public T RewardView;

		public override MonoBehaviour GetRewardView()
		{
			return RewardView;
		}
	}

	public abstract class RewardDisplaySettings
	{
		public GameObject[] FrontObjectsToShow;

		public GameObject[] BackObjectsToShow;

		public abstract MonoBehaviour GetRewardView();
	}

	[SerializeField]
	private RewardDisplaySettings<RewardDecorationCardView> _decorationCardView;

	[SerializeField]
	private RewardDisplaySettings<RewardCardView> _cardView;

	[SerializeField]
	private RewardDisplaySettings<ResourceView> _resourceView;

	[SerializeField]
	private RewardDisplaySettings<RewardMergeItemCardView> _mergeCardView;

	[SerializeField]
	private RewardDisplaySettings<SkinView> _skinView;

	[SerializeField]
	private RewardDisplaySettings<CardBattlePassLevelView> _battlePassLevelView;

	[SerializeField]
	private DuplicateCardAnimation _duplicateCardAnimation;

	[SerializeField]
	private float _waitTime = 1f;

	public void Init(LinkedContent reward, LinkedContent alternativeReward)
	{
		RewardDisplaySettings rewardView = GetRewardView(reward);
		RewardDisplaySettings rewardView2 = GetRewardView(alternativeReward);
		DuplicateCardAnimation.CardSettings settings = new DuplicateCardAnimation.CardSettings(rewardView.GetRewardView().gameObject, rewardView2.GetRewardView().gameObject, _waitTime, rewardView.FrontObjectsToShow, rewardView.BackObjectsToShow, rewardView2.FrontObjectsToShow, rewardView2.BackObjectsToShow);
		_duplicateCardAnimation.Init(settings);
	}

	public override void Play()
	{
		_duplicateCardAnimation.Play().OnComplete(Stop);
	}

	public override void Stop()
	{
		base.gameObject.SetActive(value: false);
		_duplicateCardAnimation.Stop();
		base.Stop();
	}

	private void OnDisable()
	{
		Stop();
	}

	private RewardDisplaySettings GetRewardView(LinkedContent content)
	{
		if (!(content is DecorationLinkedContent source))
		{
			if (!(content is CardLinkedContent cardLinkedContent))
			{
				if (!(content is CurrencyLinkedContent currencyLinkedContent))
				{
					if (!(content is MergeItemLinkedContent content2))
					{
						if (!(content is SkinLinkedContent skinLinkedContent))
						{
							if (content is BattlePassLevelLinkedContent content3)
							{
								_battlePassLevelView.RewardView.Set(content3);
								return _battlePassLevelView;
							}
							throw new Exception("AddDecorationDuplicateClip cant get view for this content type");
						}
						_skinView.RewardView.Set(skinLinkedContent.Skin);
						return _skinView;
					}
					_mergeCardView.RewardView.Set(content2);
					return _mergeCardView;
				}
				_resourceView.RewardView.Set(currencyLinkedContent.Currency, currencyLinkedContent.Quantity, currencyLinkedContent.CompositeIdentificator);
				return _resourceView;
			}
			_cardView.RewardView.Set(cardLinkedContent.Card);
			return _cardView;
		}
		_decorationCardView.RewardView.Set(source);
		return _decorationCardView;
	}
}
