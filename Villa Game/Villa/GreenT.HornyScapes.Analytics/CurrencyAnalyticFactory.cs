using System.Collections.Generic;
using GreenT.HornyScapes.Presents.Analytics;
using GreenT.Types;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.Analytics;

public class CurrencyAnalyticFactory : IFactory<CurrencyType, int, string, CompositeIdentificator, ContentType, AmplitudeEvent>, IFactory
{
	private const string EVENT_NAME = "currency";

	private const string SPENT_KEY = "spent";

	private const string RECEIVED_KEY = "received";

	private const string MINIEVENT_NAME = "currency_miniEvent";

	private const string MINIEVENT_SPENT_KEY = "spend";

	private const string MINIEVENT_RECEIVED_KEY = "received";

	private Dictionary<CurrencyType, string> typeKeyDict = new Dictionary<CurrencyType, string>
	{
		{
			CurrencyType.Soft,
			"soft"
		},
		{
			CurrencyType.Hard,
			"hard"
		},
		{
			CurrencyType.Event,
			"event"
		},
		{
			CurrencyType.EventXP,
			"event_xp"
		},
		{
			CurrencyType.BP,
			"bp"
		},
		{
			CurrencyType.MiniEvent,
			"miniEvent"
		},
		{
			CurrencyType.Energy,
			"energy"
		},
		{
			CurrencyType.EventEnergy,
			"event_energy"
		},
		{
			CurrencyType.Contracts,
			"contracts"
		},
		{
			CurrencyType.Present1,
			"present_1"
		},
		{
			CurrencyType.Present2,
			"present_2"
		},
		{
			CurrencyType.Present3,
			"present_3"
		},
		{
			CurrencyType.Present4,
			"present_4"
		}
	};

	public AmplitudeEvent Create(CurrencyType type, int diff, string source, CompositeIdentificator compositeIdentificator, ContentType contentType)
	{
		if (!typeKeyDict.ContainsKey(type))
		{
			return null;
		}
		if (type == CurrencyType.MiniEvent)
		{
			string text = ((diff > 0) ? "received" : "spend");
			return new MiniEventCurrencyAmplitudeEvent(string.Format("currency_miniEvent_" + text), diff, source, compositeIdentificator[0], contentType);
		}
		if (IsPresent(type))
		{
			return new PresentReceivedAnalyticEvent(typeKeyDict[type], diff, source);
		}
		string text2 = ((diff > 0) ? "received" : "spent");
		return new CurrencyAmplitudeEvent(string.Format("currency_" + typeKeyDict[type] + "_" + text2), typeKeyDict[type], diff, source, contentType, compositeIdentificator);
	}

	private bool IsPresent(CurrencyType type)
	{
		if (type != CurrencyType.Present1 && type != CurrencyType.Present2 && type != CurrencyType.Present3)
		{
			return type == CurrencyType.Present4;
		}
		return true;
	}
}
