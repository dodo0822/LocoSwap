using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocoSwap
{
    public class DirectoryItem
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool Populated { get; set; }
        public ObservableCollection<DirectoryItem> SubDirectories { get; set; }

        public DirectoryItem()
        {
            this.Populated = false;
            this.SubDirectories = new ObservableCollection<DirectoryItem>();
        }

        public void PopulateSubDirectories()
        {
            if (this.Populated) return;
            SubDirectories.Clear();
            var dirInfo = new DirectoryInfo(Path);

            foreach (var directory in dirInfo.GetDirectories())
            {
                var item = new DirectoryItem
                {
                    Name = directory.Name,
                    Path = directory.FullName
                };
                item.SubDirectories.Add(new DirectoryItem
                {
                    Name = "Loading...",
                    Path = "dummy"
                });

                SubDirectories.Add(item);
            }
            Populated = true;
        }
    }
}
