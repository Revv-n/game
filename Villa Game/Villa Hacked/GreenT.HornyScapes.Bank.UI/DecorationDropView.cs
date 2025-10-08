using GreenT.HornyScapes.Meta.RoomObjects;
using StripClub.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Bank.UI;

public class DecorationDropView : MonoView
{
	public class Manager : ViewManager<DecorationDropView>
	{
	}

	[SerializeField]
	private Image _icon;

	public BaseObjectConfig Config { get; private set; }

	public void Set(BaseObjectConfig config)
	{
		Config = config;
		_icon.sprite = config.CardViewIcon;
	}
}
