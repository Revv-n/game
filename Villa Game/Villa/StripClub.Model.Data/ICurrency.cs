using UniRx;

namespace StripClub.Model.Data;

public interface ICurrency<T> where T : struct
{
	ReactiveProperty<T> Get();
}
