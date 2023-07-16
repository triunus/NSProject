namespace GameSystem.InGameUI.Skill
{
    // PlayerSkillInformationStruct는 GAmeDataStruct에 포함되어 Local에 저장된다.
    [System.Serializable]
    public struct PlayerSkillInformationStruct
    {
        private int skillNumber;        // 특정 SkillNumber
        private int currentLevel;       // 의 Level

        public PlayerSkillInformationStruct(int skillNumber, int currentLevel)
        {
            this.skillNumber = skillNumber;
            this.currentLevel = currentLevel;
        }

        public int SkillNumber { get { return this.skillNumber; } set { this.skillNumber = value; } }
        public int CurrentLevel { get { return this.currentLevel; } set { this.currentLevel = value; } }
    }
}
