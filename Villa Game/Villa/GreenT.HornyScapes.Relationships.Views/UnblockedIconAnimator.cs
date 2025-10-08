using DG.Tweening;
using GreenT.HornyScapes.Animations;
using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes.Relationships.Views;

public sealed class UnblockedIconAnimator : MonoBehaviour
{
	[SerializeField]
	private GameObject _blockedIcon;

	[SerializeField]
	private GameObject _blockedDescription;

	[SerializeField]
	private SpriteStates _blockedState;

	[SerializeField]
	private ChainAnimationGroup _unblockedIconAnimation;

	[SerializeField]
	private ChainAnimationGroup _unblockedDescriptionAnimation;

	private Sequence _unblockedIconSequence;

	public void Init()
	{
		_unblockedIconAnimation.Init();
		_unblockedDescriptionAnimation.Init();
	}

	public void Play()
	{
		_unblockedIconSequence?.Kill();
		_unblockedIconSequence = DOTween.Sequence().AppendCallback(SetPreviousState).Append(_unblockedIconAnimation.Play().Join(_unblockedDescriptionAnimation.Play()))
			.AppendCallback(SetCurrentState);
		void SetActive(bool isActive)
		{
			_blockedIcon.SetActive(isActive);
			_blockedDescription.SetActive(isActive);
		}
		void SetCurrentState()
		{
			SetActive(isActive: false);
			_blockedState.Set(0);
		}
		void SetPreviousState()
		{
			SetActive(isActive: true);
			_blockedState.Set(1);
		}
	}

	public void Stop()
	{
		_unblockedIconSequence?.Kill();
		_unblockedIconSequence = null;
		_unblockedIconAnimation.Stop();
		_unblockedDescriptionAnimation.Stop();
	}

	private void OnDisable()
	{
		Stop();
	}
}
