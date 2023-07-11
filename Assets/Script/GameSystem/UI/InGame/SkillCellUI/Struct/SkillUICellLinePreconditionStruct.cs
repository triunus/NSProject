namespace GameSystem.InGameUI.Skill
{
    /// <summary>
    /// List<LinkedList<SkillUICellNumberPreconditionStruct>> ���·� ���ȴ�.
    /// List�� i�� SkillNumber�� �����ϰ� ����Ѵ�.
    /// ��, i ��° skillNumber ����(or ����)�� ������ skillNumber���� ǥ���ϴµ� ���ȴ�.
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