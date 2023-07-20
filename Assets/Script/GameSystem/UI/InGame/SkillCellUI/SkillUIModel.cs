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
        public void DecideActivateOrInActivateCell(SkillUICellMainSubStruct SkillUICellMainSubStruct);     // Ȱ��ȭ or ��Ȱ��ȭ
        public void LearnSkill(int skillNumber);

        public ref SkillTreeStruct SkillTreeStruct { get; }
        public ref List<SkillUICellMainSubStruct> SkillUICellMainSubStructs { get; }
        public ref List<SkillInformationStruct> SkillInformationStructs { get; }
        public LinkedList<SkillNumberPreconditionStruct> GetMainSubPreconditionStruct(int skillNumber);
    }

    public class SkillUIModel : MonoBehaviour, ISkillUIModel
    {
        private ISkillManagerForModel skillCellUIManager;

        // ������ ����Ǿ�� �� ������ Cell ����� ��ϵ� ���� �����Ѵ�.
        private List<ICellOrderToBeChangedObserverForView> cellOrderToBeChangedObservers;       // UI ������ ����Ǿ�� �� CellNumber�� �ʿ���ϴ� View ���.
        private List<List<int>> cellOrderToBeChanged;                                           // UI ������ ����Ǿ�� �� CellNumber ���.

        private List<IPlayerSkillInformationObserverForView> playerSkillInformationObservers;   // ����ڰ� ���� �н��� ��ų�� Level ������ �ʿ���ϴ� View ���.
        private List<PlayerSkillInformationStruct> playerSkillInformationStructs;               // ����ڰ� ���� �н��� ��ų�� Level ����.

        private SkillTreeStruct skillTreeStruct;                                     // SkillUICell�� �⺻ ������ ���� �ִ�.
        private List<SkillUICellMainSubStruct> skillUICellMainSubStructs;                            // SkillNumber�� CellNumber�� ���� �־�, �� SkillUICell GameObject���� �����ϴµ� ���ȴ�.

        private List<LinkedList<SkillUICellVertexAndWeightPreconditionStruct>> adjacentCellNumberStructs;    // SkillUICell.CellNumber �� ���� ������ ǥ���ϴ� ������ ��ϵ�.
        private List<LinkedList<SkillUICellVertexAndWeightPreconditionStruct>> adjacentSkillNumberAndWeightStructs;   // skillInformationStructs.SkillNumber �� ���� ������ ǥ���ϴ� ������ ��ϵ�.

        private List<LinkedList<SkillNumberPreconditionStruct>> skillNumberPreconditionStructs;     // List i ��° ��ų�� ���� ���� ��ų �� ����ġ�� ����ϴ� ����ü�̴�.

        private List<SkillInformationStruct> skillInformationStructs;                               // Skill�� �⺻ �����͸� ���� �ִ�.

        private List<int> cellNumberStartPosition;                                              // skillUICell ���̺� ��, ���ʷ� ���۵Ǵ� cell�� cellNumber
        private List<int> SkillNumberStartPosition;                                             // skillUICell ���̺� ��, ���ʷ� ���۵Ǵ� skill�� skillNumber

        private List<List<int>> cellNumberOrder;                                                // Ư�� cellNumber�� �����ϱ� ����, cellNumber ����.
        private List<List<int>> skillNumberOrder;                                               // Ư�� skillNumber�� �����ϱ� ����, skillNumber ����.

        private int beClickedCellNumber;                                                        // ���� Ŭ���� ��ų ���.

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

            this.skillTreeStruct = new SkillTreeStruct();
            this.skillUICellMainSubStructs = new List<SkillUICellMainSubStruct>();

            this.adjacentCellNumberStructs = new List<LinkedList<SkillUICellVertexAndWeightPreconditionStruct>>();
            this.adjacentSkillNumberAndWeightStructs = new List<LinkedList<SkillUICellVertexAndWeightPreconditionStruct>>();
            this.skillNumberPreconditionStructs = new List<LinkedList<SkillNumberPreconditionStruct>>();

            this.cellNumberStartPosition = new List<int>();
            this.SkillNumberStartPosition = new List<int>();

            this.cellNumberOrder = new List<List<int>>();
            this.skillNumberOrder = new List<List<int>>();

            this.beClickedCellNumber = -1;
        }

        // ISkillCellUIModel ����
        // Manager���� �ʿ��� ������ �������� + SkillModel �ʱ� ����.
        public void InitialSetting(ISkillManagerForModel skillCellUIManager)
        {
            this.skillCellUIManager = skillCellUIManager;

            this.skillTreeStruct = this.skillCellUIManager.SkillTreeStruct;
            this.skillUICellMainSubStructs = this.skillCellUIManager.SkillUICellMainSubStructs;
            this.skillInformationStructs = this.skillCellUIManager.SkillInformationStruct;
            this.playerSkillInformationStructs = this.skillCellUIManager.PlayerSkillInformationStruct;

            this.FindCellNumberStartPosition();         // CellNumber�� �������� ����ϴ� �׷����� ���� ���� ã��.
            this.FindSkillNumberStartPosition();             // SkillNumber�� �������� ����ϴ� �׷����� ���� ���� ã��.

            this.MakeAdjacentCellNumberLinkedList();       // CellNumber ���� ������ ����� �����͸� �о��, ��������Ʈ�� �����.
            this.MakeAdjacentSkillNumberAndWeightLinkedList();         // SkillNumber ���� ������ ����ġ�� ����� �����͸� �о��, ��������Ʈ�� �����.

            this.MakeSkillNumberPreconditionLinkedList();   // SkillNumber ���� ������ ����ġ�� ����� �����͸� �о��, Ư�� ��ų�� �ʿ��� skillNumber�� ����ġ LinkedList�� �����.
        }
        public void DecideActivateOrInActivateCell(SkillUICellMainSubStruct SkillUICellMainSubStruct)
        {
            // ���õ� ���� ������. �� Ȱ��ȭ.
            if (this.beClickedCellNumber == -1)
            {
                this.beClickedCellNumber = SkillUICellMainSubStruct.CellNumber;

                this.CellNumberTopologySort(this.beClickedCellNumber);
                this.SkillNumberTopologySort(SkillUICellMainSubStruct);
                this.ExcludeCellNumberOrderToBeActivated();
                this.ExtractCellNumberOrderToBeChanged();

                this.NotifyCellOrderToBeChangedObservers(ActivateOrNot.Activate);   // �� Ȱ��ȭ
            }
            // ���õ� �Ͱ�, ���Ӱ� ������ ���� ������. �� ��Ȱ��ȭ.
            else if (this.beClickedCellNumber == SkillUICellMainSubStruct.CellNumber)
            {
                this.beClickedCellNumber = -1;

                this.NotifyCellOrderToBeChangedObservers(ActivateOrNot.Not);  // �� ��Ȱ��ȭ

                this.cellNumberOrder.Clear();
                this.skillNumberOrder.Clear();
                this.cellOrderToBeChanged.Clear();
            }
            // ���õ� �Ͱ�, ���Ӱ� ���õ� ���� �ٸ�. �� ��Ȱ��ȭ �� �� Ȱ��ȭ.
            else
            {
                this.beClickedCellNumber = SkillUICellMainSubStruct.CellNumber;

                this.NotifyCellOrderToBeChangedObservers(ActivateOrNot.Not);  // �� ��Ȱ��ȭ

                this.cellNumberOrder.Clear();
                this.skillNumberOrder.Clear();
                this.cellOrderToBeChanged.Clear();

                this.CellNumberTopologySort(this.beClickedCellNumber);
                this.SkillNumberTopologySort(SkillUICellMainSubStruct);
                this.ExcludeCellNumberOrderToBeActivated();
                this.ExtractCellNumberOrderToBeChanged();

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
        public ref SkillTreeStruct SkillTreeStruct { get { return ref this.skillTreeStruct; } }
        public ref List<SkillUICellMainSubStruct> SkillUICellMainSubStructs { get { return ref this.skillUICellMainSubStructs; } }
        public ref List<SkillInformationStruct> SkillInformationStructs { get { return ref this.skillInformationStructs; } }

        public LinkedList<SkillNumberPreconditionStruct> GetMainSubPreconditionStruct(int skillNumber) { return this.skillNumberPreconditionStructs[skillNumber]; }
        // -------------------------

        // CellNumber�� �������� ����ϴ� �׷����� ���� ���� ã��.
        private void FindCellNumberStartPosition()
        {
            for(int i=0; i < this.skillTreeStruct.SkillUICellInformation.Count; ++i)
            {
                if (this.skillTreeStruct.SkillUICellInformation[i].CellContent == CellContent.main && (this.skillTreeStruct.SkillUICellInformation[i].LineNumber == 4 || this.skillTreeStruct.SkillUICellInformation[i].LineNumber == 0))
                    this.cellNumberStartPosition.Add(this.skillTreeStruct.SkillUICellInformation[i].CellNumber);
            }
        }
        // SkillNumber�� �������� ����ϴ� �׷����� ���� ���� ã��.
        private void FindSkillNumberStartPosition()
        {
            for (int i = 0; i < cellNumberStartPosition.Count; ++i)
            {
                this.SkillNumberStartPosition.Add(this.skillUICellMainSubStructs[skillUICellMainSubStructs.FindIndex(x => x.CellNumber == cellNumberStartPosition[i])].SkillNumber);
            }
        }

        // CellNumber ���� ������ ����� �����͸� �о��, ��������Ʈ�� �����.
        private void MakeAdjacentCellNumberLinkedList()
        {
            // CellNumber ���� ���� ������ ����� Local ������ �о����.
            TextAsset skillUICellLinePrecondition_TextAsset = Resources.Load<TextAsset>("GameSystem/SkillData/UI/CellNumberPrecondition");
            JArray skillUICellLinePrecondition = JArray.Parse(skillUICellLinePrecondition_TextAsset.ToString());

            // adjacentCellNumberStructs LinkedList ����.
            for (int i = 0; i < this.skillTreeStruct.SkillUICellInformation.Count; ++i)
            {
                LinkedList<SkillUICellVertexAndWeightPreconditionStruct> perSkillUICellLine = new LinkedList<SkillUICellVertexAndWeightPreconditionStruct>();
                this.adjacentCellNumberStructs.Add(perSkillUICellLine);
            }

            // adjacentCellNumberStructs ��������Ʈ ��ü ����.
            for (int i = 0; i < skillUICellLinePrecondition.Count; ++i)
            {
                this.adjacentCellNumberStructs[(int)skillUICellLinePrecondition[i]["CurrentVertex"]].AddLast(
                    new SkillUICellVertexAndWeightPreconditionStruct(
                        nextVertex : (int)skillUICellLinePrecondition[i]["NextVertex"]));
            }
        }
        // SkillNumber ���� ������ ����ġ�� ����� �����͸� �о��, ��������Ʈ�� �����.
        private void MakeAdjacentSkillNumberAndWeightLinkedList()
        {
            // SkillNumber ���� ���� ������ ����ġ�� ����� Local ������ �о����.
            TextAsset skillUICellMSPrecondition_TextAsset = Resources.Load<TextAsset>("GameSystem/SkillData/UI/SkillNumberPrecondition");
            JArray skillUICellMSPrecondition = JArray.Parse(skillUICellMSPrecondition_TextAsset.ToString());

            // adjacentSkillNumberAndWeightStructs LinkedList ����.
            for (int i = 0; i < this.skillUICellMainSubStructs.Count; ++i)
            {
                LinkedList<SkillUICellVertexAndWeightPreconditionStruct> perSkillUICellMS = new LinkedList<SkillUICellVertexAndWeightPreconditionStruct>();
                this.adjacentSkillNumberAndWeightStructs.Add(perSkillUICellMS);
            }

            // adjacentSkillNumberAndWeightStructs ��������Ʈ ��ü ����.
            for (int i = 0; i < skillUICellMSPrecondition.Count; ++i)
            {
                this.adjacentSkillNumberAndWeightStructs[(int)skillUICellMSPrecondition[i]["CurrentVertex"]].AddLast(
                    new SkillUICellVertexAndWeightPreconditionStruct(
                        nextVertex: (int)skillUICellMSPrecondition[i]["NextVertex"],
                        preVertex_weight: (int)skillUICellMSPrecondition[i]["CurrentVertex_Weight"]));
            }
        }
        // SkillNumber ���� ������ ����ġ�� ����� �����͸� �о��, Ư�� ��ų�� �ʿ��� skillNumber�� ����ġ LinkedList�� �����.
        private void MakeSkillNumberPreconditionLinkedList()
        {
            // SkillNumber ���� ���� ������ ����ġ�� ����� Local ������ �о����.
            TextAsset skillUICellMainSubPrecondition_TextAsset = Resources.Load<TextAsset>("GameSystem/SkillData/UI/SkillNumberPrecondition");
            JArray skillUICellMainSubPrecondition = JArray.Parse(skillUICellMainSubPrecondition_TextAsset.ToString());

            // skillNumberPreconditionStructs LinkedList ����.
            for (int i = 0; i < this.skillUICellMainSubStructs.Count; ++i)
            {
                LinkedList<SkillNumberPreconditionStruct> perSkillUICellMS = new LinkedList<SkillNumberPreconditionStruct>();
                this.skillNumberPreconditionStructs.Add(perSkillUICellMS);
            }

            // skillNumberPreconditionStructs ��������Ʈ ��ü ����.
            for (int i = 0; i < skillUICellMainSubPrecondition.Count; ++i)
            {
                this.skillNumberPreconditionStructs[(int)skillUICellMainSubPrecondition[i]["NextVertex"]].AddLast(
                    new SkillNumberPreconditionStruct(
                        preVertex: (int)skillUICellMainSubPrecondition[i]["CurrentVertex"],
                        preVertex_weight: (int)skillUICellMainSubPrecondition[i]["CurrentVertex_Weight"]));
            }
        }

        // CellNumber �������� ����.
        private void CellNumberTopologySort(int destinationCellNumber)
        {
            List<int> visited = new List<int>();
            bool destinationIsVisited;

            for (int i = 0; i < this.cellNumberStartPosition.Count; ++i)
            {
                List<int> order = new List<int>();
                visited = Enumerable.Repeat(0, this.skillTreeStruct.SkillUICellInformation.Count).ToList();      // ��� ������ ���� �湮����.
                destinationIsVisited = false;                                           // ��ǥ ���� �湮���� ��.

                // ���� cellNumber, �湮����, ������ �湮����, �湮���, �湮 ����.
                this.CellNumberDFS(cellNumberStartPosition[i], ref destinationCellNumber, ref destinationIsVisited, ref visited, ref order);

                // ��ǥ���� ������ �湮�Ͽ��ٸ�,
                // '��ǥ ���� -> ... -> ���� ����'���� �Ǿ��ִ� ���� ������ �����´�.
                // �������� ���ƿ� �湮������ cellNumberOrder ��ü�� ����.
                if (destinationIsVisited) { this.cellNumberOrder.Add( Enumerable.Reverse(order).ToList()); }
            }
        }
        // CellNumber DFS ����
        private void CellNumberDFS(int startCellNumber, ref int destinationCellNumber, ref bool destinationIsVisited, ref List<int> visited, ref List<int> order)
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
            foreach (var item in adjacentCellNumberStructs[startCellNumber])
            {
                // ���� �湮�� ������ �湮������ ������ && ��ǥ�� �� ������ �湮�� ���� ������ Ȯ��.
                if (visited[item.NextVertex] == 0 && !destinationIsVisited)
                {
                    // ���� ���� Ž��.
                    CellNumberDFS(item.NextVertex, ref destinationCellNumber, ref destinationIsVisited, ref visited, ref order);
                }
            }

            // ���� ������ ���弱���� ��ǥ���� ������ �湮�� ���� �ִٸ�, ���� ������ ����Ѵ�.
            if (destinationIsVisited) { order.Add(startCellNumber); }
        }
        // SkillNumber �������� ����.
        private void SkillNumberTopologySort(SkillUICellMainSubStruct SkillUICellMainSubStruct)
        {
            List<int> visited = new List<int>();
            bool destinationIsVisited;
            int destinationSkillNumber = SkillUICellMainSubStruct.SkillNumber;

            for (int i = 0; i < this.SkillNumberStartPosition.Count; ++i)
            {
                List<int> order = new List<int>();
                visited = Enumerable.Repeat(0, skillUICellMainSubStructs.Count).ToList();    // ��� ������ ���� �湮����.
                destinationIsVisited = false;                                           // ��ǥ ���� �湮���� ��.

                // ���� cellNumber, �湮����, ������ �湮����, �湮���, �湮 ����.
                this.SkillNumberDFS(SkillNumberStartPosition[i], ref destinationSkillNumber, ref destinationIsVisited, ref visited, ref order);

                // ��ǥ���� ������ �湮�Ͽ��ٸ�,
                // '��ǥ ���� -> ... -> ���� ����'���� �Ǿ��ִ� ���� ������ �����´�.
                // �������� ���ƿ� �湮������ skillNumberOrder ��ü�� ����.
                if (destinationIsVisited) { this.skillNumberOrder.Add(Enumerable.Reverse(order).ToList()); }

                order.Clear();
            }
        }
        // Skillumber DFS ����
        private void SkillNumberDFS(int startSkillNumber, ref int destinationSkillNumber, ref bool destinationIsVisited, ref List<int> visited, ref List<int> order)
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
            foreach (var item in adjacentSkillNumberAndWeightStructs[startSkillNumber])
            {
                // ���� �湮�� ������ �湮������ ������ && ��ǥ�� �� ������ �湮�� ���� ������ Ȯ��.
                if (visited[item.NextVertex] == 0 && !destinationIsVisited)
                {
                    // ���� ���� Ž��.
                    SkillNumberDFS(item.NextVertex, ref destinationSkillNumber, ref destinationIsVisited, ref visited, ref order);

                    // ���� ������ ���弱���� ��ǥ���� ������ �湮�� ���� ������,
                    // ���� SkillNumber�� ��ų ������, �� ���� SkillNumber�� ��ų ���� ������ �������� ���ߴٸ� = True
                    if (destinationIsVisited && (this.playerSkillInformationStructs[startSkillNumber].CurrentLevel < item.PreVertex_weight)) { order.Add(startSkillNumber); }
                }
            }
        }
        // ������ ������ ��ų���� UI ���� ǥ�ø� ���� �ʵ��� ��ȹ�Ͽ���.
        // SkillOrder���� ������ ������ SkillNumber���� ��ϵ��� �ʴ´�.
        // ���� SkillOrder�� skillUICellMainSubStructs���� �̿��Ͽ�, UI ���� ��ų �ʿ䰡 ���� CellNumberOrder�� ���� �� �ִ�.
        private void ExcludeCellNumberOrderToBeActivated()
        {
            for(int i = 0; i < this.cellNumberOrder.Count; ++i)
            {
                for(int j =0; j < this.cellNumberOrder[i].Count; ++j)
                {
                    // SkillNumberOrder�� ���۵Ǵ� ���������� CellNumber�� ��� ����.
                    if(this.cellNumberOrder[i][j] == this.skillUICellMainSubStructs[this.skillNumberOrder[i][0]].CellNumber)
                    {
                        this.cellNumberOrder[i].RemoveRange(0, j);
                        break;
                    }
                }
            }
        }
        // CellContent�� Main, Sub, Interchange Cell�� ���´�. ���� CellContent�� Non�� Line�� ���� Cell�� ������� ���Ѵ�.
        // ��, Ȱ��ȭ ��Ű�� ���� Cell�� ������ �����.
        // ����, ���� Cell���� CellNumber�� CellNumberOrder�� �־��ִ� ������ ��ģ��.
        private void ExtractCellNumberOrderToBeChanged()
        {
            for (int i = 0; i < cellNumberOrder.Count; ++i)
            {
                List<int> temp = new List<int>();

                for(int j = 1; j < cellNumberOrder[i].Count; ++j)
                {
                    if(cellNumberOrder[i][j-1]/ this.skillTreeStruct.ColumnCount == cellNumberOrder[i][j]/ this.skillTreeStruct.ColumnCount)
                    {
                        for(int k = 0; k < cellNumberOrder[i][j] - cellNumberOrder[i][j - 1]; ++k)
                        {
                            temp.Add(cellNumberOrder[i][j - 1] + k);
                        }
                    }
                    else
                    {
                        for(int l = 0; l < (cellNumberOrder[i][j]/ this.skillTreeStruct.ColumnCount) - (cellNumberOrder[i][j - 1]/ this.skillTreeStruct.ColumnCount); ++l)
                        {
                            temp.Add(cellNumberOrder[i][j - 1] + this.skillTreeStruct.ColumnCount * l);
                        }
                    }
                }

                // ���� ������ ���� ���߿� �߰�����.
                temp.Add(cellNumberOrder[i][cellNumberOrder[i].Count-1]);

                this.cellOrderToBeChanged.Add(temp);
            }
        }
    }
}