using System;
using GreenT.HornyScapes.Data;

namespace GreenT.HornyScapes.MiniEvents;

[Serializable]
[Mapper]
public sealed class ActivityQuestMapper
{
	public int tab_id;

	public int quest_massive_id;

	public string icon;

	public string bundle;
}
