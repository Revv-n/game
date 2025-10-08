using GreenT.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Monetization;

public class MonetizationPopupFactory<TWindow> : IInitializable where TWindow : Window
{
	protected Canvas canvasParent;

	protected TWindow window;

	private DiContainer diContainer;

	public MonetizationPopupFactory(DiContainer diContainer, Canvas canvasParent, TWindow window)
	{
		this.diContainer = diContainer;
		this.canvasParent = canvasParent;
		this.window = window;
	}

	public void Initialize()
	{
		Create(window);
	}

	public TWindow Create<TWindow>(TWindow windowPrefab) where TWindow : Window
	{
		return diContainer.InstantiatePrefabForComponent<TWindow>((Object)windowPrefab, canvasParent.transform);
	}
}
