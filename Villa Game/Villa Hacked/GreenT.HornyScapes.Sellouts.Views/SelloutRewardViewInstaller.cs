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

namespace GreenT.HornyScapes.Sellouts.Views;

public class SelloutRewardViewInstaller : MonoInstaller<SelloutRewardViewInstaller>
{
	private const string StandartBindID = "Standart";

	private const string PremiumBindID = "Premium";

	[SerializeField]
	private Transform _itemContainer;

	[SerializeField]
	private Transform _premiumRewardsContainer;

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

	[Header("Small options premium")]
	[SerializeField]
	private CurrencyDropView _premiumCurrencyDropView;

	[SerializeField]
	private CardDropView _premiumCardDropView;

	[SerializeField]
	private MergeItemDropView _premiumMergeItemDropView;

	[SerializeField]
	private SkinDropView _premiumSkinDropView;

	[SerializeField]
	private DecorationDropView _premiumDecorationDropView;

	[SerializeField]
	private LevelDropView _premiumLevelDropView;

	[SerializeField]
	private BoosterDropView _premiumBoosterDropView;

	[SerializeField]
	private LootboxDropView _premiumLootboxDropView;

	[SerializeField]
	private PresentDropView _premiumPresentDropView;

	public override void InstallBindings()
	{
		BindStructure<StandardSmallCardsViewManager>("Standart", _itemContainer, _currencyDropView, _cardDropView, _mergeItemDropView, _skinDropView, _decorationDropView, _levelDropView, _boosterDropView, _lootboxDropView, _presentDropView);
		BindStructure<PremiumSmallCardsViewManager>("Premium", _premiumRewardsContainer, _premiumCurrencyDropView, _premiumCardDropView, _premiumMergeItemDropView, _premiumSkinDropView, _premiumDecorationDropView, _premiumLevelDropView, _premiumBoosterDropView, _premiumLootboxDropView, _premiumPresentDropView);
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<GirlPromoOpener>()).AsSingle();
	}

	private void BindStructure<TManager>(string id, Transform container, CurrencyDropView currencyDropView, CardDropView cardDropView, MergeItemDropView mergeItemDropView, SkinDropView skinDropView, DecorationDropView decorationDropView, LevelDropView levelDropView, BoosterDropView boosterDropView, LootboxDropView lootboxDropView, PresentDropView presentDropView)
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<TManager>()).AsSingle();
		((MonoInstallerBase)this).Container.BindViewStructure<TManager, CurrencyDropView, CurrencyDropView.Manager>(currencyDropView, container, id);
		((MonoInstallerBase)this).Container.BindViewStructure<TManager, MergeItemDropView, MergeItemDropView.Manager>(mergeItemDropView, container, id);
		((MonoInstallerBase)this).Container.BindViewStructure<TManager, SkinDropView, SkinDropView.Manager>(skinDropView, container, id);
		((MonoInstallerBase)this).Container.BindViewStructure<TManager, DecorationDropView, DecorationDropView.Manager>(decorationDropView, container, id);
		((MonoInstallerBase)this).Container.BindViewStructure<TManager, DecorationDropViewWithRarity, DecorationDropViewWithRarity.Manager>(decorationDropView, container, id);
		((MonoInstallerBase)this).Container.BindViewStructure<TManager, LevelDropView, LevelDropView.Manager>(levelDropView, container, id);
		((MonoInstallerBase)this).Container.BindViewStructure<TManager, BoosterDropView, BoosterDropView.Manager>(boosterDropView, container, id);
		((MonoInstallerBase)this).Container.BindViewStructure<TManager, LootboxDropView, LootboxDropView.Manager>(lootboxDropView, container, id);
		((MonoInstallerBase)this).Container.BindViewStructure<TManager, PresentDropView, PresentDropView.Manager>(presentDropView, container, id);
		BindSmallCard<TManager>(id, cardDropView, container);
	}

	private void BindSmallCard<TManager>(string id, CardDropView cardDropView, Transform container)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		((ConditionCopyNonLazyBinder)((FromBinder)((MonoInstallerBase)this).Container.Bind<CardDropView.Manager>().WithId((object)id)).FromNewComponentOn(container.gameObject).AsCached()).WhenInjectedInto<TManager>();
		((ConditionCopyNonLazyBinder)((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<CardDropView>()).FromComponentInNewPrefab((Object)cardDropView)).UnderTransform(container)).When((BindingCondition)((InjectContext context) => context.ParentContext.Identifier != null && context.ParentContext.Identifier.Equals(id)));
	}
}
