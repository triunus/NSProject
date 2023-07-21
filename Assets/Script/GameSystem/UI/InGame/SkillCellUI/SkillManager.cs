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
        public ref SkillTreeStruct SkillTreeStruct { get; }
        public ref List<SkillAndCellNumberStruct> SkillAndCellNumberStructs { get; }
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

        private SkillTreeStruct skillTreeStruct;

        private List<SkillAndCellNumberStruct> skillAndCellNumberStructs;

        private List<SkillInformationStruct> skillInformationStructs;
        private List<PlayerSkillInformationStruct> playerSkillInformationStructs;

        // ISkillManagerForModel 구현
        public ref SkillTreeStruct SkillTreeStruct { get { return ref this.skillTreeStruct; } }
        public ref List<SkillAndCellNumberStruct> SkillAndCellNumberStructs { get { return ref this.skillAndCellNumberStructs; } }
        public ref List<SkillInformationStruct> SkillInformationStruct { get { return ref this.skillInformationStructs; } }
        public ref List<PlayerSkillInformationStruct> PlayerSkillInformationStruct { get { return ref this.playerSkillInformationStructs; } }

        public int OwnManaStone { get { return this.playerManager.OwnManaStone; } set { this.playerManager.OwnManaStone = value; } }

        private void Awake()
        {
            this.skillTreeStruct = new SkillTreeStruct(0, 0);
            this.skillAndCellNumberStructs = new List<SkillAndCellNumberStruct>();
            this.skillInformationStructs = new List<SkillInformationStruct>();
            this.playerSkillInformationStructs = new List<PlayerSkillInformationStruct>();

            this.RecordSkillUICellInformation();
            this.RecodeSkillAndCellNumberInformation();
            this.RecordSkillInformation();

            this.playerManager = GameObject.FindWithTag("PlayerManager").GetComponent<Player.PlayerManager>();

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
            if (skillDataStruct.GetSkillDataCount() == 0) this.playerSkillInformationStructs = skillInformationStructs.ConvertAll(tmp => new PlayerSkillInformationStruct(tmp.SkillNumber, 0));
            else this.playerSkillInformationStructs = skillDataStruct.PlayerSkillData.ConvertAll(tmp => new Skill.PlayerSkillInformationStruct(tmp.SkillNumber, tmp.CurrentLevel));

            this.skillUIModel.InitialSetting(this);
        }
        public SaveAndLoad.SkillDataStruct GatherData()
        {
            SaveAndLoad.SkillDataStruct skillDataStruct = new SaveAndLoad.SkillDataStruct();
            skillDataStruct.PlayerSkillData = this.playerSkillInformationStructs;

            return skillDataStruct;
        }

        // SkillManager 내부 로직.
        private void RecordSkillUICellInformation()
        {
            TextAsset skillTree_TextAsset = Resources.Load<TextAsset>("GameSystem/SkillData/UI/SkillUICellInformation");
            JObject skillTree = JObject.Parse(skillTree_TextAsset.ToString());

            this.skillTreeStruct.RowCount = (int)skillTree["RowCount"];
            this.skillTreeStruct.ColumnCount = (int)skillTree["ColumnCount"];
            JArray skillUICellInformation = (JArray)skillTree["CellInformation"];

            for (int i = 0; i < skillUICellInformation.Count; ++i)
            {
                SkillUICellStruct SkillUICellStruct = new SkillUICellStruct(
                    cellNumber: (int)skillUICellInformation[i]["CellNumber"],
                    cellContent: (CellContent)System.Enum.Parse(typeof(CellContent), skillUICellInformation[i]["CellContent"].ToString()),
                    lineNumber: (int)skillUICellInformation[i]["LineNumber"]
                    );

                this.skillTreeStruct.SkillUICellInformation.Add(SkillUICellStruct);
            }
        }
        private void RecodeSkillAndCellNumberInformation()
        {
            TextAsset skillAndCellNumber_TextAsset = Resources.Load<TextAsset>("GameSystem/SkillData/UI/SkillAndCellNumber");
            JArray skillAndCellNumbe = JArray.Parse(skillAndCellNumber_TextAsset.ToString());

            for (int i = 0; i < skillAndCellNumbe.Count; ++i)   
            {
                SkillAndCellNumberStruct skillAndCellNumberStruct = new SkillAndCellNumberStruct(
                    skillNumber: (int)skillAndCellNumbe[i]["SkillNumber"],
                    cellNumber: (int)skillAndCellNumbe[i]["CellNumber"]
                    );

                this.skillAndCellNumberStructs.Add(skillAndCellNumberStruct);
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