using System;

namespace GameSystem.SaveAndLoad
{
    [Serializable]
    public class GameDataStruct
    {
        private SceneName sceneName;
        private float inGameTime;

        private PlayerDataStruct playerDataStruct;
        private EnemyDataStruct enemyDataStruct;
        private SummonDataStruct summonDataStruct;
        private CorpseDataStruct corpseDataStruct;
        private SkillDataStruct skillDataStruct;

        public GameDataStruct(SceneName sceneName, float inGameTime)
        {
            this.sceneName = sceneName;     // SceneName.Scene01;
            this.inGameTime = inGameTime;   // 0

            this.playerDataStruct = null;
            this.enemyDataStruct = null;
            this.summonDataStruct = null;
            this.corpseDataStruct = null;
            this.skillDataStruct = new SkillDataStruct();
        }

        public SceneName SceneName { get { return this.sceneName; } set { this.sceneName = value; } }
        public float InGameTime { get { return this.inGameTime; } set { this.inGameTime = value; } }

        public PlayerDataStruct PlayerDataStruct { get { return this.playerDataStruct; } set { this.playerDataStruct = value; } }
        public EnemyDataStruct EnemyDataStruct { get { return this.enemyDataStruct; } set { this.enemyDataStruct = value; } }
        public SummonDataStruct SummonDataStruct { get { return this.summonDataStruct; } set { this.summonDataStruct = value; } }
        public CorpseDataStruct CorpseDataStruct { get { return this.corpseDataStruct; } set { this.corpseDataStruct = value; } }
        public SkillDataStruct SkillDataStruct { get { return this.skillDataStruct; } set { this.skillDataStruct = value; } }
    }

    [Serializable]
    public class SummaryGameDataStruct
    {
        private SceneName sceneName;
        private float inGameTime;
        private int stage;

        private int ownManaStone;

        private int enemyCount;

        private int summonCount;


        public SceneName SceneName { get { return this.sceneName; } set { this.sceneName = value; } }
        public float InGameTime { get { return this.inGameTime; } set { this.inGameTime = value; } }
        public int Stage { get { return this.stage; } set { this.stage = value; } }

        public int OwnManaStone { get { return this.ownManaStone; } set { this.ownManaStone = value; } }

        public int EnemyCount { get { return this.enemyCount; } set { this.enemyCount = value; } }

        public int SummonCount { get { return this.summonCount; } set { this.summonCount = value; } }
    }
}