using System.Collections.Generic;

using UnityEngine;

namespace GameSystem.InGameUI.Skill
{
    public enum ActivateOrNot
    {
        Activate,
        Not
    }

    public interface ISkillUICellView
    {
        public void InitialNonCell(ref ISkillUIModel skillUIModel, ISkillUIController skillUIController);
        public void InitizlLineCell(ref ISkillUIModel skillUIModel, ISkillUIController skillUIController, SkillUICellStruct skillUICellStruct, int skillTreeColumnCount);
        public void InitializeMainAndSubCell(ref ISkillUIModel skillUIModel, ISkillUIController skillUIController, SkillUICellStruct skillUICellStruct, int skillTreeColumnCount,
            SkillAndCellNumberStruct SkillAndCellNumberStruct, SkillInformationStruct skillInformationStruct);
    }

    public class SkillUICellView : MonoBehaviour, ISkillUICellView, ICellOrderToBeChangedObserverForView, IPlayerSkillInformationObserverForView
    {
        private ICellOrderToBeChangedObserver cellOrderToBeChangedObserver;     // 동적 Observer
        private IPlayerSkillInformationObserver playerSkillInformationObserver; // 동적 Observer

        private ISkillUIController skillUIController;

        private SkillUICellStruct SkillUICellStruct;                    // 고정
        private SkillAndCellNumberStruct SkillAndCellNumberStruct;                        // 고정
        private SkillInformationStruct skillInformationStruct;                  // 고정
        private int skillTreeColumnCount;

        private PlayerSkillInformationStruct playerSkillInformationStruct;      // 동적
        private List<List<int>> cellOrderToBeChanged;                           // 동적

        private RectTransform myRectTransform;                                  // 현재 RectTransform
        private UnityEngine.UI.Image degreeOfSkillLevel; 
        

        private void Awake()
        {
            this.myRectTransform = this.GetComponent<RectTransform>();
            this.cellOrderToBeChangedObserver = null;
            this.playerSkillInformationObserver = null;
        }

        // SkillUICell Prefab의 역할에 따라, ISkillUICellView 초기 설정 메소드를 나누었다.
        public void InitialNonCell(ref ISkillUIModel skillUIModel, ISkillUIController skillUIController) {
            this.skillUIController = skillUIController;
        }
        public void InitizlLineCell(ref ISkillUIModel skillUIModel, ISkillUIController skillUIController, SkillUICellStruct SkillUICellStruct, int skillTreeColumnCount)
        {
            this.skillUIController = skillUIController;

            this.SkillUICellStruct = SkillUICellStruct;
            this.skillTreeColumnCount = skillTreeColumnCount;

            this.RegisterCellOrderToBeChangedObserver(ref skillUIModel);    // CellOrderToBeChanged 데이터에 대한 Obserever 등록(구독).

            this.InitializeCellLine();                                      // SkillUICellStruct.LineNumber를 이용한, 그래프 간선 그리기.
        }
        public void InitializeMainAndSubCell(ref ISkillUIModel skillUIModel, ISkillUIController skillUIController, SkillUICellStruct skillUICellStruct, int skillTreeColumnCount, 
            SkillAndCellNumberStruct SkillAndCellNumberStruct, SkillInformationStruct skillInformationStruct)
        {
            this.skillUIController = skillUIController;

            this.SkillUICellStruct = skillUICellStruct;
            this.SkillAndCellNumberStruct = SkillAndCellNumberStruct;
            this.skillInformationStruct = skillInformationStruct;
            this.skillTreeColumnCount = skillTreeColumnCount;

            this.RegisterCellOrderToBeChangedObserver(ref skillUIModel);    // CellOrderToBeChanged 데이터에 대한 Obserever 등록(구독).
            this.RegisterPlayerSkillInformationObserver(ref skillUIModel);  // PlayerSkillInformation 데이터에 대한 Obserever 등록(구독).

            this.InitializeCellLine();                                      // SkillUICellStruct.LineNumber를 이용한, 그래프 간선 그리기.
            this.InitializeCellMainAndSub();                                // SkillUICellStruct.CellContent를 이용한, 해당되는 하위 오브젝트 활성화.
            this.DisplayDegreeOfSkillLevel();                               // PlayerSkillInformation.currentLevel과 skillInformationStruct.SkillMaxLevel을 이용한, 스킬 학습량 표시.
        }

        // Observer 등록 메소드
        private void RegisterCellOrderToBeChangedObserver(ref ISkillUIModel skillUIModel)
        {
            this.cellOrderToBeChanged = new List<List<int>>();              // CellOrderToBeChanged 멤버 정의
            this.cellOrderToBeChangedObserver = skillUIModel;               // Observer 패턴의 발행자(SkillModel) 참조.
            this.cellOrderToBeChangedObserver.RegisterCellOrderToBeChangedObserver(this);   // Observer 패턴의 발행물(CellOrderToBeChanged) 등록(구독).
        }
        private void RegisterPlayerSkillInformationObserver(ref ISkillUIModel skillUIModel)
        {
            this.playerSkillInformationObserver = skillUIModel;
            this.playerSkillInformationObserver.RegisterPlayerSkillInformationObserver(this);
            this.playerSkillInformationStruct = this.playerSkillInformationObserver.GetPlayerSkillInformation(this.skillInformationStruct.SkillNumber);
        }

        // 최초 셀 그리기.
        private void InitializeCellLine()
        {
            int temp = this.SkillUICellStruct.LineNumber;

            // 선 정리.
            for(int i = 0; 0 < temp; ++i)
            {
                if (temp % 2 != 0) { this.myRectTransform.GetChild(0).GetChild(i).gameObject.SetActive(true); }
                else { this.myRectTransform.GetChild(0).GetChild(i).gameObject.SetActive(false); }

                temp /= 2;
            }
        }
        // main과 sub 객체 초기 설정.
        private void InitializeCellMainAndSub()
        {
            if (this.SkillUICellStruct.CellContent == CellContent.main)
            {
                this.myRectTransform.GetChild(1).gameObject.SetActive(true);
                this.myRectTransform.GetChild(1).GetChild(0).gameObject.SetActive(true);
                this.myRectTransform.GetChild(1).GetChild(0).GetChild(1).gameObject.SetActive(true);
                // 스킬 이미지에 존재하는 IInteractionSkillImageUIAndMouseInSkillMenuUIView와 SkillUIController 연결하기.
                this.myRectTransform.GetChild(1).GetChild(0).GetChild(1).GetChild(1).GetChild(0).
                    GetComponent<IInteractionSkillImageUIAndMouseInSkillMenuUIView>().InitialSetting(ref this.skillUIController, ref this.SkillAndCellNumberStruct);

                this.degreeOfSkillLevel = this.myRectTransform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<UnityEngine.UI.Image>();
            }
            else if (this.SkillUICellStruct.CellContent == CellContent.sub)
            {
                this.myRectTransform.GetChild(1).gameObject.SetActive(true);
                this.myRectTransform.GetChild(1).GetChild(1).gameObject.SetActive(true);
                this.myRectTransform.GetChild(1).GetChild(1).GetChild(1).gameObject.SetActive(true);
                // 스킬 이미지에 존재하는 IInteractionSkillImageUIAndMouseInSkillMenuUIView와 SkillUIController 연결하기.
                this.myRectTransform.GetChild(1).GetChild(1).GetChild(1).GetChild(1).GetChild(0).
                    GetComponent<IInteractionSkillImageUIAndMouseInSkillMenuUIView>().InitialSetting(ref this.skillUIController, ref this.SkillAndCellNumberStruct);

                this.degreeOfSkillLevel = this.myRectTransform.GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetComponent< UnityEngine.UI.Image>();
            }
        }
        // -----------------------------

        // Observer 변경 시, 호출되는 부분.
        public void UpdateCellOrderToBeChangedObserver(ActivateOrNot activateOrNot)
        {
            this.cellOrderToBeChanged = this.cellOrderToBeChangedObserver.GetCellOrderToBeChanged();

            bool isChanged = false;
            int lineWantToActivate = 0;

            for (int i = 0; i < this.cellOrderToBeChanged.Count; ++i)
            {
                for(int j = 0; j < this.cellOrderToBeChanged[i].Count; ++j)
                {
                    // 내 cellNumber와 동일한지 확인.
                    if (cellOrderToBeChanged[i][j] == SkillUICellStruct.CellNumber)
                    {
                        isChanged = true;

                        if (j > 0 && (SkillUICellStruct.CellNumber - cellOrderToBeChanged[i][j-1] == 1))
                        {
                            lineWantToActivate |= 1;
                        }
                        else if(j > 0 && (SkillUICellStruct.CellNumber - cellOrderToBeChanged[i][j - 1] == this.skillTreeColumnCount))
                        {
                            lineWantToActivate |= 8;
                        }

                        if(j < cellOrderToBeChanged[i].Count-1 && (cellOrderToBeChanged[i][j + 1] - SkillUICellStruct.CellNumber == 1))
                        {
                            lineWantToActivate |= 4;
                        }
                        else if(j < cellOrderToBeChanged[i].Count - 1 && (cellOrderToBeChanged[i][j + 1] - SkillUICellStruct.CellNumber == this.skillTreeColumnCount))
                        {
                            lineWantToActivate |= 2;
                        }
                    }
                }
            }

            if(isChanged) ActivateOrNotCellLines(activateOrNot, lineWantToActivate);
        }
        public void UpdatePlayerSkillInformationObserver()
        {
            this.playerSkillInformationStruct = this.playerSkillInformationObserver.GetPlayerSkillInformation(this.SkillAndCellNumberStruct.SkillNumber);
            this.DisplayDegreeOfSkillLevel();
        }

        // Skill Tree UI 강조 활성화 or 비활성화
        private void ActivateOrNotCellLines(ActivateOrNot activateOrNot, int lineWantToActivate)
        {
            bool isActivate;
            if (activateOrNot == ActivateOrNot.Activate)    isActivate = true;
            else                                            isActivate = false;

            // Line(그래프의 간선) UI 강조 객체 비활성화
            for (int i = 0; 0 < lineWantToActivate; ++i)
            {
                if (lineWantToActivate % 2 != 0) this.myRectTransform.GetChild(0).GetChild(i).GetChild(0).gameObject.SetActive(isActivate);
                lineWantToActivate /= 2;
            }

            // main과 sub의 UI 강조 객체 비활성화
            if (SkillUICellStruct.CellContent == CellContent.main)
                this.myRectTransform.GetChild(1).GetChild(0).GetChild(0).gameObject.SetActive(isActivate);
            if (SkillUICellStruct.CellContent == CellContent.sub)
                this.myRectTransform.GetChild(1).GetChild(1).GetChild(0).gameObject.SetActive(isActivate);
        }
        // 스킬 학습량 표시.
        private void DisplayDegreeOfSkillLevel()
        {
            this.degreeOfSkillLevel.fillAmount = (float)this.playerSkillInformationStruct.CurrentLevel / (float)this.skillInformationStruct.MaxLevel;
        }
    }

}