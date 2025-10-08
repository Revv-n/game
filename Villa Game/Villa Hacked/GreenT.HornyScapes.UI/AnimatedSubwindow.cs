using System;
using GreenT.UI;
using StripClub.UI;

namespace GreenT.HornyScapes.UI;

public class AnimatedSubwindow : Subwindow
{
	private void OnClose(object sender, EventArgs e)
	{
		if (e is WindowArgs && !parentWindow.IsOpened)
		{
			parentWindow.OnChangeState -= OnClose;
			base.Close();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		parentWindow.OnChangeState -= OnClose;
	}
}
