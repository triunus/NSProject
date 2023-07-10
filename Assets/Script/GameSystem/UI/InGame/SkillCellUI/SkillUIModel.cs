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

        public ref List<SkillUICellLineStruct> SkillUICellLineStructs { get; }
        public ref List<SkillUICellMSStruct> SkillUICellMSStructs { get; }
        public ref List<SkillInformationStruct> SkillInformationStructs { get; }
        public LinkedList<SkillUICellMSPreconditionStruct> GetMSPreconditionStruct(int skillNumber);
    }

    public class SkillUIModel : MonoBehaviour, ISkillUIModel
    {
        private ISkillManagerForModel skillCellUIManager;

        // 변경이 수행되어야 할 라인의 Cell 목록이 기록된 값을 전달한다.
        private List<ICellOrderToBeChangedObserverForView> cellOrderToBeChangedObservers;
        private List<List<int>> cellOrderToBeChanged;

        private List<IPlayerSkillInformationObserverForView> playerSkillInformationObservers;
        private List<PlayerSkillInformationStruct> playerSkillInformationStructs;

        // SkillUICell의 Line을 나타내는데 사용한다.
        private List<SkillUICellLineStruct> skillUICellLineStructs;
        private List<LinkedList<SkillUICellLinePreconditionStruct>> adjacentLinePreconditionStructs;

        // SkillUICell의 main과 sub를 나타내는데 사용한다.
        private List<SkillUICellMSStruct> skillUICellMSStructs;
        private List<LinkedList<SkillUICellMSPreconditionStruct>> adjacentMSPreconditionStructs;

        // 기존 skill 데이터를 갖고 있다.
        private List<SkillInformationStruct> skillInformationStructs;
        private List<LinkedList<SkillUICellMSPreconditionStruct>> MSPreconditionStructs;

        private List<int> cellLIneStartPosition;   // skillUICell 테이블 중, 최초로 시작되는 cell의 intersectinoNumber와 cellNumber
        private List<int> cellMSStartPosition;

        private List<List<int>> activatedLineOrder;                                     // 특정 cellNumber에 도착하기 위한, cellNumber순서.
        private List<List<int>> activatedMSOrder;

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

        // SkillUIModel 멤버 정의.
        private void Awake()
        {
            this.cellOrderToBeChangedObservers = new List<ICellOrderToBeChangedObserverForView>();
            this.cellOrderToBeChanged = new List<List<int>>();
            this.playerSkillInformationObservers = new List<IPlayerSkillInformationObserverForView>();

            this.skillUICellLineStructs = new List<SkillUICellLineStruct>();
            this.adjacentLinePreconditionStructs = new List<LinkedList<SkillUICellLinePreconditionStruct>>();

            this.skillUICellMSStructs = new List<SkillUICellMSStruct>();
            this.adjacentMSPreconditionStructs = new List<LinkedList<SkillUICellMSPreconditionStruct>>();
            this.MSPreconditionStructs = new List<LinkedList<SkillUICellMSPreconditionStruct>>();

            this.cellLIneStartPosition = new List<int>();
            this.cellMSStartPosition = new List<int>();

            this.activatedLineOrder = new List<List<int>>();
            this.activatedMSOrder = new List<List<int>>();

            this.beClickedCellNumber = -1;
        }

        // ISkillCellUIModel 구현
        // SkillModel 초기 설정. Manager에서 필요한 데이터 가져오기.
        public void InitialSetting(ISkillManagerForModel skillCellUIManager)
        {
            this.skillCellUIManager = skillCellUIManager;

            this.skillUICellLineStructs = this.skillCellUIManager.GetSkillUICellLineStructs();
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
            // 선택된 것이 없었음. 선 활성화.
            if (this.beClickedCellNumber == -1)
            {
                this.beClickedCellNumber = skillUICellMSStruct.CellNumber;

                this.CellUILineTopologySort(this.beClickedCellNumber);
                this.CellUIMSTopologySort(this.beClickedCellNumber);
                this.ExcludeCellNumberToBeActivated();
                this.ExtractCellNumberToBeChanged();

                this.NotifyCellOrderToBeChangedObservers(ActivateOrNot.Activate);   // 선 활성화
            }
            // 선택된 것과, 새롭게 선택한 것이 동일함. 선 비활성화.
            else if (this.beClickedCellNumber == skillUICellMSStruct.CellNumber)
            {
                this.beClickedCellNumber = -1;

                this.NotifyCellOrderToBeChangedObservers(ActivateOrNot.Not);  // 선 비활성화

                this.activatedLineOrder.Clear();
                this.activatedMSOrder.Clear();
                this.cellOrderToBeChanged.Clear();
            }
            // 선택된 것과, 새롭게 선택된 것이 다름. 선 비활성화 후 선 활성화.
            else
            {
                this.beClickedCellNumber = skillUICellMSStruct.CellNumber;

                this.NotifyCellOrderToBeChangedObservers(ActivateOrNot.Not);  // 선 비활성화

                this.activatedLineOrder.Clear();
                this.activatedMSOrder.Clear();
                this.cellOrderToBeChanged.Clear();

                this.CellUILineTopologySort(this.beClickedCellNumber);
                this.CellUIMSTopologySort(this.beClickedCellNumber);
                this.ExcludeCellNumberToBeActivated();
                this.ExtractCellNumberToBeChanged();

                this.NotifyCellOrderToBeChangedObservers(ActivateOrNot.Activate);    // 선 활성화
            }

            /*            foreach (var item01 in cellOrderToBeChanged)
                        {
                            foreach (var item02 in item01)
                            {
                                Debug.Log("DecideActivateOrInActivateCell_cellOrderToBeChanged : " + item02.ToString());
                            }
                        }*/
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
        public ref List<SkillUICellLineStruct> SkillUICellLineStructs { get { return ref this.skillUICellLineStructs; } }
        public ref List<SkillUICellMSStruct> SkillUICellMSStructs { get { return ref this.skillUICellMSStructs; } }
        public ref List<SkillInformationStruct> SkillInformationStructs { get { return ref this.skillInformationStructs; } }
        public LinkedList<SkillUICellMSPreconditionStruct> GetMSPreconditionStruct(int skillNumber) { return this.MSPreconditionStructs[skillNumber]; }
        // -------------------------

        // DFS를 수행하기 위한, 최초의 숫자 4개 간추리기.
        private void FindCellLineStartPosition()
        {
            for(int i=0; i < skillUICellLineStructs.Count; ++i)
            {
                if (skillUICellLineStructs[i].CellContent == CellContent.main && (skillUICellLineStructs[i].LineNumber == 4 || skillUICellLineStructs[i].LineNumber == 0))
                    this.cellLIneStartPosition.Add(skillUICellLineStructs[i].CellNumber);
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

            for (int i = 0; i < skillUICellLineStructs.Count; ++i)
            {
                LinkedList<SkillUICellLinePreconditionStruct> perSkillUICellLine = new LinkedList<SkillUICellLinePreconditionStruct>();
                this.adjacentLinePreconditionStructs.Add(perSkillUICellLine);
            }

            for (int i = 0; i < skillUICellLinePrecondition.Count; ++i)
            {
                this.adjacentLinePreconditionStructs[(int)skillUICellLinePrecondition[i]["Precondition_p"]].AddLast(
                    new SkillUICellLinePreconditionStruct(
                        precondition_q: (int)skillUICellLinePrecondition[i]["Precondition_q"]));
            }
        }
        private void MakeSkillUICellMSLinkedList()
        {
            TextAsset skillUICellMSPrecondition_TextAsset = Resources.Load<TextAsset>("GameSystem/SkillData/UI/SkillUICellMSPrecondition");
            JArray skillUICellMSPrecondition = JArray.Parse(skillUICellMSPrecondition_TextAsset.ToString());

            // adjacentMSPreconditionStructs
            for (int i = 0; i < skillUICellMSStructs.Count; ++i)
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
            for (int i = 0; i < skillUICellMSStructs.Count; ++i)
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
                visited = Enumerable.Repeat(0, skillUICellLineStructs.Count).ToList();
                destinationIsVisited = false;

                // 시작 intersection, 시작 cellNumber, 방문지점, 목적지 방문여부, 방문기록, 방문 순서.
                this.CellUILineDFS(cellLIneStartPosition[i], ref destinationCellNumber, ref destinationIsVisited, ref visited, ref order);

                if (destinationIsVisited) { this.activatedLineOrder.Add( Enumerable.Reverse(order).ToList()); }
            }

/*            foreach (var item01 in activatedLineOrder)
            {
                foreach (var item02 in item01)
                {
                    Debug.Log("CellUILineTopologySort : " + item02.ToString());
                }
            }*/
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

            foreach (var item in adjacentLinePreconditionStructs[startCellNumber])
            {
                if (visited[item.Precondition_q] == 0 && !destinationIsVisited)
                {
                    CellUILineDFS(item.Precondition_q, ref destinationCellNumber, ref destinationIsVisited, ref visited, ref order);
                }
            }

            if (destinationIsVisited) { order.Add(startCellNumber); }
        }
        private void CellUIMSTopologySort(int destinationCellNumber)
        {
            List<int> visited = new List<int>();
            bool destinationIsVisited;
            int destinationSkillNumber = this.skillUICellMSStructs[skillUICellMSStructs.FindIndex(x => x.CellNumber == destinationCellNumber)].SkillNumber;

            for (int i = 0; i < this.cellMSStartPosition.Count; ++i)
            {
                List<int> order = new List<int>();
                visited = Enumerable.Repeat(0, skillUICellMSStructs.Count).ToList();
                destinationIsVisited = false;

                // 시작 intersection, 시작 cellNumber, 방문지점, 목적지 방문여부, 방문기록, 방문 순서.
                this.CellUIMSDFS(cellMSStartPosition[i], ref destinationSkillNumber, ref destinationIsVisited, ref visited, ref order);

                if (destinationIsVisited) { this.activatedMSOrder.Add(Enumerable.Reverse(order).ToList()); }

                order.Clear();
            }

/*            foreach (var item01 in activatedMSOrder)
            {
                foreach (var item02 in item01)
                {
                    Debug.Log("CellUIMSTopologySort : " + item02.ToString());
                }
            }*/
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
                    foreach(var item in adjacentMSPreconditionStructs[activatedMSOrder[i][j]])  // // skillNumber를 통해 비교가 이루어진다.
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

/*            foreach (var item01 in satisfyCondition)
            {
                foreach (var item02 in item01)
                {
                    Debug.Log("ExcludeCellNumberToBeActivated_satisfyCondition.Key : " + item02.Key + ", ExcludeCellNumberToBeActivated_satisfyCondition.Value : " + item02.Value);
                }
            }*/

/*            foreach (var item01 in activatedLineOrder)
            {
                foreach (var item02 in item01)
                {
                    Debug.Log("01. ExcludeCellNumberToBeActivated_activatedLineOrder : " + item02.ToString());
                }
            }*/


            for (int i = 0; i < satisfyCondition.Count; ++i)
            {
                while (satisfyCondition[i][0].Value)
                {
//                    Debug.Log("밖 : satisfyCondition[i][0].Value : " + satisfyCondition[i][0].Value);

                    while (true) // true야 반복, 두 값이 다르면 같으면 지속.
                    {
//                        Debug.Log("안 : satisfyCondition[i][1].Key, activatedLineOrder[i][0] : " + satisfyCondition[i][1].Key + ", " + activatedLineOrder[i][0]);

                        if (satisfyCondition[i][1].Key == activatedLineOrder[i][0]) { break; }

                        activatedLineOrder[i].RemoveAt(0);
                    }

                    satisfyCondition[i].RemoveAt(0);
                }
            }

/*            foreach (var item01 in activatedLineOrder)
            {
                foreach (var item02 in item01)
                {
                    Debug.Log("02. ExcludeCellNumberToBeActivated_activatedLineOrder : " + item02.ToString());
                }
            }*/
        }
        private void ExtractCellNumberToBeChanged()
        {
            int rowCount = skillUICellLineStructs.Count / 11;

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

                // 가장 마지막 값은 나중에 추가해줌.
                temp.Add(activatedLineOrder[i][activatedLineOrder[i].Count-1]);

/*                foreach (var item in temp)
                {
                    Debug.Log("temp " + item.ToString());
                }*/

                this.cellOrderToBeChanged.Add(temp);
            }
        }
    }
}