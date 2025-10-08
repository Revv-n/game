using System.Collections.Generic;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.UI;
using GreenT.Types;
using StripClub.Stories;
using StripClub.UI.Story;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Stories.UI;

public class StoryViewInstaller : MonoInstaller
{
	public class StoryContentSelector : IContentSelector, ISelector<ContentType>
	{
		private readonly StoryCluster managerCluster;

		private readonly Dictionary<ContentType, ScreenIndicator> screenIndicators;

		private readonly StoryController storyController;

		public StoryContentSelector(StoryCluster managerCluster, StoryController storyController, MainScreenIndicator mainScreenIndicator, EventMergeScreenIndicator eventScreenIndicator)
		{
			this.managerCluster = managerCluster;
			this.storyController = storyController;
			screenIndicators = new Dictionary<ContentType, ScreenIndicator>();
			screenIndicators[ContentType.Main] = mainScreenIndicator;
			screenIndicators[ContentType.Event] = eventScreenIndicator;
		}

		public void Initialize()
		{
			Select(ContentType.Main);
		}

		public void Select(ContentType type)
		{
			StoryManager source = managerCluster[type];
			ScreenIndicator indicator = screenIndicators[type];
			storyController.Set(source, indicator);
		}
	}

	[SerializeField]
	private PhraseView _phraseView;

	[SerializeField]
	private SpeakersView speakersView;

	[SerializeField]
	private StoryWindow _storyWindow;

	[SerializeField]
	private OverView _overView;

	public override void InstallBindings()
	{
		base.Container.Bind<PhraseView>().FromInstance(_phraseView);
		base.Container.Bind<SpeakersView>().FromInstance(speakersView);
		base.Container.Bind<StoryWindow>().FromInstance(_storyWindow);
		base.Container.Bind<OverView>().FromInstance(_overView);
		base.Container.BindInterfacesAndSelfTo<StoryViewController>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StoryContentSelector>().AsSingle().OnInstantiated<StoryContentSelector>(ContentSelectorInstaller.AddSelectorToContainer)
			.NonLazy();
	}
}
