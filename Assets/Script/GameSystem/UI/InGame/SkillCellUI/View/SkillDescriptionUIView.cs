using UnityEngine;

namespace GameSystem.InGameUI.Skill
{
    public interface ISkillDescriptionUIView
    {
        public void InitialSetting(ref ISkillUIModel skillUIModel, ISkillUIController skillUIController, SkillDescriptionType skillDescriptionType, SkillUICellMSStruct skillUICellMSStruct,
            ref System.Collections.Generic.List<SkillInformationStruct> skillInformationStructs, System.Collections.Generic.LinkedList<SkillUICellMSPreconditionStruct> linkedList);

        public int SkillNumber { get; }
        public void Destroy();
    }

    public class SkillDescriptionUIView : MonoBehaviour, ISkillDescriptionUIView, IPlayerSkillInformationObserverForView
    {
        private IPlayerSkillInformationObserver playerSkillInformationObserver;

        private ISkillUIController skillUIController;

        private RectTransform myRectTransform;

        private System.Collections.Generic.List<RectTransform> skillConditions;

        private SkillUICellMSStruct skillUICellMSStruct;
        private System.Collections.Generic.List<SkillInformationStruct> skillInformationStructs;
        private PlayerSkillInformationStruct playerSkillInformation;
        private System.Collections.Generic.LinkedList<SkillUICellMSPreconditionStruct> skillUICellMSPreconditionStructs;

        private SkillDescriptionType skillDescriptionType;
        private string isAvailable;

        public int SkillNumber { get { return this.skillUICellMSStruct.SkillNumber; } }

        private void Awake()
        {
            this.skillConditions = new System.Collections.Generic.List<RectTransform>();

            this.skillUICellMSPreconditionStructs = new System.Collections.Generic.LinkedList<SkillUICellMSPreconditionStruct>();

            this.myRectTransform = this.GetComponent<RectTransform>();
 
            isAvailable = "Not Available";
        }

        public void InitialSetting(ref ISkillUIModel skillUIModel, ISkillUIController skillUIController, SkillDescriptionType skillDescriptionType, SkillUICellMSStruct skillUICellMSStruct,
            ref System.Collections.Generic.List<SkillInformationStruct> skillInformationStructs, System.Collections.Generic.LinkedList<SkillUICellMSPreconditionStruct> linkedList)
        {
            this.playerSkillInformationObserver = skillUIModel;
            this.skillUIController = skillUIController;

            skillUIModel.RegisterPlayerSkillInformationObserver(this);

            this.skillDescriptionType = skillDescriptionType;
            this.skillUICellMSStruct = skillUICellMSStruct;
            this.skillInformationStructs = skillInformationStructs;
            this.skillUICellMSPreconditionStructs = linkedList;
            this.playerSkillInformation = this.playerSkillInformationObserver.GetPlayerSkillInformation(skillUICellMSStruct.SkillNumber);

            this.CreateSkillConditionPanel();

            this.Display();
        }
        private void ConnectButton()
        {
            this.myRectTransform.GetChild(0).GetChild(3).GetChild(2).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { this.skillUIController.LearnSkill(this.skillUICellMSStruct.SkillNumber); });
        }

        public void UpdatePlayerSkillInformationObserver()
        {
            this.playerSkillInformation = this.playerSkillInformationObserver.GetPlayerSkillInformation(this.skillUICellMSStruct.SkillNumber);

            this.Display();
        }

        private void Display()
        {
            // ��ų �̸�
            this.myRectTransform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = this.skillInformationStructs[this.skillUICellMSStruct.SkillNumber].SkillName;

            var linkedListPointer = this.skillUICellMSPreconditionStructs.First;

            for (int i = 0; i < this.skillUICellMSPreconditionStructs.Count; ++i)
            {
                // ���� ��ų �̸�
                this.skillConditions[i].GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = this.skillInformationStructs[linkedListPointer.Value.Precondition_q].SkillName;

                // currentLevel / MaxLevel
                this.skillConditions[i].GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text
                    = System.Convert.ToString(this.playerSkillInformationObserver.GetPlayerSkillInformation(linkedListPointer.Value.Precondition_q).CurrentLevel) + " / " + System.Convert.ToString(this.skillInformationStructs[linkedListPointer.Value.Precondition_q].MaxLevel);

                // ��밡�� ���� �Ǵ�
                if (this.playerSkillInformationObserver.GetPlayerSkillInformation(linkedListPointer.Value.Precondition_q).CurrentLevel >= this.skillInformationStructs[linkedListPointer.Value.Precondition_q].MaxLevel) this.isAvailable = "Available";

                linkedListPointer = linkedListPointer.Next;
            }

            // ��밡�� ����
            this.myRectTransform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = this.isAvailable;
            // ��ų ����
            this.myRectTransform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = this.skillInformationStructs[this.skillUICellMSStruct.SkillNumber].SkillDescription;
            // �Һ� ����
            this.myRectTransform.GetChild(0).GetChild(3).GetChild(1).GetChild(0).GetChild(3).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text
                = System.Convert.ToString(this.skillInformationStructs[this.skillUICellMSStruct.SkillNumber].Cost + this.skillInformationStructs[this.skillUICellMSStruct.SkillNumber].Cost * this.playerSkillInformation.CurrentLevel);
        }
        private void CreateSkillConditionPanel()
        {
            if (this.skillConditions.Count != 0) this.DestroySkillConditionPanel();

            foreach (var item in this.skillUICellMSPreconditionStructs)
            {
                RectTransform tempSkillCondition = Instantiate(Resources.Load<RectTransform>("Prefab/UI/SkillUI/SkillCondition"), this.myRectTransform.GetChild(0).GetChild(1)).GetComponent<RectTransform>();

                this.skillConditions.Add(tempSkillCondition);
            }

            // Prefab ũ�� ����.
            this.myRectTransform.GetChild(0).GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2 (256, this.skillConditions.Count * 30);

            this.myRectTransform.sizeDelta
                = new Vector2(256, this.myRectTransform.GetComponent<RectTransform>().sizeDelta.y + this.myRectTransform.GetChild(0).GetChild(1).GetComponent<RectTransform>().sizeDelta.y);

            // Prefab ��ġ ����.
            if (this.skillDescriptionType == SkillDescriptionType.Temporary)
                this.myRectTransform.anchoredPosition = new Vector2(600, 130 - this.myRectTransform.GetChild(0).GetChild(1).GetComponent<RectTransform>().sizeDelta.y/2);
            else
            {
                this.ConnectButton();
                this.myRectTransform.anchoredPosition = new Vector2(600, -200 + this.myRectTransform.GetChild(0).GetChild(1).GetComponent<RectTransform>().sizeDelta.y/2);
            }
        }
        private void DestroySkillConditionPanel()
        {
            foreach(var item in skillConditions)
                Destroy(item.gameObject);

            this.skillConditions.Clear();
        }
        public void Destroy()
        {
            this.playerSkillInformationObserver.RemovePlayerSkillInformationObserver(this);
            Destroy(this.gameObject);
        }
    }

}