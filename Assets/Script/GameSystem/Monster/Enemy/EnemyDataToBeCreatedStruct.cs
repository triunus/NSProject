namespace GameSystem.Enemy
{
    public class EnemyDataToBeCreatedStruct
    {
        private Monster.MonsterName  monsterName;
        private int countToBeCreated;

        public Monster.MonsterName MonsterName { get { return this.monsterName; } set { this.monsterName = value; } }
        public int CountToBeCreated { get { return this.countToBeCreated; } set { this.countToBeCreated = value; } }

        public EnemyDataToBeCreatedStruct(Monster.MonsterName monsterName, int countToBeCreated)
        {
            this.monsterName = monsterName;
            this.countToBeCreated = countToBeCreated;
        }

    }
}
