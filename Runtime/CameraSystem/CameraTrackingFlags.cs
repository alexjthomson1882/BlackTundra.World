﻿using System;

namespace BlackTundra.World.CameraSystem {

    /// <summary>
    /// Describes how the camera should track a target.
    /// </summary>
    [Flags]
    public enum CameraTrackingFlags : int {

        /// <summary>
        /// Empty flag.
        /// </summary>
        None = 0,

        /// <summary>
        /// The camera will use smooth movement.
        /// </summary>
        Smooth = 1 << 1,

        /// <summary>
        /// The camera will clamp the minimum distance from the target.
        /// </summary>
        MinClamp = 1 << 2,

        /// <summary>
        /// The camera will clamp the maximum distance from the target.
        /// </summary>
        MaxClamp = 1 << 3,

        /// <summary>
        /// The camera will make itself a child of the target and skip any other type of tracking.
        /// </summary>
        Parent = 1 << 4,

    }

}