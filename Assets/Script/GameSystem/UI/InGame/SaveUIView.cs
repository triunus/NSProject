using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.InGameUI
{
    public class SaveUIView : MonoBehaviour, ISummaryGameDataStructObserverForView
    {
        private ISummaryGameDataStructObserver summaryGameDataStructModel;
        private Message.IMessageManager messageManager;

        private RectTransform UIManager;
        private RectTransform SaveUI;

        private SaveAndLoad.SummaryGameDataStruct[] summaryGameDataStruct = new SaveAndLoad.SummaryGameDataStruct[6];

        private void Awake()
        {
            this.summaryGameDataStructModel = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
            this.summaryGameDataStructModel.RegisterSummaryGameDataStructObserver(this);

//                        this.messageManager = GameObject.FindWithTag("MessageManager").GetComponent<Message.MessageManager>();
            this.messageManager = GetComponent<Message.MessageManager>();
            this.UIManager = GetComponent<RectTransform>();

            this.SaveUI = UIManager.GetChild(1).GetChild(1).GetComponent<RectTransform>();

            this.ConnetButton();
        }

        private void ConnetButton()
        {
            this.SaveUI.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { this.SaveGame(1); });
            this.SaveUI.GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { this.SaveGame(2); });
            this.SaveUI.GetChild(1).GetChild(1).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { this.SaveGame(3); });
            this.SaveUI.GetChild(1).GetChild(1).GetChild(3).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { this.SaveGame(4); });
            this.SaveUI.GetChild(1).GetChild(1).GetChild(4).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { this.SaveGame(5); });

            this.SaveUI.GetChild(2).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { this.ExitSavePanel(); });
        }
        private void ActivateParentButton()
        {
            this.UIManager.GetChild(1).GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().interactable = true;
            this.UIManager.GetChild(1).GetChild(0).GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().interactable = true;
            this.UIManager.GetChild(1).GetChild(0).GetChild(1).GetChild(1).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().interactable = true;
            this.UIManager.GetChild(1).GetChild(0).GetChild(1).GetChild(1).GetChild(3).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().interactable = true;

            this.UIManager.GetChild(1).GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().interactable = true;
        }


        private void SetSaveText()
        {
            Debug.Log("SaveUIView - SetSaveText : " + summaryGameDataStruct.Length);

/*            for(int j = 0; j < summaryGameDataStruct.Length; ++j)
            {
                if(summaryGameDataStruct[j] == null) Debug.Log("summaryGameDataStruct[" + j + "].InGameTime : NULL");
                else Debug.Log("summaryGameDataStruct[" + j + "].InGameTime : " + summaryGameDataStruct[j].InGameTime);
            }*/

            for(int i =1; i< summaryGameDataStruct.Length; ++i)
            {
                if(summaryGameDataStruct[i] == null) // ¸ðµÎ null
                {
                    this.SaveUI.GetChild(1).GetChild(1).GetChild(i - 1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Null";
                    this.SaveUI.GetChild(1).GetChild(1).GetChild(i - 1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(3).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Null";
                    this.SaveUI.GetChild(1).GetChild(1).GetChild(i - 1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(5).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Null";

                    this.SaveUI.GetChild(1).GetChild(1).GetChild(i - 1).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Null";
                    this.SaveUI.GetChild(1).GetChild(1).GetChild(i - 1).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(3).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Null";
                    this.SaveUI.GetChild(1).GetChild(1).GetChild(i - 1).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(5).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Null";
                }
                else
                {
                    this.SaveUI.GetChild(1).GetChild(1).GetChild(i - 1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = this.summaryGameDataStruct[i].SceneName.ToString();
                    this.SaveUI.GetChild(1).GetChild(1).GetChild(i - 1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(3).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = System.Convert.ToString(this.summaryGameDataStruct[i].Stage);
                    this.SaveUI.GetChild(1).GetChild(1).GetChild(i - 1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(5).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = System.Convert.ToString(this.summaryGameDataStruct[i].InGameTime);

                    this.SaveUI.GetChild(1).GetChild(1).GetChild(i - 1).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = System.Convert.ToString(this.summaryGameDataStruct[i].OwnManaStone);
                    this.SaveUI.GetChild(1).GetChild(1).GetChild(i - 1).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(3).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = System.Convert.ToString(this.summaryGameDataStruct[i].EnemyCount);
                    this.SaveUI.GetChild(1).GetChild(1).GetChild(i - 1).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(5).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = System.Convert.ToString(this.summaryGameDataStruct[i].SummonCount);
                }
            }
        }

        private void SaveGame(int index)
        {
            if(this.summaryGameDataStruct[index] != null)
            {
                this.messageManager.SpawnMessage(1, index);
            }
            else
            {
                GameManager.Instance.SaveGameData(SaveLoadType.LoadGame, index);
            }
        }
        private void ExitSavePanel()
        {
            this.ActivateParentButton();
            this.SaveUI.gameObject.SetActive(false);
        }

        public void UpdateSummaryGameDataStructObserver()
        {
            this.summaryGameDataStruct = this.summaryGameDataStructModel.GetSummaryGameDataStruct();
            this.SetSaveText();
        }

        private void OnDestroy()
        {
            this.summaryGameDataStructModel.RemoveSummaryGameDataStructObserver(this);
        }
    }
}
