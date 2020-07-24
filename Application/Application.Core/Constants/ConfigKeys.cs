using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Core.Constants
{
    public static class ConfigKeys
    {
        public static readonly string Vision = "VISION";
        public static class VisionConfigKeys
        {
            public static readonly string MorphologyOperatorSize = "MorphologyOperatorSize";
            public static readonly string MedinBlurSize = "MedinBlurSize";
            public static readonly string WindowSizeTolerance = "WindowSizeTolerance";
            public static readonly string PermissibleDistanceOfRotation = "PermissibleDistanceOfRotation";
        }


        public static readonly string Camera = "CAMERA";
        public static class CameraConfigKeys
        {
            public static readonly string CapturedImagesFolder = "CapturedImagesFolder";
        }

        public static readonly string GeneralPreferences = "GENERAL_PREFERENCES";
    }
}
