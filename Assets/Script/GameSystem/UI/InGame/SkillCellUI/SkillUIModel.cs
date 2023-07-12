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

        private List<SkillUICellStruct> skillUICellStructs;                                                     // SkillUICell�� �⺻ ������ ���� �ִ�.                       
        private List<LinkedList<SkillUICellNumberPreconditionStruct>> adjacentCellNumberPreconditionStructs;    // SkillUICell.CellNumber ���� ������ �ִ� �׷����� ǥ���ϱ� ����
                                                                                                                // CellNumber�� ���� ������ ǥ���ϴ� ������ ��ϵǾ� �ִ�.

        private List<SkillUICellMSStruct> skillUICellMSStructs;                                         // 'SkillCellUI GameObject'�� �����ϱ� ���� ����Ѵ�.      

        // List i ��° ��ų�� ���� ���� ��ų �� ����ġ�� ����ϴ� ����ü�̴�.
        private List<LinkedList<SkillUICellMSPreconditionStruct>> adjacentMSPreconditionStructs;        // 'i ��° SkillNumber�� q SkillNumber�� ���� ����, i skillNumber ��ų�� weight��ŭ �ʿ�'�ϴٴ� ������ ���´�.
        private List<LinkedList<SkillUICellMSPreconditionStruct>> MSPreconditionStructs;                // 'i ��° SkillNumber�� ���� ����, q SkillNumberfmf weight��ŭ �ø� �ʿ�'�ϴٴ� ������ ���´�.

        // Skill�� �⺻ �����͸� ���� �ִ�.
        private List<SkillInformationStruct> skillInformationStructs;

        private List<int> cellStartPosition;        // skillUICell ���̺� ��, ���ʷ� ���۵Ǵ� cell�� cellNumber
        private List<int> cellMSStartPosition;          // skillUICell ���̺� ��, ���ʷ� ���۵Ǵ� skill�� skillNumber

        private List<List<int>> CellNumberOrder;                                     // Ư�� cellNumber�� �����ϱ� ����, cellNumber ����.
        private List<List<int>> MSOrder;                                       // Ư�� skillNumber�� �����ϱ� ����, skillNumber ����.

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
        // CellOrderToBeChanged ���� 2���� ������� ���Ǿ�� �ϹǷ�,
        // View�� ȣ���ϴ� �޼����� �Ű������� ����� activateOrNot ������ش�.
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
        // �ڽ��� SkillNumber ���� �˷��ָ�, skillNumber�� ���õ� playerSkillInformaionStructs ������ �����Ѵ�.
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

        // SkillUIModel ��� ���� �� �ʱⰪ �Է�.
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

            this.cellStartPosition = new List<int>();
            this.cellMSStartPosition = new List<int>();

            this.CellNumberOrder = new List<List<int>>();
            this.MSOrder = new List<List<int>>();

            this.beClickedCellNumber = -1;
        }

        // ISkillCellUIModel ����
        // Manager���� �ʿ��� ������ �������� + SkillModel �ʱ� ����.
        public void InitialSetting(ISkillManagerForModel skillCellUIManager)
        {
            this.skillCellUIManager = skillCellUIManager;

            this.skillUICellStructs = this.skillCellUIManager.GetSkillUICellLineStructs();
            this.skillUICellMSStructs = this.skillCellUIManager.GetSkillUICellMSStructs();
            this.skillInformationStructs = this.skillCellUIManager.GetSkillInformationStruct();
            this.playerSkillInformationStructs = this.skillCellUIManager.GetPlayerSkillInformationStruct();

            this.FindCellNumberStartPosition();         // CellNumber�� �������� ����ϴ� �׷����� ���� ���� ã��.
            this.FindCellMSStartPosition();             // SkillNumber�� �������� ����ϴ� �׷����� ���� ���� ã��.

            this.MakeSkillUICellLineLinkedList();       // CellNumber ���� ������ ����� �����͸� �о��, ��������Ʈ�� �����.
            this.MakeSkillUICellMSLinkedList();         // SkillNumber ���� ������ ����ġ�� ����� �����͸� �о��, ��������Ʈ�� �����.

            this.MakeMSPreconditionLinkedList();   // SkillNumber ���� ������ ����ġ�� ����� �����͸� �о��, Ư�� ��ų�� �ʿ��� skillNumber�� ����ġ LinkedList�� �����.
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

                this.CellNumberOrder.Clear();
                this.MSOrder.Clear();
                this.cellOrderToBeChanged.Clear();
            }
            // ���õ� �Ͱ�, ���Ӱ� ���õ� ���� �ٸ�. �� ��Ȱ��ȭ �� �� Ȱ��ȭ.
            else
            {
                this.beClickedCellNumber = skillUICellMSStruct.CellNumber;

                this.NotifyCellOrderToBeChangedObservers(ActivateOrNot.Not);  // �� ��Ȱ��ȭ

                this.CellNumberOrder.Clear();
                this.MSOrder.Clear();
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

        // CellNumber�� �������� ����ϴ� �׷����� ���� ���� ã��.
        private void FindCellNumberStartPosition()
        {
            for(int i=0; i < skillUICellStructs.Count; ++i)
            {
                if (skillUICellStructs[i].CellContent == CellContent.main && (skillUICellStructs[i].LineNumber == 4 || skillUICellStructs[i].LineNumber == 0))
                    this.cellStartPosition.Add(skillUICellStructs[i].CellNumber);
            }
        }
        // SkillNumber�� �������� ����ϴ� �׷����� ���� ���� ã��.
        private void FindCellMSStartPosition()
        {
            for (int i = 0; i < cellStartPosition.Count; ++i)
            {
                this.cellMSStartPosition.Add(this.skillUICellMSStructs[skillUICellMSStructs.FindIndex(x => x.CellNumber == cellStartPosition[i])].SkillNumber);
            }
        }

        // CellNumber ���� ������ ����� �����͸� �о��, ��������Ʈ�� �����.
        private void MakeSkillUICellLineLinkedList()
        {
            // CellNumber ���� ���� ������ ����� Local ������ �о����.
            TextAsset skillUICellLinePrecondition_TextAsset = Resources.Load<TextAsset>("GameSystem/SkillData/UI/SkillUICellPrecondition");
            JArray skillUICellLinePrecondition = JArray.Parse(skillUICellLinePrecondition_TextAsset.ToString());

            // adjacentCellNumberPreconditionStructs LinkedList ����.
            for (int i = 0; i < skillUICellStructs.Count; ++i)
            {
                LinkedList<SkillUICellNumberPreconditionStruct> perSkillUICellLine = new LinkedList<SkillUICellNumberPreconditionStruct>();
                this.adjacentCellNumberPreconditionStructs.Add(perSkillUICellLine);
            }

            // adjacentCellNumberPreconditionStructs ��������Ʈ ��ü ����.
            for (int i = 0; i < skillUICellLinePrecondition.Count; ++i)
            {
                this.adjacentCellNumberPreconditionStructs[(int)skillUICellLinePrecondition[i]["Precondition_p"]].AddLast(
                    new SkillUICellNumberPreconditionStruct(
                        precondition_q: (int)skillUICellLinePrecondition[i]["Precondition_q"]));
            }
        }
        // SkillNumber ���� ������ ����ġ�� ����� �����͸� �о��, ��������Ʈ�� �����.
        private void MakeSkillUICellMSLinkedList()
        {
            // SkillNumber ���� ���� ������ ����ġ�� ����� Local ������ �о����.
            TextAsset skillUICellMSPrecondition_TextAsset = Resources.Load<TextAsset>("GameSystem/SkillData/UI/SkillUICellMSPrecondition");
            JArray skillUICellMSPrecondition = JArray.Parse(skillUICellMSPrecondition_TextAsset.ToString());

            // adjacentMSPreconditionStructs LinkedList ����.
            for (int i = 0; i < this.skillUICellMSStructs.Count; ++i)
            {
                LinkedList<SkillUICellMSPreconditionStruct> perSkillUICellMS = new LinkedList<SkillUICellMSPreconditionStruct>();
                this.adjacentMSPreconditionStructs.Add(perSkillUICellMS);
            }

            // adjacentMSPreconditionStructs ��������Ʈ ��ü ����.
            for (int i = 0; i < skillUICellMSPrecondition.Count; ++i)
            {
                this.adjacentMSPreconditionStructs[(int)skillUICellMSPrecondition[i]["Precondition_p"]].AddLast(
                    new SkillUICellMSPreconditionStruct(
                        precondition_q: (int)skillUICellMSPrecondition[i]["Precondition_q"],
                        precondition_Weight: (int)skillUICellMSPrecondition[i]["Precondition_Weight"]));
            }
        }
        // SkillNumber ���� ������ ����ġ�� ����� �����͸� �о��, Ư�� ��ų�� �ʿ��� skillNumber�� ����ġ LinkedList�� �����.
        private void MakeMSPreconditionLinkedList()
        {
            // SkillNumber ���� ���� ������ ����ġ�� ����� Local ������ �о����.
            TextAsset skillUICellMSPrecondition_TextAsset = Resources.Load<TextAsset>("GameSystem/SkillData/UI/SkillUICellMSPrecondition");
            JArray skillUICellMSPrecondition = JArray.Parse(skillUICellMSPrecondition_TextAsset.ToString());

            // MSPreconditionStructs LinkedList ����.
            for (int i = 0; i < this.skillUICellMSStructs.Count; ++i)
            {
                LinkedList<SkillUICellMSPreconditionStruct> perSkillUICellMS = new LinkedList<SkillUICellMSPreconditionStruct>();
                this.MSPreconditionStructs.Add(perSkillUICellMS);
            }

            // MSPreconditionStructs ��������Ʈ ��ü ����.
            for (int i = 0; i < skillUICellMSPrecondition.Count; ++i)
            {
                this.MSPreconditionStructs[(int)skillUICellMSPrecondition[i]["Precondition_q"]].AddLast(
                    new SkillUICellMSPreconditionStruct(
                        precondition_q: (int)skillUICellMSPrecondition[i]["Precondition_p"],
                        precondition_Weight: (int)skillUICellMSPrecondition[i]["Precondition_Weight"]));
            }
        }

        // CellNumber �������� ����.
        private void CellUILineTopologySort(int destinationCellNumber)
        {
            List<int> visited = new List<int>();
            bool destinationIsVisited;

            for (int i = 0; i < this.cellStartPosition.Count; ++i)
            {
                List<int> order = new List<int>();
                visited = Enumerable.Repeat(0, skillUICellStructs.Count).ToList();      // ��� ������ ���� �湮����.
                destinationIsVisited = false;                                           // ��ǥ ���� �湮���� ��.

                // ���� cellNumber, �湮����, ������ �湮����, �湮���, �湮 ����.
                this.CellUILineDFS(cellStartPosition[i], ref destinationCellNumber, ref destinationIsVisited, ref visited, ref order);

                // ��ǥ���� ������ �湮�Ͽ��ٸ�,
                // '��ǥ ���� -> ... -> ���� ����'���� �Ǿ��ִ� ���� ������ �����´�.
                // �������� ���ƿ� �湮������ CellNumberOrder ��ü�� ����.
                if (destinationIsVisited) { this.CellNumberOrder.Add( Enumerable.Reverse(order).ToList()); }
            }
        }
        private void CellUILineDFS(int startCellNumber, ref int destinationCellNumber, ref bool destinationIsVisited, ref List<int> visited, ref List<int> order)
        {
            visited[startCellNumber] = 1;                       // ���� ���� �湮���� ǥ��.

            // ���� �湮�� ������, ��ǥ���� ������ �����ϴٸ�,
            if (startCellNumber == destinationCellNumber) 
            {
                destinationIsVisited = true;                    // ��ǥ ���� �湮���� = true;
                order.Add(startCellNumber);                     // ���� ���� ���.
                return;                                         // ���� �������� ���ư�.
            }

            // ���� �������� �� �� �ִ� �������� Ž��.
            foreach (var item in adjacentCellNumberPreconditionStructs[startCellNumber])
            {
                // ���� �湮�� ������ �湮������ ������ && ��ǥ�� �� ������ �湮�� ���� ������ Ȯ��.
                if (visited[item.Precondition_q] == 0 && !destinationIsVisited)
                {
                    // ���� ���� Ž��.
                    CellUILineDFS(item.Precondition_q, ref destinationCellNumber, ref destinationIsVisited, ref visited, ref order);
                }
            }

            // ���� ������ ���弱���� ��ǥ���� ������ �湮�� ���� �ִٸ�, ���� ������ ����Ѵ�.
            if (destinationIsVisited) { order.Add(startCellNumber); }
        }
        // SkillNumber �������� ����.
        private void CellUIMSTopologySort(SkillUICellMSStruct skillUICellMSStruct)
        {
            List<int> visited = new List<int>();
            bool destinationIsVisited;
            int destinationSkillNumber = skillUICellMSStruct.SkillNumber;

            for (int i = 0; i < this.cellMSStartPosition.Count; ++i)
            {
                List<int> order = new List<int>();
                visited = Enumerable.Repeat(0, skillUICellMSStructs.Count).ToList();    // ��� ������ ���� �湮����.
                destinationIsVisited = false;                                           // ��ǥ ���� �湮���� ��.

                // ���� cellNumber, �湮����, ������ �湮����, �湮���, �湮 ����.
                this.CellUIMSDFS(cellMSStartPosition[i], ref destinationSkillNumber, ref destinationIsVisited, ref visited, ref order);

                // ��ǥ���� ������ �湮�Ͽ��ٸ�,
                // '��ǥ ���� -> ... -> ���� ����'���� �Ǿ��ִ� ���� ������ �����´�.
                // �������� ���ƿ� �湮������ MSOrder ��ü�� ����.
                if (destinationIsVisited) { this.MSOrder.Add(Enumerable.Reverse(order).ToList()); }

                order.Clear();
            }
        }
        private void CellUIMSDFS(int startSkillNumber, ref int destinationSkillNumber, ref bool destinationIsVisited, ref List<int> visited, ref List<int> order)
        {
            visited[startSkillNumber] = 1;                      // ���� ���� �湮���� ǥ��.

            // ���� �湮�� ������, ��ǥ���� ������ �����ϴٸ�,
            if (startSkillNumber == destinationSkillNumber)
            {
                destinationIsVisited = true;                    // ��ǥ ���� �湮���� = true;
                order.Add(startSkillNumber);                    // ���� ���� ���.
                return;                                         // ���� �������� ���ư�.
            }

            // ���� �������� �� �� �ִ� �������� Ž��.
            foreach (var item in adjacentMSPreconditionStructs[startSkillNumber])
            {
                // ���� �湮�� ������ �湮������ ������ && ��ǥ�� �� ������ �湮�� ���� ������ Ȯ��.
                if (visited[item.Precondition_q] == 0 && !destinationIsVisited)
                {
                    // ���� ���� Ž��.
                    CellUIMSDFS(item.Precondition_q, ref destinationSkillNumber, ref destinationIsVisited, ref visited, ref order);
                }
            }

            // ���� ������ ���弱���� ��ǥ���� ������ �湮�� ���� �ִٸ�, ���� ������ ����Ѵ�.
            if (destinationIsVisited) { order.Add(startSkillNumber); }
        }

        private void ExcludeCellNumberToBeActivated()
        {
            List<List<KeyValuePair<int, bool>>> satisfyCondition = new List<List<KeyValuePair<int, bool>>>();

            for (int i = 0; i < MSOrder.Count; ++i)
            {
                // LinkedList ����.
                List<KeyValuePair<int, bool>> perKeyValuePair = new List<KeyValuePair<int, bool>>();
                satisfyCondition.Add(perKeyValuePair);

                for (int j = 0; j < MSOrder[i].Count-1; ++j)
                {
                    foreach(var item in adjacentMSPreconditionStructs[MSOrder[i][j]])  // // skillNumber�� ���� �񱳰� �̷������.
                    {
                        if (item.Precondition_q == MSOrder[i][j + 1])
                        {
                            if (playerSkillInformationStructs[MSOrder[i][j]].CurrentLevel >= item.Precondition_Weight)
                                satisfyCondition[i].Add(new KeyValuePair<int, bool>(skillUICellMSStructs[MSOrder[i][j]].CellNumber, true));
                            else satisfyCondition[i].Add(new KeyValuePair<int, bool>(skillUICellMSStructs[MSOrder[i][j]].CellNumber, false));
                        }
                    }
                }

                satisfyCondition[i].Add(new KeyValuePair<int, bool>(skillUICellMSStructs[MSOrder[i][MSOrder[i].Count-1]].CellNumber, false));
            }


            for (int i = 0; i < satisfyCondition.Count; ++i)
            {
                while (satisfyCondition[i][0].Value)
                {

                    while (true) // true�� �ݺ�, �� ���� �ٸ��� ������ ����.
                    {

                        if (satisfyCondition[i][1].Key == CellNumberOrder[i][0]) { break; }

                        CellNumberOrder[i].RemoveAt(0);
                    }

                    satisfyCondition[i].RemoveAt(0);
                }
            }
        }
        private void ExtractCellNumberToBeChanged()
        {
            int rowCount = skillUICellStructs.Count / 11;

            for (int i = 0; i < CellNumberOrder.Count; ++i)
            {
                List<int> temp = new List<int>();

                for(int j = 1; j < CellNumberOrder[i].Count; ++j)
                {
                    if(CellNumberOrder[i][j-1]/ rowCount == CellNumberOrder[i][j]/ rowCount)
                    {
                        for(int k = 0; k < CellNumberOrder[i][j] - CellNumberOrder[i][j - 1]; ++k)
                        {
                            temp.Add(CellNumberOrder[i][j - 1] + k);
                        }
                    }
                    else
                    {
                        for(int l = 0; l < (CellNumberOrder[i][j]/ rowCount) - (CellNumberOrder[i][j - 1]/ rowCount); ++l)
                        {
                            temp.Add(CellNumberOrder[i][j - 1] + rowCount * l);
                        }
                    }
                }

                // ���� ������ ���� ���߿� �߰�����.
                temp.Add(CellNumberOrder[i][CellNumberOrder[i].Count-1]);

                this.cellOrderToBeChanged.Add(temp);
            }
        }
    }
}