using UnityEngine;

namespace StripClub.Model.Data;

public abstract class ShopObject : ScriptableObject
{
	[SerializeField]
	private SaleTime saleTime;

	public SaleTime SaleTime => saleTime;
}
