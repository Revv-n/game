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

public class StandardSmallCardsViewManager : SmallCardsViewManager
{
	private const string BindID = "Standart";

	public StandardSmallCardsViewManager(FakeAssetService fakeAssetService, [Inject(Id = "Standart")] MergeItemDropView.Manager itemOptionViewManager, [Inject(Id = "Standart")] CurrencyDropView.Manager currencyOptionViewManager, [Inject(Id = "Standart")] BoosterDropView.Manager boosterOptionManager, [Inject(Id = "Standart")] CardDropView.Manager cardOptionViewManager, [Inject(Id = "Standart")] SkinDropView.Manager skinOptionViewManager, [Inject(Id = "Standart")] LevelDropView.Manager levelManager, [Inject(Id = "Standart")] DecorationDropView.Manager decorationOptionViewManager, [Inject(Id = "Standart")] DecorationDropViewWithRarity.Manager decorationOptionViewWithRarityManager, [Inject(Id = "Standart")] LootboxDropView.Manager lootboxOptionViewManager, LootboxCollection lootboxCollection, CardsCollection cards, MergeIconService iconProvider, RoomManager house, SkinManager skinManager, DecorationManager decorationManager, BoosterMapperManager boosterMapperManager, GameItemConfigManager gameItemConfigManager, [Inject(Id = "Standart")] PresentDropView.Manager presentOptionViewManager)
		: base(itemOptionViewManager, currencyOptionViewManager, boosterOptionManager, cardOptionViewManager, cards, iconProvider, skinOptionViewManager, skinManager, decorationManager, house, decorationOptionViewManager, fakeAssetService, decorationOptionViewWithRarityManager, levelManager, boosterMapperManager, lootboxOptionViewManager, lootboxCollection, gameItemConfigManager, presentOptionViewManager)
	{
	}
}
