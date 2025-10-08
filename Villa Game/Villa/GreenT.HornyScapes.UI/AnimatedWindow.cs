using GreenT.HornyScapes.Animations;
using GreenT.UI;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.UI;

[RequireComponent(typeof(DefaultWindowAnimController))]
public class AnimatedWindow : Window
{
	[SerializeField]
	private DefaultWindowAnimController animationController;

	private CompositeDisposable closeAnimationStream = new CompositeDisposable();

	private bool isAnimating;

	public override void Init(IWindowsManager windowsOpener)
	{
		base.Init(windowsOpener);
		animationController.Init();
	}

	public override void Open()
	{
		base.Open();
		if (!isAnimating)
		{
			isAnimating = true;
			animationController.PlayShowAnimation().DoOnCancel(delegate
			{
				isAnimating = false;
			}).Subscribe(delegate
			{
				isAnimating = false;
			})
				.AddTo(closeAnimationStream);
		}
	}

	public override void Close()
	{
		if (!isAnimating)
		{
			isAnimating = true;
			animationController.PlayCloseAnimation().DoOnCancel(delegate
			{
				Close();
			}).Subscribe(delegate
			{
				Close();
			})
				.AddTo(closeAnimationStream);
		}
		void Close()
		{
			base.Close();
			isAnimating = false;
		}
	}

	protected virtual void OnDisable()
	{
		closeAnimationStream?.Dispose();
	}

	private new void OnValidate()
	{
		if (animationController == null)
		{
			animationController = GetComponent<DefaultWindowAnimController>();
		}
	}
}
