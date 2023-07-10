using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using UnityEngine;

namespace GameSystem.Enemy
{
    public enum EnemySpawnType
    {
        SpawnByTime,
        SpawnByLoadFile,
        SpawnByObject
    }

    public interface IEnemyManagerForGameManager
    {
        public void Clearing();
        public void AllocateData(SaveAndLoad.EnemyDataStruct enemyDataStruct);
        public SaveAndLoad.EnemyDataStruct GatherData();

        public int GetEnemyCount();
        public void SortOperationEnemyPositionX();
        public void GatherSummonAndPlayerPositionX();
    }

    public interface IEnemyManager
    {
        public float[] GetEnemyPositionX();
        public void SpawnEnemy(Monster.MonsterName MonsterName, Vector3 startPosition, EnemySpawnType enemySpawnType, int enemyCurrentHP = 3);
    }

    public interface IEnemyManagerForEnemyModel
    {
        public void RegisterEnemyView(IEnemyView enemyView);
        public void RemoveEnemyView(IEnemyView enemyView);

        public float[] GetSummonAndPlayerPositionX();

        public void SpawnAttackObject(AttackObject.PublicSkillStruct publicSkillStruct);

        public void SpawnCorpse(Monster.MonsterName MonsterName, Vector3 enemyPosition);
    }

