using Minesweeper.Logic.Enums;

namespace Minesweeper.Logic;

public interface IRecordsService
{
    int GetNewRecordPlace(GameDifficulty gameDifficulty, TimeSpan elapsedTime);

    bool AddNewRecord(int placeNumber, GameDifficulty gameDifficulty, string name, TimeSpan elapsedTime);

    List<RecordTime> GetAllRecordsTimes(GameDifficulty gameDifficulty);
}