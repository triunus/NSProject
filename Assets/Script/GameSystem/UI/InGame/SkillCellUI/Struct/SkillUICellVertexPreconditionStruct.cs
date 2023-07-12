namespace GameSystem.InGameUI.Skill
{
    /// <summary>
    /// List<LinkedList<SkillUICellVertexPreconditionStruct>> 형태로 사용된다.
    /// List의 i는 SkillNumber와 동일하게 사용한다.
    /// 즉, i 번째 skillNumber 다음(or 이전)에 나오는 skillNumber들을 표현하는데 사용된다.
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