public class QuestsConfig
{
	public Quest[] Quests;

	public QuestsConfig()
	{
		Quests = new Quest[]
		{
			// You are in a dungeon, and you have to defeat the slimes
			new Quest("Slime Dungeon", "The slimy dungeon has been slippery for far too long...", new QuestEncounter[]
			{ 
				new QuestEncounter("Spotted One!", "Will you attack it?", "No", (x)=>
				{
					x.Quest.GameStats.AffectStat(-10, GameStats.Stat.Objective);
				}, "Yes", (x) =>
				{
					x.Quest.GameStats.AffectStat(10, GameStats.Stat.Objective);
					x.Quest.GameStats.AffectStat(-5, GameStats.Stat.Health);
				})
			}),
		};
	}
}
