namespace GameSystem.AttackObject
{
    public class ImmediateAttackObjectStruct
    {
        private PublicSkillStruct publicSkillStruct;

        private int damage = 0;
        private int hitCount = 0;

        private float inAttackTime = 0;

        public ImmediateAttackObjectStruct(PublicSkillStruct publicSkillStruct)
        {
            this.publicSkillStruct = publicSkillStruct;

            switch (publicSkillStruct.SkillName)
            {
                case SkillName.SwordSlash:
                    this.damage = 1;
                    this.hitCount = 3;

                    this.inAttackTime = 1f;
                    break;
                case SkillName.Lightning:
                    this.damage = 1;
                    this.hitCount = 5;

                    this.inAttackTime = 1f;
                    break;
            }
        }

        public PublicSkillStruct PublicSkillStruct { get { return this.publicSkillStruct; } set { this.publicSkillStruct = value; } }

        public int Damage { get { return this.damage; } set { this.damage = value; } }
        public int HitCount { get { return this.hitCount; } set { this.hitCount = value; } }
        public float InAttackTime { get { return this.inAttackTime; } set { this.inAttackTime = value; } }
    }
}
