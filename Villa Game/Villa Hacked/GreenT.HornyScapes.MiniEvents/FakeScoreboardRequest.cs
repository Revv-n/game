using System;
using GreenT.Net;
using Newtonsoft.Json;
using UniRx;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class FakeScoreboardRequest : HTTPGetRequest<Response<Leaderboard>>
{
	public IObservable<Response<Leaderboard>> GetRequest(params string[] args)
	{
		return Observable.Return<Response<Leaderboard>>(JsonConvert.DeserializeObject<Response<Leaderboard>>("{\"status\":0,\"error\":\"\",\"data\":{\"Leaderboard\":[{\"Nickname\":\"Alfonso Trahuilo\",\"Score\":150000},{\"Nickname\":\"Lopata\",\"Score\":140000},{\"Nickname\":\"Sobaka\",\"Score\":130000},{\"Nickname\":\"Diryavii Joe\",\"Score\":120000},{\"Nickname\":\"Pes\",\"Score\":110000},{\"Nickname\":\"Martishka\",\"Score\":100000},{\"Nickname\":\"Goose\",\"Score\":90000},{\"Nickname\":\"Stalin\",\"Score\":80000},{\"Nickname\":\"Matryona\",\"Score\":70000},{\"Nickname\":\"Petuh\",\"Score\":75000},{\"Nickname\":\"Shabka\",\"Score\":70000},{\"Nickname\":\"Pikaa\",\"Score\":80000},{\"Nickname\":\"Trash\",\"Score\":80000},{\"Nickname\":\"Mraz\",\"Score\":80000},{\"Nickname\":\"Dyatel\",\"Score\":800},{\"Nickname\":\"Bug\",\"Score\":800},{\"Nickname\":\"Lox\",\"Score\":800},{\"Nickname\":\"Vatrushka\",\"Score\":800},{\"Nickname\":\"Kiss\",\"Score\":800},{\"Nickname\":\"Freddy\",\"Score\":800},{\"Nickname\":\"Socks\",\"Score\":800},{\"Nickname\":\"Slope\",\"Score\":800},{\"Nickname\":\"Mac Traher\",\"Score\":800},{\"Nickname\":\"Alfonso Trahuilo\",\"Score\":800},{\"Nickname\":\"Hui\",\"Score\":800},{\"Nickname\":\"Hui12\",\"Score\":800},{\"Nickname\":\"ubica228\",\"Score\":800},{\"Nickname\":\"Lave\",\"Score\":800},{\"Nickname\":\"MoneyTree\",\"Score\":800}],\"PlayerInfo\":{\"Nickname\":\"Boyarin\",\"Place\":255,\"Score\":1500}}}")).Debug("FAKE SCOREBOARD RESPONSE", LogType.Net);
	}
}
