using UnityEngine;

namespace GameSystem.GameLobby
{
    public class GameLobbyView : MonoBehaviour, ISummaryGameDataStructObserverForView
    {
        private ISummaryGameDataStructObserver summaryGameDataStructModel;
        private Message.IMessageManager messageManager;

        private RectTransform UIManager;

        private SaveAndLoad.SummaryGameDataStruct[] summaryGameDataStruct = new SaveAndLoad.SummaryGameDataStruct[6];

        private void Awake()
        {
//            Debug.Log("GameStartController - Awake");
            this.summaryGameDataStructModel = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
            this.summaryGameDataStructModel.RegisterSummaryGameDataStructObserver(this);

            this.messageManager = GetComponent<Message.MessageManager>();
            this.UIManager = GetComponent<RectTransform>();
            this.ConnectButton();

            this.UIManager.GetChild(1).gameObject.SetActive(false);
        }

        private void ConnectButton()
        {
            this.UIManager.GetChild(0).GetChild(2).GetChild(2).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { this.OnClickedNewGameButton(); });
            this.UIManager.GetChild(0).GetChild(2).GetChild(2).GetChild(1).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { this.OnClickedContinueButton(); });
            this.UIManager.GetChild(0).GetChild(2).GetChild(2).GetChild(2).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { this.OnClickedLoadGameButton(); });
            this.UIManager.GetChild(0).GetChild(2).GetChild(2).GetChild(3).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { this.OnClickedExitGameButton(); });
        }
        private void UnActivateButton()
        {
            this.UIManager.GetChild(0).GetChild(2).GetChild(2).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().interactable = false;
            this.UIManager.GetChild(0).GetChild(2).GetChild(2).GetChild(1).GetChild(0).GetComponent<UnityEngine.UI.Button>().interactable = false;
            this.UIManager.GetChild(0).GetChild(2).GetChild(2).GetChild(2).GetChild(0).GetComponent<UnityEngine.UI.Button>().interactable = false;
            this.UIManager.GetChild(0).GetChild(2).GetChild(2).GetChild(3).GetChild(0).GetComponent<UnityEngine.UI.Button>().interactable = false;
        }

        public void OnClickedNewGameButton()
        {
            GameManager.Instance.LoadGameData(SaveLoadType.NewGame);
            GameManager.Instance.ChoiceSceneChangeOperationType(SceneChangeType.GameLobbyToInGame);
        }

        public void OnClickedContinueButton()
        {
            if (this.summaryGameDataStruct[0] == null)
            {
                this.messageManager.SpawnMessage(21 ,0);
            }
            else
            {
                GameManager.Instance.LoadGameData(SaveLoadType.Continue);
                GameManager.Instance.ChoiceSceneChangeOperationType(SceneChangeType.GameLobbyToInGame);
            }
        }

        public void OnClickedLoadGameButton()
        {
            // 버튼 비활성화.
            this.UnActivateButton();
            // Load Panel 생성.
            this.UIManager.GetChild(1).gameObject.SetActive(true);
        }

        public void OnClickedExitGameButton()
        {
            Debug.Log("Exit Game");
            Application.Quit();            // 게임 종료
        }

        public void UpdateSummaryGameDataStructObserver()
        {
//            Debug.Log("GameStartContoller - UpdateSummaryGameDataStructObserver");

            this.summaryGameDataStruct = this.summaryGameDataStructModel.GetSummaryGameDataStruct();
        }

        private void OnDestroy()
        {
            this.summaryGameDataStructModel.RemoveSummaryGameDataStructObserver(this);
        }
    }
}