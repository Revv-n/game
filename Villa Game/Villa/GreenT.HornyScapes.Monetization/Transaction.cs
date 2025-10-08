namespace GreenT.HornyScapes.Monetization;

public class Transaction
{
	public int monetizationID;

	public string Id;

	public bool IsValidated;

	public int StatusCode = -1;

	public Transaction()
	{
	}

	public Transaction(int monetizationID, bool isValidated, string id = "")
	{
		this.monetizationID = monetizationID;
		IsValidated = isValidated;
		Id = id;
	}
}
