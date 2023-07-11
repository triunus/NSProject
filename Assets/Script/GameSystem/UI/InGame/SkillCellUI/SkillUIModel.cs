using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using UnityEngine;

namespace GameSystem.InGameUI.Skill
{
    public interface ICellOrderToBeChangedObserverForView
    {
        // Ȱ��ȭ/��Ȱ��ȭ �۾������� �Ű������� �˷��ش�.
        public void UpdateCellOrderToBeChangedObserver(ActivateOrNot activateOrNot);
    }
    public interface ICellOrderToBeChangedObserver
    {
        public void RegisterCellOrderToBeChangedObserver(ICellOrderToBeChangedObserverForView observer);
        public void RemoveCellOrderToBeChangedObserver(ICellOrderToBeChangedObserverForView observer);
        public List<List<int>> GetCellOrderToBeChanged();
    }
    public interface IPlayerSkillInformationObserverForView
    {
        public void UpdatePlayerSkillInformationObserver();
    }
    public interface IPlayerSkillInformationObserver
    {
        public void RegisterPlayerSkillInformationObserver(IPlayerSkillInformationObserverForView observer);
        public void RemovePlayerSkillInformationObserver(IPlayerSkillInformationObserverForView observer);
        public PlayerSkillInformationStruct GetPlayerSkillInformation(int skillNumber);
    }

    public interface ISkillUIModel : ICellOrderToBeChangedObserver, IPlayerSkillInformationObserver
    {
        public void InitialSetting(ISkillManagerForModel skillCellUIManagerForModel);
        public void DecideActivateOrInActivateCell(SkillUICellMSStruct skillUICellMSStruct);     // Ȱ��ȭ or ��Ȱ��ȭ
        public void LearnSkill(int skillNumber);

        public ref List<SkillUICellStruct> SkillUICellStructs { get; }
        public ref List<SkillUICellMSStruct> SkillUICellMSStructs { get; }
        public ref List<SkillInformationStruct> SkillInformationStructs { get; }
        public LinkedList<SkillUICellMSPreconditionStruct> GetMSPreconditionStruct(int skillNumber);
    }

    public class SkillUIModel : MonoBehaviour, ISkillUIModel
    {
        private ISkillManagerForModel skillCellUIManager;

        // ������ ����Ǿ�� �� ������ Cell ����� ��ϵ� ���� �����Ѵ�.
        private List<ICellOrderToBeChangedObserverForView> cellOrderToBeChangedObservers;       // UI ������ ����Ǿ�� �� CellNumber�� �ʿ���ϴ� View ���.
        private List<List<int>> cellOrderToBeChanged;                                           // UI ������ ����Ǿ�� �� CellNumber ���.

        private List<IPlayerSkillInformationObserverForView> playerSkillInformationObservers;   // ����ڰ� ���� �н��� ��ų�� Level ������ �ʿ���ϴ� View ���.
        private List<PlayerSkillInformationStruct> playerSkillInformationStructs;               // ����ڰ� ���� �н��� ��ų�� Level ����.


        // SkillUICell�� �⺻ ������ ���� �ִ�.
        // SkillUICell.CellNumber�� �׷����� ������ ��Ÿ���µ� ����Ѵ�.
        // SkillUICell.Line�� �׷����� ������ ��Ÿ���µ� ����Ѵ�.
        private List<SkillUICellStruct> skillUICellStructs;                                     
        private List<LinkedList<SkillUICellNumberPreconditionStruct>> adjacentCellNumberPreconditionStructs;    // SkillUICell.CellNumber ���� ������ �ִ� �׷����� ǥ���ϱ� ����
                                                                                                        // CellNumber�� ���� ������ ǥ���ϴ� ������ ��ϵǾ� �ִ�.

        // SkillNumber�� CellNumber ������ ���� �ִ�.
        // SkillUICell.main�� sub�� ���� '��ų' ��ü�� ����ϴ� GameObject�� �ʿ�� �ϴ� �����̴�.
        // ����ڿ� 'SkillCellUI' GameObject ���� ��ȣ�ۿ� ��, ���õ� 'SkillCellUI'�� �����ϱ� ���� ����Ѵ�.
        private List<SkillUICellMSStruct> skillUICellMSStructs;                                         
        private List<LinkedList<SkillUICellMSPreconditionStruct>> adjacentMSPreconditionStructs;        // skillUICellMSStructs.skillNumber ���� ������ �ִ� �׷����� ǥ���ϱ� ����
                                                                                                        // SkillNumber�� ���� ������ ����ġ�� ǥ���ϴ� ������ ��ϵǾ� �ִ�.

