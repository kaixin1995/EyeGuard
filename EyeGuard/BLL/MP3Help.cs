using NAudio.Wave;
using System;
using System.IO;

namespace EyeGuard.BLL
{
    /// <summary>
    /// MP3 播放帮助类
    /// </summary>
    public sealed class MP3Help : IDisposable
    {
        private static readonly object SyncRoot = new object();
        private static MP3Help currentPlayer;

        private AudioFileReader audioFile;
        private WaveOutEvent outputDevice;
        private bool disposed;

        private MP3Help(string fileName)
        {
            try
            {
                audioFile = new AudioFileReader(fileName);
                outputDevice = new WaveOutEvent();
                outputDevice.Init(audioFile);
                outputDevice.PlaybackStopped += OutputDevice_PlaybackStopped;
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        /// <summary>
        /// 播放提示音；新提示音会替换尚未结束的旧提示音。
        /// </summary>
        /// <param name="fileName">提示音文件路径</param>
        public static void Play(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName))
            {
                return;
            }

            lock (SyncRoot)
            {
                StopCurrentPlayer();
                MP3Help player = null;
                try
                {
                    player = new MP3Help(fileName);
                    player.outputDevice.Play();
                    currentPlayer = player;
                }
                catch
                {
                    player?.Dispose();
                }
            }
        }

        /// <summary>
        /// 停止并释放当前提示音资源。
        /// </summary>
        public static void Stop()
        {
            lock (SyncRoot)
            {
                StopCurrentPlayer();
            }
        }

        private void OutputDevice_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            lock (SyncRoot)
            {
                if (ReferenceEquals(currentPlayer, this))
                {
                    currentPlayer = null;
                }
                Dispose();
            }
        }

        private static void StopCurrentPlayer()
        {
            if (currentPlayer == null)
            {
                return;
            }

            MP3Help player = currentPlayer;
            currentPlayer = null;
            player.Dispose();
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }
            disposed = true;

            if (outputDevice != null)
            {
                outputDevice.PlaybackStopped -= OutputDevice_PlaybackStopped;
                outputDevice.Stop();
                outputDevice.Dispose();
                outputDevice = null;
            }

            audioFile?.Dispose();
            audioFile = null;
        }
    }
}
