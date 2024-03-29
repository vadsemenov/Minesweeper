﻿using Minesweeper.Logic.Enums;

namespace Minesweeper.Logic;

public record Cell
{
    public CellContent CellContent { get; set; }

    public CellStatus Status { get; set; }
}