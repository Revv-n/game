using DG.Tweening;
using GreenT.Utilities;
using StripClub.UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Animations;

public class BezierAnimation : Animation
{
	[SerializeField]
	private Bezier bezier;

	[SerializeField]
	private float duration = 1f;

	[SerializeField]
	private float startAlphaDuration = 0.1f;

	[SerializeField]
	private Ease ease = Ease.InCubic;

	public ImagePool imagePool;

	private Image image;

	public Transform StartPosition { get; set; }

	public Transform TargetPosition { get; set; }

	[Inject]
	public void Init(ImagePool imagePool)
	{
		this.imagePool = imagePool;
	}

	public override Sequence Play()
	{
		base.Play();
		Vector3[] path = bezier.GetPath(StartPosition.position, TargetPosition.position);
		image = imagePool.GetInstance();
		image.transform.localScale = Vector3.one;
		image.transform.position = StartPosition.position;
		image.gameObject.SetActive(value: true);
		Color color = image.color;
		Color color2 = color;
		color2.a = 0f;
		image.color = color2;
		sequence = sequence.Append(image.transform.DOPath(path, duration)).Join(DOTweenModuleUI.DOColor(image, color, startAlphaDuration)).SetEase(ease);
		return sequence;
	}

	private void DisposeResources()
	{
		imagePool.Return(image);
	}

	protected override void Complete()
	{
		DisposeResources();
		base.Complete();
	}

	public override void Stop()
	{
		base.Stop();
		DisposeResources();
	}

	public override void ResetToAnimStart()
	{
		DisposeResources();
	}
}
