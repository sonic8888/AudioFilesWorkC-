using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AudioFilesWorkC_
{
    internal class Track:IComparable
    {
        private string name = "unknown";
        private string? artist = null;
        public string Name
        {
            get { return name; }
            set { name = Track.NormalizeName(value); }
        }
        public string? Artist
        {
            get { return artist; }
            set { artist = Track.NormalizeName(value); }
        }
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
        private static string NormalizeName(string? name)
        {
            if (name == null) return "";
            string pattern = @"\W";
            string target = ".";
            Regex regex = new Regex(pattern);
            string result = regex.Replace(name, target);
            return result;
        }

        public int CompareTo(object? obj)
        {
            if (obj is Track track) return Name.CompareTo(track.Name);
            else throw new ArgumentException("Некорректное значение параметра"); ;
        }
    }
}
