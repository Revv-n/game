using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Stories;
using GreenT.Types;
using UniRx;

namespace GreenT.HornyScapes.Analytics;

public class StoryAnalytic : BaseEntityAnalytic<Story>
{
	private const string ANALYTIC_EVENT = "story_comleted";

	private StoryCluster storyManagerCluster;

	public StoryAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude, StoryCluster storyManagerCluster)
		: base(amplitude)
	{
		this.storyManagerCluster = storyManagerCluster;
	}

	public override void Track()
	{
		ClearStreams();
		IEnumerable<StoryManager> source = storyManagerCluster.Select((System.Collections.Generic.KeyValuePair<ContentType, StoryManager> x) => x.Value);
		foreach (Story item in from _story in source.SelectMany((StoryManager x) => x.Collection)
			where !_story.IsComplete
			select _story)
		{
			AddToTrack(item);
		}
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Story>(Observable.Merge<Story>(source.Select((StoryManager x) => x.OnNew)), (Action<Story>)AddToTrack), (ICollection<IDisposable>)onNewStream);
	}

	private void AddToTrack(Story story)
	{
		if (!itemsStreams.ContainsKey(story.ID))
		{
			IDisposable value = ObservableExtensions.Subscribe<Story>(Observable.Where<Story>(story.OnUpdate, (Func<Story, bool>)IsValid), (Action<Story>)SendEventByPass);
			itemsStreams.Add(story.ID, value);
		}
	}

	protected override bool IsValid(Story entity)
	{
		return entity.IsComplete;
	}

	public override void SendEventByPass(Story tuple)
	{
		AmplitudeEvent amplitudeEvent = new AmplitudeEvent("story_comleted");
		((AnalyticsEvent)amplitudeEvent).AddEventParams("story_comleted", (object)$"{tuple.ID}");
		((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent(amplitudeEvent);
		FreeStream(tuple.ID);
	}
}
