using System;
using System.Collections.Generic;
using Sanctum_Core;

public class OpponentRotator
{
	public List<string> opponentUUIDs;
	public Action<Player> onPlayerChanged;
	int currentIndex = 0;
	private Playtable table;
	public OpponentRotator(List<string> opponents, Playtable table)
	{
		this.opponentUUIDs = opponents;
		this.table = table;
		this.opponentUUIDs.Sort();
	}

	public bool HasOpponents()
	{
		return this.opponentUUIDs.Count != 0;
	}

	public Player GetCurrentOpponent()
	{
		return this.table.GetPlayer(this.opponentUUIDs[currentIndex]);
	}

	public void Left()
	{
		if(opponentUUIDs.Count == 0)
		{
			return;
		}
		this.currentIndex = this.currentIndex - 1 < 0 ? this.opponentUUIDs.Count - 1 : this.currentIndex - 1;
		this.onPlayerChanged(GetCurrentOpponent());
	}

	public void Right()
	{
		if(opponentUUIDs.Count == 0)
		{
			return;
		}
		this.currentIndex  += 1;
		this.currentIndex %= opponentUUIDs.Count;
		this.onPlayerChanged(GetCurrentOpponent());

	}
}