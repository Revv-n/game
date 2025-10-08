using Merge;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Cheats;

public class CheatItemCollectionSystem : MonoBehaviour
{
	public Button OpenPanel;

	public Button ClosePanel;

	public Button RefreshBtn;

	public CheatItemCollectionManagerView panel;

	private void Awake()
	{
		RefreshBtn.onClick.AddListener(Refresh);
		OpenPanel.onClick.AddListener(Show);
		ClosePanel.onClick.AddListener(Hide);
	}

	public void Refresh()
	{
		panel.Refresh();
	}

	public void Show()
	{
		panel.Initialize();
		panel.SetActive(active: true);
	}

	public void Hide()
	{
		panel.SetActive(active: false);
	}

	private void OnDestroy()
	{
		OpenPanel.onClick.RemoveAllListeners();
		ClosePanel.onClick.RemoveAllListeners();
	}
}
