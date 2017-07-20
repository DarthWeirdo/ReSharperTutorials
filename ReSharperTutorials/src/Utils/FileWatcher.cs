using System;
using System.IO;
using JetBrains.DataFlow;

namespace ReSharperTutorials.Utils
{
    class FileWatcher
    {
        public ISignal<FileSystemEventArgs> OnWatchedEvent { get; private set; }

        public FileWatcher(Lifetime lifetime, string directory, string fileMask, NotifyFilters notifyFilter, WatchFile watchFile)
        {
            OnWatchedEvent = new Signal<FileSystemEventArgs>(lifetime,
                "FileWatcher.OnWatchedEvent");

            var watcher = new FileSystemWatcher()
            {
                Path = directory,
                Filter = fileMask,
                NotifyFilter = notifyFilter
            };

            var handler = new FileSystemEventHandler(OnChanged);

            switch (watchFile)
            {
                case WatchFile.Create:                    
                    lifetime.AddBracket(() => watcher.Created += handler, () => watcher.Created -= handler);
                    break;
                case WatchFile.Change:
                    lifetime.AddBracket(() => watcher.Changed += handler, () => watcher.Changed -= handler);
                    break;                
                default:
                    throw new ArgumentOutOfRangeException(nameof(watchFile), watchFile, null);
            }

        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            OnWatchedEvent.Fire(e);
        }
    }
}