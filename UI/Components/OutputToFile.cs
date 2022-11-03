using LiveSplit.Model;
using LiveSplit.TimeFormatters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    public class OutputToFile : IComponent {
        enum FileOutputLists { 
            Names,
            RT_FinishTimes,
            RT_Upcoming_FinishTimes,
			RT_Upcoming_SegmentTimes,
            RT_BigDeltas,
            RT_LittleDeltas,
            RT_GoldDeltas,
            RT_GoldHighlights,
            RT_Upcoming_GoldTimes,
            CurrentSplitIndicator,
		};
        
        public OutputToFileSettings Settings { get; set; }
        public float VerticalHeight { get; set; }
        public GraphicsCache Cache { get; set; }

        public float MinimumWidth => 0f;

        public float HorizontalWidth {
            get {
                return 0f;
            }
        }

        public IDictionary<string, Action> ContextMenuControls => null;

        public float PaddingTop => 0f;
        public float PaddingLeft => 0f;
        public float PaddingBottom => 0f;
        public float PaddingRight => 0f;

        public float MinimumHeight { get; set; }

        protected ShortTimeFormatter Formatter = new ShortTimeFormatter();

        public OutputToFile() {
            VerticalHeight = 0;
            Settings = new OutputToFileSettings();
            Cache = new GraphicsCache();
            //MakeDirectory();
        }

        public string ComponentName => "Output to File";

        public Control GetSettingsControl(LayoutMode mode) {
            return Settings;
        }

        public XmlNode GetSettings(XmlDocument document) {
            return Settings.GetSettings(document);
        }

        public void SetSettings(XmlNode settings) {
            Settings.SetSettings(settings);
        }

        /// <summary>
        /// Write current run game name, category name, & attempt count. Basically all that's in the Title component.
        /// </summary>
        /// <param name="state"></param>
        public void WriteRunInformation(LiveSplitState state) {
            // attempt history count
            Cache.Restart();
            Cache["AttemptHistoryCount"] = state.Run.AttemptCount;
            Cache["Run"] = state.Run;
            // run & category information
            Cache["GameName"] = state.Run.GameName;
            Cache["CategoryName"] = state.Run.CategoryName;
            Cache["State"] = state.CurrentPhase;
            if (Cache.HasChanged) {
                MakeFile("GameName.txt", Cache["GameName"].ToString());
                MakeFile("CategoryName.txt", Cache["CategoryName"].ToString());
                MakeFile("AttemptCount.txt", Cache["AttemptHistoryCount"].ToString());
            }
        }

        /// <summary>
        /// Writes non-timing related information about a split: Name and split index.
        /// </summary>
        /// <param name="state"></param>
        public void WriteSplitInformation(LiveSplitState state) {
            Cache.Restart();
            var currentSplit = state.CurrentSplit;
            if (state.CurrentPhase == TimerPhase.NotRunning) {
                Cache["CurrentSplitName"] = "-";
                Cache["CurrentSplitIndex"] = -1;
                Cache["PreviousSplitName"] = "-";
            }
            else if (state.CurrentPhase == TimerPhase.Ended) {
                Cache["CurrentSplitName"] = "-";
                Cache["CurrentSplitIndex"] = state.Run.Count;
                Cache["PreviousSplitName"] = state.Run[state.Run.Count - 1].Name;
            }
            else {
                Cache["CurrentSplitName"] = currentSplit.Name;
                Cache["CurrentSplitIndex"] = state.CurrentSplitIndex;
                if (state.CurrentSplitIndex >= 1) {
                    Cache["PreviousSplitName"] = state.Run[state.CurrentSplitIndex - 1].Name;
                }
                else {
                    Cache["PreviousSplitName"] = "-";
                }
            }
            if (Cache.HasChanged) {
                MakeFile("CurrentSplit_Name.txt", Cache["CurrentSplitName"].ToString());
                MakeFile("CurrentSplit_Index.txt", Cache["CurrentSplitIndex"].ToString());
                MakeFile("PreviousSplit_Name.txt", Cache["PreviousSplitName"].ToString());
            }
        }

        ///// <summary>
        ///// Writes non-timing related information about a split: Name and split index.
        ///// </summary>
        ///// <param name="state"></param>
        //public void WritePreviousSplitInformation(LiveSplitState state) {
        //    Cache.Restart();
        //    var previousSplit = state.CurrentSplitIndex - 1;
        //    if (state.CurrentPhase == TimerPhase.NotRunning) {
        //        Cache["PreviousSplitName"] = null;
        //        if (Cache.HasChanged) {
        //            MakeFile("PreviousSplit_Name.txt", "-");
        //        }
        //    }
        //    else if (state.CurrentPhase == TimerPhase.Ended) {
        //        Cache["PreviousSplitName"] = state.Run[state.Run.Count - 1].Name;
        //        if (Cache.HasChanged) {
        //            MakeFile("PreviousSplit_Name.txt", Cache["PreviousSplitName"].ToString());
        //        }
        //    }
        //    else {
        //        if (previousSplit >= 0) {
        //            Cache["PreviousSplitName"] = state.Run[previousSplit].Name;
        //            if (Cache.HasChanged) {
        //                MakeFile("PreviousSplit_Name.txt", Cache["PreviousSplitName"].ToString());
        //            }
        //        }
        //        else {
        //            Cache["PreviousSplitName"] = null;
        //            if (Cache.HasChanged) {
        //                MakeFile("PreviousSplit_Name.txt", "-");
        //            }
        //        }
        //    }
        //}

        public void WriteSplitTimes(LiveSplitState state, TimingMethod method) {
            string currentSplit_SegmentTime = "CurrentSplit_" + method.ToString() + "_Segment";
            string currentSplit_GoldTime = "CurrentSplit_" + method.ToString() + "_Gold";
            string currentSplit_FinishedAtTime = "CurrentSplit_" + method.ToString() + "_FinishedAt";

            string previousSplit_Sign = "PreviousSplit_" + method.ToString() + "_Sign";
            string previousSplit_SegmentTime = "PreviousSplit_" + method.ToString() + "_Segment";
            string previousSplit_GoldTime = "PreviousSplit_" + method.ToString() + "_Gold";
            string previousSplit_FinishedAtTime = "PreviousSplit_" + method.ToString() + "_FinishedAt";
            
            Cache.Restart();
            var currentSplit = state.CurrentSplit;

            if (state.CurrentPhase == TimerPhase.NotRunning) {
                Cache["CurrentSplitGoldTime"] = null;
                Cache["CurrentSplitPBTime"] = null;
                Cache["previousSplit_Sign"] = "Undetermined";
                if (Cache.HasChanged) {
                    MakeFile(currentSplit_SegmentTime + ".txt", "-");
                    MakeFile(currentSplit_GoldTime + ".txt", "-");
                    MakeFile(currentSplit_FinishedAtTime + ".txt", "-");
                    MakeFile(previousSplit_Sign + ".txt", "Undetermined");
                    MakeFile(previousSplit_SegmentTime + ".txt", "-");
                    MakeFile(previousSplit_GoldTime + ".txt", "-");
                    MakeFile(previousSplit_FinishedAtTime + ".txt", "-");
                }
            }
            else if (state.CurrentPhase == TimerPhase.Ended) {
                Cache["CurrentSplitGoldTime"] = null;
                Cache["CurrentSplitPBTime"] = null;

                TimeSpan? previousSplitTime = state.Run[state.CurrentSplitIndex - 1].SplitTime[method];
                TimeSpan? previousSplitPBTime = state.Run[state.CurrentSplitIndex - 1].PersonalBestSplitTime[method];

                TimeSpan? segmentTimeCurrent = previousSplitTime - state.Run[state.CurrentSplitIndex - 2].SplitTime[method];
                TimeSpan? segmentTimePB = previousSplitPBTime - state.Run[state.CurrentSplitIndex - 2].PersonalBestSplitTime[method];

                TimeSpan? differenceRun = previousSplitTime - previousSplitPBTime;
                string differenceRunString = Formatter.Format(differenceRun, TimeFormat.Minutes);
                differenceRunString = FormatTime(differenceRunString.ToString());

                string differenceGoldString = "-";
                string differenceSegmentString = "-";
                if (segmentTimeCurrent != null && segmentTimePB != null) {
                    TimeSpan? differenceSegment = segmentTimeCurrent - segmentTimePB;
                    differenceSegmentString = Formatter.Format(differenceSegment, TimeFormat.Minutes);
                    differenceSegmentString = FormatTime(differenceSegmentString.ToString());

                    TimeSpan? differenceGold = segmentTimeCurrent - state.Run[state.CurrentSplitIndex - 1].BestSegmentTime[method];
                    differenceGoldString = Formatter.Format(differenceGold, TimeFormat.Minutes);
                    differenceGoldString = FormatTime(differenceGoldString.ToString());
                }
                Cache["previousSplit_SplitIndex"] = currentSplit; // todo: Just take this all back to the Update thing that calls all these functions. They always all apply at once.
                Cache["previousSplit_Sign"] = previousSplitTime - previousSplitPBTime > TimeSpan.Zero ? "NoPB" : "PB";
                if (Cache.HasChanged) {
                    MakeFile(currentSplit_SegmentTime + ".txt", "-");
                    MakeFile(currentSplit_GoldTime + ".txt", "-");
                    MakeFile(currentSplit_FinishedAtTime + ".txt", "-");
                    MakeFile(previousSplit_Sign + ".txt", Cache["previousSplit_Sign"].ToString());
                    MakeFile(previousSplit_SegmentTime + ".txt", differenceSegmentString.ToString());
                    MakeFile(previousSplit_GoldTime + ".txt", differenceGoldString.ToString());
                    MakeFile(previousSplit_FinishedAtTime + ".txt", differenceRunString.ToString());
                }
            }
            else {
                Cache["CurrentSplitGoldTime"] = currentSplit.BestSegmentTime[TimingMethod.RealTime];

                // calculate the PB split as this value tracks the full run time instead of individual split time
                if (state.CurrentSplitIndex > 0) {
                    var previousSplit = state.Run[state.CurrentSplitIndex - 1];
                    Cache["CurrentSplitPBTime"] = currentSplit.PersonalBestSplitTime[TimingMethod.RealTime] - previousSplit.PersonalBestSplitTime[TimingMethod.RealTime];
                }
                else {
                    Cache["CurrentSplitPBTime"] = currentSplit.PersonalBestSplitTime[TimingMethod.RealTime];
                    Cache["PreviousSplitSign"] = "Undetermined";
                }

                if (Cache.HasChanged) {
                    // write the gold split
                    if (Cache["CurrentSplitGoldTime"] == null)
                        MakeFile(currentSplit_GoldTime + ".txt", "-");
                    else {
                        var timeString = Formatter.Format((TimeSpan)Cache["CurrentSplitGoldTime"], TimeFormat.Seconds);
                        int dotIndex = timeString.IndexOf(".");
                        MakeFile(currentSplit_GoldTime + ".txt", timeString.Substring(0, dotIndex + 3).ToString());
                    }

                    // write the PB split
                    if (Cache["CurrentSplitPBTime"] == null) {
                        MakeFile(currentSplit_SegmentTime + ".txt", "-");
                        MakeFile(currentSplit_FinishedAtTime + ".txt", "-");
                    }
                    else {
                        var timeString = Formatter.Format((TimeSpan)Cache["CurrentSplitPBTime"], TimeFormat.Seconds);
                        int dotIndex = timeString.IndexOf(".");
                        MakeFile(currentSplit_SegmentTime + ".txt", timeString.Substring(0, dotIndex + 3).ToString());
                        timeString = Formatter.Format(currentSplit.PersonalBestSplitTime[TimingMethod.RealTime], TimeFormat.Seconds);
                        dotIndex = timeString.IndexOf(".");
                        MakeFile(currentSplit_FinishedAtTime + ".txt", timeString.Substring(0, dotIndex + 3).ToString());
                    }

                    // calculate the PB split as this value tracks the full run time instead of individual split time
                    string differenceSegmentString = " ";
                    string differenceRunString = " ";
                    string differenceGoldString = " ";

                    Cache["previousSplit_SplitIndex"] = currentSplit;
                    if (!Cache.HasChanged) {
                        return;
                    }
                    Cache.Restart();
                    if (state.CurrentSplitIndex > 0) {
                        var previousSplit = state.Run[state.CurrentSplitIndex - 1];
                        // calculate whether the run is ahead or behind
                        TimeSpan? previousSplitTime = previousSplit.SplitTime[method];
                        TimeSpan? previousSplitPBTime = previousSplit.PersonalBestSplitTime[method];

                        bool gilded = false;
                        if (state.CurrentSplitIndex == 1) {
                            if (previousSplitTime < previousSplit.BestSegmentTime[method]) {
                                Cache["previousSplit_Sign"] = "Gold";
                                gilded = true;
                            }
                            TimeSpan? differenceSegment = previousSplit.SplitTime[method] - previousSplit.PersonalBestSplitTime[method];
                            TimeSpan? differenceRun = differenceSegment;
                            TimeSpan? differenceGold = previousSplit.SplitTime[method] - previousSplit.BestSegmentTime[method];
                            differenceSegmentString = Formatter.Format(differenceSegment, TimeFormat.Minutes);
                            differenceRunString = Formatter.Format(differenceRun, TimeFormat.Minutes);
                            differenceGoldString = Formatter.Format(differenceGold, TimeFormat.Minutes);
                            differenceSegmentString = FormatTime(differenceSegmentString.ToString());
                            differenceRunString = FormatTime(differenceRunString.ToString());
                            differenceGoldString = FormatTime(differenceGoldString.ToString());
                        }
                        else if (state.Run[state.CurrentSplitIndex - 2].SplitTime[method] != null) {
                            var segmentTime = previousSplitTime - state.Run[state.CurrentSplitIndex - 2].SplitTime[method];
                            if (segmentTime < previousSplit.BestSegmentTime[method]) {
                                Cache["previousSplit_Sign"] = "Gold";
                                gilded = true;
                            }
                            var pbSegmentTime = previousSplitPBTime - state.Run[state.CurrentSplitIndex - 2].PersonalBestSplitTime[method];
                            TimeSpan? differenceSegment = segmentTime - pbSegmentTime;
                            TimeSpan? differenceRun = previousSplitTime - previousSplitPBTime;
                            TimeSpan? differenceGold = segmentTime - previousSplit.BestSegmentTime[method];
                            differenceSegmentString = Formatter.Format(differenceSegment, TimeFormat.Minutes);
                            differenceRunString = Formatter.Format(differenceRun, TimeFormat.Minutes);
                            differenceGoldString = Formatter.Format(differenceGold, TimeFormat.Minutes);
                            differenceSegmentString = FormatTime(differenceSegmentString.ToString());
                            differenceRunString = FormatTime(differenceRunString.ToString());
                            differenceGoldString = FormatTime(differenceGoldString.ToString());
                        }
                        else if (previousSplitTime != null && previousSplitPBTime != null) {
                            Cache["previousSplit_Sign"] = previousSplitTime - previousSplitPBTime > TimeSpan.Zero ? "Behind" : "Ahead";
                            TimeSpan? differenceRun = previousSplit.SplitTime[method] - previousSplit.PersonalBestSplitTime[method];
                            differenceRunString = Formatter.Format(differenceRun, TimeFormat.Minutes);
                            differenceRunString = FormatTime(differenceRunString.ToString());
                            differenceSegmentString = "-";
                            differenceGoldString = "-";
                        }
                        if (!gilded) {
                            if (previousSplitTime != null && previousSplitPBTime != null) {
                                Cache["previousSplit_Sign"] = previousSplitTime - previousSplitPBTime > TimeSpan.Zero ? "Behind" : "Ahead";
                            }
                            else {
                                Cache["previousSplit_Sign"] = "Undetermined";
                                differenceSegmentString = "-";
                                differenceRunString = "-";
                                differenceGoldString = "-";
                            }
                        }
                    }
                    else {
                        Cache["previousSplit_Sign"] = "Undetermined";
                        differenceSegmentString = "-";
                        differenceRunString = "-";
                        differenceGoldString = "-";
                    }
                    MakeFile(previousSplit_Sign + ".txt", Cache["previousSplit_Sign"].ToString());
                    MakeFile(previousSplit_SegmentTime + ".txt", differenceSegmentString.ToString());
                    MakeFile(previousSplit_GoldTime + ".txt", differenceGoldString.ToString());
                    MakeFile(previousSplit_FinishedAtTime + ".txt", differenceRunString.ToString());
                }
            }
        }

        //public void WritePreviousSplitTimes(LiveSplitState state, TimingMethod method) {
        //    string previousSplit_Sign = "PreviousSplit_" + method.ToString() + "_Sign";
        //    string previousSplit_SegmentTime = "PreviousSplit_" + method.ToString() + "_Segment";
        //    string previousSplit_GoldTime = "PreviousSplit_" + method.ToString() + "_Gold";
        //    string previousSplit_RunTime = "PreviousSplit_" + method.ToString() + "_Run";
        //    Cache.Restart();
        //    var currentSplit = state.CurrentSplit;

        //    if (state.CurrentPhase == TimerPhase.NotRunning) {
        //        Cache["previousSplit_Sign"] = "Undetermined";
        //        if (Cache.HasChanged) {
        //            MakeFile(previousSplit_Sign + ".txt", "Undetermined");
        //            MakeFile(previousSplit_SegmentTime + ".txt", "-");
        //            MakeFile(previousSplit_GoldTime + ".txt", "-");
        //            MakeFile(previousSplit_RunTime + ".txt", "-");
        //        }
        //    }
        //    else if (state.CurrentPhase == TimerPhase.Ended) {
        //        TimeSpan? previousSplitTime = state.Run[state.CurrentSplitIndex - 1].SplitTime[method];
        //        TimeSpan? previousSplitPBTime = state.Run[state.CurrentSplitIndex - 1].PersonalBestSplitTime[method];

        //        TimeSpan? segmentTimeCurrent = previousSplitTime - state.Run[state.CurrentSplitIndex - 2].SplitTime[method];
        //        TimeSpan? segmentTimePB = previousSplitPBTime - state.Run[state.CurrentSplitIndex - 2].PersonalBestSplitTime[method];

        //        TimeSpan? differenceRun = previousSplitTime - previousSplitPBTime;
        //        string differenceRunString = Formatter.Format(differenceRun, TimeFormat.Minutes);
        //        differenceRunString = FormatTime(differenceRunString.ToString());

        //        string differenceGoldString = "-";
        //        string differenceSegmentString = "-";
        //        if (segmentTimeCurrent != null && segmentTimePB != null) {
        //            TimeSpan? differenceSegment = segmentTimeCurrent - segmentTimePB;
        //            differenceSegmentString = Formatter.Format(differenceSegment, TimeFormat.Minutes);
        //            differenceSegmentString = FormatTime(differenceSegmentString.ToString());

        //            TimeSpan? differenceGold = segmentTimeCurrent - state.Run[state.CurrentSplitIndex - 1].BestSegmentTime[method];
        //            differenceGoldString = Formatter.Format(differenceGold, TimeFormat.Minutes);
        //            differenceGoldString = FormatTime(differenceGoldString.ToString());
        //        }
        //        Cache["previousSplit_SplitIndex"] = currentSplit;
        //        Cache["previousSplit_Sign"] = previousSplitTime - previousSplitPBTime > TimeSpan.Zero ? "NoPB" : "PB";
        //        if (Cache.HasChanged) {
        //            MakeFile(previousSplit_Sign + ".txt", Cache["previousSplit_Sign"].ToString());
        //            MakeFile(previousSplit_SegmentTime + ".txt", differenceSegmentString.ToString());
        //            MakeFile(previousSplit_GoldTime + ".txt", differenceGoldString.ToString());
        //            MakeFile(previousSplit_RunTime + ".txt", differenceRunString.ToString());
        //        }
        //    }
        //    else {
        //        // calculate the PB split as this value tracks the full run time instead of individual split time
        //        string differenceSegmentString = " ";
        //        string differenceRunString = " ";
        //        string differenceGoldString = " ";

        //        Cache["previousSplit_SplitIndex"] = currentSplit;
        //        if (!Cache.HasChanged) {
        //            return;
        //        }
        //        Cache.Restart();
        //        if (state.CurrentSplitIndex > 0) {
        //            var previousSplit = state.Run[state.CurrentSplitIndex - 1];
        //            // calculate whether the run is ahead or behind
        //            TimeSpan? previousSplitTime = previousSplit.SplitTime[method];
        //            TimeSpan? previousSplitPBTime = previousSplit.PersonalBestSplitTime[method];

        //            bool gilded = false;
        //            if (state.CurrentSplitIndex == 1) {
        //                if (previousSplitTime < previousSplit.BestSegmentTime[method]) {
        //                    Cache["previousSplit_Sign"] = "Gold";
        //                    gilded = true;
        //                }
        //                TimeSpan? differenceSegment = previousSplit.SplitTime[method] - previousSplit.PersonalBestSplitTime[method];
        //                TimeSpan? differenceRun = differenceSegment;
        //                TimeSpan? differenceGold = previousSplit.SplitTime[method] - previousSplit.BestSegmentTime[method];
        //                differenceSegmentString = Formatter.Format(differenceSegment, TimeFormat.Minutes);
        //                differenceRunString = Formatter.Format(differenceRun, TimeFormat.Minutes);
        //                differenceGoldString = Formatter.Format(differenceGold, TimeFormat.Minutes);
        //                differenceSegmentString = FormatTime(differenceSegmentString.ToString());
        //                differenceRunString = FormatTime(differenceRunString.ToString());
        //                differenceGoldString = FormatTime(differenceGoldString.ToString());
        //            }
        //            else if (state.Run[state.CurrentSplitIndex - 2].SplitTime[method] != null) {
        //                var segmentTime = previousSplitTime - state.Run[state.CurrentSplitIndex - 2].SplitTime[method];
        //                if (segmentTime < previousSplit.BestSegmentTime[method]) {
        //                    Cache["previousSplit_Sign"] = "Gold";
        //                    gilded = true;
        //                }
        //                var pbSegmentTime = previousSplitPBTime - state.Run[state.CurrentSplitIndex - 2].PersonalBestSplitTime[method];
        //                TimeSpan? differenceSegment = segmentTime - pbSegmentTime;
        //                TimeSpan? differenceRun = previousSplitTime - previousSplitPBTime;
        //                TimeSpan? differenceGold = segmentTime - previousSplit.BestSegmentTime[method];
        //                differenceSegmentString = Formatter.Format(differenceSegment, TimeFormat.Minutes);
        //                differenceRunString = Formatter.Format(differenceRun, TimeFormat.Minutes);
        //                differenceGoldString = Formatter.Format(differenceGold, TimeFormat.Minutes);
        //                differenceSegmentString = FormatTime(differenceSegmentString.ToString());
        //                differenceRunString = FormatTime(differenceRunString.ToString());
        //                differenceGoldString = FormatTime(differenceGoldString.ToString());
        //            }
        //            else if (previousSplitTime != null && previousSplitPBTime != null) {
        //                Cache["previousSplit_Sign"] = previousSplitTime - previousSplitPBTime > TimeSpan.Zero ? "Behind" : "Ahead"; 
        //                TimeSpan? differenceRun = previousSplit.SplitTime[method] - previousSplit.PersonalBestSplitTime[method];
        //                differenceRunString = Formatter.Format(differenceRun, TimeFormat.Minutes);
        //                differenceRunString = FormatTime(differenceRunString.ToString());
        //                differenceSegmentString = "-";
        //                differenceGoldString = "-";
        //            }
        //            if (!gilded) {
        //                if (previousSplitTime != null && previousSplitPBTime != null) {
        //                    Cache["previousSplit_Sign"] = previousSplitTime - previousSplitPBTime > TimeSpan.Zero ? "Behind" : "Ahead";
        //                }
        //                else {
        //                    Cache["previousSplit_Sign"] = "Undetermined";
        //                    differenceSegmentString = "-";
        //                    differenceRunString = "-";
        //                    differenceGoldString = "-";
        //                }
        //            }
        //        }
        //        else {
        //            Cache["previousSplit_Sign"] = "Undetermined";
        //            differenceSegmentString = "-";
        //            differenceRunString = "-";
        //            differenceGoldString = "-";
        //        }
        //        MakeFile(previousSplit_Sign + ".txt", Cache["previousSplit_Sign"].ToString());
        //        MakeFile(previousSplit_SegmentTime + ".txt", differenceSegmentString.ToString());
        //        MakeFile(previousSplit_GoldTime + ".txt", differenceGoldString.ToString());
        //        MakeFile(previousSplit_RunTime + ".txt", differenceRunString.ToString());
        //    }
        //}
    
        public void WriteSplitList(LiveSplitState state) {
            Cache.Restart();
            var currentSplit = state.CurrentSplit;
            int splitIndex = -2;
            if (state.CurrentPhase == TimerPhase.NotRunning) {
                Cache["CurrentSplitListIndex"] = -1;
                splitIndex = -1;
            }
            else if (state.CurrentPhase == TimerPhase.Ended) {
                Cache["CurrentSplitListIndex"] = state.Run.Count;
                splitIndex = state.Run.Count;
            }
            else {
                Cache["CurrentSplitListIndex"] = state.CurrentSplitIndex;
                splitIndex = state.CurrentSplitIndex;
            }

            if (Cache.HasChanged) {
                Dictionary<FileOutputLists, string> outputLists = new Dictionary<FileOutputLists, string>();
                foreach (FileOutputLists fol in Enum.GetValues(typeof(FileOutputLists))) {
                    outputLists.Add(fol, "");
				}

                // determine whether we are too close to the start or end of the run and if so adjust boundaries to still fit the desired amount of splits
                int lowerB = splitIndex - Settings.SplitsBefore;
                int upperB = splitIndex + Settings.SplitsAfter;
                int lowerBound = 0;
                int upperBound = state.Run.Count;
                if (lowerB < 0) {
                    upperBound = Math.Min(Settings.SplitsBefore + Settings.SplitsAfter, state.Run.Count);
				}
                else if (upperB >= state.Run.Count) {
                    lowerBound = Math.Max(state.Run.Count - Settings.SplitsAfter - Settings.SplitsBefore - 1, 0);
                }
                else {
                    lowerBound = Math.Max(splitIndex - Settings.SplitsBefore, 0);
                    upperBound = Math.Min(splitIndex + Settings.SplitsAfter, state.Run.Count);
                }

                // now we go through each split
                for (int i = lowerBound; i <= upperBound; i++) {
                    if (i == upperBound && upperBound != state.Run.Count) {
                        i = state.Run.Count - 1;
					}
                    
                    if (i >= 0 && i < state.Run.Count) {
                        outputLists[FileOutputLists.Names] += state.Run[i].Name;

                        // time the split was finished at, else information about upcoming splits
                        string finishTime = "";
                        string finishTimeUpcoming = "";
                        string goldTimeUpcoming = "";
                        if (i < splitIndex) {
                            finishTime = Formatter.Format(state.Run[i].SplitTime[TimingMethod.RealTime], TimeFormat.Minutes);
                        }
                        else {
                            finishTimeUpcoming = Formatter.Format(state.Run[i].PersonalBestSplitTime[TimingMethod.RealTime], TimeFormat.Minutes);
                            goldTimeUpcoming = Formatter.Format(state.Run[i].BestSegmentTime[TimingMethod.RealTime], TimeFormat.Minutes);
						}

                        if (!string.IsNullOrEmpty(finishTime)) finishTime = finishTime[0] == '0' ? finishTime.Substring(1, finishTime.Length - 4) : finishTime.Substring(0, finishTime.Length - 3);
                        if (!string.IsNullOrEmpty(finishTimeUpcoming)) finishTimeUpcoming = finishTimeUpcoming[0] == '0' ? finishTimeUpcoming.Substring(1, finishTimeUpcoming.Length - 4) : finishTimeUpcoming.Substring(0, finishTimeUpcoming.Length - 3);
                        if (!string.IsNullOrEmpty(goldTimeUpcoming)) goldTimeUpcoming = goldTimeUpcoming[0] == '0' ? goldTimeUpcoming.Substring(1, goldTimeUpcoming.Length - 4) : goldTimeUpcoming.Substring(0, goldTimeUpcoming.Length - 3);

                        outputLists[FileOutputLists.RT_FinishTimes] += finishTime;
                        outputLists[FileOutputLists.RT_Upcoming_FinishTimes] += finishTimeUpcoming;
                        outputLists[FileOutputLists.RT_Upcoming_GoldTimes] += goldTimeUpcoming;

                        if (i == splitIndex) {
                            outputLists[FileOutputLists.CurrentSplitIndicator] += "███████████████████████████████████████████";
                        }

                        // deltas
                        if (i < splitIndex) {
                            // big delta
                            var difference = state.Run[i].SplitTime[TimingMethod.RealTime] - state.Run[i].PersonalBestSplitTime[TimingMethod.RealTime];
                            string diffStr = Formatter.Format(difference, TimeFormat.Minutes);
                            diffStr = FormatTime(diffStr);
                            outputLists[FileOutputLists.RT_BigDeltas] += diffStr;


                            // little delta
                            string goldDifferenceString;
                            string individualDifferenceString;
                            if (i == 0) { 
                                individualDifferenceString = diffStr;
                                
                                // gold for first split
                                if (state.Run[i].SplitTime[TimingMethod.RealTime] < state.Run[i].BestSegmentTime[TimingMethod.RealTime]) {
                                    outputLists[FileOutputLists.RT_GoldHighlights] += "███████████████████████████████████████████";
                                }
                                var goldDifference = state.Run[i].SplitTime[TimingMethod.RealTime] - state.Run[i].BestSegmentTime[TimingMethod.RealTime];
                                string goldDiffStr = Formatter.Format(goldDifference, TimeFormat.Minutes);
                                goldDiffStr = FormatTime(goldDiffStr);
                                goldDifferenceString = goldDiffStr;
                            }
                            else { 
                                var individualSplitTimeOld = state.Run[i].PersonalBestSplitTime[TimingMethod.RealTime] - state.Run[i - 1].PersonalBestSplitTime[TimingMethod.RealTime];
                                var individualSplitTimeNew = state.Run[i].SplitTime[TimingMethod.RealTime] - state.Run[i - 1].SplitTime[TimingMethod.RealTime];
                                var littleDifference = individualSplitTimeNew - individualSplitTimeOld;
                                individualDifferenceString = Formatter.Format(littleDifference, TimeFormat.Minutes);
                                individualDifferenceString = FormatTime(individualDifferenceString);

                                // gold for second through last split
                                if (individualSplitTimeNew < state.Run[i].BestSegmentTime[TimingMethod.RealTime]) {
                                    outputLists[FileOutputLists.RT_GoldHighlights] += "███████████████████████████████████████████";
                                }
                                var individualGoldDifference = individualSplitTimeNew - state.Run[i].BestSegmentTime[TimingMethod.RealTime];
                                goldDifferenceString = Formatter.Format(individualGoldDifference, TimeFormat.Minutes);
                                goldDifferenceString = FormatTime(goldDifferenceString);
                            }
                            outputLists[FileOutputLists.RT_LittleDeltas] += individualDifferenceString;
                            outputLists[FileOutputLists.RT_GoldDeltas] += goldDifferenceString;
                        }
                        else {
                            // write the individual segment time for upcoming segments
                            string upcomingSegTime;
                            if (i == 0) {
                                var time = state.Run[i].PersonalBestSplitTime[TimingMethod.RealTime];
                                upcomingSegTime = Formatter.Format(time, TimeFormat.Minutes);
                            }
                            else {
                                var time = state.Run[i].PersonalBestSplitTime[TimingMethod.RealTime] - state.Run[i - 1].PersonalBestSplitTime[TimingMethod.RealTime];
                                upcomingSegTime = Formatter.Format(time, TimeFormat.Minutes);
                            }
                            if (!string.IsNullOrEmpty(upcomingSegTime)) upcomingSegTime = upcomingSegTime[0] == '0' ? upcomingSegTime.Substring(1, upcomingSegTime.Length - 4) : upcomingSegTime.Substring(0, upcomingSegTime.Length - 3);
                            outputLists[FileOutputLists.RT_Upcoming_SegmentTimes] += upcomingSegTime;
                        }
                    }
                    // add new lines to every file
                    if (i < upperBound) {
                        foreach (FileOutputLists k in outputLists.Keys.ToList()) {
                            outputLists[k] += Environment.NewLine;
						}
                    } 
                }
                // write the files
                foreach (FileOutputLists k in outputLists.Keys) {
                    MakeFile("List_" + k + ".txt", outputLists[k]);
                }
            }
        }

        public string FormatTime(string diffStr) {
            if (diffStr.Length < 5) {
                return "";
			}
            if (diffStr[0] != '−') {
                diffStr = "+" + diffStr;
            }
            if (diffStr.Substring(1, 5) == "00:00") {
                //±00:00.xx > ±.xx
                return diffStr.Remove(1, 5);
            }
            else if (diffStr.Substring(1, 4) == "00:0") {
                //±00:0x.xx > ±x.x
                diffStr = diffStr.Remove(1, 4);
                return diffStr.Substring(0, diffStr.Length - 1);
            }
            else if (diffStr.Substring(1, 3) == "00:") {
                //±00:xx.xx > ±xx.x
                diffStr = diffStr.Remove(1, 3);
                return diffStr.Substring(0, diffStr.Length - 1);
            }
            else if (diffStr[1] == '0') {
                //±0x:xx.xx > ±x:xx
                diffStr = diffStr.Remove(1, 1);
                return diffStr.Substring(0, diffStr.Length - 3);
            }
            else {
                //±xx:xx.xx > ±xx:xx
                return diffStr.Substring(0, diffStr.Length - 3);
            }
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode) { 
            WriteRunInformation(state);
            WriteSplitInformation(state);
            WriteSplitTimes(state, TimingMethod.RealTime);
            WriteSplitTimes(state, TimingMethod.GameTime);
            //WritePreviousSplitInformation(state);
            //WritePreviousSplitTimes(state, TimingMethod.RealTime);
            //WritePreviousSplitTimes(state, TimingMethod.GameTime);
            WriteSplitList(state);
        }

        public void Dispose() { }
        
        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion) { }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion) { }

        public int GetSettingsHashCode() => Settings.GetSettingsHashCode();

        void MakeFile(string fileName, string items) {
            string settingsPath = Settings.FolderPath;
            if (string.IsNullOrEmpty(settingsPath)) return;
            string path = Path.Combine(settingsPath, fileName);
            File.WriteAllText(path, items);
        }
    }
}
