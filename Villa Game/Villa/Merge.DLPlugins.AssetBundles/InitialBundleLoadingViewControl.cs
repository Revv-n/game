using System;
using System.Collections.Generic;
using UnityEngine;

namespace Merge.DLPlugins.AssetBundles;

internal class InitialBundleLoadingViewControl
{
	private Action<string> m_Set_Text;

	private Action<float> m_Set_Progress;

	private Action<string> m_Set_NameStatus;

	private Action<string> m_Set_QueueStatus;

	private Dictionary<string, string> cachedText;

	public InitialBundleLoadingViewControl(Action<string> setText, Action<float> setProgress, Action<string> setNameStatus, Action<string> setQueueStatus)
	{
		m_Set_Text = setText;
		m_Set_Progress = setProgress;
		m_Set_NameStatus = setNameStatus;
		m_Set_QueueStatus = setQueueStatus;
	}

	public void OnCachingDetecting(bool isChaching)
	{
		string obj = (isChaching ? GetText("Resources_Loading") : GetText("Resources_Downloading"));
		m_Set_Text?.Invoke(obj);
	}

	public void OnSuccesComplete()
	{
		m_Set_Text?.Invoke(GetText("Resources_Loading"));
	}

	public void OnDownloadingBundles(float progress, float length, string unit)
	{
		float b = 1f;
		progress = Mathf.Lerp(0f, b, progress);
		m_Set_Progress?.Invoke(progress);
	}

	public void OnBundleProgress(int index, int bundle_count, float progress, float file_length, string unit, string name)
	{
		double num = Math.Round(progress, 2) * 100.0;
		m_Set_QueueStatus?.Invoke($"{num}%");
		m_Set_NameStatus?.Invoke(name ?? "");
	}

	public void OnErrorDownloadingBundles()
	{
		m_Set_Text?.Invoke(GetText("Resources_Error"));
	}

	private string GetText(string tag)
	{
		if (cachedText == null)
		{
			cachedText = new Dictionary<string, string>();
		}
		if (cachedText.TryGetValue(tag, out var value))
		{
			return value;
		}
		return cachedText[tag] = tag;
	}
}