        // Skill�� �⺻ �����͸� ���� �ִ�.
        private List<SkillInformationStruct> skillInformationStructs;

        private List<LinkedList<SkillUICellMSPreconditionStruct>> MSPreconditionStructs;

        private List<int> cellLIneStartPosition;        // skillUICell ���̺� ��, ���ʷ� ���۵Ǵ� cell�� cellNumber
        private List<int> cellMSStartPosition;          // skillUICell ���̺� ��, ���ʷ� ���۵Ǵ� skill�� skillNumber

        private List<List<int>> activatedLineOrder;                                     // Ư�� cellNumber�� �����ϱ� ����, cellNumber ����.
        private List<List<int>> activatedMSOrder;                                       // Ư�� skillNumber�� �����ϱ� ����, skillNumber ����.

        private int beClickedCellNumber;                                        // ���� Ŭ���� ��ų ���.

        // ICellOrderToBeChangedObserver ����
        public void RegisterCellOrderToBeChangedObserver(ICellOrderToBeChangedObserverForView observer)
        {
            this.cellOrderToBeChangedObservers.Add(observer);
        }
        public void RemoveCellOrderToBeChangedObserver(ICellOrderToBeChangedObserverForView observer)
        {
            this.cellOrderToBeChangedObservers.Remove(observer);
        }
        public List<List<int>> GetCellOrderToBeChanged()
        {
            return cellOrderToBeChanged;
        }
        private void NotifyCellOrderToBeChangedObservers(ActivateOrNot activateOrNot)
        {
            int i = 0;

            while (i < cellOrderToBeChangedObservers.Count)
            {
                cellOrderToBeChangedObservers[i].UpdateCellOrderToBeChangedObserver(activateOrNot);
                ++i;
            }
        }
        // -----------------

        // IPlayerSkillInformationObserver ����
        public void RegisterPlayerSkillInformationObserver(IPlayerSkillInformationObserverForView observer)
        {
            this.playerSkillInformationObservers.Add(observer);
        }
        public void RemovePlayerSkillInformationObserver(IPlayerSkillInformationObserverForView observer)
        {
            this.playerSkillInformationObservers.Remove(observer);
        }
        public PlayerSkillInformationStruct GetPlayerSkillInformation(int skillNumber)
        {
            return this.playerSkillInformationStructs[skillNumber];
        }
        private void NotifyPlayerSkillInformationObservers()
        {
            int i = 0;

            while (i < playerSkillInformationObservers.Count)
            {
                playerSkillInformationObservers[i].UpdatePlayerSkillInformationObserver();
                ++i;
            }
        }
        // --------------------------

        // SkillUIModel ��� ����.
        private void Awake()
        {
            this.cellOrderToBeChangedObservers = new List<ICellOrderToBeChangedObserverForView>();
            this.cellOrderToBeChanged = new List<List<int>>();
            this.playerSkillInformationObservers = new List<IPlayerSkillInformationObserverForView>();

            this.skillUICellStructs = new List<SkillUICellStruct>();
            this.adjacentCellNumberPreconditionStructs = new List<LinkedList<SkillUICellNumberPreconditionStruct>>();

            this.skillUICellMSStructs = new List<SkillUICellMSStruct>();
            this.adjacentMSPreconditionStructs = new List<LinkedList<SkillUICellMSPreconditionStruct>>();
            this.MSPreconditionStructs = new List<LinkedList<SkillUICellMSPreconditionStruct>>();

            this.cellLIneStartPosition = new List<int>();
            this.cellMSStartPosition = new List<int>();

            this.activatedLineOrder = new List<List<int>>();
            this.activatedMSOrder = new List<List<int>>();

            this.beClickedCellNumber = -1;
        }

