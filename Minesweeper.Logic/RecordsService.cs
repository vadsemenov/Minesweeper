using System.Text.Json;
using Minesweeper.Logic.Enums;

namespace Minesweeper.Logic;

public class RecordsService : IRecordsService
{
    private const string RecordsFilePath = "records.txt";

    private const int TopRecordsAmount = 5;

    public int GetNewRecordPlace(GameDifficulty gameDifficulty, TimeSpan elapsedTime)
    {
        var records = ReadRecordsFromFile();

        var insertIndex = records
            .Where(x => x.GameDifficulty == gameDifficulty)
            .TakeWhile(rt => rt.Time <= elapsedTime)
            .Count();

        if (insertIndex < TopRecordsAmount)
        {
            return insertIndex;
        }

        return -1;
    }

    public bool AddNewRecord(int placeNumber, GameDifficulty gameDifficulty, string name, TimeSpan elapsedTime)
    {
        var records = ReadRecordsFromFile();

        if (placeNumber is < 0 or >= TopRecordsAmount)
        {
            return false;
        }

        records.Insert(placeNumber, new RecordTime(gameDifficulty, name ?? "Unknown", elapsedTime));

        WriteRecordsToFile(records);

        return true;
    }

    public List<RecordTime> GetAllRecordsTimes(GameDifficulty gameDifficulty)
    {
        return ReadRecordsFromFile()
            .Where(x => x.GameDifficulty == gameDifficulty)
            .ToList();
    }

    private List<RecordTime> ReadRecordsFromFile()
    {
        if (!File.Exists(RecordsFilePath))
        {
            return new List<RecordTime>();
        }

        var text = File.ReadAllText(RecordsFilePath);

        try
        {
            var records = JsonSerializer.Deserialize<List<RecordTime>>(text);

            return records?
                .OrderBy(rt => rt.Time)
                .ToList();
        }
        catch
        {
            return new List<RecordTime>();
        }
    }

    private void WriteRecordsToFile(List<RecordTime> recordsList)
    {
        var text = JsonSerializer.Serialize(recordsList);

        File.WriteAllText(RecordsFilePath, text);
    }
}