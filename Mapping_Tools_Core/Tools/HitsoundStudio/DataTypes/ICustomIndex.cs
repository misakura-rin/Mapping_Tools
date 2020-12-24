﻿using System;
using System.Collections.Generic;

namespace Mapping_Tools_Core.Tools.HitsoundStudio.DataTypes {
    public interface ICustomIndex : ICloneable {
        /// <summary>
        /// The index.
        /// The default value is -1 which means the index is undefined.
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// The samples in this custom index.
        /// Maps a sample name to a set of <see cref="ISampleGeneratingArgs"/>
        /// to allow multiple samples mixed in a single sample name.
        /// </summary>
        Dictionary<string, HashSet<ISampleGeneratingArgs>> Samples { get; }

        /// <summary>
        /// Checks if this custom index fits all the needs of the other
        /// without having to assign any extra samples to this.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool Fits(ICustomIndex other);

        /// <summary>
        /// Checks if this custom index can fit all the needs of the other
        /// if extra samples may be assigned to this aswell.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool CanMerge(ICustomIndex other);

        /// <summary>
        /// Merges the other custom index into this one.
        /// Combining all the samples and index.
        /// </summary>
        /// <param name="other"></param>
        void MergeWith(ICustomIndex other);

        /// <summary>
        /// Gets the string to append to the sample names in <see cref="Samples"/>
        /// to get real osu! sample file names.
        /// </summary>
        /// <returns></returns>
        string GetNumberExtension();
    }
}