        // ISkillCellUIModel ����
        // SkillModel �ʱ� ����. Manager���� �ʿ��� ������ ��������.
        public void InitialSetting(ISkillManagerForModel skillCellUIManager)
        {
            this.skillCellUIManager = skillCellUIManager;

            this.skillUICellStructs = this.skillCellUIManager.GetSkillUICellLineStructs();
            this.skillUICellMSStructs = this.skillCellUIManager.GetSkillUICellMSStructs();
            this.skillInformationStructs = this.skillCellUIManager.GetSkillInformationStruct();
            this.playerSkillInformationStructs = this.skillCellUIManager.GetPlayerSkillInformationStruct();

            this.FindCellLineStartPosition();
            this.FindCellMSStartPosition();

            this.MakeSkillUICellLineLinkedList();
            this.MakeSkillUICellMSLinkedList();
        }
        public void DecideActivateOrInActivateCell(SkillUICellMSStruct skillUICellMSStruct)
        {
            // ���õ� ���� ������. �� Ȱ��ȭ.
            if (this.beClickedCellNumber == -1)
            {
                this.beClickedCellNumber = skillUICellMSStruct.CellNumber;

                this.CellUILineTopologySort(this.beClickedCellNumber);
                this.CellUIMSTopologySort(skillUICellMSStruct);
                this.ExcludeCellNumberToBeActivated();
                this.ExtractCellNumberToBeChanged();

                this.NotifyCellOrderToBeChangedObservers(ActivateOrNot.Activate);   // �� Ȱ��ȭ
            }
            // ���õ� �Ͱ�, ���Ӱ� ������ ���� ������. �� ��Ȱ��ȭ.
            else if (this.beClickedCellNumber == skillUICellMSStruct.CellNumber)
            {
                this.beClickedCellNumber = -1;

                this.NotifyCellOrderToBeChangedObservers(ActivateOrNot.Not);  // �� ��Ȱ��ȭ

                this.activatedLineOrder.Clear();
                this.activatedMSOrder.Clear();
                this.cellOrderToBeChanged.Clear();
            }
            // ���õ� �Ͱ�, ���Ӱ� ���õ� ���� �ٸ�. �� ��Ȱ��ȭ �� �� Ȱ��ȭ.
            else
            {
                this.beClickedCellNumber = skillUICellMSStruct.CellNumber;

                this.NotifyCellOrderToBeChangedObservers(ActivateOrNot.Not);  // �� ��Ȱ��ȭ

                this.activatedLineOrder.Clear();
                this.activatedMSOrder.Clear();
                this.cellOrderToBeChanged.Clear();

                this.CellUILineTopologySort(this.beClickedCellNumber);
                this.CellUIMSTopologySort(skillUICellMSStruct);
                this.ExcludeCellNumberToBeActivated();
                this.ExtractCellNumberToBeChanged();

                this.NotifyCellOrderToBeChangedObservers(ActivateOrNot.Activate);    // �� Ȱ��ȭ
            }
        }
        public void LearnSkill(int skillNumber)
        {
            int tempCost = this.skillInformationStructs[skillNumber].Cost + this.skillInformationStructs[skillNumber].Cost * this.playerSkillInformationStructs[skillNumber].CurrentLevel;

            if (this.skillCellUIManager.OwnManaStone >= tempCost)
            {
                this.skillCellUIManager.OwnManaStone = this.skillCellUIManager.OwnManaStone - tempCost;

                playerSkillInformationStructs[skillNumber] = new PlayerSkillInformationStruct(
                    skillNumber: this.playerSkillInformationStructs[skillNumber].SkillNumber,
                    currentLevel: this.playerSkillInformationStructs[skillNumber].CurrentLevel + 1
                    );

                this.NotifyPlayerSkillInformationObservers();
            }
            else
            {
                Debug.Log("Mana ����");
                // error �޽���.
            }

        }
        public ref List<SkillUICellStruct> SkillUICellStructs { get { return ref this.skillUICellStructs; } }
        public ref List<SkillUICellMSStruct> SkillUICellMSStructs { get { return ref this.skillUICellMSStructs; } }
        public ref List<SkillInformationStruct> SkillInformationStructs { get { return ref this.skillInformationStructs; } }
        public LinkedList<SkillUICellMSPreconditionStruct> GetMSPreconditionStruct(int skillNumber) { return this.MSPreconditionStructs[skillNumber]; }
        // -------------------------

        // DFS�� �����ϱ� ����, ������ ���� 4�� ���߸���.
        private void FindCellLineStartPosition()
        {
            for(int i=0; i < skillUICellStructs.Count; ++i)
            {
                if (skillUICellStructs[i].CellContent == CellContent.main && (skillUICellStructs[i].LineNumber == 4 || skillUICellStructs[i].LineNumber == 0))
                    this.cellLIneStartPosition.Add(skillUICellStructs[i].CellNumber);
            }
        }
        private void FindCellMSStartPosition()
        {
            for (int i = 0; i < cellLIneStartPosition.Count; ++i)
            {
                this.cellMSStartPosition.Add(this.skillUICellMSStructs[skillUICellMSStructs.FindIndex(x => x.CellNumber == cellLIneStartPosition[i])].SkillNumber);
            }
        }
        private void MakeSkillUICellLineLinkedList()
        {
            TextAsset skillUICellLinePrecondition_TextAsset = Resources.Load<TextAsset>("GameSystem/SkillData/UI/SkillUICellLinePrecondition");
            JArray skillUICellLinePrecondition = JArray.Parse(skillUICellLinePrecondition_TextAsset.ToString());

            for (int i = 0; i < skillUICellStructs.Count; ++i)
            {
                LinkedList<SkillUICellNumberPreconditionStruct> perSkillUICellLine = new LinkedList<SkillUICellNumberPreconditionStruct>();
                this.adjacentCellNumberPreconditionStructs.Add(perSkillUICellLine);
            }

            for (int i = 0; i < skillUICellLinePrecondition.Count; ++i)
            {
                this.adjacentCellNumberPreconditionStructs[(int)skillUICellLinePrecondition[i]["Precondition_p"]].AddLast(
                    new SkillUICellNumberPreconditionStruct(
                        precondition_q: (int)skillUICellLinePrecondition[i]["Precondition_q"]));
            }
        }
        private void MakeSkillUICellMSLinkedList()
        {
            TextAsset skillUICellMSPrecondition_TextAsset = Resources.Load<TextAsset>("GameSystem/SkillData/UI/SkillUICellMSPrecondition");
            JArray skillUICellMSPrecondition = JArray.Parse(skillUICellMSPrecondition_TextAsset.ToString());

            // adjacentMSPreconditionStructs
            for (int i = 0; i < this.skillUICellMSStructs.Count; ++i)
            {
                LinkedList<SkillUICellMSPreconditionStruct> perSkillUICellMS = new LinkedList<SkillUICellMSPreconditionStruct>();
                this.adjacentMSPreconditionStructs.Add(perSkillUICellMS);
            }

            for (int i = 0; i < skillUICellMSPrecondition.Count; ++i)
            {
                this.adjacentMSPreconditionStructs[(int)skillUICellMSPrecondition[i]["Precondition_p"]].AddLast(
                    new SkillUICellMSPreconditionStruct(
                        precondition_q: (int)skillUICellMSPrecondition[i]["Precondition_q"],
                        precondition_Weight: (int)skillUICellMSPrecondition[i]["Precondition_Weight"]));
            }

            // MSPreconditionStructs
            for (int i = 0; i < this.skillUICellMSStructs.Count; ++i)
            {
                LinkedList<SkillUICellMSPreconditionStruct> perSkillUICellMS = new LinkedList<SkillUICellMSPreconditionStruct>();
                this.MSPreconditionStructs.Add(perSkillUICellMS);
            }

            for (int i = 0; i < skillUICellMSPrecondition.Count; ++i)
            {
                this.MSPreconditionStructs[(int)skillUICellMSPrecondition[i]["Precondition_q"]].AddLast(
                    new SkillUICellMSPreconditionStruct(
                        precondition_q: (int)skillUICellMSPrecondition[i]["Precondition_p"],
                        precondition_Weight: (int)skillUICellMSPrecondition[i]["Precondition_Weight"]));
            }
        }

        private void CellUILineTopologySort(int destinationCellNumber)
        {
            List<int> visited = new List<int>();
            bool destinationIsVisited;

            for (int i = 0; i < this.cellLIneStartPosition.Count; ++i)
            {
                List<int> order = new List<int>();
                visited = Enumerable.Repeat(0, skillUICellStructs.Count).ToList();
                destinationIsVisited = false;

                // ���� intersection, ���� cellNumber, �湮����, ������ �湮����, �湮���, �湮 ����.
                this.CellUILineDFS(cellLIneStartPosition[i], ref destinationCellNumber, ref destinationIsVisited, ref visited, ref order);

                if (destinationIsVisited) { this.activatedLineOrder.Add( Enumerable.Reverse(order).ToList()); }
            }
        }
        private void CellUILineDFS(int startCellNumber, ref int destinationCellNumber, ref bool destinationIsVisited, ref List<int> visited, ref List<int> order)
        {
            visited[startCellNumber] = 1;

            if (startCellNumber == destinationCellNumber) 
            {
                destinationIsVisited = true;    
                order.Add(startCellNumber);
                return;
            }

            foreach (var item in adjacentCellNumberPreconditionStructs[startCellNumber])
            {
                if (visited[item.Precondition_q] == 0 && !destinationIsVisited)
                {
                    CellUILineDFS(item.Precondition_q, ref destinationCellNumber, ref destinationIsVisited, ref visited, ref order);
                }
            }

            if (destinationIsVisited) { order.Add(startCellNumber); }
        }
        private void CellUIMSTopologySort(SkillUICellMSStruct skillUICellMSStruct)
        {
            List<int> visited = new List<int>();
            bool destinationIsVisited;
            int destinationSkillNumber = skillUICellMSStruct.SkillNumber;

            for (int i = 0; i < this.cellMSStartPosition.Count; ++i)
            {
                List<int> order = new List<int>();
                visited = Enumerable.Repeat(0, skillUICellMSStructs.Count).ToList();
                destinationIsVisited = false;

                // ���� cellNumber, �湮����, ������ �湮����, �湮���, �湮 ����.
                this.CellUIMSDFS(cellMSStartPosition[i], ref destinationSkillNumber, ref destinationIsVisited, ref visited, ref order);

                if (destinationIsVisited) { this.activatedMSOrder.Add(Enumerable.Reverse(order).ToList()); }

                order.Clear();
            }
        }
        private void CellUIMSDFS(int startSkillNumber, ref int destinationSkillNumber, ref bool destinationIsVisited, ref List<int> visited, ref List<int> order)
        {
            visited[startSkillNumber] = 1;

            if (startSkillNumber == destinationSkillNumber)
            {
                destinationIsVisited = true;
                order.Add(startSkillNumber);
                return;
            }

            foreach (var item in adjacentMSPreconditionStructs[startSkillNumber])
            {
                if (visited[item.Precondition_q] == 0 && !destinationIsVisited)
                {
                    CellUIMSDFS(item.Precondition_q, ref destinationSkillNumber, ref destinationIsVisited, ref visited, ref order);
                }
            }

            if (destinationIsVisited) { order.Add(startSkillNumber); }
        }

        private void ExcludeCellNumberToBeActivated()
        {
            List<List<KeyValuePair<int, bool>>> satisfyCondition = new List<List<KeyValuePair<int, bool>>>();

            for (int i = 0; i < activatedMSOrder.Count; ++i)
            {
                List<KeyValuePair<int, bool>> temp = new List<KeyValuePair<int, bool>>();
                satisfyCondition.Add(temp);

                for (int j = 0; j < activatedMSOrder[i].Count-1; ++j)
                {
                    foreach(var item in adjacentMSPreconditionStructs[activatedMSOrder[i][j]])  // // skillNumber�� ���� �񱳰� �̷������.
                    {
                        if (item.Precondition_q == activatedMSOrder[i][j + 1])
                        {
                            if (playerSkillInformationStructs[activatedMSOrder[i][j]].CurrentLevel >= item.Precondition_Weight)
                                satisfyCondition[i].Add(new KeyValuePair<int, bool>(skillUICellMSStructs[activatedMSOrder[i][j]].CellNumber, true));
                            else satisfyCondition[i].Add(new KeyValuePair<int, bool>(skillUICellMSStructs[activatedMSOrder[i][j]].CellNumber, false));
                        }
                    }
                }

                satisfyCondition[i].Add(new KeyValuePair<int, bool>(skillUICellMSStructs[activatedMSOrder[i][activatedMSOrder[i].Count-1]].CellNumber, false));
            }


            for (int i = 0; i < satisfyCondition.Count; ++i)
            {
                while (satisfyCondition[i][0].Value)
                {

                    while (true) // true�� �ݺ�, �� ���� �ٸ��� ������ ����.
                    {

                        if (satisfyCondition[i][1].Key == activatedLineOrder[i][0]) { break; }

                        activatedLineOrder[i].RemoveAt(0);
                    }

                    satisfyCondition[i].RemoveAt(0);
                }
            }
        }
        private void ExtractCellNumberToBeChanged()
        {
            int rowCount = skillUICellStructs.Count / 11;

            for (int i = 0; i < activatedLineOrder.Count; ++i)
            {
                List<int> temp = new List<int>();

                for(int j = 1; j < activatedLineOrder[i].Count; ++j)
                {
                    if(activatedLineOrder[i][j-1]/ rowCount == activatedLineOrder[i][j]/ rowCount)
                    {
                        for(int k = 0; k < activatedLineOrder[i][j] - activatedLineOrder[i][j - 1]; ++k)
                        {
                            temp.Add(activatedLineOrder[i][j - 1] + k);
                        }
                    }
                    else
                    {
                        for(int l = 0; l < (activatedLineOrder[i][j]/ rowCount) - (activatedLineOrder[i][j - 1]/ rowCount); ++l)
                        {
                            temp.Add(activatedLineOrder[i][j - 1] + rowCount * l);
                        }
                    }
                }

                // ���� ������ ���� ���߿� �߰�����.
                temp.Add(activatedLineOrder[i][activatedLineOrder[i].Count-1]);

                this.cellOrderToBeChanged.Add(temp);
            }
        }
    }
}