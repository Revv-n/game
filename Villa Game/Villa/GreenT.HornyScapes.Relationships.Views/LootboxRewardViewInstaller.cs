using GreenT.HornyScapes.Bank.UI;
using GreenT.HornyScapes.Booster;
using GreenT.HornyScapes.Characters.Skins.UI;
using GreenT.HornyScapes.Extensions;
using GreenT.HornyScapes.Presents.UI;
using GreenT.HornyScapes.UI;
using StripClub.Model.Shop.UI;
using StripClub.UI.Rewards;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Relationships.Views;

public class LootboxRewardViewInstaller : MonoInstaller<LootboxRewardViewInstaller>
{
	[Header("Small options")]
	[SerializeField]
	private CurrencyDropView _currencyDropView;

	[SerializeField]
	private CardDropView _cardDropView;

	[SerializeField]
	private MergeItemDropView _mergeItemDropView;

	[SerializeField]
	private SkinDropView _skinDropView;

	[SerializeField]
	private DecorationDropView _decorationDropView;

	[SerializeField]
	private LevelDropView _levelDropView;

	[SerializeField]
	private BoosterDropView _boosterDropView;

	[SerializeField]
	private LootboxDropView _lootboxDropView;

	[SerializeField]
	private PresentDropView _presentDropView;

	[SerializeField]
	private MessageDropView _messageDropView;

	[SerializeField]
	private Transform _itemContainer;

	public override void InstallBindings()
	{
		base.Container.Bind<SmallCardsViewManager>().AsSingle();
		base.Container.BindViewStructure<CurrencyDropView, CurrencyDropView.Manager>(_currencyDropView, _itemContainer);
		base.Container.BindViewStructure<CardDropView, CardDropView.Manager>(_cardDropView, _itemContainer);
		base.Container.BindViewStructure<MergeItemDropView, MergeItemDropView.Manager>(_mergeItemDropView, _itemContainer);
		base.Container.BindViewStructure<SkinDropView, SkinDropView.Manager>(_skinDropView, _itemContainer);
		base.Container.BindViewStructure<DecorationDropView, DecorationDropView.Manager>(_decorationDropView, _itemContainer);
		base.Container.BindViewStructure<DecorationDropViewWithRarity, DecorationDropViewWithRarity.Manager>(_decorationDropView, _itemContainer);
		base.Container.BindViewStructure<LevelDropView, LevelDropView.Manager>(_levelDropView, _itemContainer);
		base.Container.BindViewStructure<BoosterDropView, BoosterDropView.Manager>(_boosterDropView, _itemContainer);
		base.Container.BindViewStructure<LootboxDropView, LootboxDropView.Manager>(_lootboxDropView, _itemContainer);
		base.Container.BindViewStructure<PresentDropView, PresentDropView.Manager>(_presentDropView, _itemContainer);
		base.Container.BindIFactory<MessageDropView>().FromComponentInNewPrefab(_messageDropView).UnderTransform(_itemContainer);
		base.Container.BindInterfacesAndSelfTo<MessageDropView.Manager>().FromNewComponentOn(_itemContainer.gameObject).AsSingle();
		base.Container.Bind<GirlPromoOpener>().AsSingle();
	}
}
