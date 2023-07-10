using UnityEngine;

namespace GameSystem.Enemy
{
    public enum EnemyState
    {
        SpawnByTime,
        SpawnByLoadFile,
        SpawnByObject,
        Revive,
        Idle,
        Walk,
        Run,
        Hurt,
        Die,
        Attack
    }

    public interface IEnemyModel
    {
        public void RegisterEnemyView(IEnemyView enemyView);
        public void RemoveEnemyView(IEnemyView enemyView);

        public bool GetNextDirection(float enemyPositionX);
        public bool DetermineWhetherOrNotAttack(float attackRange, float enemyPositionX);

        public void SpawnAttackObject(AttackObject.PublicSkillStruct publicSkillStruct);
        public void SpawnCorpse(Monster.MonsterName MonsterName, Vector3 enemyPosition);
    }

    public class EnemyModel : MonoBehaviour, IEnemyModel
    {
        private IEnemyManagerForEnemyModel enemyManager;

        private float[] summonAndPlayerPositionX;
        private int minIndex;

        private void Awake()
        {
            this.enemyManager = GameObject.FindWithTag("EnemyManager").GetComponent<EnemyManager>();
        }

        private void FixedUpdate()
        {
            this.summonAndPlayerPositionX = this.enemyManager.GetSummonAndPlayerPositionX();
        }

        public void RegisterEnemyView(IEnemyView enemyView)
        {
            this.enemyManager.RegisterEnemyView(enemyView);
        }
        public void RemoveEnemyView(IEnemyView enemyView)
        {
            this.enemyManager.RemoveEnemyView(enemyView);
        }

        public bool GetNextDirection(float enemyPositionX)
        {
//            Debug.Log("EnemyModel - GetNextDirection : " + enemyPositionX);
            if (Mathf.Abs(this.summonAndPlayerPositionX[0] - enemyPositionX) < Mathf.Abs(this.summonAndPlayerPositionX[this.summonAndPlayerPositionX.Length - 1] - enemyPositionX))
            {
                // 0ºÎÅÍ Å½»ö -> ¿À¸§Â÷¼ø Å½»ö
                this.AscendingSearch(enemyPositionX);
//                Debug.Log("EnemyModel - GetNextDirection : " + this.summonAndPlayerPositionX[this.minIndex]);
                if (enemyPositionX > this.summonAndPlayerPositionX[this.minIndex]) return true;
                else return false;
            }
            else
            {
                // enemyPositionX.lenght ºÎÅÍ Å½»ö -> ³»¸²Â÷¼ø Å½»ö
                this.DescendingSearch(enemyPositionX);
//                Debug.Log("EnemyModel - GetNextDirection : " + this.summonAndPlayerPositionX[this.minIndex]);
                if (enemyPositionX > this.summonAndPlayerPositionX[this.minIndex]) return true;
                else return false;
            }
        }
        public bool DetermineWhetherOrNotAttack(float attackRange, float enemyPositionX)
        {
//            Debug.Log("EnemyModel - DetermineWhetherOrNotAttack : " + this.summonAndPlayerPositionX[this.minIndex]);
            if (Mathf.Abs(this.summonAndPlayerPositionX[this.minIndex] - enemyPositionX) < attackRange) return true;
            else return false;
        }

        public void SpawnAttackObject(AttackObject.PublicSkillStruct publicSkillStruct)
        {
            this.enemyManager.SpawnAttackObject(publicSkillStruct);
        }
        public void SpawnCorpse(Monster.MonsterName MonsterName, Vector3 enemyPosition)
        {
            this.enemyManager.SpawnCorpse(MonsterName, enemyPosition);
        }

        private void AscendingSearch(float enemyPositionX)
        {
            int minIndex = 0;
            float minValue = Mathf.Abs(this.summonAndPlayerPositionX[0] - enemyPositionX);
            float tempValue = 0;

            for (int i = 1; i < this.summonAndPlayerPositionX.Length; ++i)
            {
                tempValue = Mathf.Abs(this.summonAndPlayerPositionX[i] - enemyPositionX);

                if (minValue > tempValue)
                {
                    minIndex = i;
                    minValue = tempValue;
                }
                else break;
            }
            this.minIndex = minIndex;
        }
        private void DescendingSearch(float enemyPositionX)
        {
            int minIndex = this.summonAndPlayerPositionX.Length - 1;
            float minValue = Mathf.Abs(this.summonAndPlayerPositionX[this.summonAndPlayerPositionX.Length - 1] - enemyPositionX);
            float tempValue = 0;

            for (int i = this.summonAndPlayerPositionX.Length - 2; i >= 0; --i)
            {
                tempValue = Mathf.Abs(this.summonAndPlayerPositionX[i] - enemyPositionX);

                if (minValue > tempValue)
                {
                    minIndex = i;
                    minValue = tempValue;
                }
                else break;
            }
            this.minIndex = minIndex;
        }
    }
}
