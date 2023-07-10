using UnityEngine;
using UnityEngine.UI;

namespace GameSystem.Corpse
{
    public interface ICorpseView : Interaction.IPlayerCorpseInteractionManagerForView
    {
        public void InitialSetting(Monster.MonsterName MonsterName, Vector3 startPosition, bool corpseFilpX = true);
        public void Destroy();

        public Vector3 CorpsePosition { get; }

        public Monster.MonsterStruct MonsterStruct { get; }
    }

    public class CorpseView : MonoBehaviour, ICorpseView
    {
        private ICorpsePresenter corpsePresenter;

        private SpriteRenderer corpseLineSpriteRenderer;

        private Transform cameraTransform;

        private RectTransform parentUI;
        private RectTransform interactionPanel;
        private RectTransform corpseInformationPanel;

        private Monster.MonsterStruct monsterStruct;
        private bool corpseFilpX = false;
        private Vector3 startPosition;

        private bool isAdjacent = false;    // ���� ���θ� ��Ÿ���� ��.
        private bool isSelected = false;    // InteractionManager�� ���� ����Ǵ� ��.
        private bool UIIsActivated = false; // ���� ��ȣ�ۿ� ������ Ȯ���ϴ� ��.
        private bool isDelete = false;

        // ICorpseView ����
        public Vector3 CorpsePosition { get { return this.gameObject.transform.position; } }

        // IPlayerInteractionManagerForView ����
        public Monster.MonsterStruct MonsterStruct { get { return this.monsterStruct; } }
        public int RevivalCost { get { return this.monsterStruct.RevivalCost; } }
        public int ExtractionProfit { get { return this.monsterStruct.ExtractionProfit; } }

        // Player.IPlayerInteractionManagerForView ����
        public void BeNotifiedSelectInformation()
        {
            this.isSelected = true;
            this.corpseLineSpriteRenderer.enabled = true;
        }
        public void BeNotifiedUnSelectInformation()
        {
            this.isSelected = false;
            this.corpseLineSpriteRenderer.enabled = false;
        }
        public Vector3 GetObjectPosition()
        {
            return this.gameObject.transform.position;
        }

        // ICorpseViewForCorpseManager ����
        /// <summary>
        /// CorpseView�� Ÿ ��ü�� ���� �����ȴ�. �̶�, ���� Event �帧�� ����Ǳ� ���� CorpseView�� �ʿ�� �ϴ� ������ ���ڷ� �����ϴ�.
        /// </summary>
        public void InitialSetting(Monster.MonsterName MonsterName, Vector3 startPosition, bool corpseFilpX = true)
        {
            this.monsterStruct = new Monster.MonsterStruct(MonsterName);
            this.startPosition = startPosition;
            this.corpseFilpX = corpseFilpX;
        }

        public void Start()
        {
            this.cameraTransform = GameObject.FindWithTag("MainCamera").GetComponent<Transform>();

            this.parentUI = GameObject.FindWithTag("InterationUI").GetComponent<RectTransform>();
            this.corpseLineSpriteRenderer = this.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();

            this.corpsePresenter = new CorpsePresenter(this);
            this.corpsePresenter.RegisterCorpseView();

            this.gameObject.transform.position = this.startPosition;
            this.gameObject.GetComponent<SpriteRenderer>().flipX = this.corpseFilpX;
            this.corpseLineSpriteRenderer.flipX = this.corpseFilpX;
            this.corpseLineSpriteRenderer.enabled = false;

            this.CreateInteractionPanel();
            this.CreateCorpseInformationPanel();
        }


        private void FixedUpdate()
        {
            if (this.isDelete) {  }
            else
            {
                if (this.corpsePresenter.WithinPlayerRangeOfCognition())    // ��Ÿ� �� ��?
                {
                    if (this.isAdjacent)                                     // �ʷϻ� ���� ����ϰ� �־�?
                    {
                        if (this.isSelected)                                    // ��ȣ�ۿ� ������� ���É��?
                        {
                            if (this.UIIsActivated) {}   // ��ȣ�ۿ� UIȰ��ȭ ���̾�?
                            else                                                // ��.
                            {
                                this.UIIsActivated = true;
                                this.interactionPanel.gameObject.SetActive(true);       // ��ȣ�ۿ� UI Ȱ��ȭ��.
                            }
                        }
                        else
                        {
                            this.UIIsActivated = false;
                            this.interactionPanel.gameObject.SetActive(false);
                        }
                    }
                    else                                                    // ���� �����.
                    {
                        this.isAdjacent = true;
                        this.UIIsActivated = false;
                        this.corpseLineSpriteRenderer.enabled = true;

                        this.corpsePresenter.RegisterCorpseInteractionView();
                    }
                }
                else                                                            // ��Ÿ� ����?
                {
                    if (this.isAdjacent)                                        // �ʷϻ� ���� ��� ���̾�?
                    {
                        this.isAdjacent = false;
                        this.UIIsActivated = false;
                        this.corpseLineSpriteRenderer.enabled = false;

                        this.interactionPanel.gameObject.SetActive(false);      // ��ȣ�ۿ� UI ��Ȱ��ȭ��
                        this.corpsePresenter.RemoveCorpseInteractionView();
                    }
                }
            }
        }

        private void LateUpdate()
        {
            if (isDelete) { }
            else
            {
                this.AdjustCorpsePanelPosition();
                this.AdjustCorpseInformationPanelPosition();
            }
        }

        private void CreateInteractionPanel()
        {
            this.interactionPanel = Instantiate(Resources.Load<RectTransform>("Prefab/Interaction/CorpsePanel"));
            this.interactionPanel.SetParent(this.parentUI);

            // ��ġ ����.            
            this.AdjustCorpsePanelPosition();

            // enemy Ÿ��Ʋ.
            this.interactionPanel.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = this.monsterStruct.MonsterName.ToString();

            // ��ư ����
            this.interactionPanel.GetChild(2).GetChild(0).GetChild(1).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { this.DisplayCropseInformationPanel(); });
            this.interactionPanel.GetChild(2).GetChild(1).GetChild(1).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { this.ReviveEnemy(); });
            this.interactionPanel.GetChild(2).GetChild(2).GetChild(1).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { this.ExtractManaStone(); });
            this.interactionPanel.GetChild(2).GetChild(3).GetChild(1).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { this.CloseInteractionView(); });

            this.interactionPanel.GetChild(2).GetChild(1).GetChild(2).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = System.Convert.ToString(this.monsterStruct.RevivalCost);
            this.interactionPanel.GetChild(2).GetChild(2).GetChild(2).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = System.Convert.ToString(this.monsterStruct.ExtractionProfit);

            this.interactionPanel.gameObject.SetActive(false);
        }
        private void CreateCorpseInformationPanel()
        {
            this.corpseInformationPanel = Instantiate(Resources.Load<RectTransform>("Prefab/Interaction/CorpseInformationPanel"));
            this.corpseInformationPanel.SetParent(this.parentUI);

            this.AdjustCorpseInformationPanelPosition();

            this.corpseInformationPanel.GetChild(0).GetChild(2).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { this.UnDisplayCorpseInformationPanel(); });

            this.corpseInformationPanel.GetChild(0).GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = this.monsterStruct.MonsterName.ToString();

            this.corpseInformationPanel.GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = System.Convert.ToString(this.monsterStruct.BaseHP);
            this.corpseInformationPanel.GetChild(2).GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = System.Convert.ToString((int)this.monsterStruct.WalkSpeed) + " - " + System.Convert.ToString((int)this.monsterStruct.RunSpeed);
            this.corpseInformationPanel.GetChild(2).GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = this.monsterStruct.NormalAttack.ToString();
            /*            this.corpseInformationPanel.GetChild(2).GetChild(2).GetChild(0).GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = System.Convert.ToString(monsterStruct.); 
                        this.corpseInformationPanel.GetChild(2).GetChild(2).GetChild(1).GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = System.Convert.ToString(monsterStruct.AttackRange);*/

            this.corpseInformationPanel.gameObject.SetActive(false);
        }


        private void AdjustCorpsePanelPosition()
        {
            this.interactionPanel.position = new Vector3(((this.gameObject.transform.position.x - cameraTransform.position.x) + (5f * (float)Screen.width / (float)Screen.height)) * 108, (this.gameObject.transform.position.y * 108) + 650, 0);
        }
        private void AdjustCorpseInformationPanelPosition()
        {
            this.corpseInformationPanel.position = new Vector3((((this.gameObject.transform.position.x - cameraTransform.position.x) + (5f * (float)Screen.width / (float)Screen.height)) * 108) - 370, (this.gameObject.transform.position.y * 108) + 650, 0);
        }
        private Vector3 AdjustRevivalOrExtractionMessagePanelPosition()
        {
            return new Vector3(((this.gameObject.transform.position.x - cameraTransform.position.x) + (5f * (float)Screen.width / (float)Screen.height)) * 108, (this.gameObject.transform.position.y * 108) + 620, 0);
        }
        private Vector3 AdjustLackOfManaStoneMessagePanelPosition()
        {
            return new Vector3(((this.gameObject.transform.position.x - cameraTransform.position.x) + (5f * (float)Screen.width / (float)Screen.height)) * 108, (this.gameObject.transform.position.y * 108) + 540, 0);
        }

        public void DisplayCropseInformationPanel()
        {
            this.corpseInformationPanel.gameObject.SetActive(true);
        }
        public void UnDisplayCorpseInformationPanel()
        {
            this.corpseInformationPanel.gameObject.SetActive(false);
        }

        public void ReviveEnemy()
        {
            if(this.corpsePresenter.GetOwnManaStone() < Mathf.Abs(this.monsterStruct.RevivalCost))
            {
                StopCoroutine("CreateLackOfManaStone");
                StartCoroutine("CreateLackOfManaStone");

                return;
            }

            this.isDelete = true;
            this.corpsePresenter.RemoveCorpseInteractionView();
            this.corpsePresenter.SubManaStoneForRevival();

            StopCoroutine("CreateRevivalOrExtractionMessage");
            StartCoroutine("CreateRevivalOrExtractionMessage", this.monsterStruct.RevivalCost);

            this.corpsePresenter.SpawnSummon();
        }

        public void ExtractManaStone()
        {
            this.isDelete = true;
            this.corpsePresenter.RemoveCorpseInteractionView();
            this.corpsePresenter.AddManaStoneForExtraction();

            StopCoroutine("CreateRevivalOrExtractionMessage");
            StartCoroutine("CreateRevivalOrExtractionMessage", this.monsterStruct.ExtractionProfit);
        }

        public void CloseInteractionView()
        {
            this.interactionPanel.gameObject.SetActive(false);
            this.corpseInformationPanel.gameObject.SetActive(false);
        }

        private System.Collections.IEnumerator CreateRevivalOrExtractionMessage(int count)
        {
            Destroy(this.interactionPanel.gameObject);
            Destroy(this.corpseInformationPanel.gameObject);
            this.corpsePresenter.RemoveCorpseView();

            RectTransform message = Instantiate(Resources.Load<RectTransform>("Prefab/Interaction/RevivalOrExtractionMessage"));
            message.SetParent(this.parentUI);

            message.position =  this.AdjustRevivalOrExtractionMessagePanelPosition();

            if (count < 0) message.GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = System.Convert.ToString(count);
            else message.GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "+" + System.Convert.ToString(count);

            yield return new WaitForSeconds(1.0f);

            Destroy(message.gameObject);
            this.corpsePresenter = null;
            Destroy(this.gameObject);
        }

        private System.Collections.IEnumerator CreateLackOfManaStone()
        {
            RectTransform errorMessage = Instantiate(Resources.Load<RectTransform>("Prefab/Interaction/LackOfManaStoneMessage"));
            errorMessage.SetParent(this.parentUI);

            errorMessage.position = this.AdjustLackOfManaStoneMessagePanelPosition();

            errorMessage.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Manastone is not enough";

            yield return new WaitForSeconds(1.0f);

            Destroy(errorMessage.gameObject);
        }

        public void Destroy()
        {
            this.corpsePresenter.RemoveCorpseView();
            this.corpsePresenter = null;
            Destroy(this.gameObject);
        }
    }
}