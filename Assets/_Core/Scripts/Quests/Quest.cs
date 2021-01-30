using System;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
	public delegate void EncounterHandler(QuestEncounter encounter);
	public event EncounterHandler EncounterSetEvent;

	public event Action QuestCompletedEvent;

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
		CleanCurrentEncounter();
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
		CleanCurrentEncounter();
		if(_encounters.Count > 0)
		{
			questEncounter = _encounters[0];
			_encounters.Remove(questEncounter);
			SetCurrentEncounter(questEncounter);
			return true;
		}
		questEncounter = null;
		SetCurrentEncounter(null);
		return false;
	}

	private void SetCurrentEncounter(QuestEncounter questEncounter)
	{
		if(CurrentEncounter != questEncounter)
		{
			CleanCurrentEncounter();
			CurrentEncounter = questEncounter;
			if(CurrentEncounter != null)
			{
				CurrentEncounter.ActionPerformedEvent += OnActionPerformedEvent;
				CurrentEncounter.Open(this);
				EncounterSetEvent?.Invoke(CurrentEncounter);
			}
		}
	}

	private void CleanCurrentEncounter()
	{
		if(CurrentEncounter != null)
		{
			CurrentEncounter.ActionPerformedEvent -= OnActionPerformedEvent;
			CurrentEncounter.Close();
			CurrentEncounter = null;
			EncounterSetEvent?.Invoke(CurrentEncounter);
		}
	}

	private void OnActionPerformedEvent(QuestEncounter encounter, QuestEncounter.ActionType actionType)
	{
		if(!TryProgressToNext(out _))
		{
			QuestCompletedEvent?.Invoke();
		}
	}
}
