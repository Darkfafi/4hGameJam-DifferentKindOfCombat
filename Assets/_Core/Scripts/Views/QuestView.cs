using UnityEngine;
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
		if(QuestDisplaying != null)
		{
			QuestDisplaying.EncounterSetEvent -= OnEncounterSetEvent;
		}

		QuestDisplaying = quest;

		_titleLabel.DOText(QuestDisplaying?.Title ?? "Home", 0.25f);
		_descriptionLabel.DOText(QuestDisplaying?.Description ?? "Just coming home after a long day at work... Let's relax..", 0.5f);

		if(QuestDisplaying != null)
		{
			QuestDisplaying.EncounterSetEvent += OnEncounterSetEvent;
		}
	}

	private void OnEncounterSetEvent(QuestEncounter encounter)
	{
		_encounterView.SetEncounter(encounter);
	}
}
