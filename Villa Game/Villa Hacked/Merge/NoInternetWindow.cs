using System;
using Merge.Core.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace Merge;

public class NoInternetWindow : PopupWindow
{
	[SerializeField]
	private Button okButton;

	[SerializeField]
	private Text textButton;

	private void Start()
	{
	}

	public void SetOk()
	{
	}

	public void SetOnRetry(Action callback)
	{
	}
}
