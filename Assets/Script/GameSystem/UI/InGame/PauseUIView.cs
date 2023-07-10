using UnityEngine;

namespace GameSystem.InGameUI
{
    public interface IPauseUIView { }

    public class PauseUIView : MonoBehaviour
    {
        private RectTransform pauseUI;

        private bool isPause = false;

        private void Awake()
        {
            this.pauseUI = GameObject.FindWithTag("PauseUI").GetComponent<RectTransform>();
            this.ConnectButton();

        }
        private void Start()
        {
            this.pauseUI.GetChild(1).gameObject.SetActive(false);
            this.pauseUI.GetChild(2).gameObject.SetActive(false);
            this.pauseUI.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (this.isPause)
                {
                    this.isPause = false;
                    GameManager.Instance.RequestReGame();
                    this.pauseUI.gameObject.SetActive(false);
                }
                else
                {
                    this.isPause = true;
                    GameManager.Instance.RequestPauseGame();
                    this.pauseUI.gameObject.SetActive(true);
                }
            }
        }

        private void ConnectButton()
        {
            this.pauseUI.GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { this.SaveGameData(); });
            this.pauseUI.GetChild(0).GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { this.LoadGameData(); });
            this.pauseUI.GetChild(0).GetChild(1).GetChild(1).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { this.ReturnToLobby(); });
            this.pauseUI.GetChild(0).GetChild(1).GetChild(1).GetChild(3).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { this.ExitTheGame(); });

            this.pauseUI.GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { this.ReturnToTheGame(); });
        }
        private void UnActivateButton()
        {
            this.pauseUI.GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().interactable = false;
            this.pauseUI.GetChild(0).GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().interactable = false;
            this.pauseUI.GetChild(0).GetChild(1).GetChild(1).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().interactable = false;
            this.pauseUI.GetChild(0).GetChild(1).GetChild(1).GetChild(3).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().interactable = false;

            this.pauseUI.GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().interactable = false;
        }

        private void SaveGameData()
        {
            this.UnActivateButton();
            this.pauseUI.GetChild(2).gameObject.SetActive(false);
            this.pauseUI.GetChild(1).gameObject.SetActive(true);
        }
        private void LoadGameData()
        {
            this.UnActivateButton();
            this.pauseUI.GetChild(1).gameObject.SetActive(false);
            this.pauseUI.GetChild(2).gameObject.SetActive(true);
        }
        private void ReturnToLobby()
        {
            GameManager.Instance.SaveGameData(SaveLoadType.Continue);
            GameManager.Instance.ChoiceSceneChangeOperationType(SceneChangeType.InGameToGameLobby, SceneName.GameLobby);

            this.gameObject.SetActive(false);
        }
        private void ExitTheGame()
        {
            GameManager.Instance.SaveGameData(SaveLoadType.Continue);
            Application.Quit();            // 게임 종료
        }

        private void ReturnToTheGame()
        {
            this.isPause = false;
            GameManager.Instance.RequestReGame();
            this.pauseUI.gameObject.SetActive(false);
        }
    }
}