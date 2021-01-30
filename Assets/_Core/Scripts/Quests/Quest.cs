using System.Collections.Generic;
using UnityEngine;

public class Quest
{
	public readonly string Title;
	public readonly string Description;

	public GameStats GameStats
	{
		get; private set;
	}

	public QuestEncounter CurrentEncounter
	{
		get; private set;
	}

	private List<QuestEncounter> _encounters;

	public Quest(string title, string description, QuestEncounter[] encounters)
	{
		Title = title;
		Description = description;
		_encounters = new List<QuestEncounter>(encounters);
	}

	public bool Init(GameStats gameStats, out QuestEncounter triggeredEncounter)
	{
		DeInit();
		GameStats = gameStats;
		return TryProgressToNext(out triggeredEncounter);
	}

	public void DeInit()
	{
		CleanCurrentQuest();
		GameStats = null;
	}

	public void AddEncounter(QuestEncounter encounter, int delay)
	{
		if(_encounters.Count == 0)
		{
			_encounters.Add(encounter);
		}
		else
		{
			_encounters.Insert(Mathf.Min(delay, _encounters.Count - 1), encounter);
		}
	}

	public bool TryProgressToNext(out QuestEncounter questEncounter)
	{
		CleanCurrentQuest();
		if(_encounters.Count > 0)
		{
			questEncounter = _encounters[0];
			_encounters.Remove(questEncounter);
			SetCurrentQuest(questEncounter);
			return true;
		}
		questEncounter = null;
		return false;
	}

	private void SetCurrentQuest(QuestEncounter questEncounter)
	{
		CleanCurrentQuest();
		CurrentEncounter = questEncounter;
		CurrentEncounter.Open(this);
	}

	private void CleanCurrentQuest()
	{
		if(CurrentEncounter != null)
		{
			CurrentEncounter.Close();
			CurrentEncounter = null;
		}
	}
}
