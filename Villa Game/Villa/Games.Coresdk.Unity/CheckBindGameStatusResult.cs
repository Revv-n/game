namespace Games.Coresdk.Unity;

public class CheckBindGameStatusResult
{
	public bool IsSuccess { get; private set; }

	public string Result { get; private set; }

	public bool IsBound { get; private set; }

	public static CheckBindGameStatusResult Parse(RawResponse rawResponse)
	{
		CheckBindGameStatusResult checkBindGameStatusResult = new CheckBindGameStatusResult();
		if (rawResponse.Exception != null)
		{
			checkBindGameStatusResult.Result = "-1";
			return checkBindGameStatusResult;
		}
		JSONNode jSONNode = JSON.Parse(rawResponse.Data);
		checkBindGameStatusResult.Result = jSONNode["result"].Value;
		checkBindGameStatusResult.IsBound = jSONNode["is_bound"].AsBool;
		checkBindGameStatusResult.IsSuccess = checkBindGameStatusResult.Result == "0000";
		return checkBindGameStatusResult;
	}
}
