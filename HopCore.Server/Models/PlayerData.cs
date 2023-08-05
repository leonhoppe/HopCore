using CitizenFX.Core;
using HopCore.Server.Database;

namespace HopCore.Server.Models {
    public sealed class PlayerData {
        [PrimaryKey] public string Id { get; set; }
        public string Rank { get; set; }
        public string Job { get; set; }
        public int JobGrade { get; set; }
        public Vector3 Position { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public char Sex { get; set; }
        public string Skin { get; set; }
        public bool Dead { get; set; }
    }
}