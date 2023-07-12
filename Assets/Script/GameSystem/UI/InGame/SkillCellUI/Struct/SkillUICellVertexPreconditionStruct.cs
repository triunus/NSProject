namespace GameSystem.InGameUI.Skill
{
    /// <summary>
    /// List<LinkedList<SkillUICellVertexPreconditionStruct>> ���·� ���ȴ�.
    /// List�� i�� SkillNumber�� �����ϰ� ����Ѵ�.
    /// ��, i ��° skillNumber ����(or ����)�� ������ skillNumber���� ǥ���ϴµ� ���ȴ�.
    /// </summary>
    public struct SkillUICellVertexPreconditionStruct
    {
        private int nextVertex;

        public SkillUICellVertexPreconditionStruct(int nextVertex)
        {
            this.nextVertex = nextVertex;
        }

        public int NextVertex { get { return this.nextVertex; } set { this.nextVertex = value; } }
    }
}