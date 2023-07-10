namespace GameSystem.AttackObject
{
    public class PublicSkillStruct
    {
        private SkillName skillName;
        private SkillType skillType;
        private UnityEngine.Vector3 startPosition;
        private bool moveDirection;
        private string attackObjectTag;
        private int attackObjectLayer;

        public PublicSkillStruct(SkillName skillName, UnityEngine.Vector3 startPosition, bool moveDirection, string ownerTag)
        {
            switch(skillName)
            {
                case SkillName.Lightning:
                    this.skillName = skillName;
                    this.skillType = SkillType.ImmediateAttack;
                    this.startPosition = new UnityEngine.Vector3((moveDirection == true ? -1f : 1f) * 0.85f, 0.3f, 0) + startPosition;
                    this.moveDirection = moveDirection;
                    this.DecideAttackObjectTag(ownerTag);
                    this.DecideAttackObjectLayer(ownerTag);
                    break;
                case SkillName.SwordSlash:
                    this.skillName = skillName;
                    this.skillType = SkillType.ImmediateAttack;
                    this.startPosition = new UnityEngine.Vector3((moveDirection == true ? -1f : 1f) * 1.4f, 0.3f, 0) + startPosition;
                    this.moveDirection = moveDirection;
                    this.DecideAttackObjectTag(ownerTag);
                    this.DecideAttackObjectLayer(ownerTag);
                    break;
                case SkillName.LightningBalt:
                    this.skillName = skillName;
                    this.skillType = SkillType.Projectile;
                    this.startPosition = new UnityEngine.Vector3((moveDirection == true ? -1f : 1f) * 0.5f, 0.38f, 0) + startPosition;
                    this.moveDirection = moveDirection;
                    this.DecideAttackObjectTag(ownerTag);
                    this.DecideAttackObjectLayer(ownerTag);
                    break;
                case SkillName.ArrowShooting:
                    this.skillName = skillName;
                    this.skillType = SkillType.Projectile;
                    this.startPosition = new UnityEngine.Vector3((moveDirection == true ? -1f : 1f) * 0.88f, 0.34f, 0) + startPosition;
                    this.moveDirection = moveDirection;
                    this.DecideAttackObjectTag(ownerTag);
                    this.DecideAttackObjectLayer(ownerTag);
                    break;
            }
        }

        private void DecideAttackObjectTag(string ownerTag)
        {
            if (string.Equals(ownerTag, "Enemy"))
            {
                this.attackObjectTag = "EnemyAttack";
            }
            else if (string.Equals(ownerTag, "Summon"))
            {
                this.attackObjectTag = "SummonAttack";
            }
            else if (string.Equals(ownerTag, "Player"))
            {
                this.attackObjectTag = "PlayerAttack";
            }
            else
            {
                this.attackObjectTag = "NonCollisionObject";
            }
        }
        private void DecideAttackObjectLayer(string ownerTag)
        {
            if (string.Equals(ownerTag, "Enemy"))
            {
                this.attackObjectLayer = 12;
            }
            else if (string.Equals(ownerTag, "Summon"))
            {
                this.attackObjectLayer = 13;
            }
            else if (string.Equals(ownerTag, "Player"))
            {
                this.attackObjectLayer = 11;
            }
            else
            {
                this.attackObjectLayer = 14;
            }
        }

        public SkillName SkillName { get { return this.skillName; } set { this.skillName = value; } }
        public SkillType SkillType { get { return this.skillType; } set { this.skillType = value; } }
        public UnityEngine.Vector3 StartPosition { get { return this.startPosition; } set { this.startPosition = value; } }
        public bool MoveDirection { get { return this.moveDirection; } set { this.moveDirection = value; } }
        public string AttackObjectTag { get { return this.attackObjectTag; } set { this.attackObjectTag = value; } }
        public int AttackObjectLayer { get { return this.attackObjectLayer; } set { this.attackObjectLayer = value; } }

    }
}
