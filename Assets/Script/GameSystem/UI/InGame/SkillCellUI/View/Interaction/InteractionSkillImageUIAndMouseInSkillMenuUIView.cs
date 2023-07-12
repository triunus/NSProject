using UnityEngine;
using UnityEngine.EventSystems;

namespace GameSystem.InGameUI.Skill
{
    public interface IInteractionSkillImageUIAndMouseInSkillMenuUIView
    {
        public void InitialSetting(ref ISkillUIController skillCellUIController, ref SkillUICellMainSubStruct SkillUICellMainSubStruct);
    }

    public class InteractionSkillImageUIAndMouseInSkillMenuUIView : MonoBehaviour, IInteractionSkillImageUIAndMouseInSkillMenuUIView, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private ISkillUIController skillUIController;

        private SkillUICellMainSubStruct SkillUICellMainSubStruct;

        public void InitialSetting(ref ISkillUIController skillCellUIController, ref SkillUICellMainSubStruct SkillUICellMainSubStruct)
        {
            this.skillUIController = skillCellUIController;
            this.SkillUICellMainSubStruct = SkillUICellMainSubStruct;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            this.skillUIController.MouseClickInteraction(this.SkillUICellMainSubStruct);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            this.skillUIController.MouseEnterInteraction(this.SkillUICellMainSubStruct);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.skillUIController.MouseExitInteration(this.SkillUICellMainSubStruct);
        }
    }
}