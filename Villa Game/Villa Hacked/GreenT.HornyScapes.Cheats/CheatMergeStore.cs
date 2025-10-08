using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.MergeStore;
using GreenT.Types;
using StripClub.NewEvent.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Cheats;

public class CheatMergeStore : MonoBehaviour
{
	private ItemFactory _itemFactory;

	private EventProvider _eventProvider;

	private ContentSelectorGroup _contentSelectorGroup;

	[SerializeField]
	private EnumDropdownSelector<SectionType> _sectionSelector;

	[SerializeField]
	private TMP_Text _contentType;

	[SerializeField]
	private TMP_InputField _count;

	[SerializeField]
	private Button _button;

	private ContentType _lastContentType;

	private DataManagerCluster _dataManagerCluster;

	private SectionFactory _sectionFactory;

	[Inject]
	private void Construct(ItemFactory itemFactory, ContentSelectorGroup contentSelectorGroup, EventProvider eventProvider, DataManagerCluster dataManagerCluster, SectionFactory sectionFactory)
	{
		_sectionFactory = sectionFactory;
		_dataManagerCluster = dataManagerCluster;
		_eventProvider = eventProvider;
		_contentSelectorGroup = contentSelectorGroup;
		_itemFactory = itemFactory;
	}

	private void Awake()
	{
		_button.onClick.AddListener(OnClick);
	}

	private void FixedUpdate()
	{
		if (_lastContentType != _contentSelectorGroup.Current)
		{
			UpdateText();
		}
	}

	private void UpdateText()
	{
		_lastContentType = _contentSelectorGroup.Current;
		_contentType.text = _contentSelectorGroup.Current.ToString();
	}

	private void OnEnable()
	{
		UpdateText();
	}

	private void OnClick()
	{
	}

	private async void CollectAndSaveStats(StoreSection section, string bundle, int iterations)
	{
	}
}
