using System;
using System.Collections.Generic;

public class QuestsConfig
{
	public const int DefaultEffectValue = 10;

	private Quest[] _quests;

	public QuestsConfig()
	{
		// Slime Quest
		QuestEncounter pukePollution = new QuestEncounter("Sickness", "The polluted water made you sick.. Where will you puke?..",
			"Land", (x) =>
			{
				x.Quest.GameStats.AffectStat(-DefaultEffectValue, GameStats.Stat.Status);

			},
			"The water", (x) =>
			{
				x.Quest.GameStats.AffectStat(-DefaultEffectValue, GameStats.Stat.Objective);
			}
		);

		QuestEncounter rescueChildPukePollution = new QuestEncounter("Polluted Child..", "The child who was sick of the polluted water.. died.. and polluted the water more with its body", "Ok..", (x) => 
		{
			x.Quest.GameStats.AffectStat(-DefaultEffectValue * 3, GameStats.Stat.Objective);
			x.Quest.GameStats.AffectStat(-DefaultEffectValue * 3, GameStats.Stat.Status);
		});

		QuestEncounter rescueChildCleanPollution = new QuestEncounter("Rescued Child", "The child you saved came together with his friends to help you clean the polluted water, will you accept?", 
			"No", (x) => 
			{
				x.Quest.GameStats.AffectStat(-(int)(DefaultEffectValue * 0.5f), GameStats.Stat.Status);
			}, 
			"Yes", (x) =>
			{
				x.Quest.GameStats.AffectStat(DefaultEffectValue * 2, GameStats.Stat.Objective);
				x.Quest.GameStats.AffectStat((int)(DefaultEffectValue * 1.5f), GameStats.Stat.Status);
			}
		);

		QuestEncounter thirstPolution = new QuestEncounter("Thirst", "You are very thirsty, will you be drinking the polluted water?",
			"No", (x) =>
			{
				x.Quest.GameStats.AffectStat(-(int)(DefaultEffectValue * 0.5f), GameStats.Stat.Health);

			}, 
			"Yes", (x) =>
			{
				x.Quest.GameStats.AffectStat((int)(DefaultEffectValue * 0.5f), GameStats.Stat.Objective);
				x.Quest.GameStats.AffectStat(-DefaultEffectValue, GameStats.Stat.Health);
				x.Quest.GameStats.AffectStat(DefaultEffectValue, GameStats.Stat.Status);
				x.Quest.AddEncounter(pukePollution, 3);
			}
		);

		_quests = new Quest[]
		{
			// You are in a dungeon, and you have to defeat the slimes
			new Quest("Slime Polution", "The slimes have polluted the waters.. Help the village by getting rid of the slimes and cleaning the water.", new QuestEncounter[]{})
			.AddEncounters(GenericQuests.CreateFightQuest("a slime", extraFight: x => x.Quest.GameStats.AffectStat(DefaultEffectValue, GameStats.Stat.Objective)).Duplicate(5))
			.AddEncounters(GenericQuests.CreateRescue("A child is sick because of the polluted water.. He might die..",
			x =>
			{
				if(GenericQuests.RollChance(40))
				{
					x.Quest.AddEncounter(rescueChildPukePollution, 3);
				}
			}, x =>
			{
				if(GenericQuests.RollChance(40))
				{
					x.Quest.AddEncounter(rescueChildCleanPollution, 3);
				}
			}).Duplicate(1))
			.AddEncounters(thirstPolution.Duplicate(1))
			.AddEncounter(GenericQuests.HealthPotion)
			.AddEncounters(GenericQuests.GenerateGenericEncounters(9)),
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

	public static class GenericQuests
	{
		public static readonly QuestEncounter HealthPotion = new QuestEncounter
		(
			"Health Potion", 
			"You found a health potion, do you wish to drink it?", 
			"No", null, 
			"Yes", x => x.Quest.GameStats.AffectStat(DefaultEffectValue * 2, GameStats.Stat.Health)
		);

		public static readonly QuestEncounter ThiefEnemy = CreateFightQuest("a thief", "Pay", x => x.Quest.GameStats.AffectStat(-DefaultEffectValue, GameStats.Stat.Resources));

		public static readonly QuestEncounter InnEncounter = new QuestEncounter
		(
			"Inn",
			"You stumbled upon an inn, do you wish to book a room to rest?",
			"No", null,
			"Yes", x => 
			{
				x.Quest.GameStats.AffectStat(-DefaultEffectValue, GameStats.Stat.Resources);
				x.Quest.GameStats.AffectStat(DefaultEffectValue, GameStats.Stat.Health);
			}
		);

		public static QuestEncounter[] GenerateGenericEncounters(int amount)
		{
			QuestEncounter[] potentialGenericEncounters = new QuestEncounter[]
			{
				InnEncounter,
				ThiefEnemy,
				HealthPotion,
				CreateStealQuest("gold"),
				CreateRescue("You see a man drowning"),
				CreateRescue("A warrion is fighting in the distance and screams to you for help!", (x)=>
				{
					if(RollChance(25))
					{
						x.Quest.AddEncounter(CreateRevenge("The warrior you left to die came back to you.. And he is not happy.."), 4);
					}
				}),
				CreateBuyWithResources("A health potion for sale!", x => x.Quest.AddEncounter(HealthPotion, 0)),
				CreateTask("A farmer asked you to help him on the field. He will pay you."),
				CreateTask("You found a bounty poster with a nice reward.", x =>
				{
					x.Quest.GameStats.AffectStat(DefaultEffectValue, GameStats.Stat.Resources);
					x.Quest.GameStats.AffectStat(-DefaultEffectValue, GameStats.Stat.Health);
					if(RollChance(5))
					{
						x.Quest.AddEncounter(CreateRevenge("The bounty you accepted has affected a gang negatively.. They will have their revenge.."), 4);
					}
				}),
				CreateBuyWithResources("You see an item in the shop which will help you with your quest", x=> x.Quest.GameStats.AffectStat(DefaultEffectValue, GameStats.Stat.Objective))
			};

			List<int> selectedQuestIndexes = new List<int>();
			for(int i = 0; i < potentialGenericEncounters.Length; i++)
			{
				selectedQuestIndexes.Add(i);
			}
			selectedQuestIndexes.Shuffle();

			QuestEncounter[] questEncounters = new QuestEncounter[amount];
			for(int i = 0; i < amount; i++)
			{
				int index = 0;
				if(selectedQuestIndexes.Count > 0)
				{
					index = selectedQuestIndexes[0];
					selectedQuestIndexes.RemoveAt(0);
				}
				else
				{
					index = UnityEngine.Random.Range(0, potentialGenericEncounters.Length);
				}
				questEncounters[i] = potentialGenericEncounters[index];
			}
			return questEncounters;
		}

		public static readonly QuestEncounter StealGuardResponse = new QuestEncounter
		(
			"Caught!",
			"The guards saw you steal. Pay, or fight!",
			"Pay", x =>
			{
				x.Quest.GameStats.AffectStat(-DefaultEffectValue, GameStats.Stat.Resources);
				x.Quest.GameStats.AffectStat(-DefaultEffectValue, GameStats.Stat.Status);
			},
			"Fight", x => 
			{
				x.Quest.GameStats.AffectStat(-DefaultEffectValue, GameStats.Stat.Health);
				x.Quest.GameStats.AffectStat(-DefaultEffectValue * 2, GameStats.Stat.Status);
			}
		);

		public static QuestEncounter CreateRevenge(string situation)
		{
			return new QuestEncounter
			(
				"Revenge",
				$"{situation}, and will now have their revenge",
				"Ow..", x =>
				{
					x.Quest.GameStats.AffectStat(-DefaultEffectValue, GameStats.Stat.Resources);
					x.Quest.GameStats.AffectStat(-DefaultEffectValue, GameStats.Stat.Health);
				}
			);
		}

		public static QuestEncounter CreateBuyWithResources(string situation, Action<QuestEncounter> extraBuy = null, Action<QuestEncounter> extraLeave = null)
		{
			return new QuestEncounter("Shopping", $"{situation}. Are you buying it?",
				"Buy", (x) =>
				{
					x.Quest.GameStats.AffectStat(-DefaultEffectValue, GameStats.Stat.Resources);
					extraBuy?.Invoke(x);
				},
				"Leave", (x) =>
				{
					extraLeave?.Invoke(x);
				}
			);
		}

		public static QuestEncounter CreateStealQuest(string itemName, Action<QuestEncounter> extraSteal = null, Action<QuestEncounter> extraLeave = null)
		{
			return new QuestEncounter("Stealing", $"You see {itemName} unguared. Are you going to steal it?",
				"Steal", (x) =>
				{
					x.Quest.GameStats.AffectStat(DefaultEffectValue, GameStats.Stat.Resources);
					if(RollChance(25))
					{
						x.Quest.AddEncounter(StealGuardResponse, 0);
					}
					extraSteal?.Invoke(x);
				},
				"Leave", (x) =>
				{
					extraLeave?.Invoke(x);
				}
			);
		}

		public static QuestEncounter CreateTask(string task, Action<QuestEncounter> extraAccept = null, Action<QuestEncounter> extraRefuse = null)
		{
			return new QuestEncounter("Task", $"{task}. Do you accept the task?",
				"Accept", (x) =>
				{
					x.Quest.GameStats.AffectStat((int)(DefaultEffectValue * 0.5f), GameStats.Stat.Resources);
					x.Quest.GameStats.AffectStat((int)(DefaultEffectValue * 0.5f), GameStats.Stat.Status);
					extraAccept?.Invoke(x);
				},
				"Refuse", (x) =>
				{
					x.Quest.GameStats.AffectStat(-(int)(DefaultEffectValue * 0.5f), GameStats.Stat.Status);
					extraRefuse?.Invoke(x);
				}
			);
		}

		public static QuestEncounter CreateFightQuest(string attackerName, string escapeText = "Flee", Action<QuestEncounter> extraEscape = null, Action<QuestEncounter> extraFight = null)
		{
			return new QuestEncounter("Battle", $"You encountered {attackerName}. Do you fight it?",
				escapeText, (x) =>
				{
					x.Quest.GameStats.AffectStat(-DefaultEffectValue, GameStats.Stat.Status);
					extraEscape?.Invoke(x);
				},
				"Fight", (x) =>
				{
					x.Quest.GameStats.AffectStat(-DefaultEffectValue, GameStats.Stat.Health);
					x.Quest.GameStats.AffectStat(DefaultEffectValue, GameStats.Stat.Status);
					extraFight?.Invoke(x);
				}
			);
		}

		public static QuestEncounter CreateRescue(string situation, Action<QuestEncounter> extraContinue = null, Action<QuestEncounter> extraRescue = null)
		{
			return new QuestEncounter("Rescue", $"{situation}. Do you rescue them?",
				"Continue Quest", (x) =>
				{
					x.Quest.GameStats.AffectStat(-DefaultEffectValue, GameStats.Stat.Status);
					x.Quest.GameStats.AffectStat((int)(DefaultEffectValue * 0.5f), GameStats.Stat.Objective);
					extraContinue?.Invoke(x);
				},
				"Rescue", (x) =>
				{
					x.Quest.GameStats.AffectStat(DefaultEffectValue, GameStats.Stat.Status);
					extraRescue?.Invoke(x);
				}
			);
		}

		public static bool RollChance(int chance)
		{
			return UnityEngine.Random.Range(0, 100) <= chance;
		}
	}
}
