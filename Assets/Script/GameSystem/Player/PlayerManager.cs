using UnityEngine;

namespace GameSystem.Player
{
    public interface IOwnManaStoneObserverForView
    {
        public void UpdateOwnManaStoneObserver();
    }
    public interface IOwnManaStoneObserver
    {
        public void RegisterOwnManaStoneObserver(IOwnManaStoneObserverForView observer);
        public void RemoveOwnManaStoneObserver(IOwnManaStoneObserverForView observer);
        public int GetOwnManaStone();
    }

    public interface IPlayerManagerForGameManager
    {
        public void Clearing();
        public void AllocateData(SaveAndLoad.PlayerDataStruct playerDataStruct);
        public SaveAndLoad.PlayerDataStruct GatherData();
        
        public int OwnManaStone { get; set; }
    }

    public interface IPlayerManager
    {
        public PlayerState PlayerState { get; set; }
        public Vector3 PlayerPosition { get; set; }
        public bool MoveDirection { get; set; }
        public int OwnManaStone { get; set; }
    }

    public interface IPlayerManagerForModel
    {
        public PlayerState PlayerState { get; set; }
        public Vector3 PlayerPosition { get; set; }
        public bool MoveDirection { get; set; }
        public int CurrentHP { get; set; }

        public void SpawnAttackObject(AttackObject.SkillName skillName);
    }


    public class PlayerManager : MonoBehaviour, IPlayerManagerForGameManager, IPlayerManager, IPlayerManagerForModel, IOwnManaStoneObserver
    {
        private AttackObject.IAttackObjectManager attackObjectManager;

        private System.Collections.Generic.List<IOwnManaStoneObserverForView> ownManaStoneObservers = new System.Collections.Generic.List<IOwnManaStoneObserverForView>();

        private IPlayerModelForPlayerManager playerModelForPlayerManager = null;
        private int ownManaStone;

        private PlayerState playerState;
        private Vector3 playerPosition;
        private bool moveDirection;
        private int currentHP;

        // IPlayerManager 구현
        public int OwnManaStone
        {
            get { return this.ownManaStone; }
            set
            {
                this.ownManaStone = value;
                this.NotifyOwnManaStoneObservers();
            }
        }
        public PlayerState PlayerState { get { return this.playerState; } set { this.playerState = value; } }
        public Vector3 PlayerPosition { get { return this.playerPosition; } set { this.playerPosition = value; } }
        public bool MoveDirection { get { return this.moveDirection; } set { this.moveDirection = value; } }
        public int CurrentHP { get { return this.currentHP; } set { this.currentHP = value; } }

        // IOwnManaStoneObserver 구현
        public void RegisterOwnManaStoneObserver(IOwnManaStoneObserverForView observer)
        {
            this.ownManaStoneObservers.Add(observer);
        }
        public void RemoveOwnManaStoneObserver(IOwnManaStoneObserverForView observer)
        {
            this.ownManaStoneObservers.Remove(observer);
        }
        public int GetOwnManaStone()
        {
            return this.ownManaStone;
        }
        private void NotifyOwnManaStoneObservers()
        {
            int i = 0;

            if (ownManaStoneObservers is null) return;

            while (i < ownManaStoneObservers.Count)
            {
                ownManaStoneObservers[i].UpdateOwnManaStoneObserver();
                ++i;
            }
        }

        private void Awake()
        {
            Debug.Log("PlayerManager - InitialSetting");

            this.attackObjectManager = GameObject.FindWithTag("AttackObjectManager").GetComponent<AttackObject.AttackObjectManager>();

            this.ownManaStone = 0;
            this.playerState = PlayerState.Idle;
            this.playerPosition = new Vector3(-6f, -3f, 0);
            this.moveDirection = false;
            this.currentHP = 10;
        }

        // IPlayerManagerForGameManager 구현
        public void Clearing()
        {
            Debug.Log("PlayerManager - Clearing");

            if (this.playerModelForPlayerManager is null) { }
            else this.playerModelForPlayerManager.Destroy();
        }
        public void AllocateData(SaveAndLoad.PlayerDataStruct playerDataStruct)
        {
            Debug.Log("PlayerManager - AllocateData");

            this.playerModelForPlayerManager = Instantiate(Resources.Load<GameObject>("Prefab/Player/Player")).GetComponent<PlayerModel>();

            this.playerState = playerDataStruct.PlayerState;
            this.moveDirection = playerDataStruct.MoveDirection;
            this.playerPosition = new Vector3(playerDataStruct.PlayerPositionX, playerDataStruct.PlayerPositionY, 0);
            this.currentHP = playerDataStruct.CurrentHP;

            this.ownManaStone = playerDataStruct.OwnManaStone;

            this.NotifyOwnManaStoneObservers();
        }
        public SaveAndLoad.PlayerDataStruct GatherData()
        {
            Debug.Log("PlayerManager - GatherData");

            SaveAndLoad.PlayerDataStruct playerDataStruct = new SaveAndLoad.PlayerDataStruct();

            playerDataStruct.PlayerState = this.playerState;
            playerDataStruct.MoveDirection = this.moveDirection;
            playerDataStruct.PlayerPositionX = this.playerPosition.x;
            playerDataStruct.PlayerPositionY = this.playerPosition.y;
            playerDataStruct.CurrentHP = this.currentHP;

            playerDataStruct.OwnManaStone = this.ownManaStone;

            return playerDataStruct;
        }

        // IPlayerManagerForModel 구현
        public void SpawnAttackObject(AttackObject.SkillName skillName)
        {
            AttackObject.PublicSkillStruct publicSkillStruct = new AttackObject.PublicSkillStruct(skillName, this.playerPosition, this.moveDirection, "Player");

            this.attackObjectManager.SpawnAttackObject(publicSkillStruct);
        }

        // GameManager가 읽어서 들여오는 것을 사용할 것임.
/*        private void RecordPlayerSkillInformationStructs()
        {
            TextAsset skillInformation_TextAsset = Resources.Load<TextAsset>("GameSystem/SkillData/SkillInformation");
            JArray skillInformation = JArray.Parse(skillInformation_TextAsset.ToString());

            for (int i = 0; i < skillInformation.Count; ++i)
            {
                Skill.SkillInformationStruct playerSkillInfo = new Skill.SkillInformationStruct(
                    skillNumber: (int)skillInformation[i]["SkillNumber"],
                    skillName : (string)skillInformation[i]["SkillName"],
                    skillDescription : (string)skillInformation[i]["SkillDescription"],
                    maxLevel: (int)skillInformation[i]["MaxLevel"],
                    currentLevel: (int)skillInformation[i]["CurrentLevel"]                
                );

                this.skillInformationStructs.Add(playerSkillInfo);
            }
        }*/
    }
}