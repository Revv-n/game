using GreenT.HornyScapes.Bank.UI;
using GreenT.HornyScapes.Booster;
using GreenT.HornyScapes.Characters.Skins.UI;
using GreenT.HornyScapes.Extensions;
using GreenT.HornyScapes.Presents.UI;
using GreenT.HornyScapes.UI;
using StripClub.Model.Shop.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class EventStartWindowInstaller : MonoInstaller<EventStartWindowInstaller>
{
	[Header("Small options")]
	public CurrencyDropView currencyPrefab;

	public CardDropView cardPrefab;

	public MergeItemDropView mergeItemPrefab;

	public SkinDropView skinPrefab;

	public DecorationDropView decorationPrefab;

	public DecorationDropViewWithRarity decorationWithRarityPrefab;

	public LevelDropView levelPrefab;

	public LootboxDropView lootboxPrefab;

	public BoosterDropView _boosterPrefab;

	public PresentDropView presentDropView;

	public Transform itemContainer;

	public override void InstallBindings()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<SmallCardsViewManager>()).AsSingle();
		((MonoInstallerBase)this).Container.BindViewStructure<CardDropView, CardDropView.Manager>(cardPrefab, itemContainer);
		((MonoInstallerBase)this).Container.BindViewStructure<CurrencyDropView, CurrencyDropView.Manager>(currencyPrefab, itemContainer);
		((MonoInstallerBase)this).Container.BindViewStructure<MergeItemDropView, MergeItemDropView.Manager>(mergeItemPrefab, itemContainer);
		((MonoInstallerBase)this).Container.BindViewStructure<SkinDropView, SkinDropView.Manager>(skinPrefab, itemContainer);
		((MonoInstallerBase)this).Container.BindViewStructure<DecorationDropView, DecorationDropView.Manager>(decorationPrefab, itemContainer);
		((MonoInstallerBase)this).Container.BindViewStructure<DecorationDropViewWithRarity, DecorationDropViewWithRarity.Manager>(decorationWithRarityPrefab, itemContainer);
		((MonoInstallerBase)this).Container.BindViewStructure<LevelDropView, LevelDropView.Manager>(levelPrefab, itemContainer);
		((MonoInstallerBase)this).Container.BindViewStructure<LootboxDropView, LootboxDropView.Manager>(lootboxPrefab, itemContainer);
		((MonoInstallerBase)this).Container.BindViewStructure<BoosterDropView, BoosterDropView.Manager>(_boosterPrefab, itemContainer);
		((MonoInstallerBase)this).Container.BindViewStructure<PresentDropView, PresentDropView.Manager>(presentDropView, itemContainer);
	}
}
