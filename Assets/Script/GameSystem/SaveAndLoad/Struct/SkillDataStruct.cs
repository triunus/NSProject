namespace GameSystem.SaveAndLoad
{
    [System.Serializable]
    public struct SkillDataStruct
    {
        private System.Collections.Generic.List<InGameUI.Skill.PlayerSkillInformationStruct> skillData;

        public SkillDataStruct(int defalut = 0)
        {
            this.skillData = new System.Collections.Generic.List<InGameUI.Skill.PlayerSkillInformationStruct>();
        }

        public System.Collections.Generic.List<InGameUI.Skill.PlayerSkillInformationStruct> SkillData
        {
            get
            {
                return skillData.ConvertAll(tmp => new InGameUI.Skill.PlayerSkillInformationStruct(tmp.SkillNumber, tmp.CurrentLevel));
            }
            set
            {
                this.skillData = value.ConvertAll(tmp => new InGameUI.Skill.PlayerSkillInformationStruct(tmp.SkillNumber, tmp.CurrentLevel));
            }
        }

        public int GetSkillDataCount()
        {
            if (this.skillData is null) return 0;
            return this.skillData.Count;
        }

        /*        public System.Collections.Generic.List<InGameUI.Skill.PlayerSkillInformationStruct> GetSkillData()
                {
                    return skillData.ConvertAll(tmp => new InGameUI.Skill.PlayerSkillInformationStruct(tmp.SkillNumber, tmp.CurrentLevel));
                }*/
        /*        public void SetSkillData(System.Collections.Generic.List<InGameUI.Skill.PlayerSkillInformationStruct> playerSkillInformationStruct)
                {
                    this.skillData = playerSkillInformationStruct;
                }*/
    }
}