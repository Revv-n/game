using GreenT.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Monetization;

public class UIFactory : MonoBehaviour
{
	[SerializeField]
	protected Canvas canvasParent;

	private DiContainer diContainer;

	[Inject]
	private void InnerInit(DiContainer diContainer)
	{
		this.diContainer = diContainer;
	}

	public TWindow Create<TWindow>(TWindow windowPrefab) where TWindow : Window
	{
		return diContainer.InstantiatePrefabForComponent<TWindow>((Object)windowPrefab, canvasParent.transform);
	}

	public TWindow Create<TWindow>(Canvas canvas, TWindow windowPrefab) where TWindow : Window
	{
		return diContainer.InstantiatePrefabForComponent<TWindow>((Object)windowPrefab, canvasParent.transform);
	}
}
