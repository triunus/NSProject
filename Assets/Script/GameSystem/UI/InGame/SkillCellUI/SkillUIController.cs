using UnityEngine;

namespace GameSystem.InGameUI.Skill
{
    public enum SkillDescriptionType
    {
        Temporary,
        Fixed
    }

    public enum SkillTreeType
    {
        Necromancy = 0,
        Class = 1
    }

    // InteractionSkillUIAndMouseInSkillUIWindow 용
    public interface ISkillUIController_For_InteractionSkillImageUIAndMouseInSkillMenuUI
    {
        // view 용
        public void MouseClickInteraction(SkillUICellMainSubStruct SkillUICellMainSubStruct);        // skill 아이콘 클릭 이벤트
        public void MouseEnterInteraction(SkillUICellMainSubStruct SkillUICellMainSubStruct);
        public void MouseExitInteration(SkillUICellMainSubStruct SkillUICellMainSubStruct);
    }

    // SkillMenuUIView 용
    public interface ISkillUIController_For_SkillMenuUIView
    {
        public void CreateSkillUICell(RectTransform skillContentRectTransform, SkillTreeType SkillTreeType);
        public void ChangeSkillMenuType(SkillTreeType SkillTreeType);   // SkillTreeType 변경
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
        private System.Collections.Generic.List<ISkillUICellView> necromancySkillCellUIViews;
        private System.Collections.Generic.List<ISkillUICellView> classSkillCellUIViews;

        private ISkillDescriptionUIView temporarySkillDescriptionUIView;
        private ISkillDescriptionUIView fixedSkillDescriptionUIView;

        private SkillTreeType SkillTreeType;

        private bool skillMenuUIIsCreated;
        private bool fixedSkillDescriptionUIIsCreated;
        private bool isPause;

