using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using YanAn.Extensions;

namespace YanAn.Logging
{
    public class LogManager
    {
        private static AutoResetEvent Pause => new(false);
        static readonly ConcurrentQueue<Tuple<string, string>> LogQueue = new();

        /// <summary>
        /// 自定义事件
        /// </summary>
        public static event Action<LogInfo>? Event;
        /// <summary>
        /// 日志存放目录，默认日志放在当前应用程序运行目录下的logs文件夹中
        /// </summary>
        public static string LogDirectory
        {
            get => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        }
        static LogManager()
        {
            var writeTask = new Task(obj =>
            {
                while (true)
                {
                    Pause.WaitOne(500, true);
                    List<string[]> temp = new();
                    foreach (var logItem in LogQueue)
                    {
                        string logPath = logItem.Item1;
                        string logMergeContent = string.Concat(logItem.Item2, Environment.NewLine, "--------------------------------------------------------------------------", Environment.NewLine);
                        string[] logArr = temp.First(d => d[0].Equals(logPath, StringComparison.Ordinal));
                        if (logArr != null)
                        {
                            logArr[1] = string.Concat(logArr[1], logMergeContent);
                        }
                        else
                        {
                            logArr = new[]
                            {
                                logPath,
                                logMergeContent
                            };
                            temp.Add(logArr);
                        }

                        LogQueue.TryDequeue(out Tuple<string, string> _);
                    }
                    foreach (var item in temp)
                    {
                        Write(item[0], item[1]);
                    }
                }
            }, null, TaskCreationOptions.LongRunning);
            writeTask.Start();
        }
        private static string GetLogPath(LogLevel logLevel, string? folder = null)
        {
            string newFilePath;
            var logDir = LogDirectory;
            if (logDir.IsNullOrEmpty())
                logDir = Path.Combine(Environment.CurrentDirectory, "logs");
            logDir = Path.Combine(logDir, logLevel.ToString());
            if (!string.IsNullOrWhiteSpace(folder))
                logDir = Path.Combine(logDir, folder.Trim('/').Trim('\\'));
            if (!Directory.Exists(logDir))
                Directory.CreateDirectory(logDir);
            string extension = ".log";
            string fileNameNotExt = DateTime.Now.ToString(Const.DateTimeConst.DateFormat);
            string fileNamePattern = string.Concat(fileNameNotExt, "(*)", extension);
            List<string> filePaths = Directory.GetFiles(logDir, fileNamePattern, SearchOption.TopDirectoryOnly).ToList();

            if (true == filePaths?.Any())
            {
                int fileMaxLen = filePaths.Max(d => d.Length);
                string lastFilePath = filePaths.Where(d => d.Length == fileMaxLen).OrderByDescending(d => d).First();
                if (new FileInfo(lastFilePath).Length > 2 * 1024 * 1024)
                {
                    var no = new Regex(@"(?is)(?<=\()(.*)(?=\))").Match(Path.GetFileName(lastFilePath)).Value;
                    var parse = int.TryParse(no, out int tempno);
                    var formatno = $"({(parse ? (tempno + 1) : tempno)})";
                    var newFileName = string.Concat(fileNameNotExt, formatno, extension);
                    newFilePath = Path.Combine(logDir, newFileName);
                }
                else
                {
                    newFilePath = lastFilePath;
                }
            }
            else
            {
                var newFileName = string.Concat(fileNameNotExt, $"({0})", extension);
                newFilePath = Path.Combine(logDir, newFileName);
            }
            return newFilePath;
        }

        private static void Write(string logPath, string logContent)
        {
            try
            {
                if (!File.Exists(logPath))
                {
                    File.CreateText(logPath).Close();
                }
                using var sw = File.AppendText(logPath);
                sw.Write(logContent);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        #region Error
        /// <summary>
        /// 写入error级别日志
        /// </summary>
        /// <param name="error">异常对象</param>
        public static void Error(Exception error)
        {
            LogInfo log = new()
            {
                LogLevel = LogLevel.Error,
                Message = error.Message,
                Source = error.Source,
                Exception = error,
                ExceptionType = error.GetType().Name
            };
            LogTo(log);
        }

        /// <summary>
        /// 写入error级别日志
        /// </summary>
        /// <param name="source">异常源的类型</param>
        /// <param name="error">异常对象</param>
        public static void Error(Type source, Exception error)
        {
            LogInfo log = new()
            {
                LogLevel = LogLevel.Error,
                Message = error.Message,
                Source = source.FullName,
                Exception = error,
                ExceptionType = error.GetType().Name
            };
            LogTo(log);
        }

        /// <summary>
        /// 写入error级别日志
        /// </summary>
        /// <param name="source">异常源的类型</param>
        /// <param name="error">异常对象</param>
        public static void Error(string source, Exception error)
        {
            LogInfo log = new()
            {
                LogLevel = LogLevel.Error,
                Message = error.Message,
                Source = source,
                Exception = error,
                ExceptionType = error.GetType().Name
            };
            LogTo(log);
        }

        #endregion Error

        #region Fatal
        /// <summary>
        /// 写入fatal级别日志
        /// </summary>
        /// <param name="fatal">异常对象</param>
        public static void Fatal(Exception fatal)
        {
            LogInfo log = new()
            {
                LogLevel = LogLevel.Fatal,
                Message = fatal.Message,
                Source = fatal.Source,
                Exception = fatal,
                ExceptionType = fatal.GetType().Name
            };
            LogTo(log);
        }

        /// <summary>
        /// 写入fatal级别日志
        /// </summary>
        /// <param name="source">异常源的类型</param>
        /// <param name="fatal">异常对象</param>
        public static void Fatal(Type source, Exception fatal)
        {
            LogInfo log = new()
            {
                LogLevel = LogLevel.Fatal,
                Message = fatal.Message,
                Source = source.FullName,
                Exception = fatal,
                ExceptionType = fatal.GetType().Name
            };
            LogTo(log);
        }

        /// <summary>
        /// 写入fatal级别日志
        /// </summary>
        /// <param name="source">异常源的类型</param>
        /// <param name="fatal">异常对象</param>
        public static void Fatal(string source, Exception fatal)
        {
            LogInfo log = new()
            {
                LogLevel = LogLevel.Fatal,
                Message = fatal.Message,
                Source = source,
                Exception = fatal,
                ExceptionType = fatal.GetType().Name
            };
            LogTo(log);
        }

        #endregion Fatal

        private static void LogTo(LogInfo log)
        {
            if (log == null)
                return;
            log.LogLevel = LogLevel.Error;
            log.Time = DateTime.Now;
            log.ThreadId = Environment.CurrentManagedThreadId;
            LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(log.LogLevel, log.SubFolder), $"{log.Time}   [{Environment.CurrentManagedThreadId}]  {log.ExceptionType}  {log.Source}  {log.Message}  {log.Exception}"));
            Event?.Invoke(log);
        }

    }
}
