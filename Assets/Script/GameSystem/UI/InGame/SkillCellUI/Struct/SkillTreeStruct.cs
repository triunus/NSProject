namespace GameSystem.InGameUI.Skill
{
    // SkillTree�� ���ԵǴ� Cell���� ��� ���� ���´�.
    // SkillTreeStruct�� rowCount�� columnCount ����� ��� ���� ũ�⸦ ����Ѵ�.
    public struct SkillTreeStruct
    {
        private int rowCount;
        private int columnCount;
        private System.Collections.Generic.List<SkillUICellStruct> skillUICellInformation;

        public SkillTreeStruct(int rowCount = 0, int columnCount = 0)
        {
            this.rowCount = rowCount;
            this.columnCount = columnCount;
            this.skillUICellInformation = new System.Collections.Generic.List<SkillUICellStruct>();
        }

        public int RowCount { get { return this.rowCount; } set { this.rowCount = value; } }
        public int ColumnCount { get { return this.columnCount; } set { this.columnCount = value; } }
        public System.Collections.Generic.List<SkillUICellStruct> SkillUICellInformation { get { return this.skillUICellInformation; } }
    }
}