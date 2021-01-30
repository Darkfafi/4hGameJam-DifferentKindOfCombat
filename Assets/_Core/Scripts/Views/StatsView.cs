﻿using DG.Tweening;
using UnityEngine;

public class StatsView : MonoBehaviour
{
	[SerializeField]
	private Transform _healthBar;

	[SerializeField]
	private Transform _objectiveBar;

	[SerializeField]
	private Transform _statusBar;

	public GameStats DisplayingGameStats
	{
		get; private set;
	}

	public void SetGameStats(GameStats gameStats)
	{
		if(DisplayingGameStats != null)
		{
			DisplayingGameStats.StatChangedEvent -= OnStatChangedEvent;
		}

		DisplayingGameStats = gameStats;

		if(DisplayingGameStats != null)
		{
			DisplayingGameStats.StatChangedEvent += OnStatChangedEvent;
		}

		GameStats.Stat[] allstats = GameStats.GetAllStatTypes();
		for(int i = 0; i < allstats.Length; i++)
		{
			OnStatChangedEvent(DisplayingGameStats?.GetStat(allstats[i]) ?? 0, 0, allstats[i]);
		}
	}

	private void OnStatChangedEvent(int currentValue, int previousValue, GameStats.Stat stat)
	{
		float normalizedValue = (float)currentValue / GameStats.MaxStatValue;
		float durationBar = 0.5f;
		switch(stat)
		{
			case GameStats.Stat.Health:
				_healthBar.DOScaleX(normalizedValue, durationBar);
				break;
			case GameStats.Stat.Objective:
				_objectiveBar.DOScaleX(normalizedValue, durationBar);
				break;
			case GameStats.Stat.Status:
				_statusBar.DOScaleX(normalizedValue, durationBar);
				break;
		}
	}
}