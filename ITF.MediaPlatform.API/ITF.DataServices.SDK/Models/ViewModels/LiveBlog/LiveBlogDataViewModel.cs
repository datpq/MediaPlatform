using System.Collections.Generic;

namespace ITF.DataServices.SDK.Models.ViewModels.LiveBlog
{
    public class LiveBlogDataViewModel
    {
        public ICollection<Blog> Blogs { get; set; }
        public ICollection<Headline> Headlines { get; set; }
        public string Date { get; set; }
        public string Editor { get; set; }
    }
}
