using System;

namespace StripClub.Model;

public class Position
{
	public int ShareID { get; set; }

	public string ShareName { get; set; }

	public double Amount => Total / (double)BuyPrice;

	public float BuyPrice { get; set; }

	public DateTime LastChange { get; set; }

	public double Total { get; set; }

	public Position(int id, double total, float price)
	{
		ShareID = id;
		Total = total;
		BuyPrice = price;
	}
}
