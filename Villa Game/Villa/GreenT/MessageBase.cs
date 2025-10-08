namespace GreenT;

public class MessageBase<T>
{
	public object Sender { get; }

	public T Message { get; }

	public MessageBase(object sender, T message)
	{
		Sender = sender;
		Message = message;
	}
}
