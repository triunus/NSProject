namespace GameSystem.InGameUI.Skill
{
    public enum CellContent{
        main,
        sub,
        interchange,
        line,
        non
    }

    // SkillUICell.CellNumber�� �׷����� ������ ��Ÿ���µ� ����Ѵ�.
    // SkillUICell.Line�� �׷����� ������ ��Ÿ���µ� ����Ѵ�.
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