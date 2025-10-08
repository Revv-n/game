using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UniversalAmplitude;

public class AmplitudeDemoController : MonoBehaviour
{
	public Amplitude amplitudeRef;

	public CanvasGroup setupGroup;

	public CanvasGroup demoGroup;

	public CanvasGroup notificationGroup;

	public Text notificationText;

	private Queue<string> notifications = new Queue<string>();

	private IEnumerator Start()
	{
		if (amplitudeRef.APIKey.Length > 8)
		{
			demoGroup.ShowImmediate();
			setupGroup.HideImmediate();
			yield break;
		}
		setupGroup.ShowImmediate();
		while (amplitudeRef.APIKey.Length <= 8)
		{
			yield return true;
		}
		Amplitude.Initialize(Application.version);
		float t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime * 3f;
			setupGroup.UpdateOpacity(shouldShow: false, 4f);
			demoGroup.UpdateOpacity(shouldShow: true, 4f);
			yield return true;
		}
	}

	public void UserIDEvent()
	{
		Amplitude.SetUserID("DemoUser_000");
		notifications.Enqueue("Set UserID for events");
	}

	public void UserLocationEvent()
	{
		AmplitudeEvent amplitudeEvent = new AmplitudeEvent("demo_user_location");
		amplitudeEvent.AddCoreProperty("country", "United States");
		amplitudeEvent.AddCoreProperty("region", "California");
		amplitudeEvent.AddCoreProperty("city", "San Francisco");
		amplitudeEvent.AddCoreProperty("dma", "San Francisco-Oakland-San Jose, CA");
		Amplitude.SendEvent(amplitudeEvent);
		notifications.Enqueue("Setting demo user location");
	}

	public void UserMetadataEvent()
	{
		AmplitudeEvent amplitudeEvent = new AmplitudeEvent("demo_user_metadata");
		string[] value = new string[3] { "chess", "running", "videogames" };
		amplitudeEvent.AddUserProperty("interests", value);
		Amplitude.SendEvent(amplitudeEvent);
		notifications.Enqueue("Setting demo user metadata");
	}

	public void UserIdentifyingEvent()
	{
		AmplitudeEvent amplitudeEvent = new AmplitudeEvent("demo_user_pii");
		amplitudeEvent.AddUserProperty("age", 25);
		amplitudeEvent.AddUserProperty("email", "demouser@games.com");
		Amplitude.SendEvent(amplitudeEvent);
		notifications.Enqueue("Setting demo user Personal Information");
	}

	public void UserLanguageEvent()
	{
		AmplitudeEvent amplitudeEvent = new AmplitudeEvent("demo_user_language");
		amplitudeEvent.AddCoreProperty("language", "French");
		Amplitude.SendEvent(amplitudeEvent);
		notifications.Enqueue("Setting demo user language");
	}

	public void GeneralSimpleEvent()
	{
		AmplitudeEvent amplitudeEvent = new AmplitudeEvent("demo_simple");
		amplitudeEvent.AddEventProperty("number_value", Random.Range(0, 100));
		amplitudeEvent.AddEventProperty("bool_value", true);
		amplitudeEvent.AddEventProperty("string_value", "Event String!");
		Amplitude.SendEvent(amplitudeEvent);
		notifications.Enqueue("Sending Simple Event");
	}

	public void GeneralArrayEvent()
	{
		AmplitudeEvent amplitudeEvent = new AmplitudeEvent("demo_array");
		List<string> list = new List<string> { "apple", "pear", "orange", "grape", "strawberry", "cherry" };
		amplitudeEvent.AddEventProperty("fruit", list);
		amplitudeEvent.AddEventProperty("selection", list[Random.Range(0, list.Count)]);
		Amplitude.SendEvent(amplitudeEvent);
		notifications.Enqueue("Sending Array Event");
	}

	public void GeneralDictionaryEvent()
	{
		AmplitudeEvent amplitudeEvent = new AmplitudeEvent("demo_dictionary");
		amplitudeEvent.AddEventProperty("dict", new Dictionary<string, object>
		{
			{
				"number_val",
				Random.Range(0, 100)
			},
			{ "string_val", "String Value!" }
		});
		amplitudeEvent.AddEventProperty("top_level_bool", true);
		Amplitude.SendEvent(amplitudeEvent);
		notifications.Enqueue("Sending Dictionary Event");
	}

	public void GeneralComplexEvent()
	{
		AmplitudeEvent amplitudeEvent = new AmplitudeEvent("demo_complex");
		amplitudeEvent.AddUserProperty("total_wins", 1);
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("used_cheats", false);
		dictionary.Add("weapons_equipped", new string[3] { "plasma_rifle", "energy_sword", "pulse_blaster" });
		dictionary.Add("first_win", true);
		dictionary.Add("teammates", new List<string> { "AceRanger005", "MarcoPolo", "Itchy99" });
		amplitudeEvent.AddEventProperty("win_conditions", dictionary);
		amplitudeEvent.AddEventProperty("completion_time", Random.Range(800, 900));
		amplitudeEvent.AddEventProperty("mode", "Cooperative");
		Amplitude.SendEvent(amplitudeEvent);
		notifications.Enqueue("Sending Complex Event");
	}

	public void PurchaseSimpleEvent()
	{
		float price = 5.99f;
		int quantity = 1;
		Amplitude.SendEvent(new AmplitudeEvent("demo_product", price, quantity, AmplitudeEvent.RevenueType.Purchase));
		notifications.Enqueue("Sending Simple Purchase Event");
	}

	public void PurchaseMultiEvent()
	{
		float price = 5.99f;
		int quantity = 5;
		Amplitude.SendEvent(new AmplitudeEvent("demo_product", price, quantity, AmplitudeEvent.RevenueType.Purchase));
		notifications.Enqueue("Sending Multi-purchase Event");
	}

	public void PurchaseRefundEvent()
	{
		float price = -5.99f;
		int quantity = 2;
		Amplitude.SendEvent(new AmplitudeEvent("demo_product", price, quantity, AmplitudeEvent.RevenueType.Refund));
		notifications.Enqueue("Sending Refund Event");
	}

	private void Update()
	{
		if (notificationGroup.alpha > 0f)
		{
			notificationGroup.alpha -= Time.deltaTime;
		}
		else if (notifications.Count > 0)
		{
			string text = notifications.Dequeue();
			notificationText.text = text;
			notificationGroup.alpha = 1f;
		}
	}
}
