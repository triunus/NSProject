using UnityEngine;

namespace GameSystem.Message
{
    public interface IMessageView
    {
        public void InitialSetting(string[] messageContent, int fileIndex);
    }

    public class MessageView : MonoBehaviour, IMessageView
    {
        private RectTransform UIManager;
        private RectTransform messagePanel;

        private string[] messageContent = new string[3];
        private int fileIndex;

        private void Awake()
        {
            this.UIManager = GameObject.FindWithTag("UIManager").GetComponent<RectTransform>();
            this.messagePanel = GetComponent<RectTransform>();
            this.messagePanel.SetParent(this.UIManager);

            this.messagePanel.position = new Vector3(960 , 540, 0);

            // 클릭 통제.
            this.BlockRaycastChildPanel();
        }
        public void InitialSetting(string[] messageContent, int fileIndex)
        {
            this.messageContent = messageContent;
            this.fileIndex = fileIndex;
            this.FillInMessageContent();
            this.ConnectButton();
        }

        private void DontBlockRaycastChildPanel()
        {
            for(int i =0; i< this.UIManager.childCount; ++i)
            {
                this.UIManager.GetChild(i).GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
        }
        private void BlockRaycastChildPanel()
        {
            for (int i = 0; i < this.UIManager.childCount; ++i)
            {
                this.UIManager.GetChild(i).GetComponent<CanvasGroup>().blocksRaycasts = false;
            }

            this.messagePanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }

        private void FillInMessageContent()
        {
            this.messagePanel.GetChild(0).GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = messageContent[1];
            this.messagePanel.GetChild(2).GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = messageContent[2];
        }
        private void ConnectButton()
        {
            this.messagePanel.GetChild(0).GetChild(2).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { this.Destroy(); });
            this.messagePanel.GetChild(4).GetChild(2).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { this.OperateRelatedFunction(); });
        }

        private void OperateRelatedFunction()
        {
            switch(System.Convert.ToInt32(this.messageContent[0]))
            {
                case 1:
                    Debug.Log("MessageView - this.fileIndex : " + this.fileIndex);
                    GameManager.Instance.SaveGameData(SaveLoadType.LoadGame, this.fileIndex);
                    break;
                case 11:
                    break;
            }

            this.Destroy();
        }

        private void Destroy()
        {
            Destroy(this.gameObject);

            this.DontBlockRaycastChildPanel();
        }
    }
}
