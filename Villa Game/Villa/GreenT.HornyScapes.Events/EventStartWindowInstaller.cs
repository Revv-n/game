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
		base.Container.Bind<SmallCardsViewManager>().AsSingle();
		base.Container.BindViewStructure<CardDropView, CardDropView.Manager>(cardPrefab, itemContainer);
		base.Container.BindViewStructure<CurrencyDropView, CurrencyDropView.Manager>(currencyPrefab, itemContainer);
		base.Container.BindViewStructure<MergeItemDropView, MergeItemDropView.Manager>(mergeItemPrefab, itemContainer);
		base.Container.BindViewStructure<SkinDropView, SkinDropView.Manager>(skinPrefab, itemContainer);
		base.Container.BindViewStructure<DecorationDropView, DecorationDropView.Manager>(decorationPrefab, itemContainer);
		base.Container.BindViewStructure<DecorationDropViewWithRarity, DecorationDropViewWithRarity.Manager>(decorationWithRarityPrefab, itemContainer);
		base.Container.BindViewStructure<LevelDropView, LevelDropView.Manager>(levelPrefab, itemContainer);
		base.Container.BindViewStructure<LootboxDropView, LootboxDropView.Manager>(lootboxPrefab, itemContainer);
		base.Container.BindViewStructure<BoosterDropView, BoosterDropView.Manager>(_boosterPrefab, itemContainer);
		base.Container.BindViewStructure<PresentDropView, PresentDropView.Manager>(presentDropView, itemContainer);
	}
}
