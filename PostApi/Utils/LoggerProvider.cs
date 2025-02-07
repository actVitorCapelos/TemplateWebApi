using Microsoft.Extensions.Options;
using PostApi.IOptionsModel;

namespace PostApi.Utils;

public class LoggerProvider
{
    private readonly LogSettings _logSettings;

    public LoggerProvider(IOptions<LogSettings> LogSettings)
    {
        _logSettings = LogSettings.Value;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= (LogLevel)Int32.Parse(_logSettings.LogLevel);
    }

    public void Log(LogLevel logLevel, EventId eventId, string title, string logMessage)
    {
        if (!IsEnabled(logLevel))
            return;

        string fileName = GetLogFilePath();

        StreamWriter writerLog;

        if (File.Exists(fileName))
            writerLog = File.AppendText(fileName);
        else
            writerLog = new StreamWriter(fileName);

        writerLog.Write($"{DateTime.Now} - Service({title}) ");
        writerLog.Write($"{logLevel}[{eventId}] - ");
        writerLog.WriteLine($"Mensagem: {logMessage}");

        writerLog.Close();
    }

    public string GetLogFilePath()
    {
        string filePath = _logSettings.LogPath;
        return string.IsNullOrEmpty(filePath) ? "" : $"{filePath}/{DateTime.Now.ToString("ddMMyyy")}.txt";
    }

}