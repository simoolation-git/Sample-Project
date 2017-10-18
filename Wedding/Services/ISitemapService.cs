using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Wedding.Services
{
    public interface ISitemapService
    {
        //Task<string> GetSitemapDocument(UrlHelper urlHelper, HtmlHelper htmlHelper);
        Task<string> GetSitemapDocument(string canonicalLinkUrl);
        Task<string> GetNewsSitemapDocument(string canonicalLinkUrl);
    }
}
