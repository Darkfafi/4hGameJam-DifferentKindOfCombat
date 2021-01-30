using UnityEngine;

public class GameStats
{
	public delegate void StatHandler(int currentValue, int previousValue, Stat stat);
	public event StatHandler StatChangedEvent;

	public int Health
	{
		get; private set;
	}

	public int Status
	{
		get; private set;
	}

	public int Objective
	{
		get; private set;
	}

	public void AffectStat(int amount, Stat stat)
	{
		int oldValue = 0, newValue = amount;
		switch(stat)
		{
			case Stat.Health:
				oldValue = Health;
				newValue = Health = Mathf.Clamp(Health + amount, 0, 100);
				break;
			case Stat.Objective:
				oldValue = Objective;
				newValue = Objective = Mathf.Clamp(Objective + amount, 0, 100);
				break;
			case Stat.Status:
				oldValue = Status;
				newValue = Status = Mathf.Clamp(Status + amount, 0, 100);
				break;
		}

		if(oldValue != newValue)
		{
			StatChangedEvent?.Invoke(newValue, oldValue, stat);
		}
	}

	public enum Stat
	{
		Health,
		Status,
		Objective
	}
}
