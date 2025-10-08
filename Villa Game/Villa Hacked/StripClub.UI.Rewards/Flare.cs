using DG.Tweening;
using UnityEngine;

namespace StripClub.UI.Rewards;

public class Flare : MonoBehaviour
{
	[SerializeField]
	private Transform flare;

	[SerializeField]
	private Vector3 to;

	[SerializeField]
	private float duration = 1f;

	[SerializeField]
	private float delay = 0.15f;

	private Vector3 from;

	private void Awake()
	{
		from = flare.transform.localPosition;
	}

	private void OnEnable()
	{
		flare.DOLocalMove(to, duration).SetDelay(delay).OnComplete(delegate
		{
			Reset();
		});
	}

	private void Reset()
	{
		flare.transform.localPosition = from;
	}

	private void OnDisable()
	{
		Reset();
	}
}
