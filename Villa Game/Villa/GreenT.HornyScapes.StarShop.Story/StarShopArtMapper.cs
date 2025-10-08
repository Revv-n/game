using System;
using GreenT.HornyScapes.Data;
using StripClub.Model;

namespace GreenT.HornyScapes.StarShop.Story;

[Serializable]
[Mapper]
public class StarShopArtMapper
{
	public int id;

	public UnlockType[] unlock_type;

	public string[] unlock_value;
}
