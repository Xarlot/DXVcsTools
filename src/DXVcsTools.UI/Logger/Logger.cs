using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace DXVcsTools.UI.Logger {
    public static class Logger {
        static readonly string FileAppender = "FileAppender";
        static volatile ILog log;
        static readonly object Locker = new object();

        static Logger() {
            if (log == null) {
                lock (Locker) {
                    if (log == null) {
                        InitializeLogger();
                        log = LogManager.GetLogger(typeof(Logger));
                    }
                }
            }
        }
        static void InitializeLogger() {
            AddAppender(CreateFileAppender());
            AddAppender(CreateSmtpAppender());
            InitRepo();
        }
        static void AddAppender(IAppender appender) {
            GetRoot().Root.AddAppender(appender);
        }
        static IAppender GetAppender(string name) {
            AppenderCollection ac = GetRoot().Root.Appenders;
            return ac.Cast<IAppender>().FirstOrDefault(appender => appender.Name == name);
        }
        static Hierarchy GetRoot() {
            return ((Hierarchy)LogManager.GetRepository());
        }
        static void InitRepo() {
            var root = GetRoot().Root;
            root.Repository.Configured = true;
        }
        static IAppender CreateFileAppender() {
            var appender = new RollingFileAppender();
            appender.Name = FileAppender;
            appender.AppendToFile = false;
            appender.MaxFileSize = 1000000;
            appender.MaxSizeRollBackups = 10;
            appender.RollingStyle = RollingFileAppender.RollingMode.Composite;
            appender.StaticLogFileName = true;
            appender.Layout = new PatternLayout("%date [%thread] %-5level [%ndc] - %message%newline");
            appender.File = Path.GetTempFileName();
            appender.Encoding = new UTF8Encoding();
            appender.Threshold = new Level(0, "DEBUG");
            appender.ActivateOptions();
            return appender;
        }
        static IAppender CreateSmtpAppender() {
            var appender = new SmtpAppender();
            appender.From = "test@mail.ru";
            appender.To = "maximatorr@bk.ru";
            appender.Authentication = SmtpAppender.SmtpAuthentication.None;
            appender.Subject = "bug";
            appender.Priority = MailPriority.Low;
            return appender;

        }

        public static void AddError(string message, Exception e = null) {
            if (e != null)
                log.Error(message, e);
            else 
                log.Error(message);
        }
        public static void AddInfo(string message, Exception e = null) {
            if (e != null)
                log.Info(message, e);
            else
                log.Info(message);
        }
        public static string GetLog() {
            RollingFileAppender fileAppender = (RollingFileAppender)GetAppender(FileAppender);
            fileAppender.ImmediateFlush = true;
            fileAppender.ImmediateFlush = false;
            return fileAppender.File;
        }
    }
}
