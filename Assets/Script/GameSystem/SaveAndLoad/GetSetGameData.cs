using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace GameSystem.SaveAndLoad
{
    public interface IGetSetGameData
    {
        public GameDataStruct GetNewGameData();
        public GameDataStruct GetAutoGameData();
        public GameDataStruct GetSaveGameData(int LoadDataIndex);

        public void SetNewGameData(GameDataStruct gameDataStruct);
        public void SetAutoGameData(GameDataStruct gameDataStruct);
        public void SetSaveGameData(GameDataStruct gameDataStruct, int saveIndex);

        public void DeleteGameData();
    }

    public class GetSetGameData : IGetSetGameData
    {
        private IFormatter binaryFormatter = new BinaryFormatter();
        // 지정된 경로를 이용하여, DirectoryInfo 클래스 인스턴스를 초기화합니다.
        DirectoryInfo directory = new DirectoryInfo(UnityEngine.Application.dataPath + "/LocalData/SaveData");

        public GameDataStruct GetNewGameData()
        {
            try
            {
                // 지정된 경로가 없으면, 지정된 경로 생성.
                if (!directory.Exists) directory.Create();

                Stream readInfo = new FileStream(UnityEngine.Application.dataPath + "/LocalData/SaveData/NewGameData.txt", FileMode.Open, FileAccess.Read, FileShare.Read);
                GameDataStruct gameDataStruct = (GameDataStruct)binaryFormatter.Deserialize(readInfo);

                readInfo.Close();
                return gameDataStruct;
            }
            catch (FileNotFoundException error01)
            {
                UnityEngine.Debug.Log("새 게임 파일이 없음 : " + error01);
                return null;
            }
        }
        public GameDataStruct GetAutoGameData()
        {
            try
            {
                // 지정된 경로가 없으면, 지정된 경로 생성.
                if (!directory.Exists) directory.Create();

                Stream readInfo = new FileStream(UnityEngine.Application.dataPath + "/LocalData/SaveData/AutoGameData.txt", FileMode.Open, FileAccess.Read, FileShare.Read);
                GameDataStruct gameDataStruct = (GameDataStruct)binaryFormatter.Deserialize(readInfo);

                readInfo.Close();
                return gameDataStruct;
            }
            catch (FileNotFoundException error01)
            {
                UnityEngine.Debug.Log("새 게임 파일이 없음 : " + error01);
                return null;
            }
        }
        public GameDataStruct GetSaveGameData(int index)
        {
            try
            {
                // 지정된 경로가 없으면, 지정된 경로 생성.
                if (!directory.Exists) directory.Create();

                Stream readInfo = new FileStream(UnityEngine.Application.dataPath + "/LocalData/SaveData/SaveGameData" + "_" + index + ".txt", FileMode.Open, FileAccess.Read, FileShare.Read);
                GameDataStruct gameDataStruct = (GameDataStruct)binaryFormatter.Deserialize(readInfo);

                readInfo.Close();
                return gameDataStruct;
            }
            catch (FileNotFoundException error01)
            {
                UnityEngine.Debug.Log("새 게임 파일이 없음 : " + error01);
                return null;
            }
        }

        public void SetNewGameData(GameDataStruct gameDataStruct)
        {
            try
            {
                // 지정된 경로가 없으면, 지정된 경로 생성.
                if (!directory.Exists) directory.Create();

                Stream writeInfo = new FileStream(UnityEngine.Application.dataPath + "/LocalData/SaveData/NewGameData.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                binaryFormatter.Serialize(writeInfo, gameDataStruct);

                writeInfo.Close();
            }
            catch(UnityEngine.UnassignedReferenceException error01)
            {
                UnityEngine.Debug.Log("GetSetGameData - SetNewGameData : " + error01);
            }
        }
        public void SetAutoGameData(GameDataStruct gameDataStruct)
        {
            try
            {
                // 지정된 경로가 없으면, 지정된 경로 생성.
                if (!directory.Exists) directory.Create();

                Stream writeInfo = new FileStream(UnityEngine.Application.dataPath + "/LocalData/SaveData/AutoGameData.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                binaryFormatter.Serialize(writeInfo, gameDataStruct);

                writeInfo.Close();
            }
            catch (UnityEngine.UnassignedReferenceException error01)
            {
                UnityEngine.Debug.Log("GetSetGameData - SetAutoGameData : " + error01);
            }
        }
        public void SetSaveGameData(GameDataStruct gameDataStruct, int index)
        {
            try
            {
                // 지정된 경로가 없으면, 지정된 경로 생성.
                if (!directory.Exists) directory.Create();

                Stream writeInfo = new FileStream(UnityEngine.Application.dataPath + "/LocalData/SaveData/SaveGameData" + "_" + index + ".txt", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                binaryFormatter.Serialize(writeInfo, gameDataStruct);

                writeInfo.Close();
            }
            catch (UnityEngine.UnassignedReferenceException error01)
            {
                UnityEngine.Debug.Log("GetSetGameData - SetSaveGameData : " + error01);
            }
        }

        public void DeleteGameData()
        {
            try
            {
                FileInfo willBeDeletedFile = new FileInfo(UnityEngine.Application.dataPath + "/LocalData/SaveData/NewGameData.txt");

                willBeDeletedFile.Delete();
            }
            catch (IOException error01)
            {
                UnityEngine.Debug.Log("파일 없음 : " + error01);
            }
        }
    }

    public interface IGetSetSummaryGameData
    {
        public SummaryGameDataStruct GetSummaryAutoGameData();
        public SummaryGameDataStruct[] GetSummarySaveGameData();

        public void SetSummaryAutoGameData(SummaryGameDataStruct summaryGameDataStruct);
        public void SetSummarySaveGameData(SummaryGameDataStruct summaryGameDataStruct, int index);
    }

    public class GetSetSummaryGameData : IGetSetSummaryGameData
    {
        private IFormatter binaryFormatter = new BinaryFormatter();
        // 지정된 경로를 이용하여, DirectoryInfo 클래스 인스턴스를 초기화합니다.
        DirectoryInfo directory = new DirectoryInfo(UnityEngine.Application.dataPath + "/LocalData/SummarySaveData");

        public SummaryGameDataStruct GetSummaryAutoGameData()
        {
            try
            {
                // 지정된 경로가 없으면, 지정된 경로 생성.
                if (!directory.Exists) directory.Create();

                Stream readInfo = new FileStream(UnityEngine.Application.dataPath + "/LocalData/SummarySaveData/SummaryAutoGameData.txt", FileMode.Open, FileAccess.Read, FileShare.Read);
                SummaryGameDataStruct gameDataStruct = (SummaryGameDataStruct)binaryFormatter.Deserialize(readInfo);

                readInfo.Close();
                return gameDataStruct;
            }
            catch (FileNotFoundException error01)
            {
                UnityEngine.Debug.Log("새 게임 파일이 없음 : " + error01);
                return null;
            }
        }
        public SummaryGameDataStruct[] GetSummarySaveGameData()
        {
            // 지정된 경로가 없으면, 지정된 경로 생성.
            if (!directory.Exists) directory.Create();

            SummaryGameDataStruct[] gameDataStruct = new SummaryGameDataStruct[5];

            for (int i = 1; i < 6; ++i)
            {
                try
                {
                    Stream readInfo = new FileStream(UnityEngine.Application.dataPath + "/LocalData/SummarySaveData/SummarySaveGameData" + "_" + i + ".txt", FileMode.Open, FileAccess.Read, FileShare.Read);
                    gameDataStruct[i-1] = (SummaryGameDataStruct)binaryFormatter.Deserialize(readInfo); 
                    readInfo.Close();
                }
                catch (FileNotFoundException error01)
                {
//                    UnityEngine.Debug.Log(i + "번째 저장된 파일이 없습니다.");
                    gameDataStruct[i-1] = null;
                }
            }

            return gameDataStruct;
        }

        public void SetSummaryAutoGameData(SummaryGameDataStruct summaryGameDataStruct)
        {
            try
            {
                // 지정된 경로가 없으면, 지정된 경로 생성.
                if (!directory.Exists) directory.Create();

                Stream writeInfo = new FileStream(UnityEngine.Application.dataPath + "/LocalData/SummarySaveData/SummaryAutoGameData.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                binaryFormatter.Serialize(writeInfo, summaryGameDataStruct);

                writeInfo.Close();
            }
            catch (UnityEngine.UnassignedReferenceException error01)
            {
                UnityEngine.Debug.Log("GetSetGameData - SetAutoGameData : " + error01);
            }
        }
        public void SetSummarySaveGameData(SummaryGameDataStruct summaryGameDataStruct, int index)
        {
            try
            {
                // 지정된 경로가 없으면, 지정된 경로 생성.
                if (!directory.Exists) directory.Create();

                Stream writeInfo = new FileStream(UnityEngine.Application.dataPath + "/LocalData/SummarySaveData/SummarySaveGameData" + "_" + index + ".txt", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                binaryFormatter.Serialize(writeInfo, summaryGameDataStruct);

                writeInfo.Close();
            }
            catch (UnityEngine.UnassignedReferenceException error01)
            {
                UnityEngine.Debug.Log("GetSetGameData - SetSaveGameData : " + error01);
            }
        }
    }


}