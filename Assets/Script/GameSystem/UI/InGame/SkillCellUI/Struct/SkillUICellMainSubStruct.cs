namespace GameSystem.InGameUI.Skill
{
    /// <summary>
    ///     SkillNumber�� CellNumber ������ ���� �ִ�.
    ///     SkillUICell.main�� sub�� ���� '��ų' ��ü�� ����ϴ� GameObject�� �ʿ�� �ϴ� �����̴�.
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
