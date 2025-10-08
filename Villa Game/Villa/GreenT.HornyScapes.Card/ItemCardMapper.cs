using System;
using GreenT.HornyScapes.Data;

namespace GreenT.HornyScapes.Card;

[Serializable]
[Mapper]
public class ItemCardMapper : CardMapper
{
	public int[] character_id;

	public int tab_id;

	public string guid;
}
