using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace ProServer.Helper
{
    public static class DownloadHelper
    {
        /// <summary>
        /// 下载文件
        /// action(DownloadFileState state, int fileSize, int recvCount, Exception error)
        /// action.state: 下载文件的状态
        /// action.fileSize: 文件大小(字节)
        /// action.recvCount: 接收到字节数
        /// action.error: 错误信息
        /// </summary>
        /// <param name="url">文件地址</param>
        /// <param name="exportFile">下载成功后输出的文件完整路径及名称</param>
        /// <param name="action">回调方法</param>
        public static void DownloadFileAsync(string url, string exportFile, Action<DownloadFileState, long, long, Exception> action)
        {
            WebClient client = new WebClient();
            client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(client_DownloadFileCompleted);
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
            client.DownloadFileAsync(new Uri(url), exportFile, action);
        }

        static void client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Action<DownloadFileState, long, long, Exception> action = e.UserState as Action<DownloadFileState, long, long, Exception>;
            action(e.Cancelled ? DownloadFileState.Error : DownloadFileState.Done, 0, 0, e.Cancelled ? e.Error : null);
        }

        static void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Action<DownloadFileState, long, long, Exception> action = e.UserState as Action<DownloadFileState, long, long, Exception>;
            action(DownloadFileState.Progress, e.TotalBytesToReceive, e.BytesReceived, null);
        }
    }

    /// <summary>
    /// 下载文件的状态
    /// </summary>
    public enum DownloadFileState
    {
        /// <summary>
        /// 返回错误
        /// </summary>
        Error,

        /// <summary>
        /// 返回进度
        /// </summary>
        Progress,

        /// <summary>
        /// 返回完成(成功)
        /// </summary>
        Done
    }
}
