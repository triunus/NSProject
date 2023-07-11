namespace GameSystem.InGameUI.Skill
{
    /// <summary>
    /// List<LinkedList<SkillUICellNumberPreconditionStruct>> 형태로 사용된다.
    /// List의 i는 SkillNumber와 동일하게 사용한다.
    /// 즉, i 번째 skillNumber 다음(or 이전)에 나오는 skillNumber들을 표현하는데 사용된다.
    /// </summary>
    public struct SkillUICellNumberPreconditionStruct
    {
        private int precondition_q;

        public SkillUICellNumberPreconditionStruct(int precondition_q)
        {
            this.precondition_q = precondition_q;
        }

        public int Precondition_q { get { return this.precondition_q; } set { this.precondition_q = value; } }


    }
}