﻿using Spectre.Console;

static class Ui
{
    public static void ShowHeader()
    {
        var header = new FigletText("Memori")
                     .LeftAligned()
                     .Color(Color.Red);

        AnsiConsole.Write(header);
    }

}