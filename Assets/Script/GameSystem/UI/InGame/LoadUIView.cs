using UnityEngine;

namespace GameSystem.InGameUI
{
    public class LoadUIView : MonoBehaviour, ISummaryGameDataStructObserverForView
    {
        private ISummaryGameDataStructObserver summaryGameDataStructModel;
        private Message.IMessageManager messageManager;

        private RectTransform UIManager;
        private RectTransform LoadUI;

        private SaveAndLoad.SummaryGameDataStruct[] summaryGameDataStruct = new SaveAndLoad.SummaryGameDataStruct[6];

        private void Awake()
        {
//            Debug.Log("LoadUIView - Awake");
            this.summaryGameDataStructModel = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
            this.summaryGameDataStructModel.RegisterSummaryGameDataStructObserver(this);

//            this.messageManager = GameObject.FindWithTag("MessageManager").GetComponent<Message.MessageManager>();
            this.messageManager = GetComponent<Message.MessageManager>();
            this.UIManager = GetComponent<RectTransform>();

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("GameLobby")) this.LoadUI = UIManager.GetChild(1).GetComponent<RectTransform>();
            else this.LoadUI = UIManager.GetChild(1).GetChild(2).GetComponent<RectTransform>();

            this.ConnetButton();
        }

        private void ActivateParentButton()
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("GameLobby"))
            {
                this.UIManager.GetChild(0).GetChild(2).GetChild(2).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().interactable = true;
                this.UIManager.GetChild(0).GetChild(2).GetChild(2).GetChild(1).GetChild(0).GetComponent<UnityEngine.UI.Button>().interactable = true;
                this.UIManager.GetChild(0).GetChild(2).GetChild(2).GetChild(2).GetChild(0).GetComponent<UnityEngine.UI.Button>().interactable = true;
                this.UIManager.GetChild(0).GetChild(2).GetChild(2).GetChild(3).GetChild(0).GetComponent<UnityEngine.UI.Button>().interactable = true;
            }
            else
            {
                this.UIManager.GetChild(1).GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().interactable = true;
                this.UIManager.GetChild(1).GetChild(0).GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().interactable = true;
                this.UIManager.GetChild(1).GetChild(0).GetChild(1).GetChild(1).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().interactable = true;
                this.UIManager.GetChild(1).GetChild(0).GetChild(1).GetChild(1).GetChild(3).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().interactable = true;

                this.UIManager.GetChild(1).GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().interactable = true;
            }
        }

        private void ConnetButton()
        {
            this.LoadUI.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { this.LoadGame(0); });
            this.LoadUI.GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { this.LoadGame(1); });
            this.LoadUI.GetChild(1).GetChild(1).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { this.LoadGame(2); });
            this.LoadUI.GetChild(1).GetChild(1).GetChild(3).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { this.LoadGame(3); });
            this.LoadUI.GetChild(1).GetChild(1).GetChild(4).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { this.LoadGame(4); });
            this.LoadUI.GetChild(1).GetChild(1).GetChild(5).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { this.LoadGame(5); });

            this.LoadUI.GetChild(2).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { this.ExitLoadPanel(); });
        }
        private void SetLoadText()
        {
            for (int i = 0; i < summaryGameDataStruct.Length; ++i)
            {
                if (summaryGameDataStruct[i] == null) // ¸ðµÎ null
                {
                    this.LoadUI.GetChild(1).GetChild(1).GetChild(i).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Null";
                    this.LoadUI.GetChild(1).GetChild(1).GetChild(i).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(3).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Null";
                    this.LoadUI.GetChild(1).GetChild(1).GetChild(i).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(5).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Null";

                    this.LoadUI.GetChild(1).GetChild(1).GetChild(i).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Null";
                    this.LoadUI.GetChild(1).GetChild(1).GetChild(i).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(3).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Null";
                    this.LoadUI.GetChild(1).GetChild(1).GetChild(i).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(5).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Null";
                }
                else
                {
                    this.LoadUI.GetChild(1).GetChild(1).GetChild(i).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = this.summaryGameDataStruct[i].SceneName.ToString();
                    this.LoadUI.GetChild(1).GetChild(1).GetChild(i).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(3).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = System.Convert.ToString(this.summaryGameDataStruct[i].Stage);
                    this.LoadUI.GetChild(1).GetChild(1).GetChild(i).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(5).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = System.Convert.ToString(this.summaryGameDataStruct[i].InGameTime);

                    this.LoadUI.GetChild(1).GetChild(1).GetChild(i).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = System.Convert.ToString(this.summaryGameDataStruct[i].OwnManaStone);
                    this.LoadUI.GetChild(1).GetChild(1).GetChild(i).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(3).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = System.Convert.ToString(this.summaryGameDataStruct[i].EnemyCount);
                    this.LoadUI.GetChild(1).GetChild(1).GetChild(i).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(5).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = System.Convert.ToString(this.summaryGameDataStruct[i].SummonCount);
                }
            }
        }

        private void LoadGame(int index)
        {
            if(this.summaryGameDataStruct[index] == null)
            {
                this.messageManager.SpawnMessage(11, index);
            }
            else
            {
                if (index == 0) GameManager.Instance.LoadGameData(SaveLoadType.Continue);
                else GameManager.Instance.LoadGameData(SaveLoadType.LoadGame, index);

                GameManager.Instance.ChoiceSceneChangeOperationType(SceneChangeType.ChangeToLoadedGame);

                this.ActivateParentButton();
                this.LoadUI.gameObject.SetActive(false);
                this.LoadUI.parent.gameObject.SetActive(false);
            }
        }

        private void ExitLoadPanel()
        {
            this.ActivateParentButton();
            this.LoadUI.gameObject.SetActive(false);
        }
        public void UpdateSummaryGameDataStructObserver()
        {
//            Debug.Log("LoadUIView - UpdateSummaryGameDataStructObserver");

            this.summaryGameDataStruct = this.summaryGameDataStructModel.GetSummaryGameDataStruct();
            this.SetLoadText();
        }

        private void OnDestroy()
        {
            this.summaryGameDataStructModel.RemoveSummaryGameDataStructObserver(this);
        }
    }
}
