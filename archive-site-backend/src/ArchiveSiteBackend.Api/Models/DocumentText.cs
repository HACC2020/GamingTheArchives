﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveSiteBackend.Api.Models
{
    public class DocumentText
    {
        /// <summary>
        /// Describes where the text was found in relation to the image. For image, the (x, y)
        /// coordinates are measured in pixels. For PDF, the (x, y) coordinates are measured in inches.
        /// </summary>
        public BoundingBox BoundingBox { get; set; }

        /// <summary>
        /// Describes the actual text that was found
        /// </summary>
        public string Text { get; set; }
    }

    public class BoundingBox
    {
        public double? Left { get; set; }
        public double? Top { get; set; }

        public double? Right { get; set; }
        public double? Bottom { get; set; }
    }
}
