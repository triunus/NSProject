namespace GameSystem.InGameUI.Skill
{
    /// <summary>
    ///     List<LinkedList<SkillUICellVertexAndWeightPreconditionStruct>> ���·� ���ȴ�.
    ///     List�� i�� SkillNumber�� ������ �����ϴ�.
    /// </summary>
    public struct SkillUICellVertexAndWeightPreconditionStruct
    {
        private int nextVertex;         // List�� i��° SkillNumber �������� �� �� �ִ� SkillNumber.
        private int preVertex_weight;   // �������� �� �� �ִ� SkillNumber�� ���� ����, List�� i ��° SkillNumber�� ���߾�ߵǴ� Level.

        public SkillUICellVertexAndWeightPreconditionStruct(int nextVertex, int preVertex_weight = 0)
        {
            this.nextVertex = nextVertex;
            this.preVertex_weight = preVertex_weight;
        }

        public int NextVertex { get { return this.nextVertex; } set { this.nextVertex = value; } }
        public int PreVertex_weight { get { return this.preVertex_weight; } set { this.preVertex_weight = value; } }
    }
}
