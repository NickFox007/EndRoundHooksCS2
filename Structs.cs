using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveryoneHGR;

public class HGRInfo
{
    public int Hooks { get; set; }
    public int Grabs { get; set; }
    public int Ropes { get; set; }

    public HGRInfo(int Hooks, int Grabs, int Ropes)
    {
        this.Hooks = Hooks;
        this.Grabs = Grabs;
        this.Ropes = Ropes;
    }
}
