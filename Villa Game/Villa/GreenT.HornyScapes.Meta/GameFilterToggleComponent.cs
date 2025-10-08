using GreenT.HornyScapes.Constants;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Meta;

public class GameFilterToggleComponent : MonoBehaviour
{
	private const string ConstantKey = "game_filter";

	[SerializeField]
	private string _requiredValue;

	[SerializeField]
	private Transform _toggleTransform;

	private IConstants<string> _constants;

	[Inject]
	private void Construct(IConstants<string> constants)
	{
		_constants = constants;
	}

	private void Awake()
	{
		_constants.TryGetValue("game_filter", out var value);
		bool active = false;
		if (!string.IsNullOrEmpty(value))
		{
			active = value.Contains(_requiredValue);
		}
		_toggleTransform.gameObject.SetActive(active);
	}
}
