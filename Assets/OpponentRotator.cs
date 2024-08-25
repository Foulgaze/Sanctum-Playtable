using System.Collections.Generic;
using Sanctum_Core;

public class OpponentRotator
{
	private List<string> opponents;
	int currentIndex = 0;
	private Playtable table;
	public OpponentRotator(List<string> opponents, Playtable table)
	{
		this.opponents = opponents;
		this.table = table;
		this.opponents.Sort();
	}

	public Player currentOpponent()
	{
		return this.table.GetPlayer(this.opponents[currentIndex]);
	}

	public void Left()
	{
		this.currentIndex = this.currentIndex - 1 < 0 ? this.opponents.Count - 1 : this.currentIndex - 1;
	}

	public void Right()
	{
		this.currentIndex  += 1;
		this.currentIndex %= opponents.Count;
	}
}