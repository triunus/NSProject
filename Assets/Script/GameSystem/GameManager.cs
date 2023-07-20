using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameSystem
{
    public enum SceneName
    {
        Empty,
        GameLobby,
        Forest,
        Desert,
        ForestDungeon,
        DesertDungeon
    }
    public enum SaveLoadType
    {
        NewGame,
        Continue,
        LoadGame
    }
    public enum SceneChangeType
    {
        GameLobbyToInGame,
        InGameToGameLobby,
        ChangeToLoadedGame,
        ChangeToConnectedScene
    }

    public interface IMainGameTimeObserverForView
    {
        public void UpdateMainGameTimeObserver();
    }
    public interface IMainGameTimeObserver
    {
        public void RegisterMainGameTimeObserver(IMainGameTimeObserverForView observer);
        public void RemoveMainGameTimeObserver(IMainGameTimeObserverForView observer);
        public float GetMainGameTime();
    }
    public interface IOneDayTimeObserverForView 
    {
        public void UpdateOneDayTimeObserver();
    }
    public interface IOneDayTimeObserver
    {
        public void RegisterOneDayTimeObserver(IOneDayTimeObserverForView observer);
        public void RemoveOneDayTimeObserver(IOneDayTimeObserverForView observer);
        public int GetOneDayTime();
    }
    public interface ISummaryGameDataStructObserverForView
    {
        public void UpdateSummaryGameDataStructObserver();
    }
    public interface ISummaryGameDataStructObserver
    {
        public void RegisterSummaryGameDataStructObserver(ISummaryGameDataStructObserverForView observer);
        public void RemoveSummaryGameDataStructObserver(ISummaryGameDataStructObserverForView observer);
        public SaveAndLoad.SummaryGameDataStruct[] GetSummaryGameDataStruct();
    }

    public interface IGameManagerForSystem
    {
        public void ChoiceSceneChangeOperationType(SceneChangeType sceneChangeType, SceneName sceneName = SceneName.Empty);
        public void LoadGameData(SaveLoadType saveLoadType, int index = 0);
        public void SaveGameData(SaveLoadType saveLoadType, int index = 0);

        public void RequestPauseGame();
        public void RequestReGame();

        public SaveAndLoad.GameDataStruct GameDataStruct { get; set; }
        public bool IsInGame { get; }
    }

    public class GameManager : MonoBehaviour, IGameManagerForSystem, IMainGameTimeObserver, IOneDayTimeObserver, ISummaryGameDataStructObserver
    {
        private static IGameManagerForSystem instance = null;

        private SaveAndLoad.IGameSaveLoadManager gameSaveLoadManager;
        private SaveAndLoad.ISummaryGameSaveLoadManager summaryGameSaveLoadManager;

        private Player.IPlayerManagerForGameManager playerManager;
        private Enemy.IEnemyManagerForGameManager enemyManager;
        private Summon.ISummonManagerForGameManager summonManager;
        private Corpse.ICorpseManagerForGameManager corpseManager;
        private AttackObject.IAttackObjectManagerForGameManager attackObjectManager;
        private InGameUI.Skill.ISkillManagerForGameManager skillManager;

        private System.Collections.Generic.List<IMainGameTimeObserverForView> mainGameTimeObservers = new System.Collections.Generic.List<IMainGameTimeObserverForView>();
        private System.Collections.Generic.List<IOneDayTimeObserverForView> oneDayTimeObservers = new System.Collections.Generic.List<IOneDayTimeObserverForView>();
        private System.Collections.Generic.List<ISummaryGameDataStructObserverForView> summaryGameDataStructObservers = new System.Collections.Generic.List<ISummaryGameDataStructObserverForView>();

        private SaveAndLoad.GameDataStruct gameDataStruct;
        private SaveAndLoad.SummaryGameDataStruct[] summaryGameDataStruct = new SaveAndLoad.SummaryGameDataStruct[6];

        private float mainGameTime = 0;
        private int oneDayTime = 24;

        private bool isInGame = false;
        private bool isPause = false;

        public bool IsInGame { get { return this.isInGame; } }
        public SaveAndLoad.GameDataStruct GameDataStruct { get { return this.gameDataStruct; } set { this.gameDataStruct = value; } }

        private void Awake()
        {
            this.SetGameManager();

            this.gameSaveLoadManager = new SaveAndLoad.GameSaveLoadManager();
            this.summaryGameSaveLoadManager = new SaveAndLoad.SummaryGameSaveLoadManager();
        }
        private void Start()
        {
            this.CreateNewGameData();
            this.UpdateSummaryGameDataFromLocalData();
            this.NotifySummaryGameDataStructObserver();
        }

        public static IGameManagerForSystem Instance
        {
            get { return instance; }
        }

        public void SetGameManager()
        {
            if(instance is null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private void FixedUpdate()
        {
            if (!isPause && this.isInGame)
            {
/*              Debug.Log("GameManager - Update : ");
                Debug.Log("GameManager - isInGame : " + this.isInGame + "||| GameManager - isPause : " + this.isPause);*/
                this.mainGameTime += Time.deltaTime;
                this.NotifyInGameTimeObservers();
            }

            // InGame���� �� ��ü���� ������ ��ġ�� �����ϱ� �ռ���,
            // ������ ���� �� �� �ݺ��ǵ��� �� ��ü���� ��ġ�� �����Ѵ�.
            // GameManager�� ���� ���� ����Ǵ� ��ũ��Ʈ�̹Ƿ�, �̰����� ������ ����Ѵ�.
            if (this.isInGame)
            {
/*                Debug.Log("GameManager - FixedUpdate : ");
                Debug.Log("GameManager - isInGame : " + this.isInGame + "||| GameManager - isPause : " + this.isPause);*/
                this.enemyManager.SortOperationEnemyPositionX();
                this.summonManager.SortOperationSummonPositionX();

                this.enemyManager.GatherSummonAndPlayerPositionX();
                this.summonManager.GatherEnemyPositionX();
            }
        }

        // IGameManagerForSystem ����
        // '�� ����' ���� �޼ҵ�
        // �ش� �Լ��� ȣ���ϱ� ����, SaveGameData() & LoadGameData()�� ������ �����̴�. ��, this.gameDataStruct�� ���ŵ� �����̴�.
        public void ChoiceSceneChangeOperationType(SceneChangeType sceneChangeType, SceneName sceneName = SceneName.Empty)
        {
            Debug.Log("GameManager - ChoiceSceneChangeOperationType");

            this.isInGame = false;
            this.isPause = true;

            switch (sceneChangeType)
            {
                case GameSystem.SceneChangeType.GameLobbyToInGame:

                    this.OperateSceneChange(this.gameDataStruct.SceneName);
                    break;

                case GameSystem.SceneChangeType.InGameToGameLobby:  // ��

                    this.OperationBeforeSceneChange();
                    this.OperateSceneChange(SceneName.GameLobby);
                    break;

                case GameSystem.SceneChangeType.ChangeToLoadedGame:

                    if ((SceneManager.GetActiveScene().name.ToString().Equals("GameLobby")))
                    {
                        this.OperateSceneChange(this.gameDataStruct.SceneName);
                    }
                    else
                    {
                        if (SceneManager.GetActiveScene().name.Equals(this.gameDataStruct.SceneName.ToString()))
                        {
                            this.OperationBeforeSceneChange();
                            this.OperateSceneChange(SceneName.Empty);
                        }
                        else
                        {
                            this.OperationBeforeSceneChange();
                            this.OperateSceneChange(this.gameDataStruct.SceneName);
                        }
                    }
                    break;

                case GameSystem.SceneChangeType.ChangeToConnectedScene:

                    this.OperationBeforeSceneChange();
                    this.OperateSceneChange(sceneName);
                    break;

                default:

                    Debug.Log("ChoiceSceneChangeOperationType() error");
                    break;
            }
        }

        private void OperateSceneChange(SceneName nextScene)                    // �� ���� ����.
        {
            Debug.Log("GameManager - OperateSceneChange");

            Debug.Log("nextScene : " + nextScene);
            Debug.Log("SceneName.Empty : " + SceneName.Empty);
            Debug.Log("nextScene == SceneName.Empty : " + (nextScene == SceneName.Empty));
            Debug.Log("nextScene.ToString() : " + nextScene.ToString());


            if (nextScene == SceneName.Empty) SceneManager.sceneLoaded += ReactEmptySceneChange;
            else SceneManager.sceneLoaded += ReactSceneChange;

            SceneManager.LoadSceneAsync(nextScene.ToString(), LoadSceneMode.Single);
        }
        private void ReactSceneChange(Scene nextScene, LoadSceneMode mode)      // �� ���� ����.
        {
            Debug.Log("GameManager - ReactSceneChange");

            this.UpdateSummaryGameDataFromLocalData();
            this.NotifySummaryGameDataStructObserver();

            if (!(nextScene.name.ToString().Equals("GameLobby"))) this.OperationAfterSceneChange();

            SceneManager.sceneLoaded -= ReactSceneChange;
        }
        private void ReactEmptySceneChange(Scene nextScene, LoadSceneMode mode)
        {
            Debug.Log("GameManager - ReactEmptySceneChange");
            SceneManager.sceneLoaded -= ReactEmptySceneChange;

            this.OperateSceneChange(this.gameDataStruct.SceneName);
        }

        private void OperationBeforeSceneChange()                               // �� ���� �� ���� ����.
        {
            Debug.Log("GameManager - OperationBeforeSceneChange");

            this.ClearingTheOthersManager();
            this.DisConnectGameManagerAndTheOthersManager();
        }
        private void OperationAfterSceneChange()                                // �� ���� �� �Ҵ� ����.
        {
            Debug.Log("GameManager - OperationAfterSceneChange");

            this.ConnectGameManagerAndTheOthersManager();
            this.AllocateGameData();

            this.isInGame = true;
            this.isPause = false;
        }

        private void ConnectGameManagerAndTheOthersManager()
        {
            Debug.Log("GameManager - ConnectGameManagerAndTheOthersManager");

            this.playerManager = GameObject.FindWithTag("PlayerManager").GetComponent<Player.PlayerManager>();
            this.enemyManager = GameObject.FindWithTag("EnemyManager").GetComponent<Enemy.EnemyManager>();
            this.summonManager = GameObject.FindWithTag("SummonManager").GetComponent<Summon.SummonManager>();
            this.corpseManager = GameObject.FindWithTag("CorpseManager").GetComponent<Corpse.CorpseManager>();
            this.attackObjectManager = GameObject.FindWithTag("AttackObjectManager").GetComponent<AttackObject.AttackObjectManager>();
            this.skillManager = GameObject.FindWithTag("SkillManager").GetComponent<InGameUI.Skill.SkillManager>();
        }           // GameManager�� InGame Scene�鿡 ���ǵǾ� �ִ� Manager���� ���� ��Ű�� �۾��̴�.
        private void DisConnectGameManagerAndTheOthersManager()
        {
            Debug.Log("GameManager - DisConnectGameManagerAndTheOthersManager");

            this.playerManager = null;
            this.enemyManager = null;
            this.summonManager = null;
            this.corpseManager = null;
            this.attackObjectManager = null;
            this.skillManager = null;
        }        // GameManager�� InGame Scene�鿡 ���ǵǾ� �ִ� Manager���� ������ �����Ѵ�.
        // �� Manager�� �ʱ� ������ �����ϴ� �κ�. (������ �Ҵ��� ���� �ִ�.)
        private void ClearingTheOthersManager()
        {
            Debug.Log("GameManager - ClearingTheOthersManager");

            this.enemyManager.Clearing();
            this.summonManager.Clearing();
            this.corpseManager.Clearing();
            this.attackObjectManager.Clearing();
        }                        // �� Manager�� ���� �ִ� �Ǵ� �����ϰ� �ִ� �����͸� ��� �����Ѵ�.                                 
        private void AllocateGameData()
        {
            Debug.Log("GameManager - AllocateGameData");

            this.mainGameTime = this.gameDataStruct.InGameTime;

            this.playerManager.AllocateData(this.gameDataStruct.PlayerDataStruct);
            this.enemyManager.AllocateData(this.gameDataStruct.EnemyDataStruct);
            this.summonManager.AllocateData(this.gameDataStruct.SummonDataStruct);
            this.corpseManager.AllocateData(this.gameDataStruct.CorpseDataStruct);
            this.skillManager.AllocateData(this.gameDataStruct.SkillDataStruct);

            this.NotifyOneDayTimeObservers();       // '1��'�� ����ϴ� �ð��� �����մϴ�.

            this.RequestReGame();
        }                                // �� Manager�� ������ �Ҵ� �Լ�.


        // 'Save & Load' ���� �޼ҵ�
        public void LoadGameData(SaveLoadType saveLoadType, int index = 0)
        {
            Debug.Log("GameManager - LoadGameData");

            this.gameDataStruct = this.gameSaveLoadManager.LoadGameData(saveLoadType, index);
            if (this.gameDataStruct is null) Debug.Log("gameDataStruct is null");  // error üũ
        }       // ���� ������ �ε�
        public void SaveGameData(SaveLoadType saveLoadType, int index = 0)
        {
            Debug.Log("GameManager - SaveGameData");

            this.summaryGameSaveLoadManager.SaveSummayGameData(saveLoadType, this.GatherSummaryGameDataFromEachManager(), index);
            this.gameSaveLoadManager.SaveGameData(saveLoadType, this.GatherGameDataFromEachManager(), index);

            this.UpdateSummaryGameDataFromLocalData();
            this.NotifySummaryGameDataStructObserver();
        }       // ���� ������ ����
        private SaveAndLoad.GameDataStruct GatherGameDataFromEachManager()
        {
            Debug.Log("GameManager - GatherGameDataFromEachManager");

            SaveAndLoad.GameDataStruct gameDataStruct = new SaveAndLoad.GameDataStruct(
                (SceneName)System.Enum.Parse(typeof(SceneName), SceneManager.GetActiveScene().name), this.mainGameTime);

            gameDataStruct.PlayerDataStruct = this.playerManager.GatherData();
            gameDataStruct.EnemyDataStruct = this.enemyManager.GatherData();
            gameDataStruct.SummonDataStruct = this.summonManager.GatherData();
            gameDataStruct.CorpseDataStruct = this.corpseManager.GatherData();
            gameDataStruct.SkillDataStruct = this.skillManager.GatherData();

            return gameDataStruct;
        }       // �� Manager���� ���� ������ ��ƿ���
        private SaveAndLoad.SummaryGameDataStruct GatherSummaryGameDataFromEachManager()
        {
            Debug.Log("GameManager - GatherSummaryGameDataFromEachManager");

            SaveAndLoad.SummaryGameDataStruct summaryGameDataStruct = new SaveAndLoad.SummaryGameDataStruct();

            summaryGameDataStruct.SceneName = (SceneName)System.Enum.Parse(typeof(SceneName), SceneManager.GetActiveScene().name);
            summaryGameDataStruct.InGameTime = (float)System.Math.Round((this.mainGameTime % this.oneDayTime), 2);
            summaryGameDataStruct.Stage = (int)(this.mainGameTime / this.oneDayTime);

            summaryGameDataStruct.OwnManaStone = this.playerManager.OwnManaStone;

            summaryGameDataStruct.EnemyCount = this.enemyManager.GetEnemyCount();

            summaryGameDataStruct.SummonCount = this.summonManager.GetSummonCount();

            return summaryGameDataStruct;
        }           // SummarySaveGameData ���ÿ��� ������ GameManager�� �����ϱ�.
        private void UpdateSummaryGameDataFromLocalData()
        {
//            Debug.Log("GameManager - UpdateSummaryGameDataFromLocalData");  

            SaveAndLoad.SummaryGameDataStruct[] temp = this.summaryGameSaveLoadManager.LoadSummaySaveGameData();

            this.summaryGameDataStruct[0] = this.summaryGameSaveLoadManager.LoadSummayAutoGameData();

            for(int i = 0; i < temp.Length; ++i)
            {
                this.summaryGameDataStruct[i + 1] = temp[i];
            }
        }


        // ���� �Ͻ� ���� ���.
        public void RequestPauseGame()
        {
            Time.timeScale = 0;
        }
        public void RequestReGame()
        {
            Time.timeScale = 1;
        }


        // Observer
        public void RegisterMainGameTimeObserver(IMainGameTimeObserverForView observer)
        {
            this.mainGameTimeObservers.Add(observer);
        }
        public void RemoveMainGameTimeObserver(IMainGameTimeObserverForView observer)
        {
            this.mainGameTimeObservers.Remove(observer);
        }
        public float GetMainGameTime()
        {
            return this.mainGameTime;
        }
        private void NotifyInGameTimeObservers()
        {
            int i = 0;
            while (i < mainGameTimeObservers.Count)
            {
                mainGameTimeObservers[i].UpdateMainGameTimeObserver();
                ++i;
            }
        }

        public void RegisterOneDayTimeObserver(IOneDayTimeObserverForView observer)
        {
            this.oneDayTimeObservers.Add(observer);
        }
        public void RemoveOneDayTimeObserver(IOneDayTimeObserverForView observer)
        {
            this.oneDayTimeObservers.Remove(observer);
        }
        public int GetOneDayTime()
        {
            return this.oneDayTime;
        }
        private void NotifyOneDayTimeObservers()
        {
            int i = 0;
            while(i < oneDayTimeObservers.Count)
            {
                oneDayTimeObservers[i].UpdateOneDayTimeObserver();
                ++i;
            }
        }

        public void RegisterSummaryGameDataStructObserver(ISummaryGameDataStructObserverForView observer)
        {
            this.summaryGameDataStructObservers.Add(observer);
        }
        public void RemoveSummaryGameDataStructObserver(ISummaryGameDataStructObserverForView observer)
        {
            this.summaryGameDataStructObservers.Remove(observer);
        }
        public SaveAndLoad.SummaryGameDataStruct[] GetSummaryGameDataStruct()
        {
            return this.summaryGameDataStruct;
        }
        private void NotifySummaryGameDataStructObserver()
        {
            int i = 0;
            while(i < this.summaryGameDataStructObservers.Count)
            {
                this.summaryGameDataStructObservers[i].UpdateSummaryGameDataStructObserver();
                ++i;
            }
        }


        // ���� NewGameData ���� �Լ�. (�ӽ�)
        private void CreateNewGameData()
        {
            SaveAndLoad.GameDataStruct gameDataStruct = new SaveAndLoad.GameDataStruct(SceneName.Forest, 0);

            gameDataStruct.PlayerDataStruct = new SaveAndLoad.PlayerDataStruct();
            gameDataStruct.EnemyDataStruct = new SaveAndLoad.EnemyDataStruct(false);
            gameDataStruct.SummonDataStruct = new SaveAndLoad.SummonDataStruct();
            gameDataStruct.CorpseDataStruct = new SaveAndLoad.CorpseDataStruct();
            gameDataStruct.SkillDataStruct = new SaveAndLoad.SkillDataStruct(0);

            this.gameSaveLoadManager.SaveGameData(SaveLoadType.NewGame, gameDataStruct);
        }
    }
}
