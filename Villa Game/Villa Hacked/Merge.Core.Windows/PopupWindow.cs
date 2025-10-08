using DG.Tweening;
using UnityEngine;

namespace Merge.Core.Windows;

public class PopupWindow : Window
{
	[SerializeField]
	private Transform title;

	protected override Tween CreateShowTween()
	{
		Sequence result = DOTween.Sequence();
		DOTween.Sequence();
		return result;
	}
}
