using Minesweeper.Logic.Enums;

namespace Minesweeper.Logic;

public record RecordTime(GameDifficulty GameDifficulty, string Name, TimeSpan Time);