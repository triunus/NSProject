namespace GameSystem.InGameUI.Skill
{
    /// <summary>
    ///     List<LinkedList<SkillNumberPreconditionStruct>> ���·� ���ȴ�.
    ///     List�� i�� SkillNumber�� ������ �����ϴ�.
    /// </summary>
    public struct SkillNumberPreconditionStruct
    {
        private int preVertex;              // List�� i��° SkillNumber�� ���� ���� �н��� �ʿ��� ���� SkillNumber.
        private int preVertex_weight;       // ���� SkillNumber�� ���߾�ߵǴ� Level.

        public SkillNumberPreconditionStruct(int preVertex, int preVertex_weight)
        {
            this.preVertex = preVertex;
            this.preVertex_weight = preVertex_weight;
        }

        public int PreVertex { get { return this.preVertex; } set { this.preVertex = value; } }
        public int PreVertex_weight { get { return this.preVertex_weight; } set { this.preVertex_weight = value; } }
    }
}
