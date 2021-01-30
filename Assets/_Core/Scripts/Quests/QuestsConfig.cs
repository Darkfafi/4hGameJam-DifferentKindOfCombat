public class QuestsConfig
{
	private Quest[] _quests;

	public QuestsConfig()
	{
		// Encounters
		QuestEncounter healthPotion = new QuestEncounter("Health Potion", "You found a health potion, do you wish to drink it?", "No", null, "Yes", x => x.Quest.GameStats.AffectStat(20, GameStats.Stat.Health));

		// Slime Quest
		QuestEncounter pukePolution = new QuestEncounter("Sickness", "The poluted water made you sick.. Where will you puke?..",
			"Land", (x) =>
			{
				x.Quest.GameStats.AffectStat(-10, GameStats.Stat.Status);

			},
			"The water", (x) =>
			{
				x.Quest.GameStats.AffectStat(-10, GameStats.Stat.Objective);
			}
		);

		QuestEncounter thirstPolution = new QuestEncounter("Thirst", "You are very thirsty, will you be drinking the polluted water?",
			"No", (x) =>
			{
				x.Quest.GameStats.AffectStat(-5, GameStats.Stat.Health);

			}, 
			"Yes", (x) =>
			{
				x.Quest.GameStats.AffectStat(5, GameStats.Stat.Objective);
				x.Quest.GameStats.AffectStat(-10, GameStats.Stat.Health);
				x.Quest.GameStats.AffectStat(5, GameStats.Stat.Status);
				x.Quest.AddEncounter(pukePolution, 3);
			}
		);

		_quests = new Quest[]
		{
			// You are in a dungeon, and you have to defeat the slimes
			new Quest("Slime Polution", "The slimes have poluted the waters.. Help the village by getting rid of the slimes and cleaning the water.", new QuestEncounter[]{})
			.AddEncounters(CreateFightQuest("a slime", true).Duplicate(5))
			.AddEncounters(CreateRescue("A child is sick because of the poluted water", false).Duplicate(3))
			.AddEncounters(CreateFightQuest("a thieve", false).Duplicate(5))
			.AddEncounters(thirstPolution.Duplicate(3))
			.AddEncounters(healthPotion.Duplicate(2)),
		};
	}

	public Quest[] GetAllQuests()
	{
		Quest[] quests = new Quest[_quests.Length];
		for(int i = 0; i < quests.Length; i++)
		{
			quests[i] = _quests[i].Copy();
		}
		return quests;
	}

	private static QuestEncounter CreateFightQuest(string attackerName, bool affectObjective)
	{
		return new QuestEncounter("Battle", $"You encountered {attackerName}. Do you fight it?", 
			"Flee", (x) =>
			{
				x.Quest.GameStats.AffectStat(-10, GameStats.Stat.Status);

			}, 
			"Fight", (x) =>
			{
				if(affectObjective)
				{
					x.Quest.GameStats.AffectStat(10, GameStats.Stat.Objective);
				}
				x.Quest.GameStats.AffectStat(-10, GameStats.Stat.Health);
				x.Quest.GameStats.AffectStat(10, GameStats.Stat.Status);
			}
		);
	}

	private static QuestEncounter CreateRescue(string situation, bool affectHealth)
	{
		return new QuestEncounter("Rescue", $"{situation}'. Do you rescue them?",
			"Continue Quest", (x) =>
			{
				x.Quest.GameStats.AffectStat(-10, GameStats.Stat.Status);
				x.Quest.GameStats.AffectStat(10, GameStats.Stat.Objective);

			}, 
			"Rescue", (x) =>
			{
				if(affectHealth)
				{
					x.Quest.GameStats.AffectStat(-10, GameStats.Stat.Health);
				}
				x.Quest.GameStats.AffectStat(10, GameStats.Stat.Status);
			}
		);
	}
}
