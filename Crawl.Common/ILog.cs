namespace Crawl.Common
{
    /// <summary>
    /// 日志接口
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// 写Info日志
        /// </summary>
        /// <param name="message"></param>
        void LogInfo(string message);

        /// <summary>
        /// 写Info日志
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="filename">保存的文件名</param>
        void LogInfo(string message,string filename);

        /// <summary>
        /// 写Error日志
        /// </summary>
        /// <param name="message"></param>
        void LogError(string message);

        /// <summary>
        /// 写Error日志
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="filename">保存的文件名</param>
        void LogError(string message,string filename);

        /// <summary>
        /// 写Warn日志
        /// </summary>
        /// <param name="message"></param>
        void LogWarn(string message);

        /// <summary>
        /// 写Warn日志
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="filename">保存的文件名</param>
        void LogWarn(string message,string filename);
    }
}
