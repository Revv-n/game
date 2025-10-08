using GreenT.HornyScapes.Bank.UI;
using GreenT.HornyScapes.Booster;
using GreenT.HornyScapes.Characters.Skins.UI;
using GreenT.HornyScapes.Extensions;
using GreenT.HornyScapes.Presents.UI;
using GreenT.HornyScapes.UI;
using StripClub.Model.Shop.UI;
using UnityEngine;
using Zenject;

namespace StripClub.UI.Shop;

public class SubscriptionShopUIInstaller : MonoInstaller
{
	private const string BoosterBindID = "booster";

	private const string ImmediateBindID = "immediate";

	private const string RechargeBindID = "recharge";

	[SerializeField]
	private Transform _boosterContainer;

	[SerializeField]
	private Transform _immediateContainer;

	[SerializeField]
	private Transform _rechargeContainer;

	[Header("Small options")]
	[SerializeField]
	private CurrencyDropView _currencyPrefab;

	[SerializeField]
	private CardDropView _cardPrefab;

	[SerializeField]
	private MergeItemDropView _mergeItemPrefab;

	[SerializeField]
	private SkinDropView _skinPrefab;

	[SerializeField]
	private DecorationDropView _decorationPrefab;

	[SerializeField]
	private LevelDropView _levelDropView;

	[SerializeField]
	private BoosterDropView _boosterDropView;

	[SerializeField]
	private LootboxDropView _lootboxDropView;

	[SerializeField]
	private PresentDropView _presentDropView;

	public override void InstallBindings()
	{
		BindStructure<SubscriptionBoosterViewManager>("booster", _boosterContainer);
		BindStructure<SubscriptionImmediateViewManager>("immediate", _immediateContainer);
		BindStructure<SubscriptionRechargeViewManager>("recharge", _rechargeContainer);
	}

	private void BindStructure<TManager>(string id, Transform container)
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<TManager>()).AsSingle();
		((MonoInstallerBase)this).Container.BindViewStructure<TManager, CurrencyDropView, CurrencyDropView.Manager>(_currencyPrefab, container, id);
		((MonoInstallerBase)this).Container.BindViewStructure<TManager, MergeItemDropView, MergeItemDropView.Manager>(_mergeItemPrefab, container, id);
		((MonoInstallerBase)this).Container.BindViewStructure<TManager, SkinDropView, SkinDropView.Manager>(_skinPrefab, container, id);
		((MonoInstallerBase)this).Container.BindViewStructure<TManager, DecorationDropView, DecorationDropView.Manager>(_decorationPrefab, container, id);
		((MonoInstallerBase)this).Container.BindViewStructure<TManager, DecorationDropViewWithRarity, DecorationDropViewWithRarity.Manager>(_decorationPrefab, container, id);
		((MonoInstallerBase)this).Container.BindViewStructure<TManager, LevelDropView, LevelDropView.Manager>(_levelDropView, container, id);
		((MonoInstallerBase)this).Container.BindViewStructure<TManager, BoosterDropView, BoosterDropView.Manager>(_boosterDropView, container, id);
		((MonoInstallerBase)this).Container.BindViewStructure<TManager, LootboxDropView, LootboxDropView.Manager>(_lootboxDropView, container, id);
		((MonoInstallerBase)this).Container.BindViewStructure<TManager, PresentDropView, PresentDropView.Manager>(_presentDropView, container, id);
		BindSmallCard<TManager>(id, container);
	}

	private void BindSmallCard<TManager>(string id, Transform container)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		((ConditionCopyNonLazyBinder)((FromBinder)((MonoInstallerBase)this).Container.Bind<CardDropView.Manager>().WithId((object)id)).FromNewComponentOn(container.gameObject).AsCached()).WhenInjectedInto<TManager>();
		((ConditionCopyNonLazyBinder)((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<CardDropView>()).FromComponentInNewPrefab((Object)_cardPrefab)).UnderTransform(container)).When((BindingCondition)((InjectContext _context) => _context.ParentContext.Identifier != null && _context.ParentContext.Identifier.Equals(id)));
	}
}
