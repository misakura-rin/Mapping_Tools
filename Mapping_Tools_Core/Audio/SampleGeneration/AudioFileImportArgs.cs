﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mapping_Tools_Core.Audio.SampleImporters;
using Mapping_Tools_Core.Audio.SampleSoundGeneration;

namespace Mapping_Tools_Core.Audio.SampleGeneration {
    public class AudioFileImportArgs : IAudioFileImportArgs {
        private static readonly string[] ValidExtensions = {".wav", ".mp3", ".aiff", ".ogg"};

        private string Extension => System.IO.Path.GetExtension(Path);

        public AudioFileImportArgs(string path) {
            Path = path;
        }

        public bool Equals(ISampleGenerator other) {
            return other is IAudioFileImportArgs o && Path.Equals(o.Path);
        }

        public object Clone() {
            return new AudioFileImportArgs(Path); 
        }

        public bool IsValid() {
            return File.Exists(Path) && ValidExtensions.Contains(Extension);
        }

        public bool IsValid(Dictionary<ISampleGenerator, ISampleSoundGenerator> loadedSamples) {
            return loadedSamples.ContainsKey(this) && loadedSamples[this] != null;
        }

        public ISampleSoundGenerator Import() {
            if (Extension == ".ogg") {
                return new VorbisFileImporter().Import(new VorbisFileImportArgs(Path));
            }

            return new AudioFileImporter().Import(this);
        }

        public string GetName() {
            return System.IO.Path.GetFileNameWithoutExtension(Path);
        }

        public bool IsDirectSource() => true;

        public string Path { get; }
    }
}