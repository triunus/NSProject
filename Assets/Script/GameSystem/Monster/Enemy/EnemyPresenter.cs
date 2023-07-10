using UnityEngine;

namespace GameSystem.Enemy
{
    public interface IEnemyPresenter
    {
        public void RegisterEnemyView();
        public void RemoveEnemyView();
        public void GetNextDirection();
        public bool DetermineWhetherOrNotAttack();
        public void SpawnNormalAttack();
        public void SpawnCorpse();
    }

    public class EnemyPresenter : IEnemyPresenter
    {
        private IEnemyModel enemyModel;
        private IEnemyView enemyView;
        public EnemyPresenter(IEnemyView enemyView)
        {
            this.enemyModel = GameObject.FindWithTag("EnemyManager").GetComponent<EnemyModel>();
            this.enemyView = enemyView;
        }
        public void RegisterEnemyView()
        {
            this.enemyModel.RegisterEnemyView(this.enemyView);
        }
        public void RemoveEnemyView()
        {
            this.enemyModel.RemoveEnemyView(this.enemyView);
        }
        public void GetNextDirection()
        {
            this.enemyView.NextDirection = this.enemyModel.GetNextDirection(this.enemyView.EnemyPosition.x);
        }
        public bool DetermineWhetherOrNotAttack()
        {
            return this.enemyModel.DetermineWhetherOrNotAttack(this.enemyView.MonsterStruct.AttackRange, this.enemyView.EnemyPosition.x);
        }
        public void SpawnNormalAttack()
        {
            AttackObject.PublicSkillStruct publicSkillStruct = new AttackObject.PublicSkillStruct(this.enemyView.MonsterStruct.NormalAttack,
                this.enemyView.EnemyPosition, this.enemyView.NextDirection, this.enemyView.EnemyTag);
            this.enemyModel.SpawnAttackObject(publicSkillStruct);
        }
        public void SpawnCorpse()
        {
            this.enemyModel.SpawnCorpse(this.enemyView.MonsterStruct.MonsterName, this.enemyView.EnemyPosition);
        }
    }

}