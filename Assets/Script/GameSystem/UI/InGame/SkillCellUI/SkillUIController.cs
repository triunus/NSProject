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

    // InteractionSkillUIAndMouseInSkillUIWindow ��
    public interface ISkillUIController_For_InteractionSkillImageUIAndMouseInSkillMenuUI
    {
        // view ��
        public void MouseClickInteraction(SkillUICellMainSubStruct SkillUICellMainSubStruct);        // skill ������ Ŭ�� �̺�Ʈ
        public void MouseEnterInteraction(SkillUICellMainSubStruct SkillUICellMainSubStruct);
        public void MouseExitInteration(SkillUICellMainSubStruct SkillUICellMainSubStruct);
    }

    // SkillMenuUIView ��
    public interface ISkillUIController_For_SkillMenuUIView
    {
        public void CreateSkillUICell(RectTransform skillContentRectTransform, SkillTreeType SkillTreeType);
        public void ChangeSkillMenuType(SkillTreeType SkillTreeType);   // SkillTreeType ����
    }

    // ISkillDescriptionUIView ��
    public interface ISkillUIController_For_ISkillDescriptionUIView
    {
        public void LearnSkill(int skillNumber);
    }

    // SkillManager ��
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

        // ISkillUIController ����
        public void InitialSetting(ref ISkillUIModel skillUIModel)
        {
            this.skillUIModel = skillUIModel;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))    // 'K' key �Է� ��,
            {
                if (this.isPause)   // UI ��Ȱ��ȭ �� �ð� ���
                {
                    this.isPause = false;
                    this.ActivateOrUnActivateSkillMenuUI(false);
                    GameManager.Instance.RequestReGame();
                }
                else                // UI Ȱ��ȭ �� �ð� ����
                {
                    this.isPause = true;
                    this.ActivateOrUnActivateSkillMenuUI(true);
                    GameManager.Instance.RequestPauseGame();
                }
            }
        }

        // SkillMenuUI �ʱ� ����.
        private void ActivateOrUnActivateSkillMenuUI(bool isActivated)
        {
            // Skill Menu UI Prefab�� ������ ���°� �ƴ϶��, Prefab ���� �޼ҵ� ȣ��.
            if (!skillMenuUIIsCreated) this.CreateSkillMenuUI();

            // Skill Menu UI Prefab�� ��Ȱ��ȭ �� ��, ���� ��ų ���� Prefab ������Ű��.
            if (this.fixedSkillDescriptionUIIsCreated) 
            { 
                this.fixedSkillDescriptionUIView.Destroy();
                this.fixedSkillDescriptionUIIsCreated = false;
            }

            this.skillMenuUIView.ActivateOrUnActivateSkillMenuUI(isActivated);
            if (isActivated) this.skillMenuUIView.ActivateSkillMenu(SkillTreeType.Necromancy);
        }
        // Skill Menu UI Prefab�� ���� �޼ҵ�.
        private void CreateSkillMenuUI()
        {
            this.skillMenuUIIsCreated = true;

            // Skill Menu UI Prefab ����.
            // �� �ش� Prefab�� �����ϰ� �ִ� SkillMenuUIView ��ü ����.
            this.skillMenuUIView = Instantiate(Resources.Load<RectTransform>("Prefab/UI/SkillUI/SkillMenuUI"),
                GameObject.FindWithTag("UIManager").GetComponent<RectTransform>()).GetComponent<SkillMenuUIView>();
            // skillMenuUIView �ʱ⼳�� �޼ҵ� ȣ��.
            this.skillMenuUIView.InitialSetting(this);
        }
        public void CreateSkillUICell(RectTransform skillContentRectTransform, SkillTreeType SkillTreeType)
        {
            System.Collections.Generic.List<SkillUICellStruct> tempSkillUICellStructs = null;

            if (SkillTreeType == SkillTreeType.Necromancy) tempSkillUICellStructs = this.skillUIModel.SkillUICellStructs;
//            else tempSkillUICellStructs = this.skillUIModel.ClassSkillUICellLineStructs;      // ���� ���� ����.

            System.Collections.Generic.List<SkillUICellMainSubStruct> tempSkillUICellMainSubStructs = this.skillUIModel.SkillUICellMainSubStructs;
            System.Collections.Generic.List<SkillInformationStruct> tempSkillInformationStructs = this.skillUIModel.SkillInformationStructs;

            int tempSkillNumber;

            // skillUICellStructs ������ŭ SkillUICell Prefab ����.
            for (int i = 0; i < tempSkillUICellStructs.Count; ++i)
            {
                ISkillUICellView SkillUICellView = Instantiate(Resources.Load<RectTransform>("Prefab/UI/SkillUI/SkillUICell"), skillContentRectTransform).GetComponent<ISkillUICellView>();

                tempSkillNumber = tempSkillUICellMainSubStructs.FindIndex(x => x.CellNumber == tempSkillUICellStructs[i].CellNumber);

                // skillUICellStructs.CellContent�� ���� �̿��Ͽ�, SkillUICell Prefab�� ������ ����. �ʱ⼳�� �޼ҵ� �����Ͽ� ȣ��.
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


        // InteractionSkillImageUIAndMouseInSkillMenuUI ����
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

        // ISkillUIController_For_SkillMenuUIView ����
        public void ChangeSkillMenuType(SkillTreeType SkillTreeType)
        {
            this.skillMenuUIView.ActivateSkillMenu(SkillTreeType);
        }

        // ISkillUIController_For_ISkillDescriptionUIView ����
        public void LearnSkill(int skillNumber)
        {
            this.skillUIModel.LearnSkill(skillNumber);
        }
    }
}