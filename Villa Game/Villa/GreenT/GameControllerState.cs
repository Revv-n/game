using UniRx;
using UnityEngine;

namespace GreenT;

public class GameControllerState : MonoBehaviour
{
	private readonly ReactiveProperty<bool> isGameDataLoaded = new ReactiveProperty<bool>(initialValue: false);

	public IReadOnlyReactiveProperty<bool> IsGameDataLoaded => isGameDataLoaded;

	public void SetState(bool state)
	{
		isGameDataLoaded.Value = state;
	}
}
