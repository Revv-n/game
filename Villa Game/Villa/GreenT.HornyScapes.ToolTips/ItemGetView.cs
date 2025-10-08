using System.Collections.Generic;
using GreenT.HornyScapes.Events;
using Merge;
using StripClub.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.ToolTips;

public class ItemGetView : MonoView<List<GIConfig>>
{
	private MergeIconService _iconProvider;

	private MergeItemCollectionView.Manager viewManager;

	[Inject]
	private void Init(MergeItemCollectionView.Manager viewManager, MergeIconService iconProvider)
	{
		this.viewManager = viewManager;
		_iconProvider = iconProvider;
	}

	public override void Set(List<GIConfig> sources)
	{
		base.Set(sources);
		viewManager.HideAll();
		if (sources == null)
		{
			return;
		}
		foreach (GIConfig source in sources)
		{
			if (_iconProvider.GetSprite(source.Key) != null)
			{
				viewManager.GetView().Set(source, _iconProvider.GetSprite(source.Key));
			}
		}
	}

	public void AddAdditionalWay(List<Sprite> additionalWays)
	{
		foreach (Sprite additionalWay in additionalWays)
		{
			viewManager.GetView().Refresh(additionalWay);
		}
	}
}
