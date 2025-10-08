using System.Collections.Generic;
using DG.Tweening;
using GreenT.Types;
using GreenT.Utilities;
using StripClub.Model;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventsBezierAnimator : MonoBehaviour
{
	[SerializeField]
	private Bezier _bezier;

	[SerializeField]
	private int _countImage = 4;

	[SerializeField]
	private float _delayBetweenElements;

	[SerializeField]
	private float _delay;

	[SerializeField]
	private float _duration = 1f;

	[SerializeField]
	private float _startAlphaDuration = 0.1f;

	[SerializeField]
	private Ease _ease = Ease.InCubic;

	private List<MiniEventFlyingCurrencyView> _images = new List<MiniEventFlyingCurrencyView>();

	private CurrencyType _currencyType;

	private CompositeIdentificator _currencyIdentificator;

	public CurrencyFlyPool Pool;

	public Transform StartPosition { get; set; }

	public Transform TargetPosition { get; set; }

	public Sequence Launch(Transform from, Transform to, CurrencyType currencyType, CompositeIdentificator currencyIdentificator, int count = 1)
	{
		_countImage = count;
		_currencyType = currencyType;
		_currencyIdentificator = currencyIdentificator;
		return Launch(from, to);
	}

	public Sequence Launch(Transform from, Transform to, int count)
	{
		_countImage = count;
		return Launch(from, to);
	}

	public Sequence Launch(Transform from, Transform to)
	{
		StartPosition = from;
		TargetPosition = to;
		return Launch();
	}

	public Sequence Launch()
	{
		InitializeImageList();
		Sequence sequence = DOTween.Sequence();
		Vector3[] path = _bezier.GetPath(StartPosition.position, TargetPosition.position);
		for (int i = 0; i < _images.Count; i++)
		{
			Image image = _images[i].Image;
			image.transform.position = StartPosition.position;
			image.gameObject.SetActive(value: true);
			float atPosition = _delayBetweenElements * (float)i + _delay;
			Color color = image.color;
			Color color2 = color;
			color2.a = 0f;
			sequence.AppendInterval(_delay);
			sequence.Insert(atPosition, image.transform.DOPath(path, _duration));
			_images[i].Image.color = color2;
			sequence.Insert(atPosition, image.DOColor(color, _startAlphaDuration));
		}
		sequence.SetEase(_ease);
		DisposeImages(sequence, _images);
		return sequence;
	}

	private void InitializeImageList()
	{
		_images.Clear();
		for (int i = 0; i < _countImage; i++)
		{
			MiniEventFlyingCurrencyView instance = Pool.GetInstance(_currencyType, _currencyIdentificator);
			instance.transform.localScale = Vector3.one;
			_images.Add(instance);
		}
	}

	private void DisposeImages(Sequence sequence, List<MiniEventFlyingCurrencyView> list)
	{
		List<MiniEventFlyingCurrencyView> newList = new List<MiniEventFlyingCurrencyView>(list);
		sequence.AppendCallback(delegate
		{
			foreach (MiniEventFlyingCurrencyView item in newList)
			{
				Pool.Return(item);
			}
			newList.Clear();
		});
	}
}
