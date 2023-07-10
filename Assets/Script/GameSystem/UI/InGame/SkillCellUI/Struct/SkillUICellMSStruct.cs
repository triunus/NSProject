namespace GameSystem.InGameUI.Skill
{
    public struct SkillUICellMSStruct
    {
        private int skillNumber;
        private int cellNumber;

        public SkillUICellMSStruct(int skillNumber , int cellNumber)
        {
            this.skillNumber = skillNumber;
            this.cellNumber = cellNumber;
        }

        public int SkillNumber { get { return this.skillNumber; } set { this.skillNumber = value; } }
        public int CellNumber { get { return this.cellNumber; } set { this.cellNumber = value; } }
    }
}
