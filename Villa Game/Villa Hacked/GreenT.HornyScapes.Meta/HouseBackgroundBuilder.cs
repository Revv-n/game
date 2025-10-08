using System;
using System.Collections.Generic;
using System.Linq;
using StripClub.Model.Data;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Meta;

public class HouseBackgroundBuilder : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer tilePrefab;

	[SerializeField]
	private int columnsCount;

	[SerializeField]
	private int rowsCount;

	private Vector2 cellSize;

	private Bounds houseBounds;

	private Subject<Bounds> onBoundsChanged = new Subject<Bounds>();

	private BackgroundSpritesCollection backgroundSpritesManager;

	private ILoader<IEnumerable<Sprite>> backgroundLoader;

	private ILoader<IEnumerable<Sprite>> backgroundPreloader;

	private GameStarter gameStarter;

	private List<SpriteRenderer> tiles = new List<SpriteRenderer>();

	private Dictionary<int, SpriteRenderer> tileLinks = new Dictionary<int, SpriteRenderer>();

	public IObservable<Bounds> HouseBounds => (IObservable<Bounds>)ReactivePropertyExtensions.ToReadOnlyReactiveProperty<Bounds>((IObservable<Bounds>)onBoundsChanged, houseBounds);

	[Inject]
	public void Init(BackgroundSpritesCollection backgroundSpritesManager, ILoader<IEnumerable<Sprite>> backgroundLoader, [Inject(Id = "Preload")] ILoader<IEnumerable<Sprite>> backgroundPreloader, GameStarter gameStarter)
	{
		this.backgroundSpritesManager = backgroundSpritesManager;
		this.backgroundLoader = backgroundLoader;
		this.backgroundPreloader = backgroundPreloader;
		this.gameStarter = gameStarter;
	}

	private void Awake()
	{
		GameStarterSubscribe();
		Prebuild();
		ObserveBackgroundExtension();
	}

	private void GameStarterSubscribe()
	{
		if (gameStarter.IsDataLoaded.Value)
		{
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<long>(Observable.Timer(TimeSpan.FromSeconds(20.0)), (Action<long>)delegate
			{
				ReloadTiles();
			}), (Component)this);
			return;
		}
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>((IObservable<bool>)gameStarter.IsDataLoaded, (Action<bool>)delegate(bool state)
		{
			if (state)
			{
				DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<long>(Observable.Timer(TimeSpan.FromSeconds(20.0)), (Action<long>)delegate
				{
					ReloadTiles();
				}), (Component)this);
			}
		}), (Component)this);
	}

	private void ObserveBackgroundExtension()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<DictionaryAddEvent<int, Sprite>>(((ReactiveDictionary<int, Sprite>)backgroundSpritesManager).ObserveAdd(), (Action<DictionaryAddEvent<int, Sprite>>)delegate(DictionaryAddEvent<int, Sprite> _pair)
		{
			SpriteRenderer spriteRenderer = CreateTile(_pair.Key, _pair.Value);
			tiles.Add(spriteRenderer);
			tileLinks.Add(_pair.Key, spriteRenderer);
			houseBounds.Encapsulate(spriteRenderer.bounds);
			onBoundsChanged.OnNext(houseBounds);
		}), (Component)this);
	}

	private void Prebuild()
	{
		cellSize = ((IEnumerable<KeyValuePair<int, Sprite>>)backgroundSpritesManager).First().Value.bounds.size;
		foreach (KeyValuePair<int, Sprite> item in (ReactiveDictionary<int, Sprite>)backgroundSpritesManager)
		{
			SpriteRenderer spriteRenderer = CreateTile(item.Key, item.Value);
			tiles.Add(spriteRenderer);
			tileLinks.Add(item.Key, spriteRenderer);
			houseBounds.Encapsulate(spriteRenderer.bounds);
		}
		onBoundsChanged.OnNext(houseBounds);
	}

	private void Build()
	{
		DestroyPreviousTiles();
		int num = columnsCount * rowsCount;
		float x = base.transform.position[0];
		float y = base.transform.position[1];
		int num2 = 0;
		while (num2 != num && num2 != ((ReactiveDictionary<int, Sprite>)backgroundSpritesManager).Count)
		{
			SpriteRenderer spriteRenderer = CreateTile(((ReactiveDictionary<int, Sprite>)backgroundSpritesManager)[num2], new Vector2(x, y));
			tiles.Add(spriteRenderer);
			houseBounds.Encapsulate(spriteRenderer.bounds);
			onBoundsChanged.OnNext(houseBounds);
			x = spriteRenderer.bounds.max.x;
			num2++;
			if (num2 % columnsCount == 0)
			{
				x = base.transform.position[0];
				y = spriteRenderer.bounds.min.y;
			}
		}
	}

	[ContextMenu("ReloadTiles")]
	private void ReloadTiles()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<IEnumerable<Sprite>>(Observable.DoOnCompleted<IEnumerable<Sprite>>(Observable.Do<IEnumerable<Sprite>>(Observable.ContinueWith<IEnumerable<Sprite>, IEnumerable<Sprite>>(Observable.Do<IEnumerable<Sprite>>(backgroundLoader.Load(), (Action<IEnumerable<Sprite>>)backgroundSpritesManager.ReplaceRange), backgroundPreloader.Load()), (Action<IEnumerable<Sprite>>)backgroundSpritesManager.ReplaceRange), (Action)ReplaceTiles), (Action<IEnumerable<Sprite>>)delegate
		{
		}), (Component)this);
	}

	private void ReplaceTiles()
	{
		foreach (KeyValuePair<int, Sprite> item in (ReactiveDictionary<int, Sprite>)backgroundSpritesManager)
		{
			if (tileLinks.TryGetValue(item.Key, out var value))
			{
				value.sprite = item.Value;
			}
		}
	}

	private SpriteRenderer CreateTile(Sprite sprite, Vector2 upperLeftCorner)
	{
		Vector2 vector = sprite.bounds.size;
		float x = upperLeftCorner[0] + base.transform.localScale[0] * vector[0] * sprite.pivot[0] / sprite.rect.size[0];
		float y = upperLeftCorner[1] - base.transform.localScale[1] * vector[1] * sprite.pivot[1] / sprite.rect.size[1];
		SpriteRenderer obj = UnityEngine.Object.Instantiate(position: new Vector2(x, y), original: tilePrefab, rotation: Quaternion.identity, parent: base.transform);
		obj.sprite = sprite;
		return obj;
	}

	private SpriteRenderer CreateTile(int serial, Sprite sprite)
	{
		int num = serial / columnsCount;
		int num2 = serial % columnsCount;
		Vector2 vector = cellSize * base.transform.localScale;
		float x = base.transform.position[0] + (float)num2 * vector[0];
		float y = base.transform.position[1] - (float)num * vector[1];
		Vector2 upperLeftCorner = new Vector2(x, y);
		return CreateTile(sprite, upperLeftCorner);
	}

	private void DestroyPreviousTiles()
	{
		foreach (SpriteRenderer tile in tiles)
		{
			UnityEngine.Object.Destroy(tile);
		}
		tiles.Clear();
		tileLinks.Clear();
	}

	public Bounds GetBounds()
	{
		Bounds result = default(Bounds);
		result.Encapsulate(tiles[0].bounds);
		result.Encapsulate(tiles[tiles.Count - 1].bounds);
		if (columnsCount < tiles.Count)
		{
			result.Encapsulate(tiles[columnsCount - 1].bounds);
		}
		return result;
	}

	private void OnDestroy()
	{
		onBoundsChanged.Dispose();
	}
}
