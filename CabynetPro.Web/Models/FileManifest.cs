using System.Collections.Generic;

namespace CabynetPro.Web.Models
{
    public class FileManifest
    {
        public List<string> Files { get; set; }
        public string Description { get; set; }
        public int Category { get; set; }
    }
}