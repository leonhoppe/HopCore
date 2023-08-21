using System;
using HopCore.Server.Database;

namespace HopCore.Server.Core.Models {
    public sealed class DbLog {
        [PrimaryKey] public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Resource { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
    }
}