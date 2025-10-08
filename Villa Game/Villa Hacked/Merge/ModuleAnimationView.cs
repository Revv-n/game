using System;
using DG.Tweening;
using GreenT;
using UnityEngine;

namespace Merge;

public class ModuleAnimationView : MonoBehaviour
{
	[Range(1f, 2f)]
	[SerializeField]
	private float _scaleAmount = 1.5f;

	[SerializeField]
	private float _scaleDuration = 1f;

	[Range(5f, 60f)]
	[SerializeField]
	private float _rotationAmount = 15f;

	[Range(0.1f, 1f)]
	[SerializeField]
	private float _rotationDuration = 0.75f;

	[Range(0.1f, 7f)]
	[SerializeField]
	private float _rotationFrequency = 5f;

	private Sequence _scaleSequence;

	private Sequence _rotationSequence;

	[SerializeField]
	private Vector2 size;

	[SerializeField]
	private SpriteRenderer spriteRenderer;

	private void OnEnable()
	{
		spriteRenderer.size = size;
	}

	private void Start()
	{
		Vector3 localScale = base.transform.localScale;
		_scaleSequence = DOTween.Sequence();
		_rotationSequence = DOTween.Sequence();
		_scaleSequence.Append(base.transform.DOScale(localScale * _scaleAmount, _scaleDuration));
		_scaleSequence.Append(base.transform.DOScale(localScale, _scaleDuration * 1.5f));
		_scaleSequence.SetLoops(-1);
		_rotationSequence.AppendInterval(_rotationFrequency);
		_rotationSequence.Append(base.transform.DORotate(Vector3.forward * _rotationAmount, _rotationDuration));
		_rotationSequence.Append(base.transform.DORotate(Vector3.forward * (0f - _rotationAmount) * 2f, _rotationDuration));
		_rotationSequence.Append(base.transform.DORotate(Vector3.forward * _rotationAmount * 2f, _rotationDuration));
		_rotationSequence.Append(base.transform.DORotate(Vector3.zero, _rotationDuration));
		_rotationSequence.SetLoops(-1);
	}

	private void OnDestroy()
	{
		try
		{
			_scaleSequence.Kill();
			_rotationSequence.Kill();
		}
		catch (Exception exception)
		{
			exception.LogException();
		}
	}
}
