using UnityEngine;

namespace GameSystem.InGameUI.Skill
{
    public enum SkillDescriptionType
    {
        Temporary,
        Fixed
    }

    public enum SkillMenuType
    {
        Necromancy = 0,
        Class = 1
    }

    // InteractionSkillUIAndMouseInSkillUIWindow 용
    public interface ISkillUIController_For_InteractionSkillImageUIAndMouseInSkillMenuUI
    {
        // view 용
        public void MouseClickInteraction(SkillUICellMSStruct skillUICellMSStruct);        // skill 아이콘 클릭 이벤트
        public void MouseEnterInteraction(SkillUICellMSStruct skillUICellMSStruct);
        public void MouseExitInteration(SkillUICellMSStruct skillUICellMSStruct);
    }

    // SkillMenuUIView 용
    public interface ISkillUIController_For_SkillMenuUIView
    {
        public void CreateSkillCellUI(RectTransform skillContentRectTransform, SkillMenuType skillMenuType);
        public void ChangeSkillMenuType(SkillMenuType skillMenuType);   // skillMenuType 변경
    }

    // ISkillDescriptionUIView 용
    public interface ISkillUIController_For_ISkillDescriptionUIView
    {
        public void LearnSkill(int skillNumber);
    }

    // SkillManager 용
    public interface ISkillUIController : ISkillUIController_For_InteractionSkillImageUIAndMouseInSkillMenuUI,
        ISkillUIController_For_SkillMenuUIView, ISkillUIController_For_ISkillDescriptionUIView
    {
        public void InitialSetting(ref ISkillUIModel skillUIModel);
    }


    public class SkillUIController : MonoBehaviour, ISkillUIController
    {
        private ISkillUIModel skillUIModel;

        private ISkillMenuUIView skillMenuUIView;
        private System.Collections.Generic.List<ISkillCellUIView> necromancySkillCellUIViews;
        private System.Collections.Generic.List<ISkillCellUIView> classSkillCellUIViews;

        private ISkillDescriptionUIView temporarySkillDescriptionUIView;
        private ISkillDescriptionUIView fixedSkillDescriptionUIView;

        private SkillMenuType skillMenuType;

        private bool skillMenuUIIsCreated;
        private bool fixedSkillDescriptionUIIsCreated;
        private bool isPause;

        private void Awake()
        {
            this.necromancySkillCellUIViews = new System.Collections.Generic.List<ISkillCellUIView>();
            this.classSkillCellUIViews = new System.Collections.Generic.List<ISkillCellUIView>();

            this.temporarySkillDescriptionUIView = null;
            this.fixedSkillDescriptionUIView = null;

            skillMenuType = SkillMenuType.Necromancy;

            skillMenuUIIsCreated = false;
            fixedSkillDescriptionUIIsCreated = false;
            isPause = false;
        }

