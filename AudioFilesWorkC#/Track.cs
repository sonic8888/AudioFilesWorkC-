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
        private string title = "unknown";
        private static string pattern = @"[\*\|\\\:\""<>\?\/]";
        private static Regex regex = new Regex(pattern);
        private static string target = ".";
        public string Extension { get; set; } = ".mp3";

        public string Name { get; set; } = "";

        public string Title
        {
            get { return title; }
            set { title = Track.NormalizeName(value); }
        }
        public string AlbumId { get; set; } = "-1";

        public string? Artist { get; set; } = "unknown";
        public string? Album { get; set; } = "unknown";
        public string? Year { get; set; } = "unknown";
        public string TrackId { get; set; } = "-1";
        public string? ArtistId { get; set; } = "-1";
        public string? DataCreate { get; set; }



        public Track() { }

        public override string ToString()
        {
            return $"Name:{Title}, Artist:{Artist}, Album:{Album}, Year:{Year}, TrackId:{TrackId})";
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
            if (obj is Track track) return Title.CompareTo(track.Title);
            else throw new ArgumentException("Некорректное значение параметра"); ;
        }
    }
}
