using UnityEngine;

namespace GameSystem.InGameUI.Skill
{
    public interface ISkillMenuUIView
    {
        public void InitialSetting(ISkillUIController skillUIController, int skillTreeColumnCount);

        public void ActivateOrUnActivateSkillMenuUI(bool isSkillMenuUIActivated);
//        public void ActivateSkillMenu(SkillTreeType skillTreeType);
    }

    // 생성 시, Canvas/UI 안에 들어간다.
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
        // Skill Menu UI Prefab에 직접적으로 존재하는 버튼들에 SkillController 메소드 연결.
        private void ConnectButton()
        {
            skillMenuUI.GetChild(0).GetChild(3).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { this.skillUIController.ChangeSkillMenuType(SkillTreeType.Necromancy); });
            skillMenuUI.GetChild(0).GetChild(3).GetChild(1).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { this.skillUIController.ChangeSkillMenuType(SkillTreeType.Class); });
        }

        // ISkillMenuUIView의 초기설정 메소드.
        public void InitialSetting(ISkillUIController skillUIController, int skillTreeColumnCount)
        {
            this.skillUIController = skillUIController;

            this.CreateSkillTree(skillTreeColumnCount);
        }
        // Necromancy Skill Tree 하위 SkillCellUI 생성 메소드
        private void CreateSkillTree(int skillTreeColumnCount)
        {
            RectTransform necromancySkillContentMenuUI_V = this.skillMenuUI.GetChild(2).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetComponent<RectTransform>();
            RectTransform VContentSV = necromancySkillContentMenuUI_V.GetChild(0).GetComponent<RectTransform>();
            RectTransform VContentBlock = necromancySkillContentMenuUI_V.GetChild(1).GetComponent<RectTransform>();
            RectTransform VContentNSName = necromancySkillContentMenuUI_V.GetChild(2).GetComponent<RectTransform>();

            RectTransform necromancySkillContentMenuUI_H = this.skillMenuUI.GetChild(2).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>();

            // skillUIController 객체에 '매개변수로 전달된 RectTransform 객체' 하위에 SkillCellUI Prefab 생성 요청.
            this.skillUIController.CreateSkillUICell(necromancySkillContentMenuUI_H);

            // Skill Tree에 생성되는 SkillUICell Prefab의 개수에 따라 Skill Menu UI 객체 크기 조정.
            UnityEngine.UI.GridLayoutGroup gridLayoutGroup = necromancySkillContentMenuUI_H.GetComponent<UnityEngine.UI.GridLayoutGroup>();
            gridLayoutGroup.constraintCount = skillTreeColumnCount;

            float widht = gridLayoutGroup.cellSize.x * gridLayoutGroup.constraintCount;
            float height = gridLayoutGroup.cellSize.y * necromancySkillContentMenuUI_H.childCount / gridLayoutGroup.constraintCount;

            necromancySkillContentMenuUI_H.sizeDelta = new Vector2(widht, height);

            VContentSV.sizeDelta = new Vector2(VContentSV.sizeDelta.x, height);
            VContentBlock.sizeDelta = new Vector2(VContentBlock.sizeDelta.x, height);
            VContentNSName.sizeDelta = new Vector2(VContentNSName.sizeDelta.x, height);

            necromancySkillContentMenuUI_V.sizeDelta = new Vector2(necromancySkillContentMenuUI_V.sizeDelta.x, height);
        }

        // 사용자와 SkillMenuUI Prefab과의 상호작용.
        // skillMenuUI의 활성화 여부를 매개변수로 전달
        public void ActivateOrUnActivateSkillMenuUI(bool isSkillMenuUIActivated)
        {
            this.skillMenuUI.gameObject.SetActive(isSkillMenuUIActivated);
        }

        // 23.07.21 : 기획이 변경되었다. 당분간은 사용하지 않을 예정이다.
/*        // Skill Menu UI의 우측 상단 버튼을 클릭하면, 호출되는 메소드이다.
        // Skill Menu UI의 내용(주제)을 변경하는데 사용한다.
        public void ActivateSkillMenu(SkillTreeType skillTreeType)
        {
            // Skill Tree 하위 오브젝트를 모두 비활성화
            for(int i = 0; i < menuTypeContentUI.childCount; ++i)
            {
                this.menuTypeContentUI.GetChild(i).gameObject.SetActive(false);
            }

            // 매개변수로 받은 SkillTreeType과 관련된 Skill Tree 게임 오브젝트를 활성화.
            switch (skillTreeType)
            {
                case SkillTreeType.Necromancy:
                    this.menuTypeContentUI.GetChild((int)SkillTreeType.Necromancy).gameObject.SetActive(true);
                    break;
                case SkillTreeType.Class:
                    this.menuTypeContentUI.GetChild((int)SkillTreeType.Class).gameObject.SetActive(true);
                    break;                   
            }
        }*/
    }
}
