namespace GameSystem.InGameUI.Skill
{
    /// <summary>
///     List<LinkedList<SkillNumberPreconditionStruct>> 형태로 사용된다.
///     List의 i는 SkillNumber와 동일하게 사용한다.
///     즉, i 번째 skillNumber와 관계된 skillNumber 필요한와 필요 가중치 등을 표현하는데 사용된다.
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
