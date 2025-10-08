using UniRx;
using UnityEngine;

namespace GreenT;

public class GameControllerState : MonoBehaviour
{
	private readonly ReactiveProperty<bool> isGameDataLoaded = new ReactiveProperty<bool>(false);

	public IReadOnlyReactiveProperty<bool> IsGameDataLoaded => (IReadOnlyReactiveProperty<bool>)(object)isGameDataLoaded;

	public void SetState(bool state)
	{
		isGameDataLoaded.Value = state;
	}
}
