using System;
using UniRx;

namespace GreenT.HornyScapes.UI;

public class PromoteCardView : ProgressCardView
{
	protected IDisposable onEndLevelUpStream;

	protected override void OnUpdateLevel(int level)
	{
		base.OnUpdateLevel(level);
		if (base.promote.Progress.Value < base.promote.Target)
		{
			onEndLevelUpStream?.Dispose();
			onEndLevelUpStream = progressBar.OnProgressComplete.First().Subscribe(delegate
			{
				UpdateProgressBar(base.promote.Progress.Value);
			}, delegate(Exception ex)
			{
				ex.LogException();
			});
		}
		else
		{
			UpdateProgressBar(base.promote.Progress.Value);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		onEndLevelUpStream?.Dispose();
	}
}
