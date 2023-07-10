namespace GameSystem.Monster
{
    public enum MonsterName
    {
        SkeletonWarrior = 0,
        SkeletonArcher = 1
    }

    public enum MonsterAttackType
    {
        MeleeAttack,
        RangeAttack
    }

    public class MonsterStruct
    {
        private MonsterName monsterName;
        private MonsterAttackType monsterAttackType;

        private int baseHP;
        private float walkSpeed;
        private float runSpeed;
        private float hurtSpeed;

        private float attackRange;

        private AttackObject.SkillName normalAttack;
        private AttackObject.SkillName skillName01;
        private AttackObject.SkillName skillName02;

        private int revivalCost;
        private int extractionProfit;
        public MonsterStruct(MonsterName monsterName)
        {
            this.monsterName = monsterName;
            this.SetSummonData();
        }

        private void SetSummonData()
        {
            switch (this.monsterName)
            {
                case MonsterName.SkeletonWarrior:
                    this.baseHP = 3;
                    this.walkSpeed = 1f;
                    this.runSpeed = 3f;
                    this.hurtSpeed = 2f;
                    this.attackRange = 2f;

                    this.monsterAttackType = MonsterAttackType.MeleeAttack;
                    this.normalAttack = AttackObject.SkillName.SwordSlash;

                    this.revivalCost = -2;
                    this.extractionProfit = 1;
                    break;
                case MonsterName.SkeletonArcher:
                    this.baseHP = 2;
                    this.walkSpeed = 1f;
                    this.runSpeed = 2f;
                    this.hurtSpeed = 2f;
                    this.attackRange = 4f;

                    this.monsterAttackType = MonsterAttackType.RangeAttack;
                    this.normalAttack = AttackObject.SkillName.ArrowShooting;

                    this.revivalCost = -3;
                    this.extractionProfit = 1;
                    break;
                default:
                    break;
            }
        }

        public MonsterName MonsterName { get { return this.monsterName; } }
        public MonsterAttackType MonsterAttackType { get { return this.monsterAttackType; } }

        public int BaseHP { get { return this.baseHP; } }
        public float WalkSpeed { get { return this.walkSpeed; } }
        public float RunSpeed { get { return this.runSpeed; } }
        public float HurtSpeed { get { return this.hurtSpeed; } }
        public float AttackRange { get { return this.attackRange; } }

        public AttackObject.SkillName NormalAttack { get { return this.normalAttack; } }
        public AttackObject.SkillName SkillName01 { get { return this.skillName01; } }
        public AttackObject.SkillName SkillName02 { get { return this.skillName02; } }

        public int RevivalCost { get { return this.revivalCost; } }
        public int ExtractionProfit { get { return this.extractionProfit; } }
    }
}
