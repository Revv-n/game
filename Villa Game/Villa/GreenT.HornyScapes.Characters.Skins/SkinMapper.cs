using System;
using GreenT.HornyScapes.Data;
using GreenT.Model.Collections;
using StripClub.Model;
using StripClub.Model.Cards;

namespace GreenT.HornyScapes.Characters.Skins;

[Serializable]
[Mapper]
public class SkinMapper
{
	public class Manager : SimpleManager<SkinMapper>
	{
	}

	public int id;

	public int girl_id;

	public int order_number;

	public Rarity rarity;

	public UnlockType[] preload_type;

	public string[] preload_value;

	public string unlock_message;
}
