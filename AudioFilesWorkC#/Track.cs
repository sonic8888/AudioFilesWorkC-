using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioFilesWorkC_
{
    internal class Track
    {
        public string Name { get; private set; }
        public string? Artist { get; private set; }
        public string? Album { get; private set; }
        public int? DurationMillis { get; private set; }
        public int RealId { get; set; }
        public int? FileSize { get; set; }

        public Track(string name, string? artist, string? album, int? durationMillis, int? fileSize, int realId)
        {
            Name = name;
            Artist = artist;
            Album = album;
            DurationMillis = durationMillis;
            FileSize = fileSize;
            RealId = realId;
        }
        public Track(string name, string artist) : this(name, artist, "", 0, 0, 0)
        { }
        public Track(string name) : this(name, "", "", 0, 0, 0) { }

        public override string ToString()
        {
            return $"{Name} {Artist} {Album}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is Track track)
            {
                return this.GetHashCode() == track.GetHashCode();
            }
            else
            {
                throw new ArgumentException("obj is not Track");
            }
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }






    }
}