        private void Awake()
        {
            this.necromancySkillCellUIViews = new System.Collections.Generic.List<ISkillUICellView>();
            this.classSkillCellUIViews = new System.Collections.Generic.List<ISkillUICellView>();

            this.temporarySkillDescriptionUIView = null;
            this.fixedSkillDescriptionUIView = null;

            SkillTreeType = SkillTreeType.Necromancy;

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
            if (isActivated) this.skillMenuUIView.ActivateSkillMenu(SkillTreeType.Necromancy);
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
        public void CreateSkillUICell(RectTransform skillContentRectTransform, SkillTreeType SkillTreeType)
        {
            System.Collections.Generic.List<SkillUICellStruct> tempSkillUICellStructs = null;

            if (SkillTreeType == SkillTreeType.Necromancy) tempSkillUICellStructs = this.skillUIModel.SkillUICellStructs;
//            else tempSkillUICellStructs = this.skillUIModel.ClassSkillUICellLineStructs;      // 차후 생성 예정.

            System.Collections.Generic.List<SkillUICellMainSubStruct> tempSkillUICellMainSubStructs = this.skillUIModel.SkillUICellMainSubStructs;
            System.Collections.Generic.List<SkillInformationStruct> tempSkillInformationStructs = this.skillUIModel.SkillInformationStructs;

            int tempSkillNumber;

            // skillUICellStructs 개수만큼 SkillUICell Prefab 생성.
            for (int i = 0; i < tempSkillUICellStructs.Count; ++i)
            {
                ISkillUICellView SkillUICellView = Instantiate(Resources.Load<RectTransform>("Prefab/UI/SkillUI/SkillUICell"), skillContentRectTransform).GetComponent<ISkillUICellView>();

                tempSkillNumber = tempSkillUICellMainSubStructs.FindIndex(x => x.CellNumber == tempSkillUICellStructs[i].CellNumber);

                // skillUICellStructs.CellContent의 값을 이용하여, SkillUICell Prefab의 역할을 구분. 초기설정 메소드 구분하여 호출.
                switch (tempSkillUICellStructs[i].CellContent)
                {
                    case CellContent.non:
                        SkillUICellView.InitialNonCell(ref this.skillUIModel, this);
                        break;
                    case CellContent.line:
                    case CellContent.interchange:
                        SkillUICellView.InitizlLineCell(ref this.skillUIModel, this, tempSkillUICellStructs[i]);
                        break;
                    case CellContent.sub:
                    case CellContent.main:
                        SkillUICellView.InitializeMainAndSubCell(ref this.skillUIModel, this, tempSkillUICellStructs[i], tempSkillUICellMainSubStructs[tempSkillNumber], tempSkillInformationStructs[tempSkillNumber]);
                        break;
                }

                this.necromancySkillCellUIViews.Add(SkillUICellView);
            }
        }
        private void CreateSkillDescriptionUI(SkillDescriptionType skillDescriptionType, SkillUICellMainSubStruct SkillUICellMainSubStruct)
        {
            if (skillDescriptionType == SkillDescriptionType.Temporary)
            {
                this.temporarySkillDescriptionUIView = Instantiate(Resources.Load<RectTransform>("Prefab/UI/SkillUI/TemporarySkillDescriptionUI"), GameObject.FindWithTag("UIManager").GetComponent<RectTransform>()).GetComponent<SkillDescriptionUIView>(); ;
                this.temporarySkillDescriptionUIView.InitialSetting(ref this.skillUIModel, this, skillDescriptionType, SkillUICellMainSubStruct,
                ref this.skillUIModel.SkillInformationStructs, this.skillUIModel.GetMainSubPreconditionStruct(SkillUICellMainSubStruct.SkillNumber));
            }
            else
            {
                this.fixedSkillDescriptionUIView = Instantiate(Resources.Load<RectTransform>("Prefab/UI/SkillUI/FixedSkillDescriptionUI"), GameObject.FindWithTag("UIManager").GetComponent<RectTransform>()).GetComponent<SkillDescriptionUIView>(); ;
                this.fixedSkillDescriptionUIView.InitialSetting(ref this.skillUIModel, this, skillDescriptionType, SkillUICellMainSubStruct,
                ref this.skillUIModel.SkillInformationStructs, this.skillUIModel.GetMainSubPreconditionStruct(SkillUICellMainSubStruct.SkillNumber));
            }
        }


        // InteractionSkillImageUIAndMouseInSkillMenuUI 구현
        public void MouseClickInteraction(SkillUICellMainSubStruct SkillUICellMainSubStruct)
        {
            this.skillUIModel.DecideActivateOrInActivateCell(SkillUICellMainSubStruct);

            if (!this.fixedSkillDescriptionUIIsCreated)
            {
                this.fixedSkillDescriptionUIIsCreated = true;
                this.CreateSkillDescriptionUI(SkillDescriptionType.Fixed, SkillUICellMainSubStruct);
            }
            else if (this.fixedSkillDescriptionUIIsCreated && this.fixedSkillDescriptionUIView.SkillNumber == SkillUICellMainSubStruct.SkillNumber)
            {
                this.fixedSkillDescriptionUIIsCreated = false;
                this.fixedSkillDescriptionUIView.Destroy();
            }
            else if (this.fixedSkillDescriptionUIIsCreated && this.fixedSkillDescriptionUIView.SkillNumber != SkillUICellMainSubStruct.SkillNumber)
            {
                this.fixedSkillDescriptionUIView.Destroy();
                this.CreateSkillDescriptionUI(SkillDescriptionType.Fixed, SkillUICellMainSubStruct);
            }
        }
        public void MouseEnterInteraction(SkillUICellMainSubStruct SkillUICellMainSubStruct)
        {
            this.CreateSkillDescriptionUI(SkillDescriptionType.Temporary, SkillUICellMainSubStruct);
        }
        public void MouseExitInteration(SkillUICellMainSubStruct SkillUICellMainSubStruct)
        {
            this.temporarySkillDescriptionUIView.Destroy();
            this.temporarySkillDescriptionUIView = null;
        }

        // ISkillUIController_For_SkillMenuUIView 구현
        public void ChangeSkillMenuType(SkillTreeType SkillTreeType)
        {
            this.skillMenuUIView.ActivateSkillMenu(SkillTreeType);
        }

        // ISkillUIController_For_ISkillDescriptionUIView 구현
        public void LearnSkill(int skillNumber)
        {
            this.skillUIModel.LearnSkill(skillNumber);
        }
    }
}