using System.Collections.Generic;
using Microsoft.Kinect;

namespace Framework.Wraps
{
    /// <summary>
    /// Really likely to be unecessary
    /// </summary>
    public class BodyWrap
    {
        public FrameEdges ClippedEdges { get; set; }
        public TrackingConfidence HandLeftConfidence { get; set; }
        public TrackingConfidence HandRightConfidence { get; set; }
        public HandState HandLeftState { get; set; }
        public HandState HandRightState { get; set; }
        public bool IsRestricted { get; set; }
        public IReadOnlyDictionary<JointType, JointOrientation> JointOrientations { get; set; }
        public IReadOnlyDictionary<JointType, Joint> Joints { get; set; }
        public bool IsTracked { get; set; }
        public PointF Lean { get; set; }
        public ulong TrackingId { get; set; }
        public TrackingState LeanTrackingState { get; set; }
    }
}