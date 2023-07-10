namespace GameSystem.AttackObject
{
    public class TrapObjectStruct
    {
        private PublicSkillStruct publicSkillStruct;

        private int damage = 0;
        private int hitCount = 0;

        private float inAttackTime = 0;
        private float durationTime = 0;

        private float installationRange = 0;

        public TrapObjectStruct(PublicSkillStruct publicSkillStruct)
        {
            this.publicSkillStruct = publicSkillStruct;

/*            switch (publicSkillStruct.SkillName)
            {
            }*/
        }

        public PublicSkillStruct PublicSkillStruct { get { return this.publicSkillStruct; } set { this.publicSkillStruct = value; } }

        public int Damage { get { return this.damage; } set { this.damage = value; } }
        public int HitCount { get { return this.hitCount; } set { this.hitCount = value; } }
        public float InAttackTime { get { return this.inAttackTime; } set { this.inAttackTime = value; } }
        public float DurationTime { get { return this.durationTime; } set { this.durationTime = value; } }
        public float InstallationRange { get { return this.installationRange; } set { this.installationRange = value; } }
    }
}