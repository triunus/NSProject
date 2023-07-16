namespace GameSystem.InGameUI.Skill
{
    /// <summary>
    ///     List<LinkedList<SkillNumberPreconditionStruct>> 형태로 사용된다.
    ///     List의 i는 SkillNumber의 순서와 동일하다.
    /// </summary>
    public struct SkillNumberPreconditionStruct
    {
        private int preVertex;              // List의 i번째 SkillNumber를 배우기 위해 학습이 필요한 이전 SkillNumber.
        private int preVertex_weight;       // 이전 SkillNumber가 갖추어야되는 Level.

        public SkillNumberPreconditionStruct(int preVertex, int preVertex_weight)
        {
            this.preVertex = preVertex;
            this.preVertex_weight = preVertex_weight;
        }

        public int PreVertex { get { return this.preVertex; } set { this.preVertex = value; } }
        public int PreVertex_weight { get { return this.preVertex_weight; } set { this.preVertex_weight = value; } }
    }
}
