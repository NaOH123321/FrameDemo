using System;
using System.Collections.Generic;
using System.Text;

namespace FrameDemo.Core.Entities
{
    public class Song : Entity
    {
        public Song()
        {
            Singers = new HashSet<Singer>();
        }

        public int Rank { get; set; }
        public int AlbumId { get; set; }
        public string AlbumMid { get; set; }
        public string AlbumName { get; set; }
        public string AlbumPic { get; set; }
        public int Interval { get; set; }
        public int SongId { get; set; }
        public string SongMid { get; set; }
        public string SongName { get; set; }
        public PlayInfo PlayInfo { get; set; }
        public ICollection<Singer> Singers { get; set; }
    }

    public class Singer : Entity
    {
        public string Mid { get; set; }
        public string Name { get; set; }
        public string Pic { get; set; }
    }

    public class PlayInfo
    {
        public int Expiration { get; set; }
        public string Filename { get; set; }
        public string Vkey { get; set; }
        public string Url { get; set; }
        public string Ip { get; set; }
    }
}
