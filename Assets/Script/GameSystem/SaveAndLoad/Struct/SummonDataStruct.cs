using System;

namespace GameSystem.SaveAndLoad
{
    [Serializable]
    public class SummonDataStruct
    {
        System.Collections.Generic.List<SummonData> summonDatas = null;

        public SummonDataStruct()
        {
            this.summonDatas = new System.Collections.Generic.List<SummonData>();
        }

        public SummonData GetEnemyData(int index)
        {
            return this.summonDatas[index];
        }
        public int GetSummonDataCount()
        {
            if (summonDatas is null) return 0;
            return this.summonDatas.Count;
        }
        public void SetEnemyData(SummonData summonData)
        {
            this.summonDatas.Add(summonData);
        }
        public void RemoveEnemyData(SummonData summonData)
        {
            this.summonDatas.Remove(summonData);
        }
    }

    [Serializable]
    public class SummonData
    {
        private Monster.MonsterName monsterName;
        private float summonPositionX;
        private float summonPositionY;
        private int summonCurrentHP;

        public Monster.MonsterName MonsterName { get { return this.monsterName; } set { this.monsterName = value; } }
        public float SummonPositionX { get { return this.summonPositionX; } set { this.summonPositionX = value; } }
        public float SummonPositionY { get { return this.summonPositionY; } set { this.summonPositionY = value; } }
        public int SummonCurrentHP { get { return this.summonCurrentHP; } set { this.summonCurrentHP = value; } }

    }
}
