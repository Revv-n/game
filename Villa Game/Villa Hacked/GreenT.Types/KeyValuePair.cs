using System.Text;

namespace GreenT.Types;

public class KeyValuePair<TKey, TValue>
{
	private TKey key;

	private TValue value;

	public TKey Key => key;

	public TValue Value => value;

	public KeyValuePair(TKey key, TValue value)
	{
		this.key = key;
		this.value = value;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append('[');
		if (Key != null)
		{
			stringBuilder.Append(Key);
		}
		stringBuilder.Append(", ");
		if (Value != null)
		{
			stringBuilder.Append(Value);
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}
}
