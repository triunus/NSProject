namespace GameSystem.InGameUI.Skill
{
    /// <summary>
    ///     SkillNumber와 CellNumber 정보를 갖고 있다.
    ///     SkillUICell.main과 sub와 같이 '스킬' 객체를 명시하는 GameObject가 필요로 하는 정보이다.
    /// </summary>
    public struct SkillUICellMainSubStruct
    {
        private int skillNumber;
        private int cellNumber;

        public SkillUICellMainSubStruct(int skillNumber , int cellNumber)
        {
            this.skillNumber = skillNumber;
            this.cellNumber = cellNumber;
        }

        public int SkillNumber { get { return this.skillNumber; } set { this.skillNumber = value; } }
        public int CellNumber { get { return this.cellNumber; } set { this.cellNumber = value; } }
    }
}
