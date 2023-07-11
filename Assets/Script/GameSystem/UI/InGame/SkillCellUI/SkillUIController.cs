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

    // InteractionSkillUIAndMouseInSkillUIWindow ��
    public interface ISkillUIController_For_InteractionSkillImageUIAndMouseInSkillMenuUI
    {
        // view ��
        public void MouseClickInteraction(SkillUICellMSStruct skillUICellMSStruct);        // skill ������ Ŭ�� �̺�Ʈ
        public void MouseEnterInteraction(SkillUICellMSStruct skillUICellMSStruct);
        public void MouseExitInteration(SkillUICellMSStruct skillUICellMSStruct);
    }

    // SkillMenuUIView ��
    public interface ISkillUIController_For_SkillMenuUIView
    {
        public void CreateSkillCellUI(RectTransform skillContentRectTransform, SkillMenuType skillMenuType);
        public void ChangeSkillMenuType(SkillMenuType skillMenuType);   // skillMenuType ����
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
            if (isActivated) this.skillMenuUIView.ActivateSkillMenu(SkillMenuType.Necromancy);
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
        public void CreateSkillCellUI(RectTransform skillContentRectTransform, SkillMenuType skillMenuType)
        {
            System.Collections.Generic.List<SkillUICellStruct> tempSkillUICellLineStructs = null;

            if (skillMenuType == SkillMenuType.Necromancy) tempSkillUICellLineStructs = this.skillUIModel.SkillUICellStructs;
//            else tempSkillUICellLineStructs = this.skillUIModel.ClassSkillUICellLineStructs;      // ���� ���� ����.

            System.Collections.Generic.List<SkillUICellMSStruct> tempSkillUICellMSStructs = this.skillUIModel.SkillUICellMSStructs;
            System.Collections.Generic.List<SkillInformationStruct> tempSkillInformationStructs = this.skillUIModel.SkillInformationStructs;

            int tempSkillNumber;

            // skillUICellStructs ������ŭ SkillUICell Prefab ����.
            for (int i = 0; i < tempSkillUICellLineStructs.Count; ++i)
            {
                ISkillCellUIView skillCellUIView = Instantiate(Resources.Load<RectTransform>("Prefab/UI/SkillUI/SkillUICell"), skillContentRectTransform).GetComponent<ISkillCellUIView>();

                tempSkillNumber = tempSkillUICellMSStructs.FindIndex(x => x.CellNumber == tempSkillUICellLineStructs[i].CellNumber);

                // skillUICellStructs.CellContent�� ���� �̿��Ͽ�, SkillUICell Prefab�� ������ ����. �ʱ⼳�� �޼ҵ� �����Ͽ� ȣ��.
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


        // InteractionSkillImageUIAndMouseInSkillMenuUI ����
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

        // ISkillUIController_For_SkillMenuUIView ����
        public void ChangeSkillMenuType(SkillMenuType skillMenuType)
        {
            this.skillMenuUIView.ActivateSkillMenu(skillMenuType);
        }

        // ISkillUIController_For_ISkillDescriptionUIView ����
        public void LearnSkill(int skillNumber)
        {
            this.skillUIModel.LearnSkill(skillNumber);
        }
    }
}