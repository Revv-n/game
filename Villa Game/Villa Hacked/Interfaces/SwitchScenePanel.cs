using UnityEngine;
using UnityEngine.UI;

namespace Interfaces;

public class SwitchScenePanel : MonoBehaviour
{
	private static SwitchScenePanel m_Instance;

	[SerializeField]
	private Slider m_LoadingSlider;

	[SerializeField]
	private Text m_AnimatedLoadingLabel;

	[SerializeField]
	private Text m_VersionText;

	[SerializeField]
	private Text m_BundleStatusText;

	[SerializeField]
	private Text m_BundleQueueCountText;

	[SerializeField]
	private Text m_BundleLoadingText;

	private static SwitchScenePanel instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = Object.FindObjectOfType<SwitchScenePanel>();
			}
			return m_Instance;
		}
	}

	public static void SetSliderValue(float value)
	{
		if (!(instance == null))
		{
			instance.m_LoadingSlider.value = value;
		}
	}

	public static void SetAnimatedLabelText(string text)
	{
		if (!(instance == null) && (bool)instance.m_AnimatedLoadingLabel)
		{
			instance.m_AnimatedLoadingLabel.text = text;
		}
	}

	public static void SetStatusLabelText(string text)
	{
		if (!(instance == null) && (bool)instance.m_BundleStatusText)
		{
			instance.m_BundleStatusText.text = text;
		}
	}

	public static void SetBundleCountLabelText(string text)
	{
		if (!(instance == null) && (bool)instance.m_BundleQueueCountText)
		{
			instance.m_BundleQueueCountText.text = text;
		}
	}

	public static void SetUnitLabelText(string text)
	{
		if (!(instance == null) && (bool)instance.m_BundleLoadingText)
		{
			instance.m_BundleLoadingText.text = text;
		}
	}

	public static void SetVersionText(string text)
	{
		if (!(instance == null) && (bool)instance.m_VersionText)
		{
			instance.m_VersionText.text = text;
		}
	}
}
