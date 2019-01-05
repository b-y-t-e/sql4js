using System;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace sql4js.Helpers.CoreHelpers
{
    /*
    public class LoggingStatic
    {
        public static String APP_DIR
        {
            get
            {
                var lUri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
                return Path.GetDirectoryName(lUri.LocalPath);
            }
        }

        public static String LOG_DIRECTORY
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["LOG_DIRECTORY"];
            }
        }

        public static String BASEDIR
        {
            get
            {
                var lLogDirectory = LOG_DIRECTORY;
                if (!string.IsNullOrEmpty(lLogDirectory))
                {
                    return lLogDirectory.TrimEnd(new[] { '/', '\\' });
                }
                else
                {
                    return APP_DIR.TrimEnd(new[] { '/', '\\' });
                }
            }
        }

        public static String DATE
        {
            get
            {
                var lYear = DateTime.Now.Year.ToString();
                var lMonth = DateTime.Now.Month.ToString();
                var lDay = DateTime.Now.Day.ToString();
                if (lMonth.Length == 1) lMonth = "0" + lMonth;
                if (lDay.Length == 1) lDay = "0" + lDay;
                return String.Format("{0}-{1}-{2}", lYear, lMonth, lDay);
            }
        }
    }

    public class LoggingFileConfiguration
    {
        // private String _realPath;
        public String GetRealPath()
        {
            var lRealPath = _filePath ?? "";
            lRealPath = lRealPath.Replace("{BASEDIR}", LoggingStatic.BASEDIR);
            lRealPath = lRealPath.Replace("{DATE}", LoggingStatic.DATE);
            return lRealPath;
        }

        private String _filePath;
        public String FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                _filePath = value;
            }
        }

        //////////////////////////////

        public LoggingFileConfiguration()
        {
            FilePath = "{BASEDIR}\\LOG\\{DATE}.log";
        }
    }

    public class LoggingConfiguration
    {
        private Dictionary<String, List<LoggingFileConfiguration>> _fileConfogurations;

        //////////////////

        public static readonly String ALL_TAG = "ALL";

        //////////////////

        public void Unregister(String Name)
        {
            var lName = Name.ToUpper().Trim();
            if (lName.Equals(ALL_TAG))
            {
                throw new NotSupportedException("Nie można modyfikować tagu ALL!");
            }
            else
            {
                if (_fileConfogurations.ContainsKey(lName))
                    _fileConfogurations.Remove(lName);
            }
        }

        public void Register(LoggingFileConfiguration FileConfiguration, String Name)
        {
            var lName = Name.ToUpper().Trim();
            if (!_fileConfogurations.ContainsKey(lName)) _fileConfogurations.Add(lName, new List<LoggingFileConfiguration>());
            _fileConfogurations[lName].Add(FileConfiguration);
        }

        public void Register(String FilePath, params String[] Names)
        {
            if (Names != null)
            {
                foreach (var lName in Names)
                {
                    this.Register(new LoggingFileConfiguration() { FilePath = FilePath }, lName);
                }
            }
        }

        //////////////////

        private LoggingFileConfiguration[] GetConfigurationsForAll()
        {
            if (_fileConfogurations.ContainsKey(ALL_TAG))
                return _fileConfogurations[ALL_TAG].ToArray();
            return new LoggingFileConfiguration[0];
        }

        private LoggingFileConfiguration[] GetConfigurations(String Name)
        {
            var lName = Name.ToUpper().Trim();
            if (_fileConfogurations.ContainsKey(lName))
                return _fileConfogurations[lName].ToArray();
            return new LoggingFileConfiguration[0];
        }

        public String[] GetFiles(String Name, Boolean AllFile)
        {
            var lName = Name.ToUpper().Trim();
            if (_fileConfogurations.ContainsKey(lName))
            {
                var lList = new List<String>();
                foreach (var lItem in GetConfigurations(lName))
                {
                    lList.Add(lItem.GetRealPath());
                }
                if (AllFile)
                {
                    foreach (var lItem in GetConfigurationsForAll())
                    {
                        lList.Add(lItem.GetRealPath());
                    }
                }
                return lList.ToArray();
            }
            return new String[0];
        }

        //////////////////

        public LoggingConfiguration()
        {
            _fileConfogurations = new Dictionary<String, List<LoggingFileConfiguration>>();
        }

        //////////////////

        public static LoggingConfiguration CreateDefault()
        {
            var lLoggingConfiguration = new LoggingConfiguration();
            lLoggingConfiguration.Register("{BASEDIR}\\LOG\\ALL_{DATE}.log", ALL_TAG);
            lLoggingConfiguration.Register("{BASEDIR}\\LOG\\SQL_{DATE}.log", "SQL");
            lLoggingConfiguration.Register("{BASEDIR}\\LOG\\DEBUG_{DATE}.log", "DEBUG");
            lLoggingConfiguration.Register("{BASEDIR}\\LOG\\INFO_{DATE}.log", "INFO");
            lLoggingConfiguration.Register("{BASEDIR}\\LOG\\WARNING_{DATE}.log", "WARNING");
            lLoggingConfiguration.Register("{BASEDIR}\\LOG\\MEMORY_{DATE}.log", "MEMORY");
            lLoggingConfiguration.Register("{BASEDIR}\\LOG\\ERROR_{DATE}.log", "ERROR");
            lLoggingConfiguration.Register("{BASEDIR}\\LOG\\EMAIL_{DATE}.log", "EMAIL");
            lLoggingConfiguration.Register("{BASEDIR}\\LOG\\IMPORTANT_ERROR_{DATE}.log", "IMPORTANT_ERROR");
            lLoggingConfiguration.Register("{BASEDIR}\\LOG\\INTERNAL_ERROR_{DATE}.log", "INTERNAL_ERROR");
            lLoggingConfiguration.Register("{BASEDIR}\\LOG\\CLIENTERROR_{DATE}.log", "CLIENTERROR");
            lLoggingConfiguration.Register("{BASEDIR}\\LOG\\TECH_{DATE}.log", "TECH");
            lLoggingConfiguration.Register("{BASEDIR}\\LOG\\POWTORZONE_PACZKI_{DATE}.log", "POWTORZONEPACZKI");
            return lLoggingConfiguration;
        }
    }

    public static class GlobalLogger
    {
        public static Logging _i;

        public static Logging I
        {
            get
            {
                if (_i == null) _i = new Logging();
                return _i;
            }
        }
    }

    public class Logging
    {
        private static DateTime? lastRemove = null;

        public static Int32 MaxDays = 31;

        public static LoggingConfiguration Configuration = new LoggingConfiguration();

        public static Action<String> AdditionalAction;

        public static Action<String> AdditionalActionError;

        public Logging()
        {
            Configuration = LoggingConfiguration.CreateDefault();
        }

        public static String ToDisplayString(Exception Ex)
        {
            StringBuilder lStr = new StringBuilder();
            Exception lEx = Ex;
            while (lEx != null)
            {
                if (lStr.Length > 0) lStr.Append(" | ");
                lStr.Append(lEx.Message);
                lEx = lEx.InnerException;
            }
            return lStr.ToString();
        }

        ////////////////////////////////////

        public void ImportantError(Exception Ex)
        {
            if (Ex != null)
            {
                Write("IMPORTANT_ERROR", ToDisplayString(Ex) + " | " + Ex.StackTrace, true);
                if (AdditionalActionError != null) AdditionalActionError.Invoke(ToDisplayString(Ex) + " | " + Ex.StackTrace);
            }
        }

        public void ImportantError(String Text, params Object[] Values)
        {
            Write("IMPORTANT_ERROR", Text, true, Values);
            if (AdditionalActionError != null) AdditionalActionError.Invoke(Text);
        }

        ////////////////////////////////////

        public void ClientError(Exception Ex)
        {
            if (Ex != null)
            {
                Write("CLIENTERROR", ToDisplayString(Ex) + " | " + Ex.StackTrace, true);
                if (AdditionalActionError != null) AdditionalActionError.Invoke(ToDisplayString(Ex) + " | " + Ex.StackTrace);
            }
        }

        public void ClientError(String Text, params Object[] Values)
        {
            Write("CLIENTERROR", Text, true, Values);
            if (AdditionalActionError != null) AdditionalActionError.Invoke(Text);
        }

        ////////////////////////////////////

        public void InternalError(Exception Ex)
        {
            if (Ex != null)
            {
                Write("INTERNAL_ERROR", ToDisplayString(Ex) + " | " + Ex.StackTrace, true);
                if (AdditionalActionError != null) AdditionalActionError.Invoke(ToDisplayString(Ex) + " | " + Ex.StackTrace);
            }
        }

        public void InternalError(String Text, params Object[] Values)
        {
            Write("INTERNAL_ERROR", Text, true, Values);
            if (AdditionalActionError != null) AdditionalActionError.Invoke(Text);
        }

        ////////////////////////////////////

        public void Error(Exception Ex)
        {
            if (Ex != null)
            {
                Write("ERROR", ToDisplayString(Ex) + " | " + Ex.StackTrace, true);
                if (AdditionalActionError != null) AdditionalActionError.Invoke(ToDisplayString(Ex) + " | " + Ex.StackTrace);
            }
        }

        public void Error(String Text, params Object[] Values)
        {
            Write("ERROR", Text, true, Values);
            if (AdditionalActionError != null) AdditionalActionError.Invoke(Text);
        }

        ////////////////////////////////////

        public void Info(String Text, params Object[] Values)
        {
            Write("INFO", Text, true, Values);
            if (AdditionalAction != null) AdditionalAction.Invoke(Text);
        }

        public void Warning(String Text, params Object[] Values)
        {
            Write("WARNING", Text, true, Values);
            if (AdditionalAction != null) AdditionalAction.Invoke(Text);
        }

        public void Debug(String Text, params Object[] Values)
        {
            Write("DEBUG", Text, true, Values);
            if (AdditionalAction != null) AdditionalAction.Invoke(Text);
        }

        public void Email(String Text, params Object[] Values)
        {
            Write("EMAIL", Text, true, Values);
            if (AdditionalAction != null) AdditionalAction.Invoke(Text);
        }

        public void SQL(String Text, params Object[] Values)
        {
            Write("SQL", Text, false, Values);
            if (AdditionalAction != null) AdditionalAction.Invoke(Text);
        }

        public void Memory(String Text, params Object[] Values)
        {
            Write("MEMORY", Text, false, Values);
            if (AdditionalAction != null) AdditionalAction.Invoke(Text);
        }

        public void Tech(String Text, params Object[] Values)
        {
            Write("TECH", Text, false, Values);
            if (AdditionalAction != null) AdditionalAction.Invoke(Text);
        }

        public void PowtorzonePaczki(String Text, params Object[] Values)
        {
            Write("POWTORZONEPACZKI", Text, false, Values);
            if (AdditionalAction != null) AdditionalAction.Invoke(Text);
        }

        ////////////////////////////////////

        private void RunSectionForAction(Action BeginAction, Action<Stoper> EndAction,  Action MainAction, Boolean InternalAction)
        {
            Stoper st = new Stoper();
            st.Start();
            try
            {
                BeginAction();
                MainAction();
            }
            catch (Exception ex)
            {
                st.Pause();
                if (InternalAction) InternalError(ex);
                else Error(ex);
                throw;
            }
            finally
            {
                st.Pause();
                EndAction(st);
            }
        }

        private T RunSectionForFunction<T>(Action BeginAction, Action<Stoper> EndAction,  Func<T> MainAction, Boolean InternalAction)
        {
            Stoper st = new Stoper();
            st.Start();
            T lResult = default(T);
            try
            {
                BeginAction();
                lResult = MainAction();
            }
            catch (Exception ex)
            {
                st.Pause();
                if (InternalAction) InternalError(ex);
                else Error(ex);
                throw;
            }
            finally
            {
                st.Pause();
                EndAction(st);
            }
            return lResult;
        }

        ////////////////////////////////////

        public void MeasureTime(String Text, Action Action)
        {
            Stoper st = new Stoper();
            st.Measure(() =>
            {
                if (Action != null) Action();
            });
            Info(Text + " " + st.Result);
        }

        public T MeasureTime<T>(String Text, Func<T> Action)
        {
            Stoper st = new Stoper();
            var r = st.Measure(() =>
            {
                if (Action != null) return Action();
                return default(T);
            });
            Info(Text + " " + st.Result);
            return r;
        }

        ////////////////////////////////////

        public void SectionInfo(String Text, String Token, Action Action) { SectionInfo(Text, Token, false, Action); }
        public T SectionInfo<T>(String Text, String Token, Func<T> Action) { return SectionInfo<T>(Text, Token, false, Action); }

        public void SectionInfo(String Text, String Token, Boolean InternalAction, Action Action)
        {
            RunSectionForAction(
                () =>
                {
                    Info("BEGIN {0}", Text);
                },
                (st) =>
                {
                    Info("END {0} ({1})", Text, st.Result);

                },
                Action, InternalAction);
        }

        public T SectionInfo<T>(String Text, String Token, Boolean InternalAction, Func<T> Action)
        {
            return RunSectionForFunction(
                () =>
                {
                    Info("BEGIN {0}", Text);
                },
                (st) =>
                {
                    Info("END {0} ({1})", Text, st.Result);
                },
                Action, InternalAction);
        }

        ////////////////////////////////////

        public void SectionWarning(String Text, String Token, Action Action) { SectionWarning(Text, Token, false, Action); }
        public T SectionWarning<T>(String Text, String Token, Func<T> Action) { return SectionWarning<T>(Text, Token, false, Action); }

        public void SectionWarning(String Text, String Token, Boolean InternalAction, Action Action)
        {
            RunSectionForAction(
                () =>
                {
                    Warning("BEGIN {0}", Text);
                },
                (st) =>
                {
                    Warning("END {0} ({1})", Text, st.Result);
                },
                Action, InternalAction);
        }

        public T SectionWarning<T>(String Text, String Token, Boolean InternalAction, Func<T> Action)
        {
            return RunSectionForFunction(
                () =>
                {
                    Warning("BEGIN {0}", Text);
                },
                (st) =>
                {
                    Warning("END {0} ({1})", Text, st.Result);
                },
                Action, InternalAction);
        }

        ////////////////////////////////////

        public void SectionDebug(String Text, String Token, Action Action) { SectionDebug(Text, Token, false, Action); }
        public T SectionDebug<T>(String Text, String Token, Func<T> Action) { return SectionDebug<T>(Text, Token, false, Action); }

        public void SectionDebug(String Text, String Token, Boolean InternalAction, Action Action)
        {
            RunSectionForAction(
                () =>
                {
                    Debug("BEGIN {0}", Text);
                },
                (st) =>
                {
                    Debug("END {0} ({1})", Text, st.Result);
                },
                Action, InternalAction);
        }

        public T SectionDebug<T>(String Text, String Token, Boolean InternalAction, Func<T> Action)
        {
            return RunSectionForFunction(
                () =>
                {
                    Debug("BEGIN {0}", Text);
                },
                (st) =>
                {
                    Debug("END {0} ({1})", Text, st.Result);
                },
                Action, InternalAction);
        }

        ////////////////////////////////////

        public void SectionTech(String Text, String Token, Action Action) { SectionTech(Text, Token, false, Action); }
        public T SectionTech<T>(String Text, String Token, Func<T> Action) { return SectionTech<T>(Text, Token, false, Action); }

        public void SectionTech(String Text, String Token, Boolean InternalAction, Action Action)
        {
            RunSectionForAction(
                () =>
                {
                    Tech("BEGIN {0}", Text);
                },
                (st) =>
                {
                    Tech("END {0} ({1})", Text, st.Result);
                },
                Action, InternalAction);
        }

        public T SectionTech<T>(String Text, String Token, Boolean InternalAction, Func<T> Action)
        {
            return RunSectionForFunction(
                () =>
                {
                    Tech("BEGIN {0}", Text);
                },
                (st) =>
                {
                    Tech("END {0} ({1})", Text, st.Result);
                },
                Action, InternalAction);
        }

        ////////////////////////////////////

        private void Write(String Type, String Text, Boolean AllFile, params Object[] Values)
        {
            if (Configuration != null)
            {
                RemoveOldLogs(MaxDays);

                try
                {
                    foreach (var lFile in Configuration.GetFiles(Type, AllFile))
                    {
                        var lDirectory = Path.GetDirectoryName(lFile);
                        if (!Directory.Exists(lDirectory)) Directory.CreateDirectory(lDirectory);

                        WriteLine(
                            lFile,
                            String.Format(
                                "{0}:{1}:{2}",
                                DateTime.Now,
                                Type,
                                String.Format(Text, Values)));
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        private void RemoveOldLogs(Int32 MaxDays)
        {
            if (lastRemove != null && TimeSpan.FromTicks(DateTime.Now.Ticks - lastRemove.Value.Ticks) < TimeSpan.FromHours(3))
                return;

            try
            {
                String directory = "{BASEDIR}\\LOG";
                directory = directory.Replace("{BASEDIR}", LoggingStatic.BASEDIR);

                foreach (String file in Directory.GetFiles(directory))
                {
                    String fileName = Path.GetFileNameWithoutExtension(file);
                    Int32 index = fileName.LastIndexOf("_");
                    if (index >= 0)
                    {
                        String dateString = fileName.Substring(index + 1);
                        DateTime? date = null;

                        try { date = DateTime.Parse(dateString, System.Globalization.CultureInfo.InvariantCulture); }
                        catch { }

                        if (date != null)
                        {
                            TimeSpan diff = TimeSpan.FromTicks(DateTime.Now.Ticks - date.Value.Ticks);
                            if (diff.TotalDays >= MaxDays)
                            {
                                try { File.Delete(file); }
                                catch { }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // ImportantError(ex);
            }
            finally
            {
                lastRemove = DateTime.Now;
            }
        }

        private void WriteLine(String File, String Text)
        {
            var lIloscProb = 10;
            while (lIloscProb > 0)
            {
                try
                {
                    using (var lSw = new StreamWriter(File, true))
                    {
                        lSw.WriteLine(Text);
                    }
                    lIloscProb = 0;
                }
                catch
                {
                    lIloscProb--;
                }
            }
        }
    }
    */
}