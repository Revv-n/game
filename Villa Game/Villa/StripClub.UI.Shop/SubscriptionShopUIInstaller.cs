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
		base.Container.Bind<TManager>().AsSingle();
		base.Container.BindViewStructure<TManager, CurrencyDropView, CurrencyDropView.Manager>(_currencyPrefab, container, id);
		base.Container.BindViewStructure<TManager, MergeItemDropView, MergeItemDropView.Manager>(_mergeItemPrefab, container, id);
		base.Container.BindViewStructure<TManager, SkinDropView, SkinDropView.Manager>(_skinPrefab, container, id);
		base.Container.BindViewStructure<TManager, DecorationDropView, DecorationDropView.Manager>(_decorationPrefab, container, id);
		base.Container.BindViewStructure<TManager, DecorationDropViewWithRarity, DecorationDropViewWithRarity.Manager>(_decorationPrefab, container, id);
		base.Container.BindViewStructure<TManager, LevelDropView, LevelDropView.Manager>(_levelDropView, container, id);
		base.Container.BindViewStructure<TManager, BoosterDropView, BoosterDropView.Manager>(_boosterDropView, container, id);
		base.Container.BindViewStructure<TManager, LootboxDropView, LootboxDropView.Manager>(_lootboxDropView, container, id);
		base.Container.BindViewStructure<TManager, PresentDropView, PresentDropView.Manager>(_presentDropView, container, id);
		BindSmallCard<TManager>(id, container);
	}

	private void BindSmallCard<TManager>(string id, Transform container)
	{
		base.Container.Bind<CardDropView.Manager>().WithId(id).FromNewComponentOn(container.gameObject)
			.AsCached()
			.WhenInjectedInto<TManager>();
		base.Container.BindIFactory<CardDropView>().FromComponentInNewPrefab(_cardPrefab).UnderTransform(container)
			.When((InjectContext _context) => _context.ParentContext.Identifier != null && _context.ParentContext.Identifier.Equals(id));
	}
}
