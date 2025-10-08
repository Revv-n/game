using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.DebugInfo;

public class DebugInfoContainer : MonoBehaviour
{
	[SerializeField]
	private RectTransform _root;

	private DebugInfoView _view;

	private DebugInfoViewFactory _factory;

	[Inject]
	public void Init(DebugInfoViewFactory factory)
	{
		_factory = factory;
	}

	private void OnValidate()
	{
		if ((object)_root == null)
		{
			_root = GetComponent<RectTransform>();
		}
	}

	public void Show(string info)
	{
	}
}
