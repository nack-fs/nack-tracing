using NackEngine.core.render;
using NackEngine.core.space;
using System;
using System.Collections.Generic;
using System.Text;

namespace NackEngine.IO
{
    using Point = NVector;

    public static class BlenderAdapter
    {
        public static Camera CreateCamera(
            double X, double Y, double Z,
            double targetX, double targetY, double targetZ,
            double focalLengthMM = 50.0,
            double sensorHeightMM = 24.0,
            double aspectRatio = 16.0 / 9.0,
            int imageWidth = 1080,
            int numSamples = 100,
            int maxDepth = 50,
            double depthFieldAngle = 0.0)
        {
            // X=X, Y=Z, Z=-Y
            Point lookPoint = new Point(X, Z, -Y);
            Point lookTarget = new Point(targetX, targetZ, -targetY);

            double vFovRadians = 2.0 * Math.Atan(sensorHeightMM / (2.0 * focalLengthMM));
            double vFovDegrees = vFovRadians * (180.0 / Math.PI);

            NVector distanceVector = lookPoint - lookTarget;
            double focusDist = distanceVector.Length();

            Camera camera = new Camera(
                aspectRatio: aspectRatio,
                imageWidth: imageWidth,
                numSamples: numSamples,
                maxDepth: maxDepth,
                fieldView: vFovDegrees,
                depthFieldAngle: depthFieldAngle,
                focusDistance: focusDist
            );

            camera.SetLookPoint(
                lookPoint,
                lookTarget,
                new NVector(0, 1, 0)
            );

            return camera;
        }
    }
}
