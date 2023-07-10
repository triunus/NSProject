namespace GameSystem.InGameUI.Skill
{
    public struct SkillInformationStruct
    {
        private int skillNumber;
        private string skillName;
        private string skillDescription;
        private int maxLevel;
        private int cost;

        public SkillInformationStruct(int skillNumber, string skillName, string skillDescription, int maxLevel, int cost)
        {
            this.skillNumber = skillNumber;
            this.skillName = skillName;
            this.skillDescription = skillDescription;
            this.maxLevel = maxLevel;
            this.cost = cost;
        }

        public int SkillNumber { get { return this.skillNumber; } set { this.skillNumber = value; } }
        public string SkillName { get { return this.skillName; } set { this.skillName = value; } }
        public string SkillDescription { get { return this.skillDescription; } set { this.skillDescription = value; } }
        public int MaxLevel { get { return this.maxLevel; } set { this.maxLevel = value; } }
        public int Cost {  get { return this.cost; } set { this.cost = value; } }
    }
}