        // ISkillUIController 구현
        public void InitialSetting(ref ISkillUIModel skillUIModel)
        {
            this.skillUIModel = skillUIModel;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))    // 'K' key 입력 시,
            {
                if (this.isPause)   // UI 비활성화 및 시간 재생
                {
                    this.isPause = false;
                    this.ActivateOrUnActivateSkillMenuUI(false);
                    GameManager.Instance.RequestReGame();
                }
                else                // UI 활성화 및 시간 정지
                {
                    this.isPause = true;
                    this.ActivateOrUnActivateSkillMenuUI(true);
                    GameManager.Instance.RequestPauseGame();
                }
            }
        }

        // SkillMenuUI 초기 설정.
        private void ActivateOrUnActivateSkillMenuUI(bool isActivated)
        {
            // Skill Menu UI Prefab이 생성된 상태가 아니라면, Prefab 생성 메소드 호출.
            if (!skillMenuUIIsCreated) this.CreateSkillMenuUI();

            // Skill Menu UI Prefab이 비활성화 될 시, 고정 스킬 설명 Prefab 삭제시키기.
            if (this.fixedSkillDescriptionUIIsCreated) 
            { 
                this.fixedSkillDescriptionUIView.Destroy();
                this.fixedSkillDescriptionUIIsCreated = false;
            }

            this.skillMenuUIView.ActivateOrUnActivateSkillMenuUI(isActivated);
            if (isActivated) this.skillMenuUIView.ActivateSkillMenu(SkillMenuType.Necromancy);
        }
        // Skill Menu UI Prefab이 생성 메소드.
        private void CreateSkillMenuUI()
        {
            this.skillMenuUIIsCreated = true;

            // Skill Menu UI Prefab 생성.
            // 및 해당 Prefab이 포함하고 있는 SkillMenuUIView 객체 참조.
            this.skillMenuUIView = Instantiate(Resources.Load<RectTransform>("Prefab/UI/SkillUI/SkillMenuUI"),
                GameObject.FindWithTag("UIManager").GetComponent<RectTransform>()).GetComponent<SkillMenuUIView>();
            // skillMenuUIView 초기설정 메소드 호출.
            this.skillMenuUIView.InitialSetting(this);
        }
        public void CreateSkillCellUI(RectTransform skillContentRectTransform, SkillMenuType skillMenuType)
        {
            System.Collections.Generic.List<SkillUICellStruct> tempSkillUICellLineStructs = null;

            if (skillMenuType == SkillMenuType.Necromancy) tempSkillUICellLineStructs = this.skillUIModel.SkillUICellStructs;
//            else tempSkillUICellLineStructs = this.skillUIModel.ClassSkillUICellLineStructs;      // 차후 생성 예정.

            System.Collections.Generic.List<SkillUICellMSStruct> tempSkillUICellMSStructs = this.skillUIModel.SkillUICellMSStructs;
            System.Collections.Generic.List<SkillInformationStruct> tempSkillInformationStructs = this.skillUIModel.SkillInformationStructs;

            int tempSkillNumber;

            // skillUICellStructs 개수만큼 SkillUICell Prefab 생성.
            for (int i = 0; i < tempSkillUICellLineStructs.Count; ++i)
            {
                ISkillCellUIView skillCellUIView = Instantiate(Resources.Load<RectTransform>("Prefab/UI/SkillUI/SkillUICell"), skillContentRectTransform).GetComponent<ISkillCellUIView>();

                tempSkillNumber = tempSkillUICellMSStructs.FindIndex(x => x.CellNumber == tempSkillUICellLineStructs[i].CellNumber);

                // skillUICellStructs.CellContent의 값을 이용하여, SkillUICell Prefab의 역할을 구분. 초기설정 메소드 구분하여 호출.
                switch (tempSkillUICellLineStructs[i].CellContent)
                {
                    case CellContent.non:
                        skillCellUIView.InitialNonCell(ref this.skillUIModel, this);
                        break;
                    case CellContent.line:
                    case CellContent.interchange:
                        skillCellUIView.InitizlLineCell(ref this.skillUIModel, this, tempSkillUICellLineStructs[i]);
                        break;
                    case CellContent.sub:
                    case CellContent.main:
                        skillCellUIView.InitializeMainAndSubCell(ref this.skillUIModel, this, tempSkillUICellLineStructs[i], tempSkillUICellMSStructs[tempSkillNumber], tempSkillInformationStructs[tempSkillNumber]);
                        break;
                }

                this.necromancySkillCellUIViews.Add(skillCellUIView);
            }
        }
        private void CreateSkillDescriptionUI(SkillDescriptionType skillDescriptionType, SkillUICellMSStruct skillUICellMSStruct)
        {
            if (skillDescriptionType == SkillDescriptionType.Temporary)
            {
                this.temporarySkillDescriptionUIView = Instantiate(Resources.Load<RectTransform>("Prefab/UI/SkillUI/TemporarySkillDescriptionUI"), GameObject.FindWithTag("UIManager").GetComponent<RectTransform>()).GetComponent<SkillDescriptionUIView>(); ;
                this.temporarySkillDescriptionUIView.InitialSetting(ref this.skillUIModel, this, skillDescriptionType, skillUICellMSStruct,
                ref this.skillUIModel.SkillInformationStructs, this.skillUIModel.GetMSPreconditionStruct(skillUICellMSStruct.SkillNumber));
            }
            else
            {
                this.fixedSkillDescriptionUIView = Instantiate(Resources.Load<RectTransform>("Prefab/UI/SkillUI/FixedSkillDescriptionUI"), GameObject.FindWithTag("UIManager").GetComponent<RectTransform>()).GetComponent<SkillDescriptionUIView>(); ;
                this.fixedSkillDescriptionUIView.InitialSetting(ref this.skillUIModel, this, skillDescriptionType, skillUICellMSStruct,
                ref this.skillUIModel.SkillInformationStructs, this.skillUIModel.GetMSPreconditionStruct(skillUICellMSStruct.SkillNumber));
            }
        }


        // InteractionSkillImageUIAndMouseInSkillMenuUI 구현
        public void MouseClickInteraction(SkillUICellMSStruct skillUICellMSStruct)
        {
            this.skillUIModel.DecideActivateOrInActivateCell(skillUICellMSStruct);

            if (!this.fixedSkillDescriptionUIIsCreated)
            {
                this.fixedSkillDescriptionUIIsCreated = true;
                this.CreateSkillDescriptionUI(SkillDescriptionType.Fixed, skillUICellMSStruct);
            }
            else if (this.fixedSkillDescriptionUIIsCreated && this.fixedSkillDescriptionUIView.SkillNumber == skillUICellMSStruct.SkillNumber)
            {
                this.fixedSkillDescriptionUIIsCreated = false;
                this.fixedSkillDescriptionUIView.Destroy();
            }
            else if (this.fixedSkillDescriptionUIIsCreated && this.fixedSkillDescriptionUIView.SkillNumber != skillUICellMSStruct.SkillNumber)
            {
                this.fixedSkillDescriptionUIView.Destroy();
                this.CreateSkillDescriptionUI(SkillDescriptionType.Fixed, skillUICellMSStruct);
            }
        }
        public void MouseEnterInteraction(SkillUICellMSStruct skillUICellMSStruct)
        {
            this.CreateSkillDescriptionUI(SkillDescriptionType.Temporary, skillUICellMSStruct);
        }
        public void MouseExitInteration(SkillUICellMSStruct skillUICellMSStruct)
        {
            this.temporarySkillDescriptionUIView.Destroy();
            this.temporarySkillDescriptionUIView = null;
        }

        // ISkillUIController_For_SkillMenuUIView 구현
        public void ChangeSkillMenuType(SkillMenuType skillMenuType)
        {
            this.skillMenuUIView.ActivateSkillMenu(skillMenuType);
        }

        // ISkillUIController_For_ISkillDescriptionUIView 구현
        public void LearnSkill(int skillNumber)
        {
            this.skillUIModel.LearnSkill(skillNumber);
        }
    }
}