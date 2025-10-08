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
		source.Select((StoryManager x) => x.OnNew).Merge().Subscribe(AddToTrack)
			.AddTo(onNewStream);
	}

	private void AddToTrack(Story story)
	{
		if (!itemsStreams.ContainsKey(story.ID))
		{
			IDisposable value = story.OnUpdate.Where(IsValid).Subscribe(SendEventByPass);
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
		amplitudeEvent.AddEventParams("story_comleted", $"{tuple.ID}");
		amplitude.AddEvent(amplitudeEvent);
		FreeStream(tuple.ID);
	}
}
