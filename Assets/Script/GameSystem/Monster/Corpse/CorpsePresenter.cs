using UnityEngine;

namespace GameSystem.Corpse
{
    public interface ICorpsePresenter
    {
        public void RegisterCorpseInteractionView();
        public void RemoveCorpseInteractionView();
        public void RegisterCorpseView();
        public void RemoveCorpseView();
        public void AddManaStoneForExtraction();
        public void SubManaStoneForRevival();
        public int GetOwnManaStone();

        public void SpawnSummon();

        public bool WithinPlayerRangeOfCognition();
    }

    public class CorpsePresenter : ICorpsePresenter
    {
        private ICorpseModel corpseModel;
        private ICorpseView corpseView;

        public CorpsePresenter(ICorpseView corpseView)
        {
            this.corpseModel = GameObject.FindWithTag("CorpseManager").GetComponent<CorpseModel>();
            this.corpseView = corpseView;
        }

        public void RegisterCorpseInteractionView()
        {
            this.corpseModel.RegisterCorpseInteractionView(this.corpseView);
        }
        public void RemoveCorpseInteractionView()
        {
            this.corpseModel.RemoveCorpseInteractionView(this.corpseView);
        }
        public void RegisterCorpseView()
        {
            this.corpseModel.RegisterCorpseView(this.corpseView);
        }
        public void RemoveCorpseView()
        {
            this.corpseModel.RemoveCorpseView(this.corpseView);
        }
        public void SubManaStoneForRevival()
        {
            this.corpseModel.AddOrSubManaStone(this.corpseView.MonsterStruct.RevivalCost);
        }
        public void AddManaStoneForExtraction()
        {
            this.corpseModel.AddOrSubManaStone(this.corpseView.MonsterStruct.ExtractionProfit);
        }
        public int GetOwnManaStone()
        {
            return this.corpseModel.GetOwnManaStone();
        }

        public void SpawnSummon()
        {
            this.corpseModel.SpawnSummon(this.corpseView.MonsterStruct.MonsterName, this.corpseView.CorpsePosition);
        }

        public bool WithinPlayerRangeOfCognition()
        {
            return this.corpseModel.WithinPlayerRangeOfCognition(this.corpseView.CorpsePosition.x);
        }
    }

}