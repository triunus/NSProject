using System.Collections.Generic;
using Newtonsoft.Json.Linq;

using UnityEngine;

namespace GameSystem.Corpse
{
    // Corpse ����
    // 1. Enemy�� ���� ��.
    // 2. GameStory �� Scene�� �̹� ������ ��.
    // 3. Enemy�� skill�� Corpse�� ��Ѹ� ��.

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

        // ICorpseManagerForGameManager ����
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

        // ICorpseManager ����
        public void SpawnCorpse(Monster.MonsterName MonsterName, Vector3 startPosition, bool corpseFilpX = true)
        {
            // ���� ��ü�� ������ �޸��ϰ� �ʹٸ�, �̰����� MonsterName�� �̿��Ͽ� ������ Corpse prefab�� ��������.
            ICorpseView corpseView = Instantiate(Resources.Load<GameObject>("Prefab/Monster/Corpse/" + MonsterName.ToString())).GetComponent<CorpseView>();
            corpseView.InitialSetting(MonsterName, startPosition, corpseFilpX);
        }

        // ICorpseManagerForModel ����
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