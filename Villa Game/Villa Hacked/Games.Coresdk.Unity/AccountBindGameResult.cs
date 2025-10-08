namespace Games.Coresdk.Unity;

public class AccountBindGameResult
{
	public bool IsSuccess { get; private set; }

	public string Result { get; private set; }

	public string Reason { get; private set; }

	public static AccountBindGameResult Parse(string url)
	{
		return new AccountBindGameResult
		{
			IsSuccess = true,
			Result = "0000",
			Reason = ""
		};
	}

	public static AccountBindGameResult Parse(RawResponse rawResponse)
	{
		AccountBindGameResult accountBindGameResult = new AccountBindGameResult();
		if (rawResponse.Exception != null)
		{
			accountBindGameResult.Result = "-1";
			accountBindGameResult.Reason = rawResponse.Exception.Message;
			return accountBindGameResult;
		}
		JSONNode jSONNode = JSON.Parse(rawResponse.Data);
		accountBindGameResult.Result = jSONNode["result"].Value;
		accountBindGameResult.Reason = jSONNode["reason"].Value;
		accountBindGameResult.IsSuccess = accountBindGameResult.Result == "0000";
		return accountBindGameResult;
	}
}
