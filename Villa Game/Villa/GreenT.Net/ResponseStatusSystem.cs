using System.Collections.Generic;

namespace GreenT.Net;

public class ResponseStatusSystem
{
	private Dictionary<int, string> statusDist = new Dictionary<int, string> { { 200, "<color=green>Succeed</color>" } };

	public string GetMessage(Response response)
	{
		string value = string.Empty;
		if (!statusDist.TryGetValue(response.Status, out value))
		{
			value = "<color=red>" + response.Error + "</color>";
		}
		return $"status = {response.Status}: " + value;
	}

	public void SendLog(Response response, LogType logType = LogType.Analytic)
	{
	}
}
