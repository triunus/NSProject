namespace GameSystem.InGameUI.Skill
{
    // SkillUICell GameObject�� ����(����?)�� ��Ÿ����.
    public enum CellContent{
        main,
        sub,
        interchange,
        line,
        non
    }

    public struct SkillUICellStruct
    {
        private int cellNumber;             // CellNumber�� Skill UI Menu�� Skill Tree ���� ��ü 1���� ���Ѵ�. ( ������ �� -> ��, �� -> �� )
        private CellContent cellContent;    // SkillUICellStruct ����, �ڽ��� ������ ��Ÿ����. ( ������ ��������, ������ �������� �� )
        private int lineNumber;             // LineNumber �׷����� ������ ������ ��Ÿ���µ� ����Ѵ�.

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
