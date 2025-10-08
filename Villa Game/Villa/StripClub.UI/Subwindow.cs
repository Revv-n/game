using System;
using GreenT.UI;
using UnityEngine;

namespace StripClub.UI;

public class Subwindow : Window
{
	[SerializeField]
	protected Window parentWindow;

	protected override void Awake()
	{
		base.Awake();
		parentWindow.OnChangeState += OnParentSwitchActivity;
	}

	private void OnParentSwitchActivity(object sender, EventArgs e)
	{
		if (e is WindowArgs { Active: false })
		{
			Close();
		}
	}

	public override void Open()
	{
		if (!parentWindow.IsOpened)
		{
			parentWindow.Open();
		}
		base.Open();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (parentWindow != null)
		{
			parentWindow.OnChangeState -= OnParentSwitchActivity;
		}
	}
}
