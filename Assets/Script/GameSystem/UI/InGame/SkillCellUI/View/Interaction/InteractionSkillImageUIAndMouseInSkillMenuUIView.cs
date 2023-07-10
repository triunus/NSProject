using UnityEngine;
using UnityEngine.EventSystems;

namespace GameSystem.InGameUI.Skill
{
    public interface IInteractionSkillImageUIAndMouseInSkillMenuUIView
    {
        public void InitialSetting(ref ISkillUIController skillCellUIController, ref SkillUICellMSStruct skillUICellMSStruct);
    }

    public class InteractionSkillImageUIAndMouseInSkillMenuUIView : MonoBehaviour, IInteractionSkillImageUIAndMouseInSkillMenuUIView, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private ISkillUIController skillUIController;

        private SkillUICellMSStruct skillUICellMSStruct;

        public void InitialSetting(ref ISkillUIController skillCellUIController, ref SkillUICellMSStruct skillUICellMSStruct)
        {
            this.skillUIController = skillCellUIController;
            this.skillUICellMSStruct = skillUICellMSStruct;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            this.skillUIController.MouseClickInteraction(this.skillUICellMSStruct);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            this.skillUIController.MouseEnterInteraction(this.skillUICellMSStruct);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.skillUIController.MouseExitInteration(this.skillUICellMSStruct);
        }
    }
}