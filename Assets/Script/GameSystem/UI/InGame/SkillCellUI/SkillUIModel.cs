using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using UnityEngine;

namespace GameSystem.InGameUI.Skill
{
    public interface ICellOrderToBeChangedObserverForView
    {
        // 활성화/비활성화 작업인지를 매개변수로 알려준다.
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
        public void DecideActivateOrInActivateCell(SkillUICellMainSubStruct SkillUICellMainSubStruct);     // 활성화 or 비활성화
        public void LearnSkill(int skillNumber);

        public ref List<SkillUICellStruct> SkillUICellStructs { get; }
        public ref List<SkillUICellMainSubStruct> SkillUICellMainSubStructs { get; }
        public ref List<SkillInformationStruct> SkillInformationStructs { get; }
        public LinkedList<SkillNumberPreconditionStruct> GetMainSubPreconditionStruct(int skillNumber);
    }

    public class SkillUIModel : MonoBehaviour, ISkillUIModel
    {
        private ISkillManagerForModel skillCellUIManager;

        // 변경이 수행되어야 할 라인의 Cell 목록이 기록된 값을 전달한다.
        private List<ICellOrderToBeChangedObserverForView> cellOrderToBeChangedObservers;       // UI 강조가 수행되어야 할 CellNumber를 필요로하는 View 목록.
        private List<List<int>> cellOrderToBeChanged;                                           // UI 강조가 수행되어야 할 CellNumber 목록.

        private List<IPlayerSkillInformationObserverForView> playerSkillInformationObservers;   // 사용자가 현재 학습한 스킬의 Level 정보를 필요로하는 View 목록.
        private List<PlayerSkillInformationStruct> playerSkillInformationStructs;               // 사용자가 현재 학습한 스킬의 Level 정보.

        private List<SkillUICellStruct> skillUICellStructs;                                     // SkillUICell의 기본 정보를 갖고 있다.
        private List<SkillUICellMainSubStruct> skillUICellMainSubStructs;                            // SkillNumber와 CellNumber를 갖고 있어, 각 SkillUICell GameObject들을 구분하는데 사용된다.
        private List<SkillInformationStruct> skillInformationStructs;                               // Skill의 기본 데이터를 갖고 있다.

        private List<LinkedList<SkillUICellVertexAndWeightPreconditionStruct>> adjacentCellNumberStructs;    // SkillUICell.CellNumber 들 간의 순서를 표현하는 정보가 기록됨.
        private List<LinkedList<SkillUICellVertexAndWeightPreconditionStruct>> adjacentSkillNumberAndWeightStructs;   // skillInformationStructs.SkillNumber 들 간의 순서를 표현하는 정보가 기록됨.

        private List<LinkedList<SkillNumberPreconditionStruct>> skillNumberPreconditionStructs;     // List i 번째 스킬에 대한 관련 스킬 및 가중치를 기록하는 구조체이다.

        private List<int> cellNumberStartPosition;                                              // skillUICell 테이블 중, 최초로 시작되는 cell의 cellNumber
        private List<int> SkillNumberStartPosition;                                             // skillUICell 테이블 중, 최초로 시작되는 skill의 skillNumber

        private List<List<int>> cellNumberOrder;                                                // 특정 cellNumber에 도착하기 위한, cellNumber 순서.
        private List<List<int>> skillNumberOrder;                                               // 특정 skillNumber에 도착하기 위한, skillNumber 순서.

        private int beClickedCellNumber;                                                        // 현재 클릭된 스킬 명시.

        // ICellOrderToBeChangedObserver 구현
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
        // CellOrderToBeChanged 값이 2가지 방식으로 사용되어야 하므로,
        // View를 호출하는 메서드의 매개변수로 사용방식 activateOrNot 명시해준다.
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

        // IPlayerSkillInformationObserver 구현
        public void RegisterPlayerSkillInformationObserver(IPlayerSkillInformationObserverForView observer)
        {
            this.playerSkillInformationObservers.Add(observer);
        }
        public void RemovePlayerSkillInformationObserver(IPlayerSkillInformationObserverForView observer)
        {
            this.playerSkillInformationObservers.Remove(observer);
        }
        // 자신의 SkillNumber 값을 알려주면, skillNumber와 관련된 playerSkillInformaionStructs 정보를 전달한다.
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

        // SkillUIModel 멤버 정의 및 초기값 입력.
        private void Awake()
        {
            this.cellOrderToBeChangedObservers = new List<ICellOrderToBeChangedObserverForView>();
            this.cellOrderToBeChanged = new List<List<int>>();
            this.playerSkillInformationObservers = new List<IPlayerSkillInformationObserverForView>();

            this.skillUICellStructs = new List<SkillUICellStruct>();
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

        // SkillModel 초기 설정.
        public void InitialSetting(ISkillManagerForModel skillCellUIManager)
        {
            this.skillCellUIManager = skillCellUIManager;

            // Manager 객체에서 필요한 데이터 가져오기.
            this.skillUICellStructs = this.skillCellUIManager.SkillUICellStructs;
            this.skillUICellMainSubStructs = this.skillCellUIManager.SkillUICellMainSubStructs;
            this.skillInformationStructs = this.skillCellUIManager.SkillInformationStruct;
            this.playerSkillInformationStructs = this.skillCellUIManager.PlayerSkillInformationStruct;

            this.FindCellNumberStartPosition();         // CellNumber를 정점으로 사용하는 그래프의 시작 정점 찾기.
            this.FindSkillNumberStartPosition();             // SkillNumber를 정점으로 사용하는 그래프의 시작 정점 찾기.

            this.MakeAdjacentCellNumberLinkedList();       // CellNumber 간에 순서를 기록한 데이터를 읽어와, 인접리스트를 만든다.
            this.MakeAdjacentSkillNumberAndWeightLinkedList();         // SkillNumber 간에 순서와 가중치를 기록한 데이터를 읽어와, 인접리스트를 만든다.

            this.MakeMainSubPreconditionLinkedList();   // SkillNumber 간에 순서와 가중치를 기록한 데이터를 읽어와, 특정 스킬에 필요한 skillNumber와 가중치 LinkedList를 만든다.
        }
        public void DecideActivateOrInActivateCell(SkillUICellMainSubStruct SkillUICellMainSubStruct)
        {
            // 선택된 것이 없었음. 선 활성화.
            if (this.beClickedCellNumber == -1)
            {
                this.beClickedCellNumber = SkillUICellMainSubStruct.CellNumber;

                // 선 활성화 로직.
                this.CellNumberTopologySort(this.beClickedCellNumber);
                this.SkillNumberTopologySort(SkillUICellMainSubStruct);
                this.ExcludeCellNumberOrderToBeActivated();
                this.ExtractCellNumberOrderToBeChanged();

                this.NotifyCellOrderToBeChangedObservers(ActivateOrNot.Activate);   // View들에게 공지.
            }
            // 선택된 것과, 새롭게 선택한 것이 동일함. 선 비활성화.
            else if (this.beClickedCellNumber == SkillUICellMainSubStruct.CellNumber)
            {
                this.beClickedCellNumber = -1;

                // 선 비활성화 로직
                this.NotifyCellOrderToBeChangedObservers(ActivateOrNot.Not);        // 활성화 되어 있는 View 비활성화 공지.

                this.cellNumberOrder.Clear();
                this.skillNumberOrder.Clear();
                this.cellOrderToBeChanged.Clear();
            }
            // 선택된 것과, 새롭게 선택된 것이 다름. 선 비활성화 후 선 활성화.
            else
            {
                this.beClickedCellNumber = SkillUICellMainSubStruct.CellNumber;

                // 선 비활성화 로직
                this.NotifyCellOrderToBeChangedObservers(ActivateOrNot.Not);  // 활성화 되어 있는 View 비활성화 공지.

                this.cellNumberOrder.Clear();
                this.skillNumberOrder.Clear();
                this.cellOrderToBeChanged.Clear();

                // 선 활성화 로직
                this.CellNumberTopologySort(this.beClickedCellNumber);
                this.SkillNumberTopologySort(SkillUICellMainSubStruct);
                this.ExcludeCellNumberOrderToBeActivated();
                this.ExtractCellNumberOrderToBeChanged();

                this.NotifyCellOrderToBeChangedObservers(ActivateOrNot.Activate);    // View들에게 공지.
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
                Debug.Log("Mana 부족");
                // error 메시지.
            }

        }
        public ref List<SkillUICellStruct> SkillUICellStructs { get { return ref this.skillUICellStructs; } }
        public ref List<SkillUICellMainSubStruct> SkillUICellMainSubStructs { get { return ref this.skillUICellMainSubStructs; } }
        public ref List<SkillInformationStruct> SkillInformationStructs { get { return ref this.skillInformationStructs; } }

        public LinkedList<SkillNumberPreconditionStruct> GetMainSubPreconditionStruct(int skillNumber) { return this.skillNumberPreconditionStructs[skillNumber]; }
        // -------------------------

        // CellNumber를 정점으로 사용하는 그래프의 시작 정점 찾기.
        private void FindCellNumberStartPosition()
        {
            for(int i=0; i < skillUICellStructs.Count; ++i)
            {
                if (skillUICellStructs[i].CellContent == CellContent.main && (skillUICellStructs[i].LineNumber == 4 || skillUICellStructs[i].LineNumber == 0))
                    this.cellNumberStartPosition.Add(skillUICellStructs[i].CellNumber);
            }
        }
        // SkillNumber를 정점으로 사용하는 그래프의 시작 정점 찾기.
        private void FindSkillNumberStartPosition()
        {
            for (int i = 0; i < cellNumberStartPosition.Count; ++i)
            {
                this.SkillNumberStartPosition.Add(this.skillUICellMainSubStructs[skillUICellMainSubStructs.FindIndex(x => x.CellNumber == cellNumberStartPosition[i])].SkillNumber);
            }
        }

        // CellNumber 간에 순서를 기록한 데이터를 읽어와, 인접리스트를 만든다.
        private void MakeAdjacentCellNumberLinkedList()
        {
            // CellNumber 정점 간의 순서를 기록한 Local 데이터 읽어오기.
            TextAsset skillUICellLinePrecondition_TextAsset = Resources.Load<TextAsset>("GameSystem/SkillData/UI/CellNumberPrecondition");
            JArray skillUICellLinePrecondition = JArray.Parse(skillUICellLinePrecondition_TextAsset.ToString());

            // adjacentCellNumberStructs LinkedList 정의.
            for (int i = 0; i < skillUICellStructs.Count; ++i)
            {
                LinkedList<SkillUICellVertexAndWeightPreconditionStruct> perSkillUICellLine = new LinkedList<SkillUICellVertexAndWeightPreconditionStruct>();
                this.adjacentCellNumberStructs.Add(perSkillUICellLine);
            }

            // adjacentCellNumberStructs 인접리스트 객체 생성.
            for (int i = 0; i < skillUICellLinePrecondition.Count; ++i)
            {
                this.adjacentCellNumberStructs[(int)skillUICellLinePrecondition[i]["CurrentVertex"]].AddLast(
                    new SkillUICellVertexAndWeightPreconditionStruct(
                        nextVertex : (int)skillUICellLinePrecondition[i]["NextVertex"]));
            }
        }
        // SkillNumber 간에 순서와 가중치를 기록한 데이터를 읽어와, 인접리스트를 만든다.
        private void MakeAdjacentSkillNumberAndWeightLinkedList()
        {
            // SkillNumber 정점 간의 순서와 가중치를 기록한 Local 데이터 읽어오기.
            TextAsset skillUICellMSPrecondition_TextAsset = Resources.Load<TextAsset>("GameSystem/SkillData/UI/SkillNumberPrecondition");
            JArray skillUICellMSPrecondition = JArray.Parse(skillUICellMSPrecondition_TextAsset.ToString());

            // adjacentSkillNumberAndWeightStructs LinkedList 정의.
            for (int i = 0; i < this.skillUICellMainSubStructs.Count; ++i)
            {
                LinkedList<SkillUICellVertexAndWeightPreconditionStruct> perSkillUICellMS = new LinkedList<SkillUICellVertexAndWeightPreconditionStruct>();
                this.adjacentSkillNumberAndWeightStructs.Add(perSkillUICellMS);
            }

            // adjacentSkillNumberAndWeightStructs 인접리스트 객체 생성.
            for (int i = 0; i < skillUICellMSPrecondition.Count; ++i)
            {
                this.adjacentSkillNumberAndWeightStructs[(int)skillUICellMSPrecondition[i]["CurrentVertex"]].AddLast(
                    new SkillUICellVertexAndWeightPreconditionStruct(
                        nextVertex: (int)skillUICellMSPrecondition[i]["NextVertex"],
                        preVertex_weight: (int)skillUICellMSPrecondition[i]["CurrentVertex_Weight"]));
            }
        }
        // SkillNumber 간에 순서와 가중치를 기록한 데이터를 읽어와, 특정 스킬에 필요한 skillNumber와 가중치 LinkedList를 만든다.
        private void MakeMainSubPreconditionLinkedList()
        {
            // SkillNumber 정점 간의 순서와 가중치를 기록한 Local 데이터 읽어오기.
            TextAsset skillUICellMainSubPrecondition_TextAsset = Resources.Load<TextAsset>("GameSystem/SkillData/UI/SkillNumberPrecondition");
            JArray skillUICellMainSubPrecondition = JArray.Parse(skillUICellMainSubPrecondition_TextAsset.ToString());

            // skillNumberPreconditionStructs LinkedList 정의.
            for (int i = 0; i < this.skillUICellMainSubStructs.Count; ++i)
            {
                LinkedList<SkillNumberPreconditionStruct> perSkillUICellMS = new LinkedList<SkillNumberPreconditionStruct>();
                this.skillNumberPreconditionStructs.Add(perSkillUICellMS);
            }

            // skillNumberPreconditionStructs 인접리스트 객체 생성.
            for (int i = 0; i < skillUICellMainSubPrecondition.Count; ++i)
            {
                this.skillNumberPreconditionStructs[(int)skillUICellMainSubPrecondition[i]["NextVertex"]].AddLast(
                    new SkillNumberPreconditionStruct(
                        preVertex: (int)skillUICellMainSubPrecondition[i]["CurrentVertex"],
                        preVertex_weight: (int)skillUICellMainSubPrecondition[i]["CurrentVertex_Weight"]));
            }
        }

        // CellNumber 위상정렬 시작.
        private void CellNumberTopologySort(int destinationCellNumber)
        {
            List<int> visited = new List<int>();
            bool destinationIsVisited;

            for (int i = 0; i < this.cellNumberStartPosition.Count; ++i)
            {
                List<int> order = new List<int>();
                visited = Enumerable.Repeat(0, skillUICellStructs.Count).ToList();      // 모든 정점에 대한 방문여부.
                destinationIsVisited = false;                                           // 목표 정점 방문여부 값.

                // 시작 cellNumber, 방문지점, 목적지 방문여부, 방문기록, 방문 순서.
                this.CellNumberDFS(cellNumberStartPosition[i], ref destinationCellNumber, ref destinationIsVisited, ref visited, ref order);

                // 목표로한 정점을 방문하였다면,
                // '목표 정점 -> ... -> 시작 정점'으로 되어있는 정점 순서를 뒤집는다.
                // 정상으로 돌아온 방문순서를 cellNumberOrder 객체에 저장.
                if (destinationIsVisited) { this.cellNumberOrder.Add( Enumerable.Reverse(order).ToList()); }
            }
        }
        // CellNumber DFS 시작
        private void CellNumberDFS(int startCellNumber, ref int destinationCellNumber, ref bool destinationIsVisited, ref List<int> visited, ref List<int> order)
        {
            visited[startCellNumber] = 1;                       // 현재 정점 방문여부 표시.

            // 현재 방문한 정점이, 목표로한 정점과 동이하다면,
            if (startCellNumber == destinationCellNumber) 
            {
                destinationIsVisited = true;                    // 목표 정점 방문여부 = true;
                order.Add(startCellNumber);                     // 현재 정점 기록.
                return;                                         // 이전 정점으로 돌아감.
            }

            // 현재 정점에서 갈 수 있는 정점들을 탐색.
            foreach (var item in adjacentCellNumberStructs[startCellNumber])
            {
                // 다음 방문할 정점이 방문한적이 없으며 && 목표로 한 지점을 방문한 적이 없는지 확인.
                if (visited[item.NextVertex] == 0 && !destinationIsVisited)
                {
                    // 다음 정점 탐색.
                    CellNumberDFS(item.NextVertex, ref destinationCellNumber, ref destinationIsVisited, ref visited, ref order);
                }
            }

            // 현재 정점의 연장선에서 목표로한 지점을 방문한 적이 있다면, 현재 정점을 기록한다.
            if (destinationIsVisited) { order.Add(startCellNumber); }
        }
        // SkillNumber 위상정렬 시작.
        private void SkillNumberTopologySort(SkillUICellMainSubStruct SkillUICellMainSubStruct)
        {
            List<int> visited = new List<int>();
            bool destinationIsVisited;
            int destinationSkillNumber = SkillUICellMainSubStruct.SkillNumber;

            for (int i = 0; i < this.SkillNumberStartPosition.Count; ++i)
            {
                List<int> order = new List<int>();
                visited = Enumerable.Repeat(0, skillUICellMainSubStructs.Count).ToList();    // 모든 정점에 대한 방문여부.
                destinationIsVisited = false;                                           // 목표 정점 방문여부 값.

                // 시작 cellNumber, 방문지점, 목적지 방문여부, 방문기록, 방문 순서.
                this.SkillNumberDFS(SkillNumberStartPosition[i], ref destinationSkillNumber, ref destinationIsVisited, ref visited, ref order);

                // 목표로한 정점을 방문하였다면,
                // '목표 정점 -> ... -> 시작 정점'으로 되어있는 정점 순서를 뒤집는다.
                // 정상으로 돌아온 방문순서를 skillNumberOrder 객체에 저장.
                if (destinationIsVisited) { this.skillNumberOrder.Add(Enumerable.Reverse(order).ToList()); }

                order.Clear();
            }
        }
        // Skillumber DFS 시작
        private void SkillNumberDFS(int startSkillNumber, ref int destinationSkillNumber, ref bool destinationIsVisited, ref List<int> visited, ref List<int> order)
        {
            visited[startSkillNumber] = 1;                      // 현재 정점 방문여부 표시.

            // 현재 방문한 정점이, 목표로한 정점과 동이하다면,
            if (startSkillNumber == destinationSkillNumber)
            {
                destinationIsVisited = true;                    // 목표 정점 방문여부 = true;
                order.Add(startSkillNumber);                    // 현재 정점 기록.
                return;                                         // 이전 정점으로 돌아감.
            }

            // 현재 정점에서 갈 수 있는 정점들을 탐색.
            foreach (var item in adjacentSkillNumberAndWeightStructs[startSkillNumber])
            {
                // 다음 방문할 정점이 방문한적이 없으며 && 목표로 한 지점을 방문한 적이 없는지 확인.
                if (visited[item.NextVertex] == 0 && !destinationIsVisited)
                {
                    // 다음 정점 탐색.
                    SkillNumberDFS(item.NextVertex, ref destinationSkillNumber, ref destinationIsVisited, ref visited, ref order);

                    // 현재 정점의 연장선에서 목표로한 지점을 방문한 적이 있으며,
                    // 현재 SkillNumber의 스킬 레벨이, 이 다음 SkillNumber의 스킬 레벨 조건을 만족하지 못했다면 = True
                    if (destinationIsVisited && (this.playerSkillInformationStructs[startSkillNumber].CurrentLevel < item.PreVertex_weight)) { order.Add(startSkillNumber); }
                }
            }
        }
        // 유저가 습득한 스킬들은 UI 강조 표시를 하지 않도록 기획하였다.
        // SkillOrder에는 유저가 습득한 SkillNumber들은 기록되지 않는다.
        // 따라서 SkillOrder와 skillUICellMainSubStructs값을 이용하여, UI 강조 시킬 필요가 없는 CellNumberOrder를 구할 수 있다.
        private void ExcludeCellNumberOrderToBeActivated()
        {
            for(int i = 0; i < this.cellNumberOrder.Count; ++i)
            {
                for(int j =0; j < this.cellNumberOrder[i].Count; ++j)
                {
                    // SkillNumberOrder가 시작되는 지점이전의 CellNumber는 모두 삭제.
                    if(this.cellNumberOrder[i][j] == this.skillUICellMainSubStructs[this.skillNumberOrder[i][0]].CellNumber)
                    {
                        this.cellNumberOrder[i].RemoveRange(0, j);
                        break;
                    }
                }
            }
        }
        // CellContent가 Main, Sub, Interchange Cell만 갖는다. 따라서 CellContent가 Non과 Line인 값의 Cell을 명시하지 못한다.
        // 즉, 활설화 시키고 싶은 Cell에 공백이 생긴다.
        // 따라서, 빠진 Cell들의 CellNumber를 CellNumberOrder에 넣어주는 과정을 거친다.
        private void ExtractCellNumberOrderToBeChanged()
        {
            int rowCount = this.skillUICellStructs.Count / 11;

            for (int i = 0; i < cellNumberOrder.Count; ++i)
            {
                List<int> temp = new List<int>();

                for(int j = 1; j < cellNumberOrder[i].Count; ++j)
                {
                    if(cellNumberOrder[i][j-1]/ rowCount == cellNumberOrder[i][j]/ rowCount)
                    {
                        for(int k = 0; k < cellNumberOrder[i][j] - cellNumberOrder[i][j - 1]; ++k)
                        {
                            temp.Add(cellNumberOrder[i][j - 1] + k);
                        }
                    }
                    else
                    {
                        for(int l = 0; l < (cellNumberOrder[i][j]/ rowCount) - (cellNumberOrder[i][j - 1]/ rowCount); ++l)
                        {
                            temp.Add(cellNumberOrder[i][j - 1] + rowCount * l);
                        }
                    }
                }

                // 가장 마지막 값은 나중에 추가해줌.
                temp.Add(cellNumberOrder[i][cellNumberOrder[i].Count-1]);

                this.cellOrderToBeChanged.Add(temp);
            }
        }
    }
}