using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace HopCore.Shared.Serialisation {
    public interface IPacket {

        void LoadData(ExpandoObject data);

    }
}