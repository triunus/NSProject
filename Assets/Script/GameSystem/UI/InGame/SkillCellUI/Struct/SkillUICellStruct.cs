namespace GameSystem.InGameUI.Skill
{
    // SkillUICell GameObject의 구성(역할?)을 나타낸다.
    public enum CellContent{
        main,
        sub,
        interchange,
        line,
        non
    }

    public struct SkillUICellStruct
    {
        private int cellNumber;             // CellNumber은 Skill UI Menu의 Skill Tree 안의 객체 1개를 뜻한다. ( 순서는 좌 -> 우, 상 -> 하 )
        private CellContent cellContent;    // SkillUICellStruct 에서, 자신의 역할을 나타낸다. ( 정점의 역할인지, 간선의 역할인지 등 )
        private int lineNumber;             // LineNumber 그래프의 간선의 종류를 나타내는데 사용한다.

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
