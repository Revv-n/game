using System;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StripClub.Stories;

public class OverView : MonoBehaviour
{
	[SerializeField]
	private SpeakerView[] _leftSpeakers;

	[SerializeField]
	private SpeakerView[] _rightSpeakers;

	[SerializeField]
	private GameObject _leftContainer;

	[SerializeField]
	private GameObject _rightContainer;

	[SerializeField]
	private GameObject _leftPhraseContainer;

	[SerializeField]
	private GameObject _rightPhraseContainer;

	[SerializeField]
	private Transform _currentSpeakerContainerParent;

	[SerializeField]
	private float _animationDuration = 0.5f;

	private Transform _parentLeft;

	private Transform _parentRight;

	private GameObject _currentContainer;

	private Vector3 _startRight;

	private Vector3 _startLeft;

	private const float HIDEPOWER = 600f;

	private const int ZERO_ALPHA = 0;

	private const int ONE_ALPHA = 1;

	private void Awake()
	{
		_parentLeft = _leftContainer.transform.parent;
		_parentRight = _rightContainer.transform.parent;
		_startLeft = _leftContainer.transform.localPosition;
		_startRight = _rightContainer.transform.localPosition;
	}

	public void StartView()
	{
		ShowFromSide(_leftContainer, _startLeft + Vector3.left * 600f, _startLeft, _animationDuration);
		ShowFromSide(_rightContainer, _startRight + Vector3.right * 600f, _startRight, _animationDuration);
	}

	public Sequence EndView()
	{
		ShowFromSide(_leftContainer, _startLeft, _startLeft + Vector3.left * 600f, _animationDuration);
		ShowFromSide(_rightContainer, _startRight, _startRight + Vector3.right * 600f, _animationDuration);
		ChangeAlphasTo(_leftContainer, 0f, _animationDuration);
		ChangeAlphasTo(_rightContainer, 0f, _animationDuration);
		return DOTween.Sequence().Append(base.transform.DOScale(base.transform.localScale, _animationDuration));
	}

	public void ApplyView(int characterData, bool isFirstApply = false)
	{
		if (characterData == 0)
		{
			return;
		}
		if (_leftSpeakers.Any((SpeakerView s) => s.CharacterID == characterData))
		{
			SetLeftContainer();
			if (isFirstApply)
			{
				DisableContainer(_rightPhraseContainer);
			}
			else
			{
				DisableContainerWithAnimation(_rightPhraseContainer);
			}
		}
		else
		{
			SetRightContainer();
			if (isFirstApply)
			{
				DisableContainer(_leftPhraseContainer);
			}
			else
			{
				DisableContainerWithAnimation(_leftPhraseContainer);
			}
		}
	}

	public void SetActive(bool isActive)
	{
		if (_currentContainer != null)
		{
			_currentContainer.SetActive(isActive);
		}
	}

	private void SetRightContainer()
	{
		_rightContainer.transform.SetParent(_currentSpeakerContainerParent);
		_leftContainer.transform.SetParent(_parentLeft);
		if (_rightPhraseContainer != null)
		{
			_rightPhraseContainer.SetActive(value: true);
			ChangeAlphasTo(_rightPhraseContainer, 1f, _animationDuration);
		}
		_currentContainer = _rightContainer;
	}

	private void SetLeftContainer()
	{
		_leftContainer.transform.SetParent(_currentSpeakerContainerParent);
		_rightContainer.transform.SetParent(_parentRight);
		if (_leftPhraseContainer != null)
		{
			_leftPhraseContainer.SetActive(value: true);
			ChangeAlphasTo(_leftPhraseContainer, 1f, _animationDuration);
		}
		_currentContainer = _leftContainer;
	}

	private void DisableContainer(GameObject container)
	{
		if (!(container == null))
		{
			container.SetActive(value: false);
		}
	}

	private void DisableContainerWithAnimation(GameObject container)
	{
		if (!(container == null))
		{
			TweenerCore<Color, Color, ColorOptions> tweenerCore = ChangeAlphasTo(container, 0f, _animationDuration);
			tweenerCore.onComplete = (TweenCallback)Delegate.Combine(tweenerCore.onComplete, (TweenCallback)delegate
			{
				DisableContainer(container);
			});
		}
	}

	private void ShowFromSide(GameObject container, Vector3 from, Vector3 to, float animationDuration)
	{
		if (!(container == null))
		{
			container.transform.localPosition = from;
			container.transform.DOLocalMove(to, animationDuration);
		}
	}

	private TweenerCore<Color, Color, ColorOptions> ChangeAlphasTo(GameObject target, float alpha, float animationDuration)
	{
		TweenerCore<Color, Color, ColorOptions> result = null;
		Image[] componentsInChildren = target.GetComponentsInChildren<Image>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			result = componentsInChildren[i].DOFade(alpha, animationDuration);
		}
		TextMeshProUGUI[] componentsInChildren2 = target.GetComponentsInChildren<TextMeshProUGUI>();
		for (int i = 0; i < componentsInChildren2.Length; i++)
		{
			componentsInChildren2[i].DOFade(alpha, animationDuration);
		}
		return result;
	}

	private TweenerCore<Color, Color, ColorOptions> ChangeAlphasToFrom(GameObject target, float alpha, float from, float animationDuration)
	{
		TweenerCore<Color, Color, ColorOptions> result = null;
		Image[] componentsInChildren = target.GetComponentsInChildren<Image>();
		foreach (Image image in componentsInChildren)
		{
			image.color = new Color(image.color.r, image.color.g, image.color.b, from);
			result = image.DOFade(alpha, animationDuration);
		}
		TextMeshProUGUI[] componentsInChildren2 = target.GetComponentsInChildren<TextMeshProUGUI>();
		foreach (TextMeshProUGUI textMeshProUGUI in componentsInChildren2)
		{
			textMeshProUGUI.color = new Color(textMeshProUGUI.color.r, textMeshProUGUI.color.g, textMeshProUGUI.color.b, from);
			textMeshProUGUI.DOFade(alpha, animationDuration);
		}
		return result;
	}
}
