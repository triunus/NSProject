namespace GameSystem.InGameUI.Skill
{
    public enum CellContent{
        main,
        sub,
        interchange,
        line,
        non
    }

    public struct SkillTreeStruct
    {
        private int rowCount;
        private int columnCount;
        private System.Collections.Generic.List<SkillUICellStruct> skillUICellInforamtion;

        public SkillTreeStruct(int rowCount, int columnCount)
        {
            this.rowCount = rowCount;
            this.columnCount = columnCount;
            this.skillUICellInforamtion = new System.Collections.Generic.List<SkillUICellStruct>();
        }

        public int RowCount { get { return this.rowCount; } set { this.rowCount = value; } }
        public int ColumnCount { get { return this.columnCount; } set { this.columnCount = value; } }
        public System.Collections.Generic.List<SkillUICellStruct> SkillUICellInforamtion { get { return this.skillUICellInforamtion; } }
    }

    // SkillUICell.cellNumber�� ��->��, ��->�Ϸ� ������ Cell�� ������ ��Ÿ���µ� ����Ѵ�.
    // SkillUICell.line�� �׷����� ������ ��Ÿ���µ� ����Ѵ�.
    // SkillUICell.cellContent�� Cell�� ����(����)�� ������ �� ����Ѵ�.
    public struct SkillUICellStruct
    {
        private int cellNumber;
        private CellContent cellContent;
        private int lineNumber;

        public SkillUICellStruct(int cellNumber, CellContent cellContent = CellContent.non, int lineNumber = 0)
        {
            this.cellNumber = cellNumber;
            this.cellContent = cellContent;
            this.lineNumber = lineNumber;
        }

        public int CellNumber { get { return this.cellNumber; } set { this.cellNumber = value; } }
        public CellContent CellContent { get { return this.cellContent; } set { this.cellContent = value; } }
        public int LineNumber { get { return this.lineNumber; } set { this.lineNumber = value; } }
    }
}
