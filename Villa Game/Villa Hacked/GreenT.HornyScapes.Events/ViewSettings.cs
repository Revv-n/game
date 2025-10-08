using JetBrains.Annotations;
using UnityEngine;

namespace GreenT.HornyScapes.Events;

public struct ViewSettings
{
	public Sprite Currency;

	public Sprite AlternativeCurrency;

	public Sprite Target;

	public Sprite ProgressGirl;

	public Sprite StartGirl;

	public Sprite BankButtonSp;

	public Sprite ButtonSp;

	public Sprite MergeBackground;

	[CanBeNull]
	public Sprite RecipeBookButton;

	[CanBeNull]
	public Sprite RecipeBook;

	public Sprite AnnouncementBackground;

	public Sprite AnnouncementTitleBackground;

	public Sprite BattlePassBackground;

	public Sprite StartWindowBackground;

	public TextAsset DefaultField;
}
