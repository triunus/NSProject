namespace GameSystem.SaveAndLoad
{
    public interface IGameSaveLoadManager
    {
        public void SaveGameData(SaveLoadType gameStartState, GameDataStruct gameDataStruct, int index = 0);

        public GameDataStruct LoadGameData(SaveLoadType gameStartState, int index = 0);
    }

    public class GameSaveLoadManager : IGameSaveLoadManager
    {
        private IGetSetGameData getSetGameData = new GetSetGameData();

        public void SaveGameData(SaveLoadType gameStartState, GameDataStruct gameDataStruct, int index = 0)
        {
            switch (gameStartState)
            {
                case SaveLoadType.NewGame:
                    this.getSetGameData.SetNewGameData(gameDataStruct);
                    break;
                case SaveLoadType.Continue:
                    this.getSetGameData.SetAutoGameData(gameDataStruct);
                    break;
                case SaveLoadType.LoadGame:
                    this.getSetGameData.SetSaveGameData(gameDataStruct, index);
                    break;
                default:
                    break;
            }
        }

        public GameDataStruct LoadGameData(SaveLoadType gameStartState, int index = 0)
        {
            switch (gameStartState)
            {
                case SaveLoadType.NewGame:
                    return this.getSetGameData.GetNewGameData();
                case SaveLoadType.Continue:
                    return this.getSetGameData.GetAutoGameData();
                case SaveLoadType.LoadGame:
                    return this.getSetGameData.GetSaveGameData(index);
                default:
                    return null;
            }
        }
    }

    public interface ISummaryGameSaveLoadManager
    {
        public void SaveSummayGameData(SaveLoadType gameStartState, SummaryGameDataStruct summaryGameDataStruct, int index = 0);

        public SummaryGameDataStruct LoadSummayAutoGameData();
        public SummaryGameDataStruct[] LoadSummaySaveGameData();
    }

    public class SummaryGameSaveLoadManager : ISummaryGameSaveLoadManager
    {
        private IGetSetSummaryGameData getSetSummaryGameData = new GetSetSummaryGameData();

        public void SaveSummayGameData(SaveLoadType gameStartState, SummaryGameDataStruct summaryGameDataStruct, int index = 0)
        {
            switch (gameStartState)
            {
                case SaveLoadType.Continue:
                    this.getSetSummaryGameData.SetSummaryAutoGameData(summaryGameDataStruct);
                    break;
                case SaveLoadType.LoadGame:
                    this.getSetSummaryGameData.SetSummarySaveGameData(summaryGameDataStruct, index);
                    break;
                default:
                    break;
            }
        }

        public SummaryGameDataStruct LoadSummayAutoGameData()
        {
            return this.getSetSummaryGameData.GetSummaryAutoGameData();
        }
        public SummaryGameDataStruct[] LoadSummaySaveGameData()
        {
            return this.getSetSummaryGameData.GetSummarySaveGameData();
        }
    }
}