namespace GameSystem.InGameUI.Skill
{
    /// <summary>
    /// List<LinkedList<SkillUICellVertexAndWeightPreconditionStruct>> ���·� ���ȴ�.
    /// List�� i�� SkillNumber�� �����ϰ� ����Ѵ�.
    /// ��, i ��° skillNumber ����(or ����)�� ������ skillNumber���� ǥ���ϴµ� ���ȴ�.
    /// </summary>
    public struct SkillUICellVertexAndWeightPreconditionStruct
    {
        private int nextVertex;
        private int preVertex_weight;

        public SkillUICellVertexAndWeightPreconditionStruct(int nextVertex, int preVertex_weight = 0)
        {
            this.nextVertex = nextVertex;
            this.preVertex_weight = preVertex_weight;
        }

        public int NextVertex { get { return this.nextVertex; } set { this.nextVertex = value; } }
        public int PreVertex_weight { get { return this.preVertex_weight; } set { this.preVertex_weight = value; } }
    }
}