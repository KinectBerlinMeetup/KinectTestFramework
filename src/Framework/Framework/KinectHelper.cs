using Microsoft.Kinect;

namespace Framework
{
    public static class KinectHelper
    {
        public static Body TrackedSkeleton(this BodyFrame bodyFrame)
        {
            int trackedSkeleton = -1;
            var bodies = new Body[bodyFrame.BodyCount];
            bodyFrame.GetAndRefreshBodyData(bodies);
            // aktuell haben wir nur ein KinectSkeleton das im KinectBitmapPresenter fest eingehangen ist - trackedSkeleton gibt an welches Skelett dort gerendert werden soll
            if (trackedSkeleton != -1)
            {
                // checken ob es noch getracked wird, sonst reset triggern
                if (!bodies[trackedSkeleton].IsTracked)
                {
                    trackedSkeleton = -1;
                }
            }

            if (trackedSkeleton == -1) // noch kein Skeleton getracked, wir versuchen eins auszusuchen
            {
                for (int i = 0; i < bodies.Length; i++)
                {
                    if (bodies[i].IsTracked)
                    {
                        trackedSkeleton = i;
                        break;
                    }
                }
            }
            if (trackedSkeleton != -1)
            {
                return bodies[trackedSkeleton];
            }
            return null;
        }
    }
}