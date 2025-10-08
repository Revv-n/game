using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class ScrollRefresher : MonoBehaviour
{
	[SerializeField]
	private ScrollRect _scrollView;

	private void OnEnable()
	{
		_scrollView.verticalNormalizedPosition = 1f;
	}
}
