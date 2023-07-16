using System.Collections.Generic;
using Newtonsoft.Json.Linq;

using UnityEngine;

namespace GameSystem.InGameUI.Skill
{
    public interface ISkillManagerForGameManager
    {
        public void Clearing();
        public void AllocateData(SaveAndLoad.SkillDataStruct skillDataStruct);
        public SaveAndLoad.SkillDataStruct GatherData();
    }

    public interface ISkillManagerForModel
    {
        public ref List<SkillUICellStruct> SkillUICellStructs { get; }
        public ref List<SkillUICellMainSubStruct> SkillUICellMainSubStructs { get; }
        public ref List<SkillInformationStruct> SkillInformationStruct { get; }
        public ref List<PlayerSkillInformationStruct> PlayerSkillInformationStruct { get; }

        public int OwnManaStone { get; set; }
    }

    public interface ISkillManager
    {

    }

    public class SkillManager : MonoBehaviour, ISkillManager, ISkillManagerForModel, ISkillManagerForGameManager
    {
        private Player.IPlayerManager playerManager;

        private ISkillUIModel skillUIModel;
        private ISkillUIController skillCellUIController;

        private List<SkillUICellStruct> skillUICellStructs;
        private List<SkillUICellMainSubStruct> skillUICellMainSubStructs;

        private List<SkillInformationStruct> skillInformationStructs;
        private List<PlayerSkillInformationStruct> playerSkillInformationStructs;

        // ISkillManagerForModel 구현
        public ref List<SkillUICellStruct> SkillUICellStructs { get { return ref this.skillUICellStructs; } }
        public ref List<SkillUICellMainSubStruct> SkillUICellMainSubStructs { get { return ref this.skillUICellMainSubStructs; } }
        public ref List<SkillInformationStruct> SkillInformationStruct { get { return ref this.skillInformationStructs; } }
        public ref List<PlayerSkillInformationStruct> PlayerSkillInformationStruct { get { return ref this.playerSkillInformationStructs; } }

        public int OwnManaStone { get { return this.playerManager.OwnManaStone; } set { this.playerManager.OwnManaStone = value; } }

        private void Awake()
        {
            this.skillUICellStructs = new List<SkillUICellStruct>();
            this.skillUICellMainSubStructs = new List<SkillUICellMainSubStruct>();
            this.skillInformationStructs = new List<SkillInformationStruct>();
            this.playerSkillInformationStructs = new List<PlayerSkillInformationStruct>();

            // Skill 로직 구현에 필요한 데이터를 Local에서 읽어와 필드 멤버에 저장하는 메소드들 수행.
            this.RecordSkillUICellInformation();
            this.RecodeSkillUICellMainSubInformation();
            this.RecordSkillInformation();

            // 타 Manager를 참조하는 부분.
            this.playerManager = GameObject.FindWithTag("PlayerManager").GetComponent<Player.PlayerManager>();

            // Skill MVC의 Model과 Controller 참조 및, Controller 초기 설정 메소드 호출.
            this.skillUIModel = GetComponent<SkillUIModel>();
            this.skillCellUIController = GetComponent<SkillUIController>();
            this.skillCellUIController.InitialSetting(ref skillUIModel);
        }

        // ISkillManagerForGameManager 구현.
        public void Clearing()
        {
            Debug.Log("SkillManager - Clearing");
        }
        public void AllocateData(SaveAndLoad.SkillDataStruct skillDataStruct)
        {
            // 게임을 최초로 실행하여, playerSkillInformation 정보가 없을 때.
            if(skillDataStruct.GetSkillDataCount() == 0) this.playerSkillInformationStructs = skillInformationStructs.ConvertAll(tmp => new PlayerSkillInformationStruct(tmp.SkillNumber, 0));
            else this.playerSkillInformationStructs = skillDataStruct.SkillData;

            this.skillUIModel.InitialSetting(this);
        }
        public SaveAndLoad.SkillDataStruct GatherData()
        {
            SaveAndLoad.SkillDataStruct skillDataStruct = new SaveAndLoad.SkillDataStruct();
            skillDataStruct.SkillData = this.playerSkillInformationStructs;

            return skillDataStruct;
        }

        // SkillManager 내부 로직.
        private void RecordSkillUICellInformation()
        {
            // Resources 파일에서 데이터 파일 읽어오기.
            TextAsset skillUICell_TextAsset = Resources.Load<TextAsset>("GameSystem/SkillData/UI/SkillUICell");
            JArray skillUICell = JArray.Parse(skillUICell_TextAsset.ToString());

            // 읽어온 Json 파일 값을 필드 멤버에 기록.
            for (int i = 0; i < skillUICell.Count; ++i)
            {
                SkillUICellStruct SkillUICellStruct = new SkillUICellStruct(
                    cellNumber: (int)skillUICell[i]["CellNumber"],
                    cellContent: (CellContent)System.Enum.Parse(typeof(CellContent), skillUICell[i]["CellContent"].ToString()),
                    lineNumber: (int)skillUICell[i]["LineNumber"]
                    );
                this.skillUICellStructs.Add(SkillUICellStruct);
            }
        }
        private void RecodeSkillUICellMainSubInformation()
        {
            TextAsset skillUICellMainSub_TextAsset = Resources.Load<TextAsset>("GameSystem/SkillData/UI/SkillUICellMainSub");
            JArray skillUICellMainSub = JArray.Parse(skillUICellMainSub_TextAsset.ToString());

            for (int i = 0; i < skillUICellMainSub.Count; ++i)   
            {
                SkillUICellMainSubStruct SkillUICellStruct = new SkillUICellMainSubStruct(
                    skillNumber: (int)skillUICellMainSub[i]["SkillNumber"],
                    cellNumber: (int)skillUICellMainSub[i]["CellNumber"]
                    );

                this.skillUICellMainSubStructs.Add(SkillUICellStruct);
            }
        }
        private void RecordSkillInformation()
        {
            TextAsset skillInformation_TextAsset = Resources.Load<TextAsset>("GameSystem/SkillData/SkillInformation");
            JArray skillInformation = JArray.Parse(skillInformation_TextAsset.ToString());

            for (int i = 0; i < skillInformation.Count; ++i)
            {
                SkillInformationStruct SkillUICellStruct = new SkillInformationStruct(
                    skillNumber: (int)skillInformation[i]["SkillNumber"],
                    skillName: (string)skillInformation[i]["SkillName"],
                    skillDescription: (string)skillInformation[i]["SkillDescription"],
                    maxLevel: (int)skillInformation[i]["MaxLevel"],
                    cost : (int)skillInformation[i]["Cost"]
                    );

                this.skillInformationStructs.Add(SkillUICellStruct);
            }
        }
    }
}