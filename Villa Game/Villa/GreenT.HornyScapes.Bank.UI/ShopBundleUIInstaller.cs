using GreenT.HornyScapes.Booster;
using GreenT.HornyScapes.Characters.Skins.UI;
using GreenT.HornyScapes.Extensions;
using GreenT.HornyScapes.Presents.UI;
using GreenT.HornyScapes.UI;
using StripClub.Model.Shop.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Bank.UI;

public class ShopBundleUIInstaller : MonoInstaller
{
	[Header("Small options")]
	[SerializeField]
	private CurrencyDropView currencyPrefab;

	[SerializeField]
	private CardDropView cardPrefab;

	[SerializeField]
	private MergeItemDropView mergeItemPrefab;

	[SerializeField]
	private SkinDropView skinPrefab;

	[SerializeField]
	private DecorationDropView decorationPrefab;

	[SerializeField]
	private LevelDropView levelDropView;

	[SerializeField]
	private BoosterDropView _boosterPrefab;

	[SerializeField]
	private LootboxDropView lootboxDropView;

	[SerializeField]
	private PresentDropView presentDropView;

	[SerializeField]
	private Transform itemContainer;

	[Space]
	[Header("Big cards")]
	[SerializeField]
	private CardDropView cardViewPrefab;

	[SerializeField]
	private MergeItemDropCardBigView mergeItemDropCardBigPrefab;

	[SerializeField]
	private ResourceDropCardBigView resourceDropCardBigViewPrefab;

	[SerializeField]
	private Transform cardViewContainer;

	public override void InstallBindings()
	{
		base.Container.Bind<SmallCardsViewManager>().AsSingle();
		base.Container.Bind<BigCardsViewManager>().AsSingle();
		base.Container.BindViewStructure<CurrencyDropView, CurrencyDropView.Manager>(currencyPrefab, itemContainer);
		base.Container.BindViewStructure<SkinDropView, SkinDropView.Manager>(skinPrefab, itemContainer);
		base.Container.BindViewStructure<DecorationDropView, DecorationDropView.Manager>(decorationPrefab, itemContainer);
		base.Container.BindViewStructure<DecorationDropViewWithRarity, DecorationDropViewWithRarity.Manager>(decorationPrefab, itemContainer);
		base.Container.BindViewStructure<LevelDropView, LevelDropView.Manager>(levelDropView, itemContainer);
		base.Container.BindViewStructure<BoosterDropView, BoosterDropView.Manager>(_boosterPrefab, itemContainer);
		base.Container.BindViewStructure<LootboxDropView, LootboxDropView.Manager>(lootboxDropView, itemContainer);
		base.Container.BindViewStructure<MergeItemDropView, MergeItemDropView.Manager>(mergeItemPrefab, itemContainer);
		base.Container.BindViewStructure<PresentDropView, PresentDropView.Manager>(presentDropView, itemContainer);
		BindSmallCard();
		BindBigCard();
	}

	private void BindSmallCard()
	{
		base.Container.Bind<CardDropView.Manager>().FromNewComponentOn(itemContainer.gameObject).AsCached()
			.WhenInjectedInto<SmallCardsViewManager>();
		base.Container.BindIFactory<CardDropView>().FromComponentInNewPrefab(cardPrefab).UnderTransform(itemContainer);
	}

	private void BindBigCard()
	{
		base.Container.Bind<CardDropView.Manager>().WithId("CardsManager").FromNewComponentOn(cardViewContainer.gameObject)
			.AsCached();
		base.Container.BindIFactory<CardDropView>().FromComponentInNewPrefab(cardViewPrefab).UnderTransform(cardViewContainer)
			.AsCached()
			.When((InjectContext _context) => _context.ParentContext.Identifier != null && _context.ParentContext.Identifier.Equals("CardsManager"));
		base.Container.Bind<MergeItemDropCardBigView.Manager>().WithId("MergeItemBigCardsManager").FromNewComponentOn(cardViewContainer.gameObject)
			.AsCached();
		base.Container.BindIFactory<MergeItemDropCardBigView>().FromComponentInNewPrefab(mergeItemDropCardBigPrefab).UnderTransform(cardViewContainer)
			.AsCached()
			.When((InjectContext _context) => _context.ParentContext.Identifier != null && _context.ParentContext.Identifier.Equals("MergeItemBigCardsManager"));
		base.Container.Bind<ResourceDropCardBigView.Manager>().WithId("ResourceBigCardsManager").FromNewComponentOn(cardViewContainer.gameObject)
			.AsCached();
		base.Container.BindIFactory<ResourceDropCardBigView>().FromComponentInNewPrefab(resourceDropCardBigViewPrefab).UnderTransform(cardViewContainer)
			.AsCached()
			.When((InjectContext _context) => _context.ParentContext.Identifier != null && _context.ParentContext.Identifier.Equals("ResourceBigCardsManager"));
	}
}
