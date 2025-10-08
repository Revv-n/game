using Merge;
using StripClub.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.ToolTips;

public class MergeItemCollectionView : MonoView
{
	public class Manager : ViewManager<MergeItemCollectionView>
	{
	}

	public Image Icon;

	public GIConfig Source;

	public void Set(GIConfig source, Sprite icon)
	{
		Source = source;
		Refresh(icon);
	}

	public void Refresh(Sprite icon)
	{
		Icon.sprite = icon;
	}
}
