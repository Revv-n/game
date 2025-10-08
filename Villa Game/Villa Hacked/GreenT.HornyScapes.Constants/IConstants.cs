namespace GreenT.HornyScapes.Constants;

public interface IConstants<T>
{
	T this[string key] { get; }

	bool TryGetValue(string key, out T value);
}
