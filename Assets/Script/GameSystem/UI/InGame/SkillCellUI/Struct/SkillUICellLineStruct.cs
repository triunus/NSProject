namespace GameSystem.InGameUI.Skill
{
    public enum CellContent{
        main,
        sub,
        interchange,
        line,
        non
    }

    // SkillUICell.CellNumber은 그래프의 정점을 나타내는데 사용한다.
    // SkillUICell.Line은 그래프의 간선을 나타내는데 사용한다.
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
