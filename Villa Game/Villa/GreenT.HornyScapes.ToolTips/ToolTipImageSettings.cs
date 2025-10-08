using System.Collections.Generic;
using UnityEngine;

namespace GreenT.HornyScapes.ToolTips;

public class ToolTipImageSettings : ToolTipSettings
{
	public Tail TailSettings;

	[Header("Спрайты обязательно отображаемые")]
	public List<Sprite> Images = new List<Sprite>();
}
