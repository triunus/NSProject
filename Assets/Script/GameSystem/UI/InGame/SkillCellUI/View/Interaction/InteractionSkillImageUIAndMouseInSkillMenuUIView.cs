using UnityEngine;
using UnityEngine.EventSystems;

namespace GameSystem.InGameUI.Skill
{
    public interface IInteractionSkillImageUIAndMouseInSkillMenuUIView
    {
        public void InitialSetting(ref ISkillUIController skillCellUIController, ref SkillAndCellNumberStruct SkillAndCellNumberStruct);
    }

    public class InteractionSkillImageUIAndMouseInSkillMenuUIView : MonoBehaviour, IInteractionSkillImageUIAndMouseInSkillMenuUIView, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private ISkillUIController skillUIController;

        private SkillAndCellNumberStruct SkillAndCellNumberStruct;

        public void InitialSetting(ref ISkillUIController skillCellUIController, ref SkillAndCellNumberStruct SkillAndCellNumberStruct)
        {
            this.skillUIController = skillCellUIController;
            this.SkillAndCellNumberStruct = SkillAndCellNumberStruct;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            this.skillUIController.MouseClickInteraction(this.SkillAndCellNumberStruct);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            this.skillUIController.MouseEnterInteraction(this.SkillAndCellNumberStruct);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.skillUIController.MouseExitInteration(this.SkillAndCellNumberStruct);
        }
    }
}