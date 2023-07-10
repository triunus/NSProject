using UnityEngine;

namespace GameSystem.Summon
{
    public interface ISummonPresenter
    {
        public void RegisterSummonView();
        public void RemoveSummonView();

        public void GetNextDirection();
        public bool DetermineWhetherOrNotAttack();
        public bool RecognizeTheEnemy();
        public void GetPlayerState();
        public float GetPlayerPositionX();

        public void SpawnNormalAttack();
        public void SpawnSkill01();
        public void SpawnSkill02();

        public void SpawnCorpse();
    }

    public class SummonPresenter : ISummonPresenter
    {
        private ISummonModel summonModel;
        private ISummonView summonView;

        public SummonPresenter(ISummonView summonView)
        {
            this.summonModel = GameObject.FindWithTag("SummonManager").GetComponent<SummonModel>();
            this.summonView = summonView;
        }

        public void RegisterSummonView()
        {
            this.summonModel.RegisterSummonView(this.summonView);
        }
        public void RemoveSummonView()
        {
            this.summonModel.RemoveSummonView(this.summonView);
        }

        public void GetNextDirection()
        {
            this.summonView.NextDirection = this.summonModel.GetNextDirection(this.summonView.SummonPosition.x);
        }
        public bool DetermineWhetherOrNotAttack()
        {
            return this.summonModel.DetermineWhetherOrNotAttack(this.summonView.MonsterStruct.AttackRange, this.summonView.SummonPosition.x);
        }
        public bool RecognizeTheEnemy()
        {
            return this.summonModel.RecognizeTheEnemy(this.summonView.SummonPosition.x);
        }
        public void GetPlayerState()
        {
            this.summonView.PlayerState = this.summonModel.GetPlayerState();
        }
        public float GetPlayerPositionX()
        {
            return this.summonModel.GetPlayerPositionX();
        }

        public void SpawnNormalAttack()
        {
            AttackObject.PublicSkillStruct publicSkillStruct = new AttackObject.PublicSkillStruct(this.summonView.MonsterStruct.NormalAttack, this.summonView.SummonPosition, this.summonView.NextDirection, this.summonView.SummonTag);
            this.summonModel.SpawnAttackObject(publicSkillStruct);
        }
        public void SpawnSkill01()
        {
            AttackObject.PublicSkillStruct publicSkillStruct = new AttackObject.PublicSkillStruct(this.summonView.MonsterStruct.SkillName01, this.summonView.SummonPosition, this.summonView.NextDirection, this.summonView.SummonTag);
            this.summonModel.SpawnAttackObject(publicSkillStruct);
        }
        public void SpawnSkill02()
        {
            AttackObject.PublicSkillStruct publicSkillStruct = new AttackObject.PublicSkillStruct(this.summonView.MonsterStruct.SkillName02, this.summonView.SummonPosition, this.summonView.NextDirection, this.summonView.SummonTag);
            this.summonModel.SpawnAttackObject(publicSkillStruct);
        }

        public void SpawnCorpse()
        {
            this.summonModel.SpawnCorpse(this.summonView.MonsterStruct.MonsterName, this.summonView.SummonPosition);
        }
    }
}
