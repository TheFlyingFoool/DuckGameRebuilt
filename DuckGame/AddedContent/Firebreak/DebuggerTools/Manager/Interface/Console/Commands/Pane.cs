using DuckGame.ConsoleInterface;
using DuckGame.ConsoleInterface.Panes;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [MMCommand(Description = "Alters the current pane.")]
        public static void Pane(string pane, params string[] args)
        {
            console.SwitchToPane = pane switch
            {
                "config" => new MMConfigPane(),
                "test"   => new MMTestPane(),
                "split" => new MMSplitPane(Enum.TryParse(args.FirstOrDefault() ?? nameof(Orientation.Horizontal), true, out Orientation result) 
                    ? result 
                    : throw new Exception($"Invalid orientation ({args.FirstOrDefault() ?? "NULL"}). [Horizontal/Vertical]"), 
                    new MMConsolePane()
                {
                    Bounds = console.Bounds,
                    Lines = console.Lines,
                    CaretIndex = console.CaretIndex,
                    ShellInstance = console.ShellInstance
                }, new MMConsolePane()),
                "hat" => new MMDrawingPane(Content.Load<Tex2D>("neon")),
                _ => console.SwitchToPane
            };
        }
    }
}