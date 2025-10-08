using System;
using DG.Tweening;
using UnityEngine;

namespace GreenT.HornyScapes.Relationships.Views;

public sealed class UnblockedRewardAnimator : MonoBehaviour
{
	[SerializeField]
	private GameObject[] _blockedObjects;

	[SerializeField]
	private GameObject[] _unblockedObjects;

	[SerializeField]
	private Vector3 _defaultRotation;

	[SerializeField]
	private Vector3 _targetRotation;

	[SerializeField]
	private float _unblockedLevelStartDelay = 0.3f;

	[SerializeField]
	private float _unblockedLevelDelay = 0.15f;

	[SerializeField]
	private float _rotationDuration = 0.15f;

	private Sequence _unblockedLevelSequence;

	public Sequence Play(int index, Action action = null)
	{
		_unblockedLevelSequence?.Kill();
		_unblockedLevelSequence = DOTween.Sequence().AppendCallback(SetPreviousState).AppendInterval(_unblockedLevelStartDelay)
			.AppendInterval(_unblockedLevelDelay * (float)index)
			.AppendCallback(SetDefaultRotation)
			.Append(RotateObjects(_blockedObjects, in _targetRotation, _rotationDuration))
			.Append(RotateObjects(_unblockedObjects, in _targetRotation, 0f))
			.AppendCallback(SetCurrentState)
			.AppendCallback(CallAction)
			.Append(RotateObjects(_unblockedObjects, in _defaultRotation, _rotationDuration))
			.Append(RotateObjects(_blockedObjects, in _defaultRotation, 0f))
			.AppendCallback(SetDefaultRotation);
		return _unblockedLevelSequence;
		void CallAction()
		{
			action?.Invoke();
		}
		static Sequence RotateObjects(GameObject[] objects, in Vector3 targetRotation, float duration)
		{
			Sequence sequence = DOTween.Sequence();
			for (int j = 0; j < objects.Length; j++)
			{
				sequence.Join(objects[j].transform.DOLocalRotate(targetRotation, duration));
			}
			return sequence;
		}
		void SetCurrentState()
		{
			GameObject[] blockedObjects = _blockedObjects;
			for (int i = 0; i < blockedObjects.Length; i++)
			{
				blockedObjects[i].SetActive(value: false);
			}
			blockedObjects = _unblockedObjects;
			for (int i = 0; i < blockedObjects.Length; i++)
			{
				blockedObjects[i].SetActive(value: true);
			}
		}
		void SetDefaultRotation()
		{
			GameObject[] blockedObjects2 = _blockedObjects;
			for (int k = 0; k < blockedObjects2.Length; k++)
			{
				blockedObjects2[k].transform.DOLocalRotate(_defaultRotation, 0f);
			}
			blockedObjects2 = _unblockedObjects;
			for (int k = 0; k < blockedObjects2.Length; k++)
			{
				blockedObjects2[k].transform.DOLocalRotate(_defaultRotation, 0f);
			}
		}
		void SetPreviousState()
		{
			GameObject[] blockedObjects3 = _blockedObjects;
			for (int l = 0; l < blockedObjects3.Length; l++)
			{
				blockedObjects3[l].SetActive(value: true);
			}
			blockedObjects3 = _unblockedObjects;
			for (int l = 0; l < blockedObjects3.Length; l++)
			{
				blockedObjects3[l].SetActive(value: false);
			}
		}
	}

	public void Stop()
	{
		_unblockedLevelSequence?.Kill();
		_unblockedLevelSequence = null;
	}

	private void OnDisable()
	{
		Stop();
	}
}
