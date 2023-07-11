namespace GameSystem.InGameUI.Skill
{
    /// <summary>
///     List<LinkedList<SkillUICellMSPreconditionStruct>> 형태로 사용된다.
///     List의 i는 SkillNumber와 동일하게 사용한다.
///     즉, i 번째 skillNumber와 관계된 skillNumber 필요한와 필요 가중치 등을 표현하는데 사용된다.
/// </summary>
    public struct SkillUICellMSPreconditionStruct
    {
        private int precondition_q;
        private int precondition_Weight;

        public SkillUICellMSPreconditionStruct(int precondition_q, int precondition_Weight)
        {
            this.precondition_q = precondition_q;
            this.precondition_Weight = precondition_Weight;
        }

        public int Precondition_q { get { return this.precondition_q; } set { this.precondition_q = value; } }
        public int Precondition_Weight { get { return this.precondition_Weight; } set { this.precondition_Weight = value; } }
    }
}
