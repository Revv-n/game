using UniRx;

namespace StripClub.Extensions;

public abstract class ReactivePropertyDecorator<T> : ReactiveProperty<T>
{
	protected ReactiveProperty<T> Property;

	public ReactiveProperty<T> GetProperty => Property;

	public ReactivePropertyDecorator(ReactiveProperty<T> property)
	{
		Property = property;
	}

	protected override void SetValue(T value)
	{
		if (Property != null)
		{
			base.SetValue(value);
		}
	}
}
