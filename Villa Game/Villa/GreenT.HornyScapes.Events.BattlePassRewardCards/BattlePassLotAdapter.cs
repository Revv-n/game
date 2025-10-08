using System;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.Sellouts.Views;
using StripClub.Model.Shop;
using StripClub.UI;
using StripClub.UI.Shop;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Events.BattlePassRewardCards;

public class BattlePassLotAdapter : MonoView<BundleLot>
{
	[SerializeField]
	private LocalizedTextMeshPro _titleField;

	[SerializeField]
	private LocalizedTextMeshPro _descriptionField;

	[SerializeField]
	private LocalizedTextMeshPro _bonusLevelField;

	[SerializeField]
	private Image _reward;

	[SerializeField]
	private Image _bonusLevelReward;

	[SerializeField]
	private PriceButtonView _priceButtonView;

	[SerializeField]
	private SelloutPointsView _selloutPointsView;

	private ContentSelectorGroup _contentSelectorGroup;

	private readonly Subject<BundleLot> _purchaseSubject = new Subject<BundleLot>();

	[Inject]
	private void Init(ContentSelectorGroup contentSelectorGroup)
	{
		_contentSelectorGroup = contentSelectorGroup;
	}

	public override void Set(BundleLot offerSettings)
	{
		base.Set(offerSettings);
		_priceButtonView.Set(offerSettings.Price, offerSettings.OldPrice);
		SetSelloutLot(offerSettings);
	}

	public void SetLocalization(string levelLocalization)
	{
		if (_titleField != null)
		{
			_titleField.Init(base.Source.NameKey, levelLocalization);
		}
		if (_descriptionField != null)
		{
			_descriptionField.Init(base.Source.DescriptionKey, levelLocalization);
		}
		if (_bonusLevelField != null)
		{
			_bonusLevelField.Init(levelLocalization);
		}
	}

	public void SetRewardSprite(Sprite reward, Sprite bonusLevelReward)
	{
		_reward.sprite = reward;
		if (_bonusLevelReward != null)
		{
			_bonusLevelReward.sprite = bonusLevelReward;
		}
	}

	public void Purchase()
	{
		_purchaseSubject.OnNext(base.Source);
	}

	public IObservable<BundleLot> OnPurchase()
	{
		return _purchaseSubject.AsObservable();
	}

	public void OnPurchaseEnded(BundleLot bundleLot)
	{
		_selloutPointsView.OnPurchase(bundleLot.Content.AnalyticData.SourceType, _contentSelectorGroup.Current);
	}

	private void SetSelloutLot(BundleLot bundleLot)
	{
		_selloutPointsView.SetLot(bundleLot);
		_selloutPointsView.CheckSellout();
	}
}
