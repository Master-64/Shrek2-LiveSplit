using LiveSplit.ComponentUtil;
using Shrek2_LiveSplit.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shrek2_LiveSplit.Game
{
    public class Shrek2Game
    {
        public Process GameProcess { get; set; }

        private DeepPointer LogBufferPtr { get; set; }
        private DeepPointer LogBufferCursorPtr { get; set; }
        private DeepPointer CurrentMap { get; set; }

        private string OldMap { get; set; }
        private int PrevBufCursor { get; set; } = 0;
        private string PrevBuf { get; set; } = "";

        public enum GameState
        {
            None, Split, NewGame, Reset, Pause, Resume
        };

        public Shrek2Game()
        {
            LogBufferPtr = new DeepPointer(0x000566B4, 0x50);
            LogBufferCursorPtr = new DeepPointer(0x000566B4, 0x4c);
            CurrentMap = new DeepPointer("Engine.dll", 0x004DFFF8, DeepPointer.DerefType.Bit32, 0x68, 0x9C, 0xA8, 0x4E0);
        }

        public GameState GetGameState()
        {
            try
            {
                if (GameProcess == null || GameProcess.HasExited) return GameState.None;
                CurrentMap.Deref(GameProcess, out int rawMapAddr);
                var cm = new DeepPointer(new IntPtr(rawMapAddr));
                cm.DerefString(GameProcess, 1024, out string rawMap);
                
                if (string.IsNullOrWhiteSpace(rawMap)) return GameState.None;
                rawMap = rawMap.ToLower();

                string formattedMap = rawMap.Replace(".unr", "").Replace(".UNR", "");

                if(OldMap == "DEFAULT")
                {
                    OldMap = formattedMap;
                }

                if(formattedMap != OldMap)
                {
                    OldMap = formattedMap;

                    if (formattedMap == "book_frontend") return GameState.Reset;
                    if (formattedMap == "book_story_1") return GameState.NewGame;

                    if (Shrek2Variables.ExcludedSplitMaps.Any(p => p == formattedMap)) return GameState.None;
                    return GameState.Split; // Master_64: This still gets fired sometimes during level loads (1 in every 150 map loads roughly)? Probably something to do with the map pointer.
                }

                return GameState.None;
            }
            catch
            {
                return GameState.None;
            }
        }

        public GameState GetLoadlessState(string cll)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cll)) return GameState.None;

                // Master_64: Simplified these calls majorly to improve consistency with loadless
                if (cll.Contains("Resetting GLevel:") || cll.Contains("LoadMap:") || cll.Contains("Level-loading")) return GameState.Pause;
                if (cll.Contains("Allocating 16384") || cll.Contains("Allocating 65536")) return GameState.Resume;
                if (cll.Contains("E_YOUWIN.CutScene]:Trig")) return GameState.Split; // This is how we figure out that the game has been finished

                // Master_64: This is all inaccurate/inconsistent and is no longer needed
                // if (cll.Contains("kwherocontroller ShowMenu")) return GameState.Pause;
                // if (cll.Contains("GUIController::CloseMenu")) return GameState.Resume;
                // if (cll.Contains("Log: Saving game...")) return GameState.Pause;
                // if (cll.Contains("Saving into slot")) return GameState.Pause;
                // if (cll.Contains("Log: Save=")) return GameState.Resume;
                // if (cll.Contains("CutLog: [GAMEMENU.CutScene]:Triggered")) return GameState.Reset;
                // if (cll.Contains("CutLog: [GAMEMENU.CutScene]:SceneStarted")) return GameState.Reset;

                return GameState.None;
            }
            catch
            {
                return GameState.None;
            }
        }

        public List<string> GetCurrentCutLogLines()
        {
            try
            {
                if (GameProcess == null || GameProcess.HasExited) return new List<string>();
                LogBufferPtr.DerefString(GameProcess, 4096, out string buf);
                LogBufferCursorPtr.Deref(GameProcess, out int bufCursor);

                string log = "";
                if ((!buf.Equals(PrevBuf) && !PrevBuf.Equals(String.Empty)))
                {
                    int length = (PrevBufCursor > bufCursor) ? 4096 - PrevBufCursor : bufCursor - PrevBufCursor;
                    log = buf.Substring(PrevBufCursor, length);
                    if (PrevBufCursor > bufCursor)
                        log += buf.Substring(0, bufCursor);

                    var logLines = Regex.Split(log, @"(?<=\r\n)");

                    PrevBuf = buf;
                    PrevBufCursor = bufCursor;

                    return logLines.ToList();
                }

                PrevBuf = buf;
                PrevBufCursor = bufCursor;
                return new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }
    }
}
