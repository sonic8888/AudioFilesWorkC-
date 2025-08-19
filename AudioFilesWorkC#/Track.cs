using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AudioFilesWorkC_
{
    internal class Track : IComparable
    {
        public string name = "unknown";
        private string title = "unknown";
        private string extension = ".mp3";
        private static string pattern = @"[\*\|\\\:\""<>\?\/]";
        private static Regex regex = new Regex(pattern);
        private static string target = ".";
        public string Extension
        {
            get { return extension; }
            set { extension = value; }
        }
        public string Name
        {
            get { return title + extension; }
            
        }
        public string Title
        {
            get { return title; }
            set { title = Track.NormalizeName(value); }
        }
        public string AlbumId { get; set; } = "-1";

        public string Artist { get; set; } = "";
        public string Album { get; set; } = "unknown";
        public string? Year { get; set; }
        public string TrackId { get; set; } = "-1";
        public string? ArtistId { get; set; } = "-1";
        public string? NameArtist
        {
            get { return $"{Name}({Artist})"; }
        }
        public string? DataCreate { get; set; }

        public string? Name_Artist { get; set; }

        public Track() { }

        public override string ToString()
        {
            return $"Name:{Name}, Artist:{Artist}, Album:{Album}, Year:{Year})";
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
            return TrackId.GetHashCode();
        }
        private static string NormalizeName(string? name)
        {
            if (name == null) return "";
            if (regex.IsMatch(name))
            {
                string result = regex.Replace(name, target);
                return result;
            }
            else
            {
                return name;
            }
        }

        public int CompareTo(object? obj)
        {
            if (obj is Track track) return Name.CompareTo(track.Name);
            else throw new ArgumentException("Некорректное значение параметра"); ;
        }
    }
}
