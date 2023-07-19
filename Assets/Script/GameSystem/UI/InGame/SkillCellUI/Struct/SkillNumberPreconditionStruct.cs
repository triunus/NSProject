namespace GameSystem.InGameUI.Skill
{
    /// <summary>
///     List<LinkedList<SkillNumberPreconditionStruct>> ���·� ���ȴ�.
///     List�� i�� SkillNumber�� �����ϰ� ����Ѵ�.
///     ��, i ��° skillNumber�� ����� skillNumber �ʿ��ѿ� �ʿ� ����ġ ���� ǥ���ϴµ� ���ȴ�.
/// </summary>
    public struct SkillNumberPreconditionStruct
    {
        private int preVertex;
        private int preVertex_weight;

        public SkillNumberPreconditionStruct(int preVertex, int preVertex_weight)
        {
            this.preVertex = preVertex;
            this.preVertex_weight = preVertex_weight;
        }

        public int PreVertex { get { return this.preVertex; } set { this.preVertex = value; } }
        public int PreVertex_weight { get { return this.preVertex_weight; } set { this.preVertex_weight = value; } }
    }
}
