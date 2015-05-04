using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Wraps
{
    public class MultiFrameWrap
    {
        public BodyFrameWrap BodyFrameWrap { get; set; }
        public ColorFrameWrap ColorFrameWrap { get; set; }
        public DepthFrameWrap DepthFrameWrap { get; set; }
        public IrFrameWrap IrFrameWrap { get; set; }

    }
}
