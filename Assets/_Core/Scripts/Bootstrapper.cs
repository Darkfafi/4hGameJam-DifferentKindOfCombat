using System;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
	[SerializeField]
	private QuestView _questView;

	[SerializeField]
	private StatsView _statsView;

	public GameStats GameStats
	{
		get; private set;
	}

	public Quest CurrentQuest
	{
		get; private set;
	}

	private QuestsConfig _questsConfig;

	private List<Quest> _quests;

	protected void Awake()
	{
		GameStats = new GameStats(50, 50, 50);
		_questsConfig = new QuestsConfig();
	}

	protected void Start()
	{
		_statsView.SetGameStats(GameStats);
		_quests = new List<Quest>(_questsConfig.Quests);
		ShuffleQuests();
		StartQuest(_quests[0]);
	}

	private void StartQuest(Quest quest)
	{
		if(CurrentQuest != null)
		{
			CurrentQuest.QuestCompletedEvent -= OnQuestCompletedEvent;
			CurrentQuest.DeInit();
		}

		CurrentQuest = quest;
		_questView.SetQuest(CurrentQuest);

		if(CurrentQuest != null)
		{
			_quests.Remove(quest);
			CurrentQuest.QuestCompletedEvent += OnQuestCompletedEvent;
			CurrentQuest.Init(GameStats, out _);
		}
	}

	private void OnQuestCompletedEvent()
	{
		if(_quests.Count > 0)
		{
			// Go to next quest
			StartQuest(_quests[0]);
		}
		else
		{
			StartQuest(null);
			// You have completed the game
			Debug.Log("Completed the game!!");
		}
	}

	private void ShuffleQuests()
	{
		int n = _quests.Count;
		while(n > 1)
		{
			int k = UnityEngine.Random.Range(0, n--);
			Quest kQuest = _quests[k];
			_quests[k] = _quests[n];
			_quests[n] = kQuest;
		}
	}
}