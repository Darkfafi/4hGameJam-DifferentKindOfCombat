using System;
using UnityEngine;

public class GameStats
{
	public delegate void StatHandler(int currentValue, int previousValue, Stat stat);
	public event StatHandler StatChangedEvent;

	public const int MaxStatValue = 100;

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

	public int Resources
	{
		get; private set;
	}

	public static Stat[] GetAllStatTypes()
	{
		return Enum.GetValues(typeof(Stat)) as Stat[];
	}

	public static void ForEachStat(Action<Stat> action)
	{
		foreach(Stat stat in GetAllStatTypes())
		{
			action(stat);
		}
	}

	public GameStats(int startHealth, int startObjective, int startStatus, int startResources)
	{
		Health = startHealth;
		Objective = startObjective;
		Status = startStatus;
		Resources = startResources;
	}

	public int GetStat(Stat stat)
	{
		switch(stat)
		{
			case Stat.Health:
				return Health;
			case Stat.Objective:
				return Objective;
			case Stat.Status:
				return Status;
			case Stat.Resources:
				return Resources;
			default:
				return 0;
		}
	}

	public void AffectStat(int amount, Stat stat)
	{
		int oldValue = 0, newValue = amount;
		switch(stat)
		{
			case Stat.Health:
				oldValue = Health;
				newValue = Health = Mathf.Clamp(Health + amount, 0, MaxStatValue);
				break;
			case Stat.Objective:
				oldValue = Objective;
				newValue = Objective = Mathf.Clamp(Objective + amount, 0, MaxStatValue);
				break;
			case Stat.Status:
				oldValue = Status;
				newValue = Status = Mathf.Clamp(Status + amount, 0, MaxStatValue);
				break;
			case Stat.Resources:
				oldValue = Resources;
				newValue = Resources = Mathf.Clamp(Resources + amount, 0, MaxStatValue);
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
		Objective,
		Resources
	}
}
