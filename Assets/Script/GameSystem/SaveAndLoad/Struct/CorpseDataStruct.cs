using System;

namespace GameSystem.SaveAndLoad
{
    [Serializable]
    public class CorpseDataStruct
    {
        System.Collections.Generic.List<CorpseData> corpseDatas = null;

        public CorpseDataStruct()
        {
            this.corpseDatas = new System.Collections.Generic.List<CorpseData>();
        }

        public CorpseData GetCorpseData(int index)
        {
            return this.corpseDatas[index];
        }
        public int GetCorpseDataCount()
        {
            if (corpseDatas is null) return 0;
            return this.corpseDatas.Count;
        }
        public void SetCorpseData(CorpseData corpseData)
        {
            this.corpseDatas.Add(corpseData);
        }
        public void RemoveCorpseData(CorpseData corpseData)
        {
            this.corpseDatas.Remove(corpseData);
        }
    }

    [Serializable]
    public class CorpseData
    {
        private Monster.MonsterName monsterName;
        private float corpsePositionX;
        private float corpsePositionY;

        public CorpseData(Monster.MonsterName monsterName, float corpsePositionX, float corpsePositionY)
        {
            this.monsterName = monsterName;
            this.corpsePositionX = corpsePositionX;
            this.corpsePositionY = corpsePositionY;
        }

        public Monster.MonsterName MonsterName { get { return this.monsterName; } set { this.monsterName = value; } }
        public float CorpsePositionX { get { return this.corpsePositionX; } set { this.corpsePositionX = value; } }
        public float CorpsePositionY { get { return this.corpsePositionY; } set { this.corpsePositionY = value; } }

    }
}