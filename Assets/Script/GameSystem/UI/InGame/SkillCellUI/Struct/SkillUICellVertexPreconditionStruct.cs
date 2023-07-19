namespace GameSystem.InGameUI.Skill
{
    /// <summary>
    /// List<LinkedList<SkillUICellVertexAndWeightPreconditionStruct>> 형태로 사용된다.
    /// List의 i는 SkillNumber와 동일하게 사용한다.
    /// 즉, i 번째 skillNumber 다음(or 이전)에 나오는 skillNumber들을 표현하는데 사용된다.
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