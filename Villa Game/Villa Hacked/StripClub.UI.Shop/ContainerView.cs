using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StripClub.UI.Shop;

public class ContainerView : MonoView<LotContainer>
{
	[SerializeField]
	public List<LotView> LotViews;

	public override void Set(LotContainer lotContainer)
	{
		for (int i = 0; i < lotContainer.Lots.Count(); i++)
		{
			LotViews[i].Set(lotContainer.Lots.ToList()[i]);
		}
	}
}
