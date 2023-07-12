using UnityEngine;

namespace GameSystem.InGameUI.Skill
{
    public interface ISkillDescriptionUIView
    {
        public void InitialSetting(ref ISkillUIModel skillUIModel, ISkillUIController skillUIController, SkillDescriptionType skillDescriptionType, SkillUICellMainSubStruct SkillUICellMainSubStruct,
            ref System.Collections.Generic.List<SkillInformationStruct> skillInformationStructs, System.Collections.Generic.LinkedList<SkillNumberPreconditionStruct> linkedList);

        public int SkillNumber { get; }
        public void Destroy();
    }

    public class SkillDescriptionUIView : MonoBehaviour, ISkillDescriptionUIView, IPlayerSkillInformationObserverForView
    {
        private IPlayerSkillInformationObserver playerSkillInformationObserver;

        private ISkillUIController skillUIController;

        private RectTransform myRectTransform;

        private System.Collections.Generic.List<RectTransform> skillConditions;

        private SkillUICellMainSubStruct SkillUICellMainSubStruct;
        private System.Collections.Generic.List<SkillInformationStruct> skillInformationStructs;
        private PlayerSkillInformationStruct playerSkillInformation;
        private System.Collections.Generic.LinkedList<SkillNumberPreconditionStruct> skillUICellMSPreconditionStructs;

        private SkillDescriptionType skillDescriptionType;
        private string isAvailable;

        public int SkillNumber { get { return this.SkillUICellMainSubStruct.SkillNumber; } }

        private void Awake()
        {
            this.skillConditions = new System.Collections.Generic.List<RectTransform>();

            this.skillUICellMSPreconditionStructs = new System.Collections.Generic.LinkedList<SkillNumberPreconditionStruct>();

            this.myRectTransform = this.GetComponent<RectTransform>();
 
            isAvailable = "Not Available";
        }

        public void InitialSetting(ref ISkillUIModel skillUIModel, ISkillUIController skillUIController, SkillDescriptionType skillDescriptionType, SkillUICellMainSubStruct SkillUICellMainSubStruct,
            ref System.Collections.Generic.List<SkillInformationStruct> skillInformationStructs, System.Collections.Generic.LinkedList<SkillNumberPreconditionStruct> linkedList)
        {
            this.playerSkillInformationObserver = skillUIModel;
            this.skillUIController = skillUIController;

            skillUIModel.RegisterPlayerSkillInformationObserver(this);

            this.skillDescriptionType = skillDescriptionType;
            this.SkillUICellMainSubStruct = SkillUICellMainSubStruct;
            this.skillInformationStructs = skillInformationStructs;
            this.skillUICellMSPreconditionStructs = linkedList;
            this.playerSkillInformation = this.playerSkillInformationObserver.GetPlayerSkillInformation(SkillUICellMainSubStruct.SkillNumber);

            this.CreateSkillConditionPanel();

            this.Display();
        }
        private void ConnectButton()
        {
            this.myRectTransform.GetChild(0).GetChild(3).GetChild(2).GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { this.skillUIController.LearnSkill(this.SkillUICellMainSubStruct.SkillNumber); });
        }

        public void UpdatePlayerSkillInformationObserver()
        {
            this.playerSkillInformation = this.playerSkillInformationObserver.GetPlayerSkillInformation(this.SkillUICellMainSubStruct.SkillNumber);

            this.Display();
        }

        private void Display()
        {
            // 스킬 이름
            this.myRectTransform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = this.skillInformationStructs[this.SkillUICellMainSubStruct.SkillNumber].SkillName;

            var linkedListPointer = this.skillUICellMSPreconditionStructs.First;

            for (int i = 0; i < this.skillUICellMSPreconditionStructs.Count; ++i)
            {
                // 선수 스킬 이름
                this.skillConditions[i].GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = this.skillInformationStructs[linkedListPointer.Value.NextVertex].SkillName;

                // currentLevel / MaxLevel
                this.skillConditions[i].GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text
                    = System.Convert.ToString(this.playerSkillInformationObserver.GetPlayerSkillInformation(linkedListPointer.Value.NextVertex).CurrentLevel) + " / " + System.Convert.ToString(this.skillInformationStructs[linkedListPointer.Value.NextVertex].MaxLevel);

                // 사용가능 여부 판단
                if (this.playerSkillInformationObserver.GetPlayerSkillInformation(linkedListPointer.Value.NextVertex).CurrentLevel >= this.skillInformationStructs[linkedListPointer.Value.NextVertex].MaxLevel) this.isAvailable = "Available";

                linkedListPointer = linkedListPointer.Next;
            }

            // 사용가능 여부
            this.myRectTransform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = this.isAvailable;
            // 스킬 설명
            this.myRectTransform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = this.skillInformationStructs[this.SkillUICellMainSubStruct.SkillNumber].SkillDescription;
            // 소비 마나
            this.myRectTransform.GetChild(0).GetChild(3).GetChild(1).GetChild(0).GetChild(3).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text
                = System.Convert.ToString(this.skillInformationStructs[this.SkillUICellMainSubStruct.SkillNumber].Cost + this.skillInformationStructs[this.SkillUICellMainSubStruct.SkillNumber].Cost * this.playerSkillInformation.CurrentLevel);
        }
        private void CreateSkillConditionPanel()
        {
            if (this.skillConditions.Count != 0) this.DestroySkillConditionPanel();

            foreach (var item in this.skillUICellMSPreconditionStructs)
            {
                RectTransform tempSkillCondition = Instantiate(Resources.Load<RectTransform>("Prefab/UI/SkillUI/SkillCondition"), this.myRectTransform.GetChild(0).GetChild(1)).GetComponent<RectTransform>();

                this.skillConditions.Add(tempSkillCondition);
            }

            // Prefab 크기 조절.
            this.myRectTransform.GetChild(0).GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2 (256, this.skillConditions.Count * 30);

            this.myRectTransform.sizeDelta
                = new Vector2(256, this.myRectTransform.GetComponent<RectTransform>().sizeDelta.y + this.myRectTransform.GetChild(0).GetChild(1).GetComponent<RectTransform>().sizeDelta.y);

            // Prefab 위치 조절.
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