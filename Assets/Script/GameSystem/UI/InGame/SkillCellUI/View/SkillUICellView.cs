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
        private ICellOrderToBeChangedObserver cellOrderToBeChangedObserver;     // ���� Observer
        private IPlayerSkillInformationObserver playerSkillInformationObserver; // ���� Observer

        private ISkillUIController skillUIController;

        private SkillUICellStruct SkillUICellStruct;                    // ����
        private SkillAndCellNumberStruct SkillAndCellNumberStruct;                        // ����
        private SkillInformationStruct skillInformationStruct;                  // ����
        private int skillTreeColumnCount;

        private PlayerSkillInformationStruct playerSkillInformationStruct;      // ����
        private List<List<int>> cellOrderToBeChanged;                           // ����

        private RectTransform myRectTransform;                                  // ���� RectTransform
        private UnityEngine.UI.Image degreeOfSkillLevel; 
        

        private void Awake()
        {
            this.myRectTransform = this.GetComponent<RectTransform>();
            this.cellOrderToBeChangedObserver = null;
            this.playerSkillInformationObserver = null;
        }

        // SkillUICell Prefab�� ���ҿ� ����, ISkillUICellView �ʱ� ���� �޼ҵ带 ��������.
        public void InitialNonCell(ref ISkillUIModel skillUIModel, ISkillUIController skillUIController) {
            this.skillUIController = skillUIController;
        }
        public void InitizlLineCell(ref ISkillUIModel skillUIModel, ISkillUIController skillUIController, SkillUICellStruct SkillUICellStruct, int skillTreeColumnCount)
        {
            this.skillUIController = skillUIController;

            this.SkillUICellStruct = SkillUICellStruct;
            this.skillTreeColumnCount = skillTreeColumnCount;

            this.RegisterCellOrderToBeChangedObserver(ref skillUIModel);    // CellOrderToBeChanged �����Ϳ� ���� Obserever ���(����).

            this.InitializeCellLine();                                      // SkillUICellStruct.LineNumber�� �̿���, �׷��� ���� �׸���.
        }
        public void InitializeMainAndSubCell(ref ISkillUIModel skillUIModel, ISkillUIController skillUIController, SkillUICellStruct skillUICellStruct, int skillTreeColumnCount, 
            SkillAndCellNumberStruct SkillAndCellNumberStruct, SkillInformationStruct skillInformationStruct)
        {
            this.skillUIController = skillUIController;

            this.SkillUICellStruct = skillUICellStruct;
            this.SkillAndCellNumberStruct = SkillAndCellNumberStruct;
            this.skillInformationStruct = skillInformationStruct;
            this.skillTreeColumnCount = skillTreeColumnCount;

            this.RegisterCellOrderToBeChangedObserver(ref skillUIModel);    // CellOrderToBeChanged �����Ϳ� ���� Obserever ���(����).
            this.RegisterPlayerSkillInformationObserver(ref skillUIModel);  // PlayerSkillInformation �����Ϳ� ���� Obserever ���(����).

            this.InitializeCellLine();                                      // SkillUICellStruct.LineNumber�� �̿���, �׷��� ���� �׸���.
            this.InitializeCellMainAndSub();                                // SkillUICellStruct.CellContent�� �̿���, �ش�Ǵ� ���� ������Ʈ Ȱ��ȭ.
            this.DisplayDegreeOfSkillLevel();                               // PlayerSkillInformation.currentLevel�� skillInformationStruct.SkillMaxLevel�� �̿���, ��ų �н��� ǥ��.
        }

        // Observer ��� �޼ҵ�
        private void RegisterCellOrderToBeChangedObserver(ref ISkillUIModel skillUIModel)
        {
            this.cellOrderToBeChanged = new List<List<int>>();              // CellOrderToBeChanged ��� ����
            this.cellOrderToBeChangedObserver = skillUIModel;               // Observer ������ ������(SkillModel) ����.
            this.cellOrderToBeChangedObserver.RegisterCellOrderToBeChangedObserver(this);   // Observer ������ ���๰(CellOrderToBeChanged) ���(����).
        }
        private void RegisterPlayerSkillInformationObserver(ref ISkillUIModel skillUIModel)
        {
            this.playerSkillInformationObserver = skillUIModel;
            this.playerSkillInformationObserver.RegisterPlayerSkillInformationObserver(this);
            this.playerSkillInformationStruct = this.playerSkillInformationObserver.GetPlayerSkillInformation(this.skillInformationStruct.SkillNumber);
        }

        // ���� �� �׸���.
        private void InitializeCellLine()
        {
            int temp = this.SkillUICellStruct.LineNumber;

            // �� ����.
            for(int i = 0; 0 < temp; ++i)
            {
                if (temp % 2 != 0) { this.myRectTransform.GetChild(0).GetChild(i).gameObject.SetActive(true); }
                else { this.myRectTransform.GetChild(0).GetChild(i).gameObject.SetActive(false); }

                temp /= 2;
            }
        }
        // main�� sub ��ü �ʱ� ����.
        private void InitializeCellMainAndSub()
        {
            if (this.SkillUICellStruct.CellContent == CellContent.main)
            {
                this.myRectTransform.GetChild(1).gameObject.SetActive(true);
                this.myRectTransform.GetChild(1).GetChild(0).gameObject.SetActive(true);
                this.myRectTransform.GetChild(1).GetChild(0).GetChild(1).gameObject.SetActive(true);
                // ��ų �̹����� �����ϴ� IInteractionSkillImageUIAndMouseInSkillMenuUIView�� SkillUIController �����ϱ�.
                this.myRectTransform.GetChild(1).GetChild(0).GetChild(1).GetChild(1).GetChild(0).
                    GetComponent<IInteractionSkillImageUIAndMouseInSkillMenuUIView>().InitialSetting(ref this.skillUIController, ref this.SkillAndCellNumberStruct);

                this.degreeOfSkillLevel = this.myRectTransform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<UnityEngine.UI.Image>();
            }
            else if (this.SkillUICellStruct.CellContent == CellContent.sub)
            {
                this.myRectTransform.GetChild(1).gameObject.SetActive(true);
                this.myRectTransform.GetChild(1).GetChild(1).gameObject.SetActive(true);
                this.myRectTransform.GetChild(1).GetChild(1).GetChild(1).gameObject.SetActive(true);
                // ��ų �̹����� �����ϴ� IInteractionSkillImageUIAndMouseInSkillMenuUIView�� SkillUIController �����ϱ�.
                this.myRectTransform.GetChild(1).GetChild(1).GetChild(1).GetChild(1).GetChild(0).
                    GetComponent<IInteractionSkillImageUIAndMouseInSkillMenuUIView>().InitialSetting(ref this.skillUIController, ref this.SkillAndCellNumberStruct);

                this.degreeOfSkillLevel = this.myRectTransform.GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetComponent< UnityEngine.UI.Image>();
            }
        }
        // -----------------------------

        // Observer ���� ��, ȣ��Ǵ� �κ�.
        public void UpdateCellOrderToBeChangedObserver(ActivateOrNot activateOrNot)
        {
            this.cellOrderToBeChanged = this.cellOrderToBeChangedObserver.GetCellOrderToBeChanged();

            bool isChanged = false;
            int lineWantToActivate = 0;

            for (int i = 0; i < this.cellOrderToBeChanged.Count; ++i)
            {
                for(int j = 0; j < this.cellOrderToBeChanged[i].Count; ++j)
                {
                    // �� cellNumber�� �������� Ȯ��.
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

        // Skill Tree UI ���� Ȱ��ȭ or ��Ȱ��ȭ
        private void ActivateOrNotCellLines(ActivateOrNot activateOrNot, int lineWantToActivate)
        {
            bool isActivate;
            if (activateOrNot == ActivateOrNot.Activate)    isActivate = true;
            else                                            isActivate = false;

            // Line(�׷����� ����) UI ���� ��ü ��Ȱ��ȭ
            for (int i = 0; 0 < lineWantToActivate; ++i)
            {
                if (lineWantToActivate % 2 != 0) this.myRectTransform.GetChild(0).GetChild(i).GetChild(0).gameObject.SetActive(isActivate);
                lineWantToActivate /= 2;
            }

            // main�� sub�� UI ���� ��ü ��Ȱ��ȭ
            if (SkillUICellStruct.CellContent == CellContent.main)
                this.myRectTransform.GetChild(1).GetChild(0).GetChild(0).gameObject.SetActive(isActivate);
            if (SkillUICellStruct.CellContent == CellContent.sub)
                this.myRectTransform.GetChild(1).GetChild(1).GetChild(0).gameObject.SetActive(isActivate);
        }
        // ��ų �н��� ǥ��.
        private void DisplayDegreeOfSkillLevel()
        {
            this.degreeOfSkillLevel.fillAmount = (float)this.playerSkillInformationStruct.CurrentLevel / (float)this.skillInformationStruct.MaxLevel;
        }
    }

}