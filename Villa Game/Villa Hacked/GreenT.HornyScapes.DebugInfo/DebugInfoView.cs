using TMPro;
using UnityEngine;

namespace GreenT.HornyScapes.DebugInfo;

public class DebugInfoView : MonoBehaviour
{
	[SerializeField]
	private TMP_Text _debugInfo;

	[SerializeField]
	private GameObject _root;

	[SerializeField]
	private GameObject _info;

	private bool _initialized;

	private void Awake()
	{
		if (!_initialized)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void Set(string info)
	{
	}
}
