using System;
using GreenT.Net;

namespace GreenT.HornyScapes.NetConnection;

public class NetConnectionRequest
{
	public IObservable<string> Ping()
	{
		return HttpRequestExecutor.GetRequest("https://pandasdeal.com/erolabs_data/time");
	}
}
