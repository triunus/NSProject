namespace GameSystem.AttackObject
{
    public class ProjectileObjectStruct
    {
        private PublicSkillStruct publicSkillStruct;

        private int damage = 0;
        private int hitCount = 0;
        private float speed = 0;

        private float durationTime = 0;

        private float limitedRagne = 0;

        public ProjectileObjectStruct(PublicSkillStruct publicSkillStruct)
        {
            this.publicSkillStruct = publicSkillStruct;

            switch (publicSkillStruct.SkillName)
            {
                case SkillName.ArrowShooting:
                    this.damage = 1;
                    this.hitCount = 1;

                    this.speed = 3f;
                    this.durationTime = 3f;
                    this.limitedRagne = 6f;
                    break;
                case SkillName.LightningBalt:
                    this.damage = 1;
                    this.hitCount = 1;

                    this.speed = 5f;
                    this.durationTime = 3f;
                    this.limitedRagne = 8f;
                    break;
            }
        }

        public PublicSkillStruct PublicSkillStruct { get { return this.publicSkillStruct; } set { this.publicSkillStruct = value; } }

        public int Damage { get { return this.damage; } set { this.damage = value; } }
        public int HitCount { get { return this.hitCount; } set { this.hitCount = value; } }
        public float Speed { get { return this.speed; } set { this.speed = value; } }
        public float DurationTime { get { return this.durationTime; } set { this.durationTime = value; } }
        public float LimitedRagne { get { return this.limitedRagne; } set { this.limitedRagne = value; } }
    }
}