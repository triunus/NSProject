using UnityEngine;

namespace GameSystem.Interaction
{
    public interface IPlayerCorpseInteractionManager
    {
        public void RegisterCorpseInteractionView(IPlayerCorpseInteractionManagerForView corpseView);
        public void RemoveCorpseInteractionView(IPlayerCorpseInteractionManagerForView corpseView);
    }

    public interface IPlayerCorpseInteractionManagerForView
    {
        public Vector3 GetObjectPosition();
        public void BeNotifiedSelectInformation();
        public void BeNotifiedUnSelectInformation();
    }

    public class PlayerCorpseInteractionManager : MonoBehaviour, IPlayerCorpseInteractionManager
    {
        private Player.IPlayerManager playerManager;

        private System.Collections.Generic.List<IPlayerCorpseInteractionManagerForView> corpseViewes
            = new System.Collections.Generic.List<IPlayerCorpseInteractionManagerForView>();

        private int selectedIndex = 0;

        private void Awake()
        {
            this.playerManager = GameObject.FindWithTag("PlayerManager").GetComponent<Player.PlayerManager>();
        }

        private void FixedUpdate()
        {
            if (corpseViewes.Count > 0) FindNearestObject();
        }

        // IPlayerCorpseInteractionManager ±¸Çö
        public void RegisterCorpseInteractionView(IPlayerCorpseInteractionManagerForView corpseView)
        {
            this.corpseViewes.Add(corpseView);
        }
        public void RemoveCorpseInteractionView(IPlayerCorpseInteractionManagerForView corpseView)
        {
            if (this.selectedIndex > 0) --this.selectedIndex;
            corpseView.BeNotifiedUnSelectInformation();
            this.corpseViewes.Remove(corpseView);
        }

        private void FindNearestObject()
        {
            if (corpseViewes.Count == 1)
            {
                this.corpseViewes[this.selectedIndex].BeNotifiedUnSelectInformation();
                this.selectedIndex = 0;
                this.corpseViewes[this.selectedIndex].BeNotifiedSelectInformation();

                return;
            }

            float minDistance = 99f;
            int minIndex = 0;

            for (int i = 0; i < corpseViewes.Count; ++i)
            {
                float distance;
                distance = Mathf.Min(minDistance, Mathf.Abs(this.playerManager.PlayerPosition.x - this.corpseViewes[i].GetObjectPosition().x));

                if (minDistance != distance)
                {
                    minDistance = distance;
                    minIndex = i;
                }
            }

            if (this.selectedIndex != minIndex)
            {
                this.corpseViewes[this.selectedIndex].BeNotifiedUnSelectInformation();
                this.selectedIndex = minIndex;
            }

            this.corpseViewes[this.selectedIndex].BeNotifiedSelectInformation();
        }

    }
}