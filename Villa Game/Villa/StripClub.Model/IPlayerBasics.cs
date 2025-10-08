using GreenT.HornyScapes;
using StripClub.Extensions;
using UniRx;

namespace StripClub.Model;

public interface IPlayerBasics
{
	Currencies Balance { get; }

	ReactiveProperty<int> Level { get; }

	RestorableValue<int> Energy { get; }

	RestorableValue<int> Replies { get; }

	PlayerExperience Experience { get; }

	void Init();

	ReactiveProperty<int> GetCurrency(CurrencyType type);
}
