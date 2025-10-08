using StripClub.Model.Data;
using UniRx;

namespace StripClub.Model.Cards;

public interface IPromote
{
	IReadOnlyReactiveProperty<int> Level { get; }

	IReadOnlyReactiveProperty<int> Progress { get; }

	int Target { get; }

	IReactiveProperty<bool> IsNew { get; }

	PromoteState State { get; }

	Cost LevelUpCost { get; }

	void AddSoul(int value);

	void LevelUp();

	void Init(int level, int progress);
}
