using UnityEngine;

namespace Merge.Core.Windows;

[CreateAssetMenu(fileName = "WindowConfig", menuName = "DL/Configs/UI/BaseWindowConfig")]
public class WindowConfig : ScriptableObject
{
	[SerializeField]
	private float showTime = 0.2f;

	[SerializeField]
	private float startScale = 0.3f;

	[SerializeField]
	private bool requersBack = true;

	[SerializeField]
	private bool allowBackClose = true;

	[SerializeField]
	private float backAlpha = 0.7f;

	[SerializeField]
	private float selfStartAlpha = 0.7f;

	public float ShowTime => showTime;

	public bool RequersBack => requersBack;

	public bool AllowBackClose => allowBackClose;

	public float BackAlpha => backAlpha;

	public float StartScale => startScale;

	public float SelfStartAlpha => selfStartAlpha;

	public bool RequersSelfAlpha => SelfStartAlpha < 1f;
}
