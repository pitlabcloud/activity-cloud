using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NooSphere.Core.ContextModel.ComponentModel
{
    public interface ITool
    {
        ToolType Type { get; set; }
        ToolState State { get; set; }
    }
    public enum ToolType
    {
        Physical,
        Digital,
        Mediating
    }
    public interface ToolState{}
}
