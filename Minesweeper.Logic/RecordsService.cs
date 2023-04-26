using System.Text.Json;
using Minesweeper.Logic.Enums;

namespace Minesweeper.Logic;

public static class RecordsService
{
    private const string RecordsFilePath = "records.txt";

    private const int TopRecordsAmount = 5;

    public static int GetNewRecordPlace(GameDifficulty gameDifficulty, double elapsedTime)
    {
        var records = ReadRecordsFromFile();

        var insertIndex = records.Where(x => x.GameDifficulty == gameDifficulty).TakeWhile(rt => rt.Time <= elapsedTime).Count();

        if (insertIndex < TopRecordsAmount)
        {
            return insertIndex;
        }

        return -1;
    }

    public static bool AddNewRecord(int placeNumber, GameDifficulty gameDifficulty, string name, double elapsedTime)
    {
        var records = ReadRecordsFromFile();

        if (placeNumber is >= 0 and < TopRecordsAmount)
        {
            records.Insert(placeNumber, new RecordTime(gameDifficulty, name ?? "Unknown", elapsedTime));

            WriteRecordsToFile(records);

            return true;
        }

        return false;
    }

    public static List<RecordTime> ReadRecordsFromFile()
    {
        if (!File.Exists(RecordsFilePath))
        {
            return new List<RecordTime>();
        }

        var text = File.ReadAllText(RecordsFilePath);

        try
        {
            List<RecordTime> records;

            records = JsonSerializer.Deserialize<List<RecordTime>>(text);

            return records?.OrderBy(rt => rt.Time).ToList();
        }
        catch
        {
            return new List<RecordTime>();
        }
    }

    private static void WriteRecordsToFile(List<RecordTime> recordsList)
    {
        var text = JsonSerializer.Serialize(recordsList);

        File.WriteAllText(RecordsFilePath, text);
    }
}