using System;

namespace GameSystem.SaveAndLoad
{
    [Serializable]
    public class EnemyDataStruct
    {
        System.Collections.Generic.List<EnemyData> enemyDatas = null;

        private bool beCreated;

        public EnemyDataStruct(bool beCreated)
        {
            this.enemyDatas = new System.Collections.Generic.List<EnemyData>();
            this.beCreated = beCreated;
        }

        public bool BeCreated { get { return this.beCreated; } set { this.beCreated = value; } }

        public EnemyData GetEnemyData(int index)
        {
            return this.enemyDatas[index];
        }
        public int GetEnemyDataCount()
        {
            if (this.enemyDatas is null) return 0;
            return this.enemyDatas.Count;
        }
        public void SetEnemyData(EnemyData enemyData)
        {
            this.enemyDatas.Add(enemyData);
        }
        public void RemoveEnemyData(EnemyData enemyData)
        {
            this.enemyDatas.Remove(enemyData);
        }
    }

    [Serializable]
    public class EnemyData
    {
        private Monster.MonsterName monsterName;
        private float enemyPositionX;
        private float enemyPositionY;
        private int enemyCurrentHP;

        public EnemyData() { }

        public Monster.MonsterName MonsterName { get { return this.monsterName; } set { this.monsterName = value; } }
        public float EnemyPositionX { get { return this.enemyPositionX; } set { this.enemyPositionX = value; } }
        public float EnemyPositionY { get { return this.enemyPositionY; } set { this.enemyPositionY = value; } }
        public int EnemyCurrentHP { get { return this.enemyCurrentHP; } set { this.enemyCurrentHP = value; } }
    }
}