using UnityEngine;

namespace GameSystem.InGameUI.Skill
{
    public interface ISkillMenuUIView
    {
        public void InitialSetting(ISkillUIController skillUIController);

        public void ActivateOrUnActivateSkillMenuUI(bool isSkillMenuUIActivated);
        public void ActivateSkillMenu(SkillTreeType skillTreeType);
    }

    // ���� ��, Canvas/UI �ȿ� ����.
    public class SkillMenuUIView : MonoBehaviour, ISkillMenuUIView
    {
        private ISkillUIController_For_SkillMenuUIView skillUIController;

        private RectTransform skillMenuUI;
        private RectTransform menuTypeContentUI;

        private void Awake()
        {
            this.skillMenuUI = this.GetComponent<RectTransform>();
            this.menuTypeContentUI = this.GetComponent<RectTransform>().GetChild(2).GetComponent<RectTransform>();

            this.ConnectButton();
        }
        // Skill Menu UI Prefab�� ���������� �����ϴ� ��ư�鿡 SkillController �޼ҵ� ����.
        private void ConnectButton()
        {
            skillMenuUI.GetChild(0).GetChild(3).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(
                delegate { this.skillUIController.ChangeSkillMenuType(SkillTreeType.Necromancy); });
            skillMenuUI.GetChild(0).GetChild(3).GetChild(1).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(
                delegate { this.skillUIController.ChangeSkillMenuType(SkillTreeType.Class); });
        }

        // ISkillMenuUIView�� �ʱ⼳�� �޼ҵ�.
        public void InitialSetting(ISkillUIController skillUIController)
        {
            this.skillUIController = skillUIController;

            this.CreateNecromancySkillUICell();
            //            this.CreateClassSkillCellUI();
        }
        // Necromancy Skill Tree ���� SkillCellUI ���� �޼ҵ�
        private void CreateNecromancySkillUICell()
        {
            RectTransform necromancySkillContentMenuUI_V = this.skillMenuUI.GetChild(2).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetComponent<RectTransform>();
            RectTransform VContentSV = necromancySkillContentMenuUI_V.GetChild(0).GetComponent<RectTransform>();
            RectTransform VContentBlock = necromancySkillContentMenuUI_V.GetChild(1).GetComponent<RectTransform>();
            RectTransform VContentNSName = necromancySkillContentMenuUI_V.GetChild(2).GetComponent<RectTransform>();

            RectTransform necromancySkillContentMenuUI_H = this.skillMenuUI.GetChild(2).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>();

            // skillUIController ��ü�� '�Ű������� ���޵� RectTransform ��ü' ������ SkillCellUI Prefab ���� ��û.
            this.skillUIController.CreateSkillUICell(necromancySkillContentMenuUI_H, SkillTreeType.Necromancy);

            // Skill Tree�� �����Ǵ� SkillUICell Prefab�� ������ ���� Skill Menu UI ��ü ũ�� ����.
            UnityEngine.UI.GridLayoutGroup gridLayoutGroup = necromancySkillContentMenuUI_H.GetComponent<UnityEngine.UI.GridLayoutGroup>();

            float widht = gridLayoutGroup.cellSize.x * gridLayoutGroup.constraintCount;
            float height = gridLayoutGroup.cellSize.y * necromancySkillContentMenuUI_H.childCount / gridLayoutGroup.constraintCount;

            necromancySkillContentMenuUI_H.sizeDelta = new Vector2(widht, height);

            VContentSV.sizeDelta = new Vector2(VContentSV.sizeDelta.x, height);
            VContentBlock.sizeDelta = new Vector2(VContentBlock.sizeDelta.x, height);
            VContentNSName.sizeDelta = new Vector2(VContentNSName.sizeDelta.x, height);

            necromancySkillContentMenuUI_V.sizeDelta = new Vector2(necromancySkillContentMenuUI_V.sizeDelta.x, height);
        }
        private void CreateClassSkillUICell()
        {
            //            this.classSkillMenuContentUI = this.skillMenuUI.GetChild(2).GetChild(1).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>();
            //            this.skillUIController.CreateSkillUICell(classSkillMenuContentUI, SkillTreeType.Class);
        }

        // ����ڿ� SkillMenuUI Prefab���� ��ȣ�ۿ�.
        /// <summary>
        /// skillMenuUI�� Ȱ��ȭ ���θ� �Ű������� ����
        /// </summary>
        /// <param name="isSkillMenuUIActivated"> skillMenuUI�� Ȱ��ȭ ���� </param>
        public void ActivateOrUnActivateSkillMenuUI(bool isSkillMenuUIActivated)
        {
            this.skillMenuUI.gameObject.SetActive(isSkillMenuUIActivated);
        }
        // Skill Menu UI�� ���� ��� ��ư�� Ŭ���ϸ�, ȣ��Ǵ� �޼ҵ��̴�.
        // Skill Menu UI�� ����(����)�� �����ϴµ� ����Ѵ�.
        public void ActivateSkillMenu(SkillTreeType skillTreeType)
        {
            // Skill Tree ���� ������Ʈ�� ��� ��Ȱ��ȭ
            for(int i = 0; i < menuTypeContentUI.childCount; ++i)
            {
                this.menuTypeContentUI.GetChild(i).gameObject.SetActive(false);
            }

            // �Ű������� ���� SkillTreeType�� ���õ� Skill Tree ���� ������Ʈ�� Ȱ��ȭ.
            switch (skillTreeType)
            {
                case SkillTreeType.Necromancy:
                    this.menuTypeContentUI.GetChild((int)SkillTreeType.Necromancy).gameObject.SetActive(true);
                    break;
                case SkillTreeType.Class:
                    this.menuTypeContentUI.GetChild((int)SkillTreeType.Class).gameObject.SetActive(true);
                    break;                   
            }
        }
    }
}
