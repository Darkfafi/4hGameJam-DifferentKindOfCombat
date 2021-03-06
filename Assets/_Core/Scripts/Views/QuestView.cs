﻿using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class QuestView : MonoBehaviour
{
	[SerializeField]
	private Text _titleLabel;

	[SerializeField]
	private Text _descriptionLabel;

	[SerializeField]
	private EncounterView _encounterView;

	public Quest QuestDisplaying
	{
		get; private set;
	}

	public void SetQuest(Quest quest)
	{
		CompleteTweeners();
		if(QuestDisplaying != null)
		{
			QuestDisplaying.EncounterSetEvent -= OnEncounterSetEvent;
		}

		QuestDisplaying = quest;

		_titleLabel.DOText(QuestDisplaying?.Title ?? "Home", 0.5f);
		_descriptionLabel.DOText(QuestDisplaying?.Description ?? "Just coming home after a long day at work... Let's relax..", 1f);

		if(QuestDisplaying != null)
		{
			QuestDisplaying.EncounterSetEvent += OnEncounterSetEvent;
		}
	}

	protected void OnDestroy()
	{
		CompleteTweeners();
	}

	private void CompleteTweeners()
	{
		_titleLabel.DOKill();
		_descriptionLabel.DOKill();
	}

	private void OnEncounterSetEvent(QuestEncounter encounter)
	{
		_encounterView.SetEncounter(encounter);
	}
}
