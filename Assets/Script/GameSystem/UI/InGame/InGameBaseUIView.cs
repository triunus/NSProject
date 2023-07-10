using UnityEngine;

namespace GameSystem.InGameUI
{
    public class InGameBaseUIView : MonoBehaviour, IMainGameTimeObserverForView, IOneDayTimeObserverForView, Player.IOwnManaStoneObserverForView
    {
        private IMainGameTimeObserver mainGameTimeModel;
        private IOneDayTimeObserver oneDayTimeModel;
        private Player.IOwnManaStoneObserver ownManaStoneModel;

        private RectTransform baseUI;

        private RectTransform timeImage;
        private TMPro.TextMeshProUGUI stageText;
        private TMPro.TextMeshProUGUI theNumberOfManaStone;

        private float mainGameTime = 1;
        private int manaStone = 0;
        private int oneDayTime = 1;
        private int stageNumber = -1;

        private void Awake()
        {
            this.mainGameTimeModel = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
            this.oneDayTimeModel = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
            this.ownManaStoneModel = GameObject.FindWithTag("PlayerManager").GetComponent<Player.PlayerManager>();

            this.mainGameTimeModel.RegisterMainGameTimeObserver(this);
            this.oneDayTimeModel.RegisterOneDayTimeObserver(this);
            this.ownManaStoneModel.RegisterOwnManaStoneObserver(this);

            this.baseUI = GameObject.FindWithTag("Scene01BaseUI").GetComponent<RectTransform>();

            this.timeImage = this.baseUI.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>();
            this.stageText = this.baseUI.GetChild(0).GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
            this.theNumberOfManaStone = this.baseUI.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
        }

        private void FixedUpdate()
        {
            this.DisplayTimeFlow();

            if(this.stageNumber != (int)(this.mainGameTime / this.oneDayTime)) this.DisplayGameStageText();
        }

        private void DisplayTimeFlow()
        {
            this.timeImage.rotation = Quaternion.Euler(new Vector3(0, 0, (float)-1 * 360 / this.oneDayTime * (this.mainGameTime % this.oneDayTime)));
        }
        private void DisplayGameStageText()
        {
            this.stageNumber = (int)(this.mainGameTime / this.oneDayTime);
            this.stageText.text = System.Convert.ToString((int)(this.mainGameTime / this.oneDayTime));
        }

        public void UpdateMainGameTimeObserver()
        {
            this.mainGameTime = this.mainGameTimeModel.GetMainGameTime();
        }
        public void UpdateOneDayTimeObserver()
        {
            this.oneDayTime = this.oneDayTimeModel.GetOneDayTime();
        }
        public void UpdateOwnManaStoneObserver()
        {
            this.manaStone = this.ownManaStoneModel.GetOwnManaStone();

            this.theNumberOfManaStone.text = System.Convert.ToString(this.manaStone);
        }

        private void OnDestroy()
        {
            this.mainGameTimeModel.RemoveMainGameTimeObserver(this);
            this.oneDayTimeModel.RemoveOneDayTimeObserver(this);
            this.ownManaStoneModel.RemoveOwnManaStoneObserver(this);
        }
    }
}