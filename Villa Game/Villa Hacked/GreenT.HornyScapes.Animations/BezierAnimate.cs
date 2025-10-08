using System.Collections.Generic;
using DG.Tweening;
using GreenT.Utilities;
using StripClub.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Animations;

public class BezierAnimate : MonoBehaviour
{
	[SerializeField]
	private Bezier bezier;

	[SerializeField]
	private int countImage = 4;

	[SerializeField]
	private float delayBetweenElements;

	[SerializeField]
	private float delay;

	[SerializeField]
	private float duration = 1f;

	[SerializeField]
	private float startAlphaDuration = 0.1f;

	[SerializeField]
	private Ease ease = Ease.InCubic;

	public ImagePool Pool;

	private List<Image> images = new List<Image>();

	public Transform StartPosition { get; set; }

	public Transform TargetPosition { get; set; }

	public Sequence Launch(Transform from, Transform to, int count)
	{
		countImage = count;
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
		AnimationBefore();
		InitializeImageList();
		Sequence sequence = DOTween.Sequence();
		Vector3[] path = bezier.GetPath(StartPosition.position, TargetPosition.position);
		for (int i = 0; i < images.Count; i++)
		{
			Image image = images[i];
			image.transform.position = StartPosition.position;
			image.gameObject.SetActive(value: true);
			float atPosition = delayBetweenElements * (float)i + delay;
			Color color = image.color;
			Color color2 = color;
			color2.a = 0f;
			sequence.AppendInterval(delay);
			sequence.Insert(atPosition, image.transform.DOPath(path, duration));
			images[i].color = color2;
			sequence.Insert(atPosition, DOTweenModuleUI.DOColor(image, color, startAlphaDuration));
		}
		sequence.SetEase(ease);
		DisposeImages(sequence, images);
		sequence.OnStepComplete(delegate
		{
			AnimateTargetOnEnd(TargetPosition);
		});
		return sequence;
	}

	private void InitializeImageList()
	{
		images.Clear();
		for (int i = 0; i < countImage; i++)
		{
			Image instance = Pool.GetInstance();
			instance.transform.localScale = Vector3.one;
			images.Add(instance);
		}
	}

	protected virtual void AnimationBefore()
	{
	}

	protected virtual void AnimateTargetOnEnd(Transform target)
	{
	}

	private void DisposeImages(Sequence sequence, List<Image> list)
	{
		List<Image> newList = new List<Image>(list);
		sequence.AppendCallback(delegate
		{
			foreach (Image item in newList)
			{
				Pool.Return(item);
			}
			newList.Clear();
		});
	}
}
