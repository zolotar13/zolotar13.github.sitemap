using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SitemapUrl.Models
{
    public class Context:DbContext
    {
        public Context()
            : base("DefaultConnection")
        {

        }
        public  DbSet<ParentUrl> ParentUrls { get; set; }
        public DbSet<Url> Urls { get; set; }
    }
}