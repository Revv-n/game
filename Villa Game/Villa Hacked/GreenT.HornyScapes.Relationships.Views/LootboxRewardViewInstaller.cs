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
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<SmallCardsViewManager>()).AsSingle();
		((MonoInstallerBase)this).Container.BindViewStructure<CurrencyDropView, CurrencyDropView.Manager>(_currencyDropView, _itemContainer);
		((MonoInstallerBase)this).Container.BindViewStructure<CardDropView, CardDropView.Manager>(_cardDropView, _itemContainer);
		((MonoInstallerBase)this).Container.BindViewStructure<MergeItemDropView, MergeItemDropView.Manager>(_mergeItemDropView, _itemContainer);
		((MonoInstallerBase)this).Container.BindViewStructure<SkinDropView, SkinDropView.Manager>(_skinDropView, _itemContainer);
		((MonoInstallerBase)this).Container.BindViewStructure<DecorationDropView, DecorationDropView.Manager>(_decorationDropView, _itemContainer);
		((MonoInstallerBase)this).Container.BindViewStructure<DecorationDropViewWithRarity, DecorationDropViewWithRarity.Manager>(_decorationDropView, _itemContainer);
		((MonoInstallerBase)this).Container.BindViewStructure<LevelDropView, LevelDropView.Manager>(_levelDropView, _itemContainer);
		((MonoInstallerBase)this).Container.BindViewStructure<BoosterDropView, BoosterDropView.Manager>(_boosterDropView, _itemContainer);
		((MonoInstallerBase)this).Container.BindViewStructure<LootboxDropView, LootboxDropView.Manager>(_lootboxDropView, _itemContainer);
		((MonoInstallerBase)this).Container.BindViewStructure<PresentDropView, PresentDropView.Manager>(_presentDropView, _itemContainer);
		((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<MessageDropView>()).FromComponentInNewPrefab((Object)_messageDropView)).UnderTransform(_itemContainer);
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MessageDropView.Manager>()).FromNewComponentOn(_itemContainer.gameObject).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<GirlPromoOpener>()).AsSingle();
	}
}
