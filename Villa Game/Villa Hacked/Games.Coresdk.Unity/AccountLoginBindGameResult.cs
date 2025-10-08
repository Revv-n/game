namespace Games.Coresdk.Unity;

public class AccountLoginBindGameResult
{
	public bool IsSuccess { get; private set; }

	public string Result { get; private set; }

	public string Reason { get; private set; }

	public string Token { get; private set; }

	public static AccountLoginBindGameResult Parse(string url)
	{
		return new AccountLoginBindGameResult
		{
			IsSuccess = true,
			Result = "0000",
			Reason = ""
		};
	}

	public static AccountLoginBindGameResult Parse(RawResponse rawResponse)
	{
		AccountLoginBindGameResult accountLoginBindGameResult = new AccountLoginBindGameResult();
		if (rawResponse.Exception != null)
		{
			accountLoginBindGameResult.Result = "-1";
			accountLoginBindGameResult.Reason = rawResponse.Exception.Message;
			return accountLoginBindGameResult;
		}
		JSONNode jSONNode = JSON.Parse(rawResponse.Data);
		accountLoginBindGameResult.Result = jSONNode["result"].Value;
		accountLoginBindGameResult.Reason = jSONNode["reason"].Value;
		accountLoginBindGameResult.Token = jSONNode["jwt"].Value;
		accountLoginBindGameResult.IsSuccess = accountLoginBindGameResult.Result == "0000";
		return accountLoginBindGameResult;
	}
}
