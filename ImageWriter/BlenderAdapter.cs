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
            float X, float Y, float Z,
            float targetX, float targetY, float targetZ,
            float focalLengthMM = 50.0f,
            float sensorHeightMM = 24.0f,
            float aspectRatio = 16.0f / 9.0f,
            int imageWidth = 1080,
            int numSamples = 100,
            int maxDepth = 50,
            float depthFieldAngle = 0.0f)
        {
            // X=X, Y=Z, Z=-Y
            Point lookPoint = new Point(X, Z, -Y);
            Point lookTarget = new Point(targetX, targetZ, -targetY);

            float vFovRadians = 2.0f * MathF.Atan(sensorHeightMM / (2.0f * focalLengthMM));
            float vFovDegrees = vFovRadians * (180.0f / MathF.PI);

            NVector distanceVector = lookPoint - lookTarget;
            float focusDist = distanceVector.Length();

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
                new NVector(0f, 1f, 0f)
            );

            return camera;
        }
    }
}
