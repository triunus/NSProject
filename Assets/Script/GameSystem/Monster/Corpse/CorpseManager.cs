using System.Collections.Generic;
using Newtonsoft.Json.Linq;

using UnityEngine;

namespace GameSystem.Corpse
{
    // Corpse 생성
    // 1. Enemy가 죽을 때.
    // 2. GameStory 상 Scene에 이미 존재할 때.
    // 3. Enemy가 skill로 Corpse를 흩뿌릴 때.

    public interface ICorpseManagerForGameManager
    {
        public void Clearing();
        public void AllocateData(SaveAndLoad.CorpseDataStruct corpseDataStruct);
        public SaveAndLoad.CorpseDataStruct GatherData();
    }

    public interface ICorpseManager
    {
        public void SpawnCorpse(Monster.MonsterName MonsterName, Vector3 startPosition, bool corpseFilpX = true);
    }

    public interface ICorpseManagerForModel
    {
        public float GetPlayerPositionX();
        public int GetOwnManaStone();
        public void UpdateOwnManaStone(int theNumberOfManaStone);

        public void RegisterCorpseInteractionView(Interaction.IPlayerCorpseInteractionManagerForView corpseView);
        public void RemoveCorpseInteractionView(Interaction.IPlayerCorpseInteractionManagerForView corpseView);

        public void RegisterCorpseView(ICorpseView corpseView);
        public void RemoveCorpseView(ICorpseView corpseView);

        public void SpawnEnemy(Monster.MonsterName MonsterName, Vector3 startPosition);
        public void SpawnSummon(Monster.MonsterName MonsterName, Vector3 startPosition);
    }

    public class CorpseManager : MonoBehaviour, ICorpseManagerForGameManager, ICorpseManager, ICorpseManagerForModel
    {
        private Player.IPlayerManager playerManager;
        private Interaction.IPlayerCorpseInteractionManager playerCorpseInteractionManager;
        private Enemy.IEnemyManager enemyManager;
        private Summon.ISummonManager summonManager;

        private List<ICorpseView> corpseViews = new List<ICorpseView>();

        private void Awake()
        {
            this.playerManager = GameObject.FindWithTag("PlayerManager").GetComponent<Player.PlayerManager>();
            this.playerCorpseInteractionManager = GameObject.FindWithTag("InteractionManager").GetComponent<Interaction.PlayerCorpseInteractionManager>();
            this.enemyManager = GameObject.FindWithTag("EnemyManager").GetComponent<Enemy.EnemyManager>();
            this.summonManager = GameObject.FindWithTag("SummonManager").GetComponent<Summon.SummonManager>();
        }

        // ICorpseManagerForGameManager 구현
        public void Clearing()
        {
            if (this.corpseViews is null) { }
            else
            {
                while (this.corpseViews.Count != 0)
                {
                    this.corpseViews[0].Destroy();
                }
            }
        }
        public void AllocateData(SaveAndLoad.CorpseDataStruct corpseDataStruct)
        {
            if (corpseDataStruct.GetCorpseDataCount() == 0) { }
            else
            {
                for (int i = 0; i < corpseDataStruct.GetCorpseDataCount(); ++i)
                {
                    SaveAndLoad.CorpseData corpseData = corpseDataStruct.GetCorpseData(i);

                    this.SpawnCorpse(corpseData.MonsterName, new Vector3(corpseData.CorpsePositionX, corpseData.CorpsePositionY, 0));
                }
            }
        }
        public SaveAndLoad.CorpseDataStruct GatherData()
        {
            SaveAndLoad.CorpseDataStruct corpseDataStruct = new SaveAndLoad.CorpseDataStruct();

            if (this.corpseViews.Count == 0) { }
            else
            {
                for (int i = 0; i < this.corpseViews.Count; ++i)
                {
                    SaveAndLoad.CorpseData corpseData = new SaveAndLoad.CorpseData(this.corpseViews[i].MonsterStruct.MonsterName,
                        this.corpseViews[i].CorpsePosition.x, this.corpseViews[i].CorpsePosition.y);

                    corpseDataStruct.SetCorpseData(corpseData);
                }
            }

            return corpseDataStruct;
        }

        // ICorpseManager 구현
        public void SpawnCorpse(Monster.MonsterName MonsterName, Vector3 startPosition, bool corpseFilpX = true)
        {
            // 만약 시체의 종률르 달리하고 싶다면, 이곳에서 MonsterName을 이용하여 생성할 Corpse prefab을 변경하자.
            ICorpseView corpseView = Instantiate(Resources.Load<GameObject>("Prefab/Monster/Corpse/" + MonsterName.ToString())).GetComponent<CorpseView>();
            corpseView.InitialSetting(MonsterName, startPosition, corpseFilpX);
        }

        // ICorpseManagerForModel 구현
        public float GetPlayerPositionX()
        {
            return this.playerManager.PlayerPosition.x;
        }
        public int GetOwnManaStone()
        {
            return this.playerManager.OwnManaStone;
        }
        public void UpdateOwnManaStone(int theNumberOfManaStone)
        {
            this.playerManager.OwnManaStone = this.playerManager.OwnManaStone + theNumberOfManaStone;
        }

        public void RegisterCorpseInteractionView(Interaction.IPlayerCorpseInteractionManagerForView corpseView)
        {
            this.playerCorpseInteractionManager.RegisterCorpseInteractionView(corpseView);
        }
        public void RemoveCorpseInteractionView(Interaction.IPlayerCorpseInteractionManagerForView corpseView)
        {
            this.playerCorpseInteractionManager.RemoveCorpseInteractionView(corpseView);
        }
        public void RegisterCorpseView(ICorpseView corpseView)
        {
            this.corpseViews.Add(corpseView);
        }
        public void RemoveCorpseView(ICorpseView corpseView)
        {
            this.corpseViews.Remove(corpseView);
        }

        public void SpawnEnemy(Monster.MonsterName MonsterName, Vector3 startPosition)
        {
            this.enemyManager.SpawnEnemy(MonsterName, startPosition, Enemy.EnemySpawnType.SpawnByObject);
        }
        public void SpawnSummon(Monster.MonsterName MonsterName, Vector3 startPosition)
        {
            this.summonManager.SpawnSummon(MonsterName, startPosition, Summon.SummonSpawnType.SpawnByRevival);
        }
    }
}