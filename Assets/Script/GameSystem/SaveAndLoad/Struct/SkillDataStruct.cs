namespace GameSystem.SaveAndLoad
{
    [System.Serializable]
    public struct SkillDataStruct
    {
        private System.Collections.Generic.List<InGameUI.Skill.PlayerSkillInformationStruct> playerSkillData;

        public SkillDataStruct(int defalut = 0)
        {
            this.playerSkillData = new System.Collections.Generic.List<InGameUI.Skill.PlayerSkillInformationStruct>();
        }

        public System.Collections.Generic.List<InGameUI.Skill.PlayerSkillInformationStruct> PlayerSkillData
        {
            get { return this.playerSkillData; }
            set { this.playerSkillData = value; }
        }

        public int GetSkillDataCount()
        {
            if (this.playerSkillData.Count == 0) return 0;
            else return this.playerSkillData.Count;
        }
    }
}