using System;
using GreenT.HornyScapes.Data;

namespace GreenT.HornyScapes;

[Serializable]
[Mapper]
public sealed class ActivityMapper
{
	public int id;

	public bool mult_tab;

	public bool activity_currency;

	public int[] quest_tabs;

	public int[] quest_tabs_priority;

	public int[] shop_tabs;

	public int[] shop_tabs_priority;
}
