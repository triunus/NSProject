namespace GameSystem.InGameUI.Skill
{
    /// <summary>
    ///     List<LinkedList<SkillUICellVertexAndWeightPreconditionStruct>> 형태로 사용된다.
    ///     List의 i는 SkillNumber의 순서와 동일하다.
    /// </summary>
    public struct SkillUICellVertexAndWeightPreconditionStruct
    {
        private int nextVertex;         // List의 i번째 SkillNumber 다음으로 갈 수 있는 SkillNumber.
        private int preVertex_weight;   // 다음으로 갈 수 있는 SkillNumber로 가기 위해, List의 i 번째 SkillNumber가 갖추어야되는 Level.

        public SkillUICellVertexAndWeightPreconditionStruct(int nextVertex, int preVertex_weight = 0)
        {
            this.nextVertex = nextVertex;
            this.preVertex_weight = preVertex_weight;
        }

        public int NextVertex { get { return this.nextVertex; } set { this.nextVertex = value; } }
        public int PreVertex_weight { get { return this.preVertex_weight; } set { this.preVertex_weight = value; } }
    }
}
