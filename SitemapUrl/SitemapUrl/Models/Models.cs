using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SitemapUrl.Models
{
    public class ParentUrl
    {
        [Key]
        public int ParentId { get; set; }
        public string Url { get; set; }
        public List<Url> ChildrenUrl { get; set; }
        public ParentUrl()
        {
            ChildrenUrl = new List<Url>();
        }
    }
    public class Url
    {
        [Key]
        public int Id { get; set; }
        public ParentUrl ParentUrl { get; set; }
        public int ParentId { get; set; }
        public string UrlName { get; set; }
        public double Time { get; set; }

    }
}