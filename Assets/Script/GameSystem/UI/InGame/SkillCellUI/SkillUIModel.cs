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
        public void DecideActivateOrInActivateCell(SkillUICellMSStruct skillUICellMSStruct);     // 활성화 or 비활성화
        public void LearnSkill(int skillNumber);

        public ref List<SkillUICellStruct> SkillUICellStructs { get; }
        public ref List<SkillUICellMSStruct> SkillUICellMSStructs { get; }
        public ref List<SkillInformationStruct> SkillInformationStructs { get; }
        public LinkedList<SkillUICellMSPreconditionStruct> GetMSPreconditionStruct(int skillNumber);
    }

    public class SkillUIModel : MonoBehaviour, ISkillUIModel
    {
        private ISkillManagerForModel skillCellUIManager;

        // 변경이 수행되어야 할 라인의 Cell 목록이 기록된 값을 전달한다.
        private List<ICellOrderToBeChangedObserverForView> cellOrderToBeChangedObservers;       // UI 강조가 수행되어야 할 CellNumber를 필요로하는 View 목록.
        private List<List<int>> cellOrderToBeChanged;                                           // UI 강조가 수행되어야 할 CellNumber 목록.

        private List<IPlayerSkillInformationObserverForView> playerSkillInformationObservers;   // 사용자가 현재 학습한 스킬의 Level 정보를 필요로하는 View 목록.
        private List<PlayerSkillInformationStruct> playerSkillInformationStructs;               // 사용자가 현재 학습한 스킬의 Level 정보.

        private List<SkillUICellStruct> skillUICellStructs;                                                     // SkillUICell의 기본 정보를 갖고 있다.                       
        private List<LinkedList<SkillUICellNumberPreconditionStruct>> adjacentCellNumberPreconditionStructs;    // SkillUICell.CellNumber 들을 순서가 있는 그래프로 표현하기 위해
                                                                                                                // CellNumber들 간의 순서를 표현하는 정보가 기록되어 있다.

        private List<SkillUICellMSStruct> skillUICellMSStructs;                                         // 'SkillCellUI GameObject'를 구분하기 위해 사용한다.      

        // List i 번째 스킬에 대한 관련 스킬 및 가중치를 기록하는 구조체이다.
        private List<LinkedList<SkillUICellMSPreconditionStruct>> adjacentMSPreconditionStructs;        // 'i 번째 SkillNumber는 q SkillNumber를 배우기 위해, i skillNumber 스킬을 weight만큼 필요'하다는 정보를 갖는다.
        private List<LinkedList<SkillUICellMSPreconditionStruct>> MSPreconditionStructs;                // 'i 번째 SkillNumber를 배우기 위해, q SkillNumberfmf weight만큼 올릴 필요'하다는 정보를 갖는다.

        // Skill의 기본 데이터를 갖고 있다.
        private List<SkillInformationStruct> skillInformationStructs;

        private List<int> cellStartPosition;        // skillUICell 테이블 중, 최초로 시작되는 cell의 cellNumber
        private List<int> cellMSStartPosition;          // skillUICell 테이블 중, 최초로 시작되는 skill의 skillNumber

        private List<List<int>> CellNumberOrder;                                     // 특정 cellNumber에 도착하기 위한, cellNumber 순서.
        private List<List<int>> MSOrder;                                       // 특정 skillNumber에 도착하기 위한, skillNumber 순서.

        private int beClickedCellNumber;                                        // 현재 클릭된 스킬 명시.

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

        // ISkillCellUIModel 구현
        // Manager에서 필요한 데이터 가져오기 + SkillModel 초기 설정.
        public void InitialSetting(ISkillManagerForModel skillCellUIManager)
        {
            this.skillCellUIManager = skillCellUIManager;

            this.skillUICellStructs = this.skillCellUIManager.GetSkillUICellLineStructs();
            this.skillUICellMSStructs = this.skillCellUIManager.GetSkillUICellMSStructs();
            this.skillInformationStructs = this.skillCellUIManager.GetSkillInformationStruct();
            this.playerSkillInformationStructs = this.skillCellUIManager.GetPlayerSkillInformationStruct();

            this.FindCellNumberStartPosition();         // CellNumber를 정점으로 사용하는 그래프의 시작 정점 찾기.
            this.FindCellMSStartPosition();             // SkillNumber를 정점으로 사용하는 그래프의 시작 정점 찾기.

            this.MakeSkillUICellLineLinkedList();       // CellNumber 간에 순서를 기록한 데이터를 읽어와, 인접리스트를 만든다.
            this.MakeSkillUICellMSLinkedList();         // SkillNumber 간에 순서와 가중치를 기록한 데이터를 읽어와, 인접리스트를 만든다.

            this.MakeMSPreconditionLinkedList();   // SkillNumber 간에 순서와 가중치를 기록한 데이터를 읽어와, 특정 스킬에 필요한 skillNumber와 가중치 LinkedList를 만든다.
        }
        public void DecideActivateOrInActivateCell(SkillUICellMSStruct skillUICellMSStruct)
        {
            // 선택된 것이 없었음. 선 활성화.
            if (this.beClickedCellNumber == -1)
            {
                this.beClickedCellNumber = skillUICellMSStruct.CellNumber;

                this.CellUILineTopologySort(this.beClickedCellNumber);
                this.CellUIMSTopologySort(skillUICellMSStruct);
                this.ExcludeCellNumberToBeActivated();
                this.ExtractCellNumberToBeChanged();

                this.NotifyCellOrderToBeChangedObservers(ActivateOrNot.Activate);   // 선 활성화
            }
            // 선택된 것과, 새롭게 선택한 것이 동일함. 선 비활성화.
            else if (this.beClickedCellNumber == skillUICellMSStruct.CellNumber)
            {
                this.beClickedCellNumber = -1;

                this.NotifyCellOrderToBeChangedObservers(ActivateOrNot.Not);  // 선 비활성화

                this.CellNumberOrder.Clear();
                this.MSOrder.Clear();
                this.cellOrderToBeChanged.Clear();
            }
            // 선택된 것과, 새롭게 선택된 것이 다름. 선 비활성화 후 선 활성화.
            else
            {
                this.beClickedCellNumber = skillUICellMSStruct.CellNumber;

                this.NotifyCellOrderToBeChangedObservers(ActivateOrNot.Not);  // 선 비활성화

                this.CellNumberOrder.Clear();
                this.MSOrder.Clear();
                this.cellOrderToBeChanged.Clear();

                this.CellUILineTopologySort(this.beClickedCellNumber);
                this.CellUIMSTopologySort(skillUICellMSStruct);
                this.ExcludeCellNumberToBeActivated();
                this.ExtractCellNumberToBeChanged();

                this.NotifyCellOrderToBeChangedObservers(ActivateOrNot.Activate);    // 선 활성화
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
        public ref List<SkillUICellMSStruct> SkillUICellMSStructs { get { return ref this.skillUICellMSStructs; } }
        public ref List<SkillInformationStruct> SkillInformationStructs { get { return ref this.skillInformationStructs; } }
        public LinkedList<SkillUICellMSPreconditionStruct> GetMSPreconditionStruct(int skillNumber) { return this.MSPreconditionStructs[skillNumber]; }
        // -------------------------

        // CellNumber를 정점으로 사용하는 그래프의 시작 정점 찾기.
        private void FindCellNumberStartPosition()
        {
            for(int i=0; i < skillUICellStructs.Count; ++i)
            {
                if (skillUICellStructs[i].CellContent == CellContent.main && (skillUICellStructs[i].LineNumber == 4 || skillUICellStructs[i].LineNumber == 0))
                    this.cellStartPosition.Add(skillUICellStructs[i].CellNumber);
            }
        }
        // SkillNumber를 정점으로 사용하는 그래프의 시작 정점 찾기.
        private void FindCellMSStartPosition()
        {
            for (int i = 0; i < cellStartPosition.Count; ++i)
            {
                this.cellMSStartPosition.Add(this.skillUICellMSStructs[skillUICellMSStructs.FindIndex(x => x.CellNumber == cellStartPosition[i])].SkillNumber);
            }
        }

        // CellNumber 간에 순서를 기록한 데이터를 읽어와, 인접리스트를 만든다.
        private void MakeSkillUICellLineLinkedList()
        {
            // CellNumber 정점 간의 순서를 기록한 Local 데이터 읽어오기.
            TextAsset skillUICellLinePrecondition_TextAsset = Resources.Load<TextAsset>("GameSystem/SkillData/UI/SkillUICellPrecondition");
            JArray skillUICellLinePrecondition = JArray.Parse(skillUICellLinePrecondition_TextAsset.ToString());

            // adjacentCellNumberPreconditionStructs LinkedList 정의.
            for (int i = 0; i < skillUICellStructs.Count; ++i)
            {
                LinkedList<SkillUICellNumberPreconditionStruct> perSkillUICellLine = new LinkedList<SkillUICellNumberPreconditionStruct>();
                this.adjacentCellNumberPreconditionStructs.Add(perSkillUICellLine);
            }

            // adjacentCellNumberPreconditionStructs 인접리스트 객체 생성.
            for (int i = 0; i < skillUICellLinePrecondition.Count; ++i)
            {
                this.adjacentCellNumberPreconditionStructs[(int)skillUICellLinePrecondition[i]["Precondition_p"]].AddLast(
                    new SkillUICellNumberPreconditionStruct(
                        precondition_q: (int)skillUICellLinePrecondition[i]["Precondition_q"]));
            }
        }
        // SkillNumber 간에 순서와 가중치를 기록한 데이터를 읽어와, 인접리스트를 만든다.
        private void MakeSkillUICellMSLinkedList()
        {
            // SkillNumber 정점 간의 순서와 가중치를 기록한 Local 데이터 읽어오기.
            TextAsset skillUICellMSPrecondition_TextAsset = Resources.Load<TextAsset>("GameSystem/SkillData/UI/SkillUICellMSPrecondition");
            JArray skillUICellMSPrecondition = JArray.Parse(skillUICellMSPrecondition_TextAsset.ToString());

            // adjacentMSPreconditionStructs LinkedList 정의.
            for (int i = 0; i < this.skillUICellMSStructs.Count; ++i)
            {
                LinkedList<SkillUICellMSPreconditionStruct> perSkillUICellMS = new LinkedList<SkillUICellMSPreconditionStruct>();
                this.adjacentMSPreconditionStructs.Add(perSkillUICellMS);
            }

            // adjacentMSPreconditionStructs 인접리스트 객체 생성.
            for (int i = 0; i < skillUICellMSPrecondition.Count; ++i)
            {
                this.adjacentMSPreconditionStructs[(int)skillUICellMSPrecondition[i]["Precondition_p"]].AddLast(
                    new SkillUICellMSPreconditionStruct(
                        precondition_q: (int)skillUICellMSPrecondition[i]["Precondition_q"],
                        precondition_Weight: (int)skillUICellMSPrecondition[i]["Precondition_Weight"]));
            }
        }
        // SkillNumber 간에 순서와 가중치를 기록한 데이터를 읽어와, 특정 스킬에 필요한 skillNumber와 가중치 LinkedList를 만든다.
        private void MakeMSPreconditionLinkedList()
        {
            // SkillNumber 정점 간의 순서와 가중치를 기록한 Local 데이터 읽어오기.
            TextAsset skillUICellMSPrecondition_TextAsset = Resources.Load<TextAsset>("GameSystem/SkillData/UI/SkillUICellMSPrecondition");
            JArray skillUICellMSPrecondition = JArray.Parse(skillUICellMSPrecondition_TextAsset.ToString());

            // MSPreconditionStructs LinkedList 정의.
            for (int i = 0; i < this.skillUICellMSStructs.Count; ++i)
            {
                LinkedList<SkillUICellMSPreconditionStruct> perSkillUICellMS = new LinkedList<SkillUICellMSPreconditionStruct>();
                this.MSPreconditionStructs.Add(perSkillUICellMS);
            }

            // MSPreconditionStructs 인접리스트 객체 생성.
            for (int i = 0; i < skillUICellMSPrecondition.Count; ++i)
            {
                this.MSPreconditionStructs[(int)skillUICellMSPrecondition[i]["Precondition_q"]].AddLast(
                    new SkillUICellMSPreconditionStruct(
                        precondition_q: (int)skillUICellMSPrecondition[i]["Precondition_p"],
                        precondition_Weight: (int)skillUICellMSPrecondition[i]["Precondition_Weight"]));
            }
        }

        // CellNumber 위상정렬 시작.
        private void CellUILineTopologySort(int destinationCellNumber)
        {
            List<int> visited = new List<int>();
            bool destinationIsVisited;

            for (int i = 0; i < this.cellStartPosition.Count; ++i)
            {
                List<int> order = new List<int>();
                visited = Enumerable.Repeat(0, skillUICellStructs.Count).ToList();      // 모든 정점에 대한 방문여부.
                destinationIsVisited = false;                                           // 목표 정점 방문여부 값.

                // 시작 cellNumber, 방문지점, 목적지 방문여부, 방문기록, 방문 순서.
                this.CellUILineDFS(cellStartPosition[i], ref destinationCellNumber, ref destinationIsVisited, ref visited, ref order);

                // 목표로한 정점을 방문하였다면,
                // '목표 정점 -> ... -> 시작 정점'으로 되어있는 정점 순서를 뒤집는다.
                // 정상으로 돌아온 방문순서를 CellNumberOrder 객체에 저장.
                if (destinationIsVisited) { this.CellNumberOrder.Add( Enumerable.Reverse(order).ToList()); }
            }
        }
        private void CellUILineDFS(int startCellNumber, ref int destinationCellNumber, ref bool destinationIsVisited, ref List<int> visited, ref List<int> order)
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
            foreach (var item in adjacentCellNumberPreconditionStructs[startCellNumber])
            {
                // 다음 방문할 정점이 방문한적이 없으며 && 목표로 한 지점을 방문한 적이 없는지 확인.
                if (visited[item.Precondition_q] == 0 && !destinationIsVisited)
                {
                    // 다음 정점 탐색.
                    CellUILineDFS(item.Precondition_q, ref destinationCellNumber, ref destinationIsVisited, ref visited, ref order);
                }
            }

            // 현재 정점의 연장선에서 목표로한 지점을 방문한 적이 있다면, 현재 정점을 기록한다.
            if (destinationIsVisited) { order.Add(startCellNumber); }
        }
        // SkillNumber 위상정렬 시작.
        private void CellUIMSTopologySort(SkillUICellMSStruct skillUICellMSStruct)
        {
            List<int> visited = new List<int>();
            bool destinationIsVisited;
            int destinationSkillNumber = skillUICellMSStruct.SkillNumber;

            for (int i = 0; i < this.cellMSStartPosition.Count; ++i)
            {
                List<int> order = new List<int>();
                visited = Enumerable.Repeat(0, skillUICellMSStructs.Count).ToList();    // 모든 정점에 대한 방문여부.
                destinationIsVisited = false;                                           // 목표 정점 방문여부 값.

                // 시작 cellNumber, 방문지점, 목적지 방문여부, 방문기록, 방문 순서.
                this.CellUIMSDFS(cellMSStartPosition[i], ref destinationSkillNumber, ref destinationIsVisited, ref visited, ref order);

                // 목표로한 정점을 방문하였다면,
                // '목표 정점 -> ... -> 시작 정점'으로 되어있는 정점 순서를 뒤집는다.
                // 정상으로 돌아온 방문순서를 MSOrder 객체에 저장.
                if (destinationIsVisited) { this.MSOrder.Add(Enumerable.Reverse(order).ToList()); }

                order.Clear();
            }
        }
        private void CellUIMSDFS(int startSkillNumber, ref int destinationSkillNumber, ref bool destinationIsVisited, ref List<int> visited, ref List<int> order)
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
            foreach (var item in adjacentMSPreconditionStructs[startSkillNumber])
            {
                // 다음 방문할 정점이 방문한적이 없으며 && 목표로 한 지점을 방문한 적이 없는지 확인.
                if (visited[item.Precondition_q] == 0 && !destinationIsVisited)
                {
                    // 다음 정점 탐색.
                    CellUIMSDFS(item.Precondition_q, ref destinationSkillNumber, ref destinationIsVisited, ref visited, ref order);
                }
            }

            // 현재 정점의 연장선에서 목표로한 지점을 방문한 적이 있다면, 현재 정점을 기록한다.
            if (destinationIsVisited) { order.Add(startSkillNumber); }
        }

        private void ExcludeCellNumberToBeActivated()
        {
            List<List<KeyValuePair<int, bool>>> satisfyCondition = new List<List<KeyValuePair<int, bool>>>();

            for (int i = 0; i < MSOrder.Count; ++i)
            {
                // LinkedList 정의.
                List<KeyValuePair<int, bool>> perKeyValuePair = new List<KeyValuePair<int, bool>>();
                satisfyCondition.Add(perKeyValuePair);

                for (int j = 0; j < MSOrder[i].Count-1; ++j)
                {
                    foreach(var item in adjacentMSPreconditionStructs[MSOrder[i][j]])  // // skillNumber를 통해 비교가 이루어진다.
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

                    while (true) // true야 반복, 두 값이 다르면 같으면 지속.
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

                // 가장 마지막 값은 나중에 추가해줌.
                temp.Add(CellNumberOrder[i][CellNumberOrder[i].Count-1]);

                this.cellOrderToBeChanged.Add(temp);
            }
        }
    }
}