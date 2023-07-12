using UnityEngine;

namespace GameSystem.Summon
{
    public enum SummonState
    {
        SpawnByRevival,
        SpawnByLoadFile,
        SpawnByObject,
        Rearrange,
        Idle,
        Walk,
        Run,
        Hurt,
        Die,
        Attack,
        Skill01,
        Skill02
    }

    public interface ISummonModel
    {
        public void RegisterSummonView(ISummonView summonView);
        public void RemoveSummonView(ISummonView summonView);

        public bool GetNextDirection(float summonPositionX);
        public bool DetermineWhetherOrNotAttack(float attackRange, float summonPositionX);
        public bool RecognizeTheEnemy(float summonPositionX);
        public Player.PlayerState GetPlayerState();
        public float GetPlayerPositionX();

        public void SpawnAttackObject(AttackObject.PublicSkillStruct publicSkillStruct);
        public void SpawnCorpse(Monster.MonsterName MonsterName, Vector3 summonPosition);
    }

    public class SummonModel : MonoBehaviour, ISummonModel
    {
        private ISummonManagerForSummonModel summonManager;

        private float recognizeRange = 6f;

        private float[] enemyPositionX;
        private int minIndex;

        private void Awake()
        {
            this.summonManager = GameObject.FindWithTag("SummonManager").GetComponent<SummonManager>();
        }
        private void FixedUpdate()
        {
            this.enemyPositionX = this.summonManager.GetEnemyPositionX();
        }

        // SummonView�� Manager�� ����ϱ� ���� ���.
        public void RegisterSummonView(ISummonView summonView)
        {
            this.summonManager.RegisterSummonView(summonView);
        }
        public void RemoveSummonView(ISummonView summonView)
        {
            this.summonManager.RemoveSummonView(summonView);
        }

        // 3. ISummonModel ����
        public bool GetNextDirection(float summonPositionX)
        {
            bool direction;

            if (this.enemyPositionX.Length == 0)
            {
                direction = this.summonManager.GetMoveDirection();
                return direction;
            }

            if(Mathf.Abs(this.enemyPositionX[0] - summonPositionX) < Mathf.Abs(this.enemyPositionX[this.enemyPositionX.Length-1] - summonPositionX))
            {
                // 0���� Ž�� -> �������� Ž��
                this.AscendingSearch(summonPositionX);
//                Debug.Log("SummonModel - GetNextDirection : " + this.enemyPositionX[this.minIndex]);
                if (summonPositionX > this.enemyPositionX[this.minIndex]) return true;
                else return false;
            }
            else
            {
                // enemyPositionX.lenght ���� Ž�� -> �������� Ž��
                this.DescendingSearch(summonPositionX);
//                Debug.Log("SummonModel - GetNextDirection : " + this.enemyPositionX[this.minIndex]);
                if (summonPositionX > this.enemyPositionX[this.minIndex]) return true;
                else return false;
            }
        }
        public bool DetermineWhetherOrNotAttack(float attackRange, float summonPositionX)
        {
            if (enemyPositionX.Length == 0) return false;

            if (Mathf.Abs(this.enemyPositionX[this.minIndex] - summonPositionX) < attackRange) return true;
            else return false;
        }
        public bool RecognizeTheEnemy(float summonPositionX)
        {
            if (enemyPositionX.Length == 0) return false;

            if (Mathf.Abs(this.enemyPositionX[this.minIndex] - summonPositionX) < recognizeRange) return true;
            else return false;
        }
        public Player.PlayerState GetPlayerState()
        {
            return this.summonManager.GetPlayerState();
        }
        public float GetPlayerPositionX()
        {
            return this.summonManager.GetPlayerPosition().x;
        }

        public void SpawnAttackObject(AttackObject.PublicSkillStruct publicSkillStruct)
        {
            this.summonManager.SpawnAttackObject(publicSkillStruct);
        }
        public void SpawnCorpse(Monster.MonsterName MonsterName, Vector3 summonPosition)
        {
            this.summonManager.SpawnCorpse(MonsterName, summonPosition);
        }

        private void AscendingSearch(float summonPositionX)
        {
            int minIndex = 0;
            float minValue = Mathf.Abs(this.enemyPositionX[0] - summonPositionX);
            float tempValue = 0;

            for (int i = 1; i < this.enemyPositionX.Length; ++i)
            {
                tempValue = Mathf.Abs(this.enemyPositionX[i] - summonPositionX);

                if (minValue > tempValue)
                {
                    minIndex = i;
                    minValue = tempValue;
                }
                else break;
            }
            this.minIndex = minIndex;
        }
        private void DescendingSearch(float summonPositionX)
        {
            int minIndex = this.enemyPositionX.Length - 1;
            float minValue = Mathf.Abs(this.enemyPositionX[this.enemyPositionX.Length - 1] - summonPositionX);
            float tempValue = 0;

            for (int i = this.enemyPositionX.Length - 2; i >= 0; --i)
            {
                tempValue = Mathf.Abs(this.enemyPositionX[i] - summonPositionX);

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
