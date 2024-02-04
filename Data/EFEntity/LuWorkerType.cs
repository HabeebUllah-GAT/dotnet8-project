using System;
using System.Collections.Generic;

namespace GATIntegrations.Data.EFEntity
{
    public partial class LuWorkerType
    {
        public byte Id { get; set; }
        public string Type { get; set; } = null!;
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}
