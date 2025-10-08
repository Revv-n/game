using DG.Tweening;
using UnityEngine;

namespace GreenT.HornyScapes._HornyScapes._Scripts.Events.ProgressWindow.BattlePassRewardCards;

public class RewardAnimationView : MonoBehaviour
{
	[Range(0.1f, 8f)]
	[SerializeField]
	private float _xRange = 5f;

	[Range(0.1f, 8f)]
	[SerializeField]
	private float _yRange = 5f;

	[Range(0.1f, 8f)]
	[SerializeField]
	private float _loopTimestamp = 4f;

	[Range(1f, 60f)]
	[SerializeField]
	private float _rotationDelta = 5f;

	[Range(0.1f, 7f)]
	[SerializeField]
	private float _rotationDuration = 4f;

	[Range(0f, 10f)]
	[SerializeField]
	private float _timingOffset;

	[SerializeField]
	private bool _playOnStart = true;

	private Sequence _positionSequence;

	private Sequence _rotationSequence;

	private void Start()
	{
		if (_playOnStart)
		{
			SetupAnimation(base.transform);
		}
	}

	public void SetupAnimation(Transform target)
	{
		float delay = Random.Range(0f, _timingOffset);
		_positionSequence = DOTween.Sequence();
		_rotationSequence = DOTween.Sequence();
		Vector3 localPosition = target.localPosition;
		_positionSequence.SetDelay(delay);
		_positionSequence.Append(target.DOLocalMove(Vector3.right * _xRange, _loopTimestamp));
		_positionSequence.Append(target.DOLocalMove(Vector3.up * _yRange, _loopTimestamp));
		_positionSequence.Append(target.DOLocalMove(Vector3.left * _xRange, _loopTimestamp));
		_positionSequence.Append(target.DOLocalMove(Vector3.down * _yRange, _loopTimestamp));
		_positionSequence.Append(target.DOLocalMove(Vector3.right * _xRange, _loopTimestamp));
		_positionSequence.Append(target.DOLocalMove(localPosition, _loopTimestamp));
		_positionSequence.SetLoops(-1);
		_rotationSequence.SetDelay(delay);
		_rotationSequence.Append(target.DORotate(Vector3.forward * _rotationDelta, _rotationDuration));
		_rotationSequence.Append(target.DORotate(Vector3.forward * ((0f - _rotationDelta) * 2f), _rotationDuration));
		_rotationSequence.Append(target.DORotate(Vector3.forward * (_rotationDelta * 2f), _rotationDuration));
		_rotationSequence.Append(target.DORotate(Vector3.zero, _rotationDuration));
		_rotationSequence.SetLoops(-1);
	}
}
