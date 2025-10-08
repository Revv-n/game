using DG.Tweening;
using GreenT.HornyScapes.UI;
using UniRx;

namespace GreenT.HornyScapes.Sellouts.Views;

public sealed class SelloutProgressSlider : ProgressSlider
{
	private const float FillDuration = 0.2f;

	private int _target;

	private readonly CompositeDisposable _disposables = new CompositeDisposable();

	public void Initialization(int target)
	{
		_target = target;
		Reset();
	}

	public void SetProgress(int current, float min = 0f, bool immediate = false)
	{
		SetProgress(current, _target, min, immediate);
	}

	public override void Init(float relativeProgress)
	{
		DOVirtual.Float(base.Slider.value, Clamp(relativeProgress), 0.2f, delegate(float x)
		{
			base.Slider.value = x;
		});
	}

	public void Reset()
	{
		base.Slider.value = 0f;
	}

	private void OnDestroy()
	{
		_disposables.Dispose();
	}

	private void Fill(float relativeProgress, bool immediate)
	{
		if (immediate)
		{
			base.Init(relativeProgress);
		}
		else
		{
			Init(relativeProgress);
		}
	}

	private void SetProgress(float value, float max, float min = 0f, bool immediate = false)
	{
		if (!(max < 0f))
		{
			if (min > max)
			{
				Fill(1f, immediate);
			}
			else if (max == 0f)
			{
				Fill(1f, immediate);
			}
			else
			{
				Fill((value - min) / (max - min), immediate);
			}
		}
	}
}
