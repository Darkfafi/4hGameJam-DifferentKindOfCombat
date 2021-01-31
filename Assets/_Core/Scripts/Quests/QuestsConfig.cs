using System;
using System.Collections.Generic;

public class QuestsConfig
{
	public const int DefaultEffectValue = 10;

	private Quest[] _quests;

	public QuestsConfig()
	{
		_quests = new Quest[]
		{
			SlimePollutionQuest.CreateQuest(),
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

	public static class IntroQuest
	{
		public static Quest CreateQuest()
		{
			return new Quest("Prologue", "You prepair yourself for the quests which are waiting for you", new QuestEncounter[] 
			{ 
				new QuestEncounter("Getting in shape", "You have to train hard and eat well in order to grow stronger. Your health must be balanced.", "Train", x => 
				{
					x.Quest.GameStats.AffectStat(30, GameStats.Stat.Health);
					x.Quest.GameStats.AffectStat(10, GameStats.Stat.Objective);
					x.Quest.GameStats.AffectStat(-10, GameStats.Stat.Resources);
				}, "Skip Training", null),
				new QuestEncounter("Promoting", "You build up connections and sell yourself as a warrior looking for adventure, hoping to get a task.", "Socialize", x => 
				{
					x.Quest.GameStats.AffectStat(30, GameStats.Stat.Status);
					x.Quest.GameStats.AffectStat(10, GameStats.Stat.Objective);
					x.Quest.GameStats.AffectStat(-10, GameStats.Stat.Resources);
				}, "Skip Socialization", null),
				new QuestEncounter("Equipment & Savings", "You work hard, prepair your equipment and savings so you have the required resources to tackle any obstacle.", "Prepare", x =>
				{
					x.Quest.GameStats.AffectStat(40, GameStats.Stat.Resources);
					x.Quest.GameStats.AffectStat(10, GameStats.Stat.Objective);
				}, "Skip Preperations", null),
				new QuestEncounter("A letter", "A letter arrived from a village requesting aid.", "Begin Adventure!", null)
			});
		}
	}

	public static class GoblinsRaidQuest
	{
		public static Quest CreateQuest()
		{
			return new Quest("Goblins Raid", "Goblins are prepairing to raid the village..", new QuestEncounter[] 
			{ 
				
			}).AddEncounters(GenericEncounters.GenerateGenericEncounters(5));
		}
	}

	public static class SlimePollutionQuest
	{
		public static Quest CreateQuest()
		{
			return new Quest("Slime Pollution", "The slimes have polluted the waters.. Help the village by getting rid of the slimes and cleaning the water.", new QuestEncounter[] { })
				.AddEncounters(GenericEncounters.CreateFight("a slime", extraFight: x => x.Quest.GameStats.AffectStat(DefaultEffectValue, GameStats.Stat.Objective)).Duplicate(5))
				.AddEncounters(GenericEncounters.CreateRescue("A child is sick because of the polluted water.. He might die..",
				x =>
				{
					if(GenericEncounters.RollChance(60))
					{
						x.Quest.AddEncounter(RescueChildPukePollution, 3);
					}
				}, x =>
				{
					if(GenericEncounters.RollChance(35))
					{
						x.Quest.AddEncounter(RescueChildCleanPollution, 3);
					}
					InteractionWithWaterEncounter(x.Quest);
				}).Duplicate(1))
				.AddEncounter(new QuestEncounter("The Water Filter", "A resident from the village came up with a design to create a water filter to fight the pollution, will you invest in the idea?", 
				"Yes", x=> 
				{
					x.Quest.GameStats.AffectStat(-(int)(DefaultEffectValue * 1.5f), GameStats.Stat.Resources);
					QuestEncounter waterFilter = new QuestEncounter("The Water Filter", "The Water Filter is now being used!", "Nice!", (y) => {
						y.Quest.GameStats.AffectStat((int)(DefaultEffectValue * 1.5f), GameStats.Stat.Objective);
						y.Quest.GameStats.AffectStat((int)(DefaultEffectValue * 1.5f), GameStats.Stat.Status);
					});
					if(GenericEncounters.RollChance(50))
					{
						x.Quest.AddEncounter(GenericEncounters.CreateBuyWithResources("The resident who invented the water filter has become greedy and wishes to hold it to himself unless you buy it", (y) =>
						{
							y.Quest.AddEncounter(waterFilter, 0);
						}), 2);
					}
					else
					{
						x.Quest.AddEncounter(waterFilter, 2);
					}
				}, "No", (y)=>
				{
					y.Quest.GameStats.AffectStat(-DefaultEffectValue, GameStats.Stat.Objective);
					y.Quest.GameStats.AffectStat(-DefaultEffectValue, GameStats.Stat.Status);
				}))
				.AddEncounter(GenericEncounters.CreateTask("A fisher offers you coin for clearing his favorite fishing pond from the pollution", extraAccept: x => 
				{
					x.Quest.GameStats.AffectStat(DefaultEffectValue, GameStats.Stat.Objective);
					InteractionWithWaterEncounter(x.Quest);
				}))
				.AddEncounter(new QuestEncounter("Pollution Effect", "You look at the current status of the pollution.. And you see something in the distance..", "Take a look", x => 
				{
					if(x.Quest.GameStats.Objective > 65)
					{
						x.Quest.AddEncounter(new QuestEncounter("Happy Residents", "Residents come up to you with gifts for all your hard work", "Receive Gifts", y =>
						{
							y.Quest.GameStats.AffectStat(DefaultEffectValue, GameStats.Stat.Resources);
							y.Quest.GameStats.AffectStat((int)(DefaultEffectValue * 0.5f), GameStats.Stat.Status);
							y.Quest.GameStats.AffectStat((int)(DefaultEffectValue * 0.5f), GameStats.Stat.Health);
						}), 0);
					}
					else if(x.Quest.GameStats.Objective < 30)
					{
						x.Quest.AddEncounter(GenericEncounters.CreateFight("a giant slime which has taken shape out of all the pollution", extraFight: y => y.Quest.GameStats.AffectStat(DefaultEffectValue * 2, GameStats.Stat.Objective)), 0);
					}
					else
					{
						x.Quest.AddEncounter(GenericEncounters.CreateFight("a slime which has taken shape out of the pollution", extraFight: y => y.Quest.GameStats.AffectStat(DefaultEffectValue, GameStats.Stat.Objective)), 0);
					}
				}))
				.AddEncounters(ThirstPollution.Duplicate(1))
				.AddEncounters(GenericEncounters.GenerateGenericEncounters(5));
		}

		public static readonly QuestEncounter PukePollution = new QuestEncounter("Sickness", "The polluted water made you sick.. Where will you puke?..",
			"Land", (x) =>
			{
				x.Quest.GameStats.AffectStat(-DefaultEffectValue, GameStats.Stat.Status);

			},
			"The water", (x) =>
			{
				x.Quest.GameStats.AffectStat(-DefaultEffectValue, GameStats.Stat.Objective);
			}
		);

		public static readonly QuestEncounter RescueChildPukePollution = new QuestEncounter("Polluted Child..", "The child who was sick of the polluted water.. died.. and polluted the water more with its body", "Ok..", (x) =>
		{
			x.Quest.GameStats.AffectStat(-DefaultEffectValue * 3, GameStats.Stat.Objective);
			x.Quest.GameStats.AffectStat(-DefaultEffectValue * 3, GameStats.Stat.Status);
		});

		public static readonly QuestEncounter RescueChildCleanPollution = new QuestEncounter("Rescued Child", "The child you saved came together with his friends to help you clean the polluted water, will you accept?",
			"Yes", (x) =>
			{
				x.Quest.GameStats.AffectStat(DefaultEffectValue * 2, GameStats.Stat.Objective);
				x.Quest.GameStats.AffectStat((int)(DefaultEffectValue * 1.5f), GameStats.Stat.Status);
			},
			"No", (x) =>
			{
				x.Quest.GameStats.AffectStat(-(int)(DefaultEffectValue * 0.5f), GameStats.Stat.Status);
			}
		);

		public static readonly QuestEncounter ThirstPollution = new QuestEncounter("Thirst", "You are very thirsty, will you be drinking the polluted water?",
			"Yes", (x) =>
			{
				x.Quest.GameStats.AffectStat((int)(DefaultEffectValue * 0.5f), GameStats.Stat.Objective);
				InteractionWithWaterEncounter(x.Quest);
			},
			"No", (x) =>
			{
				x.Quest.GameStats.AffectStat(-(int)(DefaultEffectValue * 0.5f), GameStats.Stat.Health);

			}
		);

		public static void InteractionWithWaterEncounter(Quest quest)
		{
			if(GenericEncounters.RollChance(100 - quest.GameStats.Objective))
			{
				quest.GameStats.AffectStat(-(int)(DefaultEffectValue * 0.5f), GameStats.Stat.Health);
				quest.AddEncounter(PukePollution, 3);
			}
		}
	}

	public static class GenericEncounters
	{
		public static readonly QuestEncounter HealthPotion = new QuestEncounter
		(
			"Health Potion", 
			"You found a health potion, do you wish to drink it?", 
			"Yes", x => x.Quest.GameStats.AffectStat(DefaultEffectValue * 2, GameStats.Stat.Health),
			"No", null
		);

		public static readonly QuestEncounter ThiefEnemy = CreateFight("a thief", "Pay", x => x.Quest.GameStats.AffectStat(-DefaultEffectValue, GameStats.Stat.Resources));

		public static readonly QuestEncounter InnEncounter = new QuestEncounter
		(
			"Inn",
			"You stumbled upon an inn, do you wish to book a room to rest?",
			"Yes", x => 
			{
				x.Quest.GameStats.AffectStat(-DefaultEffectValue, GameStats.Stat.Resources);
				x.Quest.GameStats.AffectStat(DefaultEffectValue, GameStats.Stat.Health);
			},
			"No", null
		);

		public static QuestEncounter[] GenerateGenericEncounters(int amount)
		{
			QuestEncounter[] potentialGenericEncounters = new QuestEncounter[]
			{
				InnEncounter,
				ThiefEnemy,
				HealthPotion,
				CreateStealResource("some gold on a table"),
				CreateRescue("You see a man drowning", extraRescue: x => 
				{
					List<QuestEncounter> drownRescueEncounters = new List<QuestEncounter>();
					GameStats.ForEachStat(stat => 
					{
						if(stat != GameStats.Stat.Status)
						{ 
							drownRescueEncounters.Add(CreateHelpWithStat($"The man you saved from drowning was able to find you through your good reputation and wishes to help you with you with your {stat}", stat));
						}
					});

					if(TryRollChanceFirst(x.Quest.GameStats.Status, drownRescueEncounters.ToArray(), out QuestEncounter selectedEncounter))
					{
						x.Quest.AddEncounter(selectedEncounter, 4);
					}
				}),
				CreateRescue("A warrion is fighting in the distance and screams to you for help!", x=>
				{
					if(RollChance(100 - x.Quest.GameStats.Status))
					{
						x.Quest.AddEncounter(CreateRevenge("The warrior you left to die was able to find you through your bad reputation, and seeks revenge"), 4);
					}
				}, x=>
				{
					if(RollChance(x.Quest.GameStats.Status))
					{
						x.Quest.AddEncounter(CreateHelpWithStat("The warrior you rescued was able to find you through your good reputation and wishes to help you with your objective", GameStats.Stat.Objective), 4);
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
				CreateBuyWithResources("You see an item in the shop which will help you with your quest", x=> x.Quest.GameStats.AffectStat(DefaultEffectValue, GameStats.Stat.Objective)),
				CreateStealResource("a nice weapon on a wall"),
				CreateBuyWithResources("Somebody offers their service to sabotage the quest, so you can get all the fame.", extraBuy: x => x.Quest.GameStats.AffectStat(-DefaultEffectValue * 2, GameStats.Stat.Objective)),
				CreateTask("A stranger asks you to sabotage the quest in extrange for some coin. They seem to benefit from the chaos..", extraAccept: x => x.Quest.GameStats.AffectStat(-(int)(DefaultEffectValue * 1.5f), GameStats.Stat.Objective)),
				CreateFight("another hero helping with the quest.. He might take your fame away..", "Ignore", x => x.Quest.GameStats.AffectStat(-(int)(DefaultEffectValue * 0.5f), GameStats.Stat.Status), x=>x.Quest.GameStats.AffectStat(-(int)(DefaultEffectValue * 1.5f), GameStats.Stat.Objective)),
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
			"Fight", x =>
			{
				x.Quest.GameStats.AffectStat(-DefaultEffectValue, GameStats.Stat.Health);
				x.Quest.GameStats.AffectStat(-DefaultEffectValue * 2, GameStats.Stat.Status);
			},
			"Pay", x =>
			{
				x.Quest.GameStats.AffectStat(-DefaultEffectValue, GameStats.Stat.Resources);
				x.Quest.GameStats.AffectStat(-DefaultEffectValue, GameStats.Stat.Status);
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

		public static QuestEncounter CreateStealResource(string itemName, Action<QuestEncounter> extraSteal = null, Action<QuestEncounter> extraLeave = null)
		{
			return new QuestEncounter("Stealing", $"You see {itemName}. Are you going to steal it?",
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

		public static QuestEncounter CreateHelpWithStat(string situation, GameStats.Stat stat)
		{
			return new QuestEncounter("Helping Hand", $"{situation}. Do you accept the helping hand?",
				"Accept", (x) =>
				{
					x.Quest.GameStats.AffectStat((int)(DefaultEffectValue * 0.5f), stat);
				},
				"Refuse", null
			);
		}

		public static QuestEncounter CreateFight(string attackerName, string escapeText = "Flee", Action<QuestEncounter> extraEscape = null, Action<QuestEncounter> extraFight = null)
		{
			return new QuestEncounter("Battle", $"You encountered {attackerName}. Will you fight?",
				"Fight", (x) =>
				{
					x.Quest.GameStats.AffectStat(-DefaultEffectValue, GameStats.Stat.Health);
					x.Quest.GameStats.AffectStat((int)(DefaultEffectValue * 0.5f), GameStats.Stat.Status);
					extraFight?.Invoke(x);
				},
				escapeText, (x) =>
				{
					x.Quest.GameStats.AffectStat(-DefaultEffectValue, GameStats.Stat.Status);
					extraEscape?.Invoke(x);
				}
			);
		}

		public static QuestEncounter CreateRescue(string situation, Action<QuestEncounter> extraContinue = null, Action<QuestEncounter> extraRescue = null)
		{
			return new QuestEncounter("Rescue", $"{situation}. Do you rescue them?",
				"Rescue", (x) =>
				{
					x.Quest.GameStats.AffectStat(DefaultEffectValue, GameStats.Stat.Status);
					extraRescue?.Invoke(x);
				},
				"Continue Quest", (x) =>
				{
					x.Quest.GameStats.AffectStat(-DefaultEffectValue, GameStats.Stat.Status);
					x.Quest.GameStats.AffectStat((int)(DefaultEffectValue * 0.5f), GameStats.Stat.Objective);
					extraContinue?.Invoke(x);
				}
			);
		}

		public static bool TryRollChanceFirst(int chance, QuestEncounter[] questEncounters, out QuestEncounter selected)
		{
			for(int i = 0; i < questEncounters.Length; i++)
			{
				if(RollChance(chance))
				{
					selected = questEncounters[i];
					return true;
				}
			}
			selected = null;
			return false;
		}

		public static bool RollChance(int chance)
		{
			return UnityEngine.Random.Range(0, 100) <= chance;
		}
	}
}
