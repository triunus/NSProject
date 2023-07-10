using UnityEngine;

namespace GameSystem.Corpse
{
    public interface ICorpseModel
    {
        public void RegisterCorpseInteractionView(Interaction.IPlayerCorpseInteractionManagerForView corpseView);
        public void RemoveCorpseInteractionView(Interaction.IPlayerCorpseInteractionManagerForView corpseView);
        public void RegisterCorpseView(ICorpseView corpseView);
        public void RemoveCorpseView(ICorpseView corpseView);
        public void AddOrSubManaStone(int theNumberOfManastone);
        public int GetOwnManaStone();

        public bool WithinPlayerRangeOfCognition(float CorpsePositionX);
        public void SpawnSummon(Monster.MonsterName MonsterName, Vector3 startPosition);
    }

    public class CorpseModel : MonoBehaviour, ICorpseModel
    {
        private ICorpseManagerForModel corpseManager;

        private float recognitionRange = 2f;

        private void Awake()
        {
            this.corpseManager = GetComponent<CorpseManager>();
        }

        // interactionManager 구현 부분.
        public void RegisterCorpseInteractionView(Interaction.IPlayerCorpseInteractionManagerForView corpseView)
        {
            this.corpseManager.RegisterCorpseInteractionView(corpseView);
        }
        public void RemoveCorpseInteractionView(Interaction.IPlayerCorpseInteractionManagerForView corpseView)
        {
            this.corpseManager.RemoveCorpseInteractionView(corpseView);
        }
        public void RegisterCorpseView(ICorpseView corpseView)
        {
            this.corpseManager.RegisterCorpseView(corpseView);
        }
        public void RemoveCorpseView(ICorpseView corpseView)
        {
            this.corpseManager.RemoveCorpseView(corpseView);
        }
        public int GetOwnManaStone()
        {
            return this.corpseManager.GetOwnManaStone();
        }
        public void AddOrSubManaStone(int theNumberOfManastone)
        {
            this.corpseManager.UpdateOwnManaStone(theNumberOfManastone);
        }

        public void SpawnSummon(Monster.MonsterName MonsterName, Vector3 startPosition)
        {
            this.corpseManager.SpawnSummon(MonsterName, startPosition);
        }

        public bool WithinPlayerRangeOfCognition(float CorpsePositionX)
        {
            return Mathf.Abs(this.corpseManager.GetPlayerPositionX() - CorpsePositionX) < recognitionRange ? true : false;
        }
    }
}