namespace GameSystem.InGameUI.Skill
{
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
