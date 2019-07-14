using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using FrameDemo.Core.Entities;
using FrameDemo.Infrastructure.Helpers;
using FrameDemo.Infrastructure.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FrameDemo.Infrastructure.Repositories
{
    public class SongRepository
    {
        private const string Guid = "876576457";//"126548448";

        private const string cid = "205361747"; //"205361747"; 

        private const string TopUrl =
            @"https://c.y.qq.com/v8/fcg-bin/fcg_v8_toplist_cp.fcg?g_tk=5381&uin=0&format=json&inCharset=utf-8&outCharset=utf-8¬ice=0&platform=h5&needNewCode=1&tpl=3&page=detail&type=top&topid=27&_=1519963122923";

        public async Task<PaginatedList<Song>> GetAllAsync(SongParameters songParameters)
        {
            var json = await HttpHelper.HttpGetAsync(TopUrl, contentType: "application/json");
            var jObject = (JObject) JsonConvert.DeserializeObject(json);
            var jsonArray = (JArray) jObject["songlist"];

            var songList = new Collection<Song>();
            for (int i = 0; i < jsonArray.Count; i++)
            {
                var data = (JObject) jsonArray[i]["data"];
                var song = new Song
                {
                    Rank = int.Parse(jsonArray[i]["cur_count"].ToString()),
                    AlbumId = int.Parse(data["albumid"].ToString()),
                    AlbumMid = data["albummid"].ToString(),
                    AlbumName = data["albumname"].ToString(),
                    Interval = int.Parse(data["interval"].ToString()),
                    SongId = int.Parse(data["songid"].ToString()),
                    SongMid = data["songmid"].ToString(),
                    SongName = data["songname"].ToString()
                };
                var singers = (JArray) data["singer"];
                var singerList = new Collection<Singer>();
                for (int j = 0; j < singers.Count; j++)
                {
                    var singer = new Singer
                    {
                        Id = int.Parse(singers[j]["id"].ToString()),
                        Mid = singers[j]["mid"].ToString(),
                        Name = singers[j]["name"].ToString(),
                        Pic = $@"https://y.gtimg.cn/music/photo_new/T001R300x300M000{singers[j]["mid"]}.jpg"
                    };
                    singerList.Add(singer);
                }
                song.AlbumPic = $@"https://y.gtimg.cn/music/photo_new/T002R300x300M000{song.AlbumMid}.jpg";
                song.Singers = singerList;
                //song.PlayInfo = await GetSongInfo(song.SongMid);

                songList.Add(song);
            }

            return new PaginatedList<Song>(songParameters.PageIndex, songParameters.PageSize, songList.Count, songList);
            ;
        }

        private async Task<ValueTuple<int, string, string>> GetToken(string songmid)
        {
            var token_url =
                $@"https://c.y.qq.com/base/fcgi-bin/fcg_music_express_mobile3.fcg?format=json&platform=yqq&cid={cid}&songmid={songmid}&filename=C400{songmid}.m4a&guid={Guid}";
            var json = await HttpHelper.HttpGetAsync(token_url, contentType: "application/json");
            var jObject = (JObject) JsonConvert.DeserializeObject(json);

            var expiration = int.Parse(jObject["data"]["expiration"].ToString());
            var jsonArray = (JArray) jObject["data"]["items"];
            var filename = jsonArray[0]["filename"].ToString();
            var vkey = jsonArray[0]["vkey"].ToString();

            return (expiration, filename, vkey);
        }

        public async Task<PlayInfo> GetSongPlayInfo(string songmid)
        {
            var (expiration, filename, vkey) = await GetToken(songmid);
            var play_url =
                $@"http://ws.stream.qqmusic.qq.com/{filename}?fromtag=0&guid={Guid}&vkey={vkey}";

            var playInfo = new PlayInfo
            {
                Expiration = expiration,
                Filename = filename,
                Vkey = vkey,
                Url = play_url
            };
            return playInfo;
        }

        public async Task<PaginatedList<Song>> GetSongsBySearch(SongParameters songParameters, string keywords)
        {
            var search_url = $@"https://c.y.qq.com/soso/fcgi-bin/client_search_cp?aggr=1&cr=1&flag_qc=0&p=1&n=30&w={keywords}";
            var json = await HttpHelper.HttpGetAsync(search_url, contentType: "application/json");
            var jObject = (JObject)JsonConvert.DeserializeObject(json);
            var songList = new Collection<Song>();


            //var URL_SONG_LYR = $@"https://c.y.qq.com/lyric/fcgi-bin/fcg_query_lyric_new.fcg?g_tk=5381&loginUin=0&hostUin=0&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq&needNewCode=0&format=json";

            //var sd = URL_SONG_LYR + $@"&songmid={"0039MnYb0qxYhV"}" + $@"&pcachetime={DateTime.Now}";

            //var ssss = await HttpHelper.HttpGetAsync(sd, contentType: "application/json");


            return new PaginatedList<Song>(songParameters.PageIndex, songParameters.PageSize, songList.Count, songList);
        }
    }
}
