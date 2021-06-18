using System.Collections.Generic;

namespace ITF.DataServices.SDK.Models.Media
{
    public class Instagram
    {
        public List<Item> items { get; set; }
        public bool more_available { get; set; }
        public string status { get; set; }

        public class Item
        {
            public object alt_media_url { get; set; }
            public bool can_delete_comments { get; set; }
            public bool can_view_comments { get; set; }
            public Caption caption { get; set; }
            public string code { get; set; }
            public Comments comments { get; set; }
            public string created_time { get; set; }
            public string id { get; set; }
            public Images images { get; set; }
            public Likes likes { get; set; }
            public string link { get; set; }
            public Location location { get; set; }
            public string type { get; set; }
            public User user { get; set; }

            public class Caption
            {
                public string created_time { get; set; }
                public From from { get; set; }
                public string id { get; set; }
                public string text { get; set; }

                public class From
                {
                    public string full_name { get; set; }
                    public string id { get; set; }
                    public string profile_picture { get; set; }
                    public string username { get; set; }
                }
            }

            public class Comments
            {
                public int count { get; set; }
                public List<object> data { get; set; }
            }

            public class Images
            {
                public LowResolution low_resolution { get; set; }
                public StandardResolution standard_resolution { get; set; }
                public Thumbnail thumbnail { get; set; }

                public class LowResolution
                {
                    public int height { get; set; }
                    public string url { get; set; }
                    public int width { get; set; }
                }

                public class StandardResolution
                {
                    public int height { get; set; }
                    public string url { get; set; }
                    public int width { get; set; }
                }

                public class Thumbnail
                {
                    public int height { get; set; }
                    public string url { get; set; }
                    public int width { get; set; }
                }
            }

            public class Likes
            {
                public int count { get; set; }
                public List<Datum> data { get; set; }

                public class Datum
                {
                    public string full_name { get; set; }
                    public string id { get; set; }
                    public string profile_picture { get; set; }
                    public string username { get; set; }
                }
            }

            public class Location
            {
                public string name { get; set; }
            }

            public class User
            {
                public string full_name { get; set; }
                public string id { get; set; }
                public string profile_picture { get; set; }
                public string username { get; set; }
            }
        }
    }
}
