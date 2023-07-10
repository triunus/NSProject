using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GameSystem.Summon
{
    public enum SummonSpawnType
    {
        SpawnByRevival,
        SpawnByLoadFile,
        SpawnByObject
    }

    public interface ISummonManagerForGameManager
    {
        public void Clearing();
        public void AllocateData(SaveAndLoad.SummonDataStruct summonDataStruct);
        public SaveAndLoad.SummonDataStruct GatherData();

        public int GetSummonCount();
        public void SortOperationSummonPositionX();
        public void GatherEnemyPositionX();
    }

    public interface ISummonManager
    {
        public float[] GetSummonPositionX();
        public void SpawnSummon(Monster.MonsterName MonsterName, Vector3 startPosition, SummonSpawnType summonSpawnType, int summonCurrentHP = 3);
    }

    public interface ISummonManagerForSummonModel
    {
        public void RegisterSummonView(ISummonView summonView);
        public void RemoveSummonView(ISummonView summonView);

        public Player.PlayerState GetPlayerState();
        public Vector3 GetPlayerPosition();
        public bool GetMoveDirection();

        public float[] GetEnemyPositionX();

        public void SpawnAttackObject(AttackObject.PublicSkillStruct publicSkillStruct);
        public void SpawnCorpse(Monster.MonsterName MonsterName, Vector3 summonPosition);
    }


    public class SummonManager : MonoBehaviour, ISummonManagerForGameManager, ISummonManagerForSummonModel, ISummonManager
    {
        private Player.IPlayerManager playerManager;
        private Enemy.IEnemyManager enemyManager;
        private Corpse.ICorpseManager corpseManager;
        private AttackObject.IAttackObjectManager attackObjectManager;

        private List<ISummonView> summonViewes = new List<ISummonView>();
        private float[] summonPositionX;

        private float[] enemyPositionX;

        private void Awake()
        {
            this.playerManager = GameObject.FindWithTag("PlayerManager").GetComponent<Player.PlayerManager>();
            this.enemyManager = GameObject.FindWithTag("EnemyManager").GetComponent<Enemy.EnemyManager>();
            this.attackObjectManager = GameObject.FindWithTag("AttackObjectManager").GetComponent<AttackObject.AttackObjectManager>();
            this.corpseManager = GameObject.FindWithTag("CorpseManager").GetComponent<Corpse.CorpseManager>();
        }

        public void Clearing()
        {
            // AllocateData()에서 값 할당됨.
            if (this.summonViewes is null) { }
            else
            {
                while (this.summonViewes.Count != 0)
                {
                    this.summonViewes[0].Destroy();
                }
            }
        }
        public void AllocateData(SaveAndLoad.SummonDataStruct summonDataStruct)
        {
            if (summonDataStruct.GetSummonDataCount() == 0) { }
            else
            {
                for (int i = 0; i < summonDataStruct.GetSummonDataCount(); ++i)
                {
                    SaveAndLoad.SummonData summonData = summonDataStruct.GetEnemyData(i);

                    this.SpawnSummon(summonData.MonsterName, new Vector3(summonData.SummonPositionX, summonData.SummonPositionY, 0), SummonSpawnType.SpawnByLoadFile, summonData.SummonCurrentHP);
                }
            }
        }
        public SaveAndLoad.SummonDataStruct GatherData()
        {
            SaveAndLoad.SummonDataStruct summonDataStruct = new SaveAndLoad.SummonDataStruct();

            if (summonViewes.Count == 0) { }
            else
            {
                for (int i = 0; i < summonViewes.Count; ++i)
                {
                    SaveAndLoad.SummonData summonData = new SaveAndLoad.SummonData();

                    summonData.MonsterName = this.summonViewes[i].MonsterStruct.MonsterName;
                    summonData.SummonPositionX = this.summonViewes[i].SummonPosition.x;
                    summonData.SummonPositionY = this.summonViewes[i].SummonPosition.y;
                    summonData.SummonCurrentHP = this.summonViewes[i].SummonCurrentHP;

                    summonDataStruct.SetEnemyData(summonData);
                }
            }

            return summonDataStruct;
        }

        public int GetSummonCount()
        {
            return this.summonViewes.Count;
        }
        // 각 Model들이 타 Model의 정보를 가져오기 전에, 각 Model들이 갖고있는 위치 정보를 정렬해 놓는다.
        // GameSysmteManager가 FixedUpdate 중 가장 먼저 호출한다.
        public void SortOperationSummonPositionX()
        {
            if (this.summonViewes.Count == 0) this.summonPositionX = null;

//            Debug.Log("SummonManager - SortOperationSummonPositionX - summonViewes.Length : " + summonViewes.Count);
            this.summonPositionX = new float[this.summonViewes.Count];

            for(int i = 0; i < this.summonPositionX.Length; ++i)
            {
                this.summonPositionX[i] = this.summonViewes[i].SummonPosition.x;
            }

            System.Array.Sort(this.summonPositionX);

/*            for (int i = 0; i < this.summonPositionX.Length; ++i)
            {
                Debug.Log("SortOperationSummonPositionX[" + i + "] : " + summonPositionX[i]);
            }*/
        }
        public void GatherEnemyPositionX()
        {
            this.enemyPositionX = this.enemyManager.GetEnemyPositionX();
        }

        // 2. ISummonManager 구현
        public float[] GetSummonPositionX()
        {
            if (summonPositionX == null) return null;
            else return this.summonPositionX;
        }
        public void SpawnSummon(Monster.MonsterName MonsterName, Vector3 startPosition, SummonSpawnType summonSpawnType, int summonCurrentHP = 3)
        {
            ISummonView summonView = Instantiate(Resources.Load<GameObject>("Prefab/Monster/Summon/" + MonsterName.ToString())).GetComponent<SummonView>();
            summonView.InitialSettingSummonView(MonsterName, startPosition, summonSpawnType);

            if (summonSpawnType == SummonSpawnType.SpawnByLoadFile) summonView.SummonCurrentHP = summonCurrentHP;
        }

        // MonsterName 순서대로 Summon 배치.
        public void RegisterSummonView(ISummonView summonView)
        {
            if (this.summonViewes.Count == 0)
            {
                this.summonViewes.Add(summonView);
                return;
            }

            int summonType = (int)summonView.MonsterStruct.MonsterName;
            int insertIndex = 0;

            for(int i = 0; i < this.summonViewes.Count; ++i)
            {
                if ((int)this.summonViewes[i].MonsterStruct.MonsterName > summonType)
                {
                    insertIndex = i;
                    break;
                }
            }

            this.summonViewes.Insert(insertIndex, summonView);
            this.UpdateSummonViewIndex();
        }
        public void RemoveSummonView(ISummonView summonView)
        {
            this.summonViewes.Remove(summonView);
            this.UpdateSummonViewIndex();
        }
        private void UpdateSummonViewIndex()
        {
            for(int i = 0; i < this.summonViewes.Count; ++i)
            {
                this.summonViewes[i].SetSummonSequenceIndex(i);
            }
        }

        // PlayerModel의 Observer를 등록해 놓아서, SummonMPV가 Player의 정보를 필요로하면 관련 정보를 전달해준다.
        public Player.PlayerState GetPlayerState()
        {
            return this.playerManager.PlayerState;
        }
        public Vector3 GetPlayerPosition()
        {
            return this.playerManager.PlayerPosition;
        }
        public bool GetMoveDirection()
        {
            return this.playerManager.MoveDirection;
        }

        // EnemyManager를 통해 Enemy 정보를 가져오기.
        public float[] GetEnemyPositionX()
        {
            return this.enemyPositionX;
        }

        // 각 객체의 Manager를 통해 해당 객체를 생성한다.
        public void SpawnAttackObject(AttackObject.PublicSkillStruct publicSkillStruct)
        {
            this.attackObjectManager.SpawnAttackObject(publicSkillStruct);
        }
        public void SpawnCorpse(Monster.MonsterName MonsterName, Vector3 summonPosition)
        {
            this.corpseManager.SpawnCorpse(MonsterName, summonPosition);
            // corpse의 filpX는 스스로 인식할 수 있도록 수정.
        }
    }
}
