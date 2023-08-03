using System;

namespace HopCore.Server.Database {
    [AttributeUsage(validOn: AttributeTargets.Property)]
    public sealed class PrimaryKeyAttribute : Attribute {}
    
    [AttributeUsage(validOn: AttributeTargets.Property)]
    public sealed class ForeignKeyAttribute : Attribute {}
}