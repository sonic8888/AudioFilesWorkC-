using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioFilesWorkC_
{
    internal class Track
    {
        public string Name { get; set; } = "unknown";
        public string? Artist { get; set; }
        public string? TrackId { get; set; }
        public string? ArtistId { get; set; }
        public string? NameArtist
        {
            get { return $"{Name} ({Artist})"; }
        }
        public string? DataCreate { get; set; }

        public string? Name_Artist { get; set; }

        public Track() { }

        public override string ToString()
        {
            return $"{Name} ({Artist})";
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

        public static string Data() => DateTime.Now.ToString("d");
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }






    }
}
