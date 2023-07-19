namespace GameSystem.InGameUI.Skill
{
    [System.Serializable]
    public struct PlayerSkillInformationStruct
    {
        private int skillNumber;
        private int currentLevel;

        public PlayerSkillInformationStruct(int skillNumber, int currentLevel)
        {
            this.skillNumber = skillNumber;
            this.currentLevel = currentLevel;
        }

        public int SkillNumber { get { return this.skillNumber; } set { this.skillNumber = value; } }
        public int CurrentLevel { get { return this.currentLevel; } set { this.currentLevel = value; } }
    }
}