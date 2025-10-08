using StripClub.UI;
using StripClub.UI.Collections;
using StripClub.UI.Collections.Promote;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.UI;

public class CollectionCardView : CardLockableView, IView
{
	public class Manager : ViewManager<CollectionCardView>
	{
	}

	public CardNoveltyIndicator NoveltyIndicator;

	[SerializeField]
	private Button openPromote;

	[SerializeField]
	private CardSetter cardSetter;

	public new bool IsActive()
	{
		return base.gameObject.activeInHierarchy;
	}

	private void Awake()
	{
		openPromote.onClick.AddListener(cardSetter.PushCard);
	}

	protected override void OnUpdateLevel(int level)
	{
		base.OnUpdateLevel(level);
		UpdateProgressBar(base.promote.Progress.Value);
	}

	protected override void UpdateProgressBar(int progressValue)
	{
		progressBar.AnimateFromZero(progressValue, base.promote.Target);
		UpdateProgressTMProValue();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		openPromote.onClick.RemoveAllListeners();
	}
}
