using Merge;
using UnityEngine;

namespace GreenT.HornyScapes;

public sealed class RatingUpdatersController : MonoBehaviour
{
	[SerializeField]
	private RatingUpdater[] _ratingUpdaters;

	private void OnDisable()
	{
		for (int i = 0; i < _ratingUpdaters.Length; i++)
		{
			_ratingUpdaters[i].SetActive(active: false);
		}
	}
}
