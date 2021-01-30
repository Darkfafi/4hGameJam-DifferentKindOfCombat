using DG.Tweening;
using UnityEngine;

public class StatsView : MonoBehaviour
{
	[SerializeField]
	private Transform _healthBar;

	[SerializeField]
	private Transform _objectiveBar;

	[SerializeField]
	private Transform _statusBar;

	[SerializeField]
	private Transform _resourcesBar;

	public GameStats DisplayingGameStats
	{
		get; private set;
	}

	public void SetGameStats(GameStats gameStats)
	{
		CompleteTweeners();
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

	protected void OnDestroy()
	{
		CompleteTweeners();
	}

	private void CompleteTweeners()
	{
		_healthBar.DOComplete();
		_objectiveBar.DOComplete();
		_statusBar.DOComplete();
		_resourcesBar.DOComplete();
	}

	private void OnStatChangedEvent(int currentValue, int previousValue, GameStats.Stat stat)
	{
		float normalizedValue = (float)currentValue / GameStats.MaxStatValue;
		float durationBar = 0.5f;
		switch(stat)
		{
			case GameStats.Stat.Health:
				_healthBar.DOKill();
				_healthBar.DOScaleX(normalizedValue, durationBar);
				break;
			case GameStats.Stat.Objective:
				_objectiveBar.DOKill();
				_objectiveBar.DOScaleX(normalizedValue, durationBar);
				break;
			case GameStats.Stat.Status:
				_statusBar.DOKill();
				_statusBar.DOScaleX(normalizedValue, durationBar);
				break;
			case GameStats.Stat.Resources:
				_resourcesBar.DOKill();
				_resourcesBar.DOScaleX(normalizedValue, durationBar);
				break;
		}
	}
}
