using UnityEngine;

namespace GreenT.HornyScapes.Events;

[CreateAssetMenu(menuName = "GreenT/HornyScapes/Events/Bundle")]
public class EventBundleData : ScriptableObject, IBundleData
{
	public string TitleKeyLoc = "ui.event.progresswindow.title.";

	public string DescriptionKeyLoc = "ui.event.progresswindow.description.";

	public string CurrencyKeyLoc = "ui.event.currency.";

	public string TargetKeyLoc = "ui.event.target.";

	[Tooltip("иконка валюты для бара")]
	public Sprite Currency;

	[Tooltip("stack иконка для карточек")]
	public Sprite AlternativeCurrency;

	[Tooltip("ивентовый опыт")]
	public Sprite Target;

	[Header("Девушки слева")]
	[Tooltip("в прогресс окне")]
	public Sprite ProgressGirl;

	[Tooltip("в стартовом окне")]
	public Sprite StartGirl;

	public Sprite BankButtonSp;

	public Sprite ButtonSp;

	public Sprite MergeBackground;

	public TextAsset DefaultField;

	public string Type;

	public Sprite StartWindowBackground;

	[field: SerializeField]
	public Sprite RecieBookButton { get; private set; }

	[field: SerializeField]
	public Sprite RecipeBook { get; private set; }

	[field: SerializeField]
	public Sprite AnnouncementBackground { get; set; }

	[field: SerializeField]
	public Sprite AnnouncementTitleBackground { get; set; }

	[field: SerializeField]
	public Sprite BattlePassBackground { get; set; }
}
