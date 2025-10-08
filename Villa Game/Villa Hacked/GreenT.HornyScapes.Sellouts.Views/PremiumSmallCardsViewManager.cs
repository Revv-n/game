using GreenT.AssetBundles;
using GreenT.HornyScapes.Bank.UI;
using GreenT.HornyScapes.Booster;
using GreenT.HornyScapes.Characters.Skins;
using GreenT.HornyScapes.Characters.Skins.UI;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.GameItems;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.Meta;
using GreenT.HornyScapes.Meta.Decorations;
using GreenT.HornyScapes.Presents.UI;
using GreenT.HornyScapes.UI;
using StripClub.Model.Cards;
using StripClub.Model.Shop.UI;
using Zenject;

namespace GreenT.HornyScapes.Sellouts.Views;

public class PremiumSmallCardsViewManager : SmallCardsViewManager
{
	private const string BindID = "Premium";

	public PremiumSmallCardsViewManager(FakeAssetService fakeAssetService, [Inject(Id = "Premium")] MergeItemDropView.Manager itemOptionViewManager, [Inject(Id = "Premium")] CurrencyDropView.Manager currencyOptionViewManager, [Inject(Id = "Premium")] BoosterDropView.Manager boosterOptionManager, [Inject(Id = "Premium")] CardDropView.Manager cardOptionViewManager, [Inject(Id = "Premium")] SkinDropView.Manager skinOptionViewManager, [Inject(Id = "Premium")] LevelDropView.Manager levelManager, [Inject(Id = "Premium")] DecorationDropView.Manager decorationOptionViewManager, [Inject(Id = "Premium")] DecorationDropViewWithRarity.Manager decorationOptionViewWithRarityManager, [Inject(Id = "Premium")] LootboxDropView.Manager lootboxOptionViewManager, LootboxCollection lootboxCollection, CardsCollection cards, MergeIconService iconProvider, RoomManager house, SkinManager skinManager, DecorationManager decorationManager, BoosterMapperManager boosterMapperManager, GameItemConfigManager gameItemConfigManager, [Inject(Id = "Premium")] PresentDropView.Manager presentOptionViewManager)
		: base(itemOptionViewManager, currencyOptionViewManager, boosterOptionManager, cardOptionViewManager, cards, iconProvider, skinOptionViewManager, skinManager, decorationManager, house, decorationOptionViewManager, fakeAssetService, decorationOptionViewWithRarityManager, levelManager, boosterMapperManager, lootboxOptionViewManager, lootboxCollection, gameItemConfigManager, presentOptionViewManager)
	{
	}
}
