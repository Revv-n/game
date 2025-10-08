using StripClub.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Characters.Skins.UI;

public class SkinDropView : MonoView
{
	public class Manager : ViewManager<SkinDropView>
	{
	}

	public Image icon;

	[SerializeField]
	private StatableComponent statable;

	public Skin Skin { get; private set; }

	public void Set(Skin skin, int? rarity)
	{
		Skin = skin;
		SetRarity(rarity);
	}

	private void SetRarity(int? rarity)
	{
		if (rarity.HasValue)
		{
			statable.Set(rarity.Value);
		}
		else
		{
			statable.Set((int)Skin.Rarity);
		}
	}
}
