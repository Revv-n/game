using UnityEngine;

namespace GreenT.HornyScapes.Messenger;

public interface IItemLot
{
	Sprite Icon { get; }

	int TargetCount { get; }

	int GetCurrentCount();

	bool CheckIsEnough();

	void Buy();
}
