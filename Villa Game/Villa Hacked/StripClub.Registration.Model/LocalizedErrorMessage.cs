using StripClub.Model;

namespace StripClub.Registration.Model;

public class LocalizedErrorMessage : IErrorMessage
{
	private string message;

	public LocalizedErrorMessage(string message)
	{
		this.message = message;
	}

	public string GetMessage()
	{
		return message;
	}
}