    public class EnemyManager : MonoBehaviour, IMainGameTimeObserverForView, IOneDayTimeObserverForView,
        IEnemyManagerForGameManager, IEnemyManager, IEnemyManagerForEnemyModel
    {
        // 시간을 책임지는 GameManager에 시간 Observer 등록.
        private IMainGameTimeObserver mainGameTimeModel;
        private IOneDayTimeObserver oneDayTimeModel;
        private int gameStage;
        private float mainGameTime;
        private int oneDayTime = 24;

        // Stage 별 Enemy 자동 생성을 위한 데이터와 사용되는 변수.
        private List<List<EnemyDataToBeCreatedStruct>> createEnemyData = null;
        private JArray enemyDataOfStage = null;
        private bool beCreated = false;

        // 각 Manager 명시.
        private Player.IPlayerManager playerManager;
        private Summon.ISummonManager summonManager;
        private Corpse.ICorpseManager corpseManager;
        private AttackObject.IAttackObjectManager attackObjectManager;

        private IEnemySpawnerManager enemySpawnerManager;

        // 각 Object의 위치를 명시하기 위한 변수.
        private List<IEnemyView> enemyViewes = new List<IEnemyView>();
        private float[] enemyPositionX;
        private float[] summonAndPlayerPositionX;

        private bool isReady = false;

        private void Awake()
        {
            // GameTime을 인지.
            this.mainGameTimeModel = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
            this.oneDayTimeModel = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

            this.mainGameTimeModel.RegisterMainGameTimeObserver(this);
            this.oneDayTimeModel.RegisterOneDayTimeObserver(this);

            this.playerManager = GameObject.FindWithTag("PlayerManager").GetComponent<Player.PlayerManager>();
            this.summonManager = GameObject.FindWithTag("SummonManager").GetComponent<Summon.SummonManager>();
            this.corpseManager = GameObject.FindWithTag("CorpseManager").GetComponent<Corpse.CorpseManager>();
            this.attackObjectManager = GameObject.FindWithTag("AttackObjectManager").GetComponent<AttackObject.AttackObjectManager>();
            this.enemySpawnerManager = GetComponent<EnemySpawnerManager>();

            this.createEnemyData = new List<List<EnemyDataToBeCreatedStruct>>();
        }

        private void FixedUpdate()
        {
            if (isReady)
            {
                if (this.mainGameTime % this.oneDayTime >= this.oneDayTime / 2 && !beCreated) {
                    this.beCreated = true;
                    StopCoroutine("SpawnEnemyEachStage");
                    StartCoroutine("SpawnEnemyEachStage");
                }
                else if (this.mainGameTime % this.oneDayTime < this.oneDayTime / 2) {
                    this.beCreated = false;
                    StopCoroutine("SpawnEnemyEachStage");
                }
            }   else { }
        }
        // Json으로 기록된 Stage 별 Enemy 정보를 List<List<EnemyDataToBeCreatedStruct>>에 저장한다.
        private void RecordEnemyInformation()
        {
            for (int i = 0; i < enemyDataOfStage.Count; ++i)
            {
                List<EnemyDataToBeCreatedStruct> enemyDataToBeCreatedStructList = new List<EnemyDataToBeCreatedStruct>();

                for (int j = 0; j < ((JArray)enemyDataOfStage[i]["EnemyInformation"]).Count; ++j)
                {
                    EnemyDataToBeCreatedStruct enemyDataToBeCreatedStruct = new EnemyDataToBeCreatedStruct(
                        (Monster.MonsterName)System.Enum.Parse(typeof(Monster.MonsterName), enemyDataOfStage[i]["EnemyInformation"][j]["MonsterName"].ToString()),
                        (int)enemyDataOfStage[i]["EnemyInformation"][j]["CountToBeCreated"]);                    

                    enemyDataToBeCreatedStructList.Add(enemyDataToBeCreatedStruct);
                }

                this.createEnemyData.Add(enemyDataToBeCreatedStructList.ConvertAll(tmp => new EnemyDataToBeCreatedStruct(tmp.MonsterName, tmp.CountToBeCreated)));
                enemyDataToBeCreatedStructList.Clear();
            }
        }
        private IEnumerator SpawnEnemyEachStage()
        {
            for (int i = 0; i < this.createEnemyData[this.gameStage].Count; i++)
            {
                EnemyDataToBeCreatedStruct createEnemyDataStruct = this.createEnemyData[this.gameStage][i];

                for (int j = 0; j < createEnemyDataStruct.CountToBeCreated; j++)
                {
                    this.SpawnEnemy(createEnemyDataStruct.MonsterName, this.enemySpawnerManager.EnemySpawnerPosition, EnemySpawnType.SpawnByTime);
                    yield return new WaitForSeconds(Time.fixedDeltaTime);
                }
            }
        }


        // IEnemyManagerForGameManager 구현
        public void Clearing()
        {
            // AllocateData()에서 값 할당됨.
            if (this.enemyViewes is null) { }
            else
            {
                Debug.Log("EnemyManager - Clearing - this.enemyViewes.Count : " + this.enemyViewes.Count);
                while (this.enemyViewes.Count != 0)
                {
                    this.enemyViewes[0].Destroy();
                }
            }

            Debug.Log("EnemyManager - Clearing - End");
        }

        public void AllocateData(SaveAndLoad.EnemyDataStruct enemyDataStruct)
        {
            this.beCreated = enemyDataStruct.BeCreated;

            if (enemyDataStruct.GetEnemyDataCount() == 0) { }
            else
            {
                for (int i = 0; i < enemyDataStruct.GetEnemyDataCount(); ++i)
                {
                    SaveAndLoad.EnemyData enemyData = enemyDataStruct.GetEnemyData(i);

                    this.SpawnEnemy(enemyData.MonsterName, new Vector3(enemyData.EnemyPositionX, enemyData.EnemyPositionY, 0), EnemySpawnType.SpawnByLoadFile, enemyData.EnemyCurrentHP);
                }
            }

            // 각 Stage에서 필요한 Enemy 데이터를 EnemyManager에 저장.
            // 차후, Scene의 종류에 따라, EnemyDataOfStage 값을 다르게 받을 계획이다.
            TextAsset enemyDataOfStageJson = Resources.Load<TextAsset>("GameSystem/EnemyData/EnemyDataOfStage");
            this.enemyDataOfStage = JArray.Parse(enemyDataOfStageJson.ToString());
            this.RecordEnemyInformation();

            this.isReady = true;
        }
        public SaveAndLoad.EnemyDataStruct GatherData()
        {
            SaveAndLoad.EnemyDataStruct enemyDataStruct = new SaveAndLoad.EnemyDataStruct(this.beCreated);

            if (this.enemyViewes.Count == 0) { }
            else
            {
                for (int i = 0; i < enemyViewes.Count; ++i)
                {
                    SaveAndLoad.EnemyData enemyData = new SaveAndLoad.EnemyData();

                    enemyData.MonsterName = this.enemyViewes[i].MonsterStruct.MonsterName;
                    enemyData.EnemyPositionX = this.enemyViewes[i].EnemyPosition.x;
                    enemyData.EnemyPositionY = this.enemyViewes[i].EnemyPosition.y;
                    enemyData.EnemyCurrentHP = this.enemyViewes[i].EnemyCurrentHP;

                    enemyDataStruct.SetEnemyData(enemyData);
                }
            }

            return enemyDataStruct;
        }


        public int GetEnemyCount()
        {
            return this.enemyViewes.Count;
        }
        public void SortOperationEnemyPositionX()
        {
            if (this.enemyViewes.Count == 0) this.enemyPositionX = null;

            this.enemyPositionX = new float[this.enemyViewes.Count];

            for (int i = 0; i < this.enemyPositionX.Length; ++i)
            {
                this.enemyPositionX[i] = this.enemyViewes[i].EnemyPosition.x;
            }

            System.Array.Sort(this.enemyPositionX);

/*            for(int i =0; i < this.enemyPositionX.Length; ++i)
            {
                Debug.Log("SortOperationEnemyPositionX[" + i + "]" + enemyPositionX[i]);
            }*/
        }
        public void GatherSummonAndPlayerPositionX()
        {
            float playerPositionX = this.playerManager.PlayerPosition.x;
            float[] summonPositionX = this.summonManager.GetSummonPositionX();
            float[] tempPositionX = new float[summonPositionX.Length + 1];

            int insertIndex = 0;
            bool isInserted = false;

            if (summonPositionX.Length == 0) { tempPositionX[0] = playerPositionX; }
            else
            {
                for (int i = 0; i <= summonPositionX.Length; ++i)
                {
                    if(i == summonPositionX.Length)
                    {
                        insertIndex = i;
                        break;
                    }

                    if (playerPositionX < summonPositionX[i])
                    {
                        insertIndex = i;
                        break;
                    }
                }

                for(int i = 0; i< tempPositionX.Length; ++i)
                {
                    if(insertIndex == i)
                    {
                        isInserted = true;
                        tempPositionX[i] = playerPositionX;
                    }
                    else
                    {
                        if (isInserted) tempPositionX[i] = summonPositionX[i - 1];
                        else tempPositionX[i] = summonPositionX[i];
                    }
                }
            }

            this.summonAndPlayerPositionX = tempPositionX;
        }

        // IEnemyManager 구현
        public float[] GetEnemyPositionX()
        {
            if (enemyPositionX == null) return null;
            else return this.enemyPositionX;
        }   
        public void SpawnEnemy(Monster.MonsterName MonsterName, Vector3 startPosition, EnemySpawnType enemySpawnType, int enemyCurrentHP = 3)
        {
            IEnemyView enemyView = Instantiate(Resources.Load<GameObject>("Prefab/Monster/Enemy/" + MonsterName.ToString())).GetComponent<Enemy.EnemyView>();
            enemyView.InitialSettingEnemyView(MonsterName, startPosition, enemySpawnType);
            if (enemySpawnType == EnemySpawnType.SpawnByLoadFile) enemyView.EnemyCurrentHP = enemyCurrentHP;
        }

        // IEnemyManagerForEnemyModel 구현
        public void RegisterEnemyView(IEnemyView enemyView)
        {
            if (this.enemyViewes.Count == 0)
            {
                this.enemyViewes.Add(enemyView);
                return;
            }

            int enemyType = (int)enemyView.MonsterStruct.MonsterName;
            int insertIndex = 0;

            for(int i =0; i <= this.enemyViewes.Count; ++i)
            {
                if (i == this.enemyViewes.Count)
                {
                    this.enemyViewes.Add(enemyView);
                    break;
                }

                if(enemyType < (int)this.enemyViewes[i].MonsterStruct.MonsterName)
                {
                    insertIndex = i;
                    this.enemyViewes.Insert(insertIndex, enemyView);
                    break;
                }
            }

            this.UpdateEnemyViewIndex();
        }
        public void RemoveEnemyView(IEnemyView enemyView)
        {
            Debug.Log("EnemyManager - RemoveEnemyView - this.enemyViewes.Count : " + this.enemyViewes.Count);
            this.enemyViewes.Remove(enemyView);
            this.UpdateEnemyViewIndex();
        }
        private void UpdateEnemyViewIndex()
        {
            Debug.Log("EnemyManager - UpdateEnemyViewIndex - this.enemyViewes.Count : " + this.enemyViewes.Count);
            for (int i = 0; i < this.enemyViewes.Count; ++i)
            {
                Debug.Log("UpdateEnemyViewIndex - this.enemyViewes.Count");
                this.enemyViewes[i].SetEnemySequenceIndex(i);
            }
        }

        public float[] GetSummonAndPlayerPositionX()
        {
            return this.summonAndPlayerPositionX;
        }

        // 타 객체에 요청하는 부분.
        public void SpawnAttackObject(AttackObject.PublicSkillStruct publicSkillStruct)
        {
            this.attackObjectManager.SpawnAttackObject(publicSkillStruct);
        }
        public void SpawnCorpse(Monster.MonsterName MonsterName, Vector3 enemyPosition)
        {
            this.corpseManager.SpawnCorpse(MonsterName, enemyPosition);
        }

        // Observer
        public void UpdateMainGameTimeObserver()
        {
            this.mainGameTime = this.mainGameTimeModel.GetMainGameTime();
            this.gameStage = (int)(this.mainGameTime / this.oneDayTime);
        }
        public void UpdateOneDayTimeObserver()
        {
            this.oneDayTime = this.oneDayTimeModel.GetOneDayTime();
        }

        private void OnDestroy()
        {
            this.mainGameTimeModel.RemoveMainGameTimeObserver(this);
            this.oneDayTimeModel.RemoveOneDayTimeObserver(this);
        }
    }
}