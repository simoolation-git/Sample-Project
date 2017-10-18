using Domain.Interfaces.Services;
using Domain.Models.SiteMap;
using Domain.Services;
using Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Wedding.ExtensionMethods;

namespace Wedding.Services
{
    public class SitemapService : ISitemapService
    {
        private readonly IPostedItemService _postedItemService;
        private ICacheService _cacheService;
        private IConfigurationService _configurationService;
        private readonly string _cacheKey;
        private readonly string _newsCacheKey;

        public SitemapService(IPostedItemService postedItemService, ICacheService cacheService, IConfigurationService configurationService)
        {
            _cacheKey = "sitemap";
            _newsCacheKey = "newsSitemap";
            _postedItemService = postedItemService;
            _cacheService = cacheService;
            _configurationService = configurationService;
        }

        private async Task<IReadOnlyCollection<SitemapNode>> GetSitemapNodes(string canonicalLinkUrl)
        {
            //var schema = urlHelper.RequestContext.HttpContext.Request.Url.Scheme;

            List<SitemapNode> nodes = new List<SitemapNode>();
            nodes.Add(new SitemapNode()
            {
                //Url = urlHelper.Action("Index", "Home", null, schema),
                Url = String.Format("{0}", canonicalLinkUrl),
                Priority = 1
            });

            nodes.Add(new SitemapNode()
            {
                Url = String.Format("{0}/Account/Login", canonicalLinkUrl),
                Priority = 0.8
            });

            nodes.Add(new SitemapNode()
            {
                Url = String.Format("{0}/Account/ForgotPassword", canonicalLinkUrl),
                Priority = 0.64
            });

            nodes.Add(new SitemapNode()
            {
                Url = String.Format("{0}/Account/Register", canonicalLinkUrl),
                Priority = 0.64
            });

            nodes.Add(new SitemapNode()
            {
                Url = String.Format("{0}/Home/Contact", canonicalLinkUrl),
                Priority = 0.64,
                Frequency = SitemapFrequency.Monthly
            });

            nodes.Add(new SitemapNode()
            {
                Url = String.Format("{0}/Home/Terms", canonicalLinkUrl),
                Priority = 0.64,
                Frequency = SitemapFrequency.Monthly
            });

            nodes.Add(new SitemapNode()
            {
                Url = String.Format("{0}/Home/Privary", canonicalLinkUrl),
                Priority = 0.64,
                Frequency = SitemapFrequency.Monthly
            });

            nodes.Add(new SitemapNode()
            {
                Url = "http://blog.zenzoy.com",
                Priority = 0.5,
                Frequency = SitemapFrequency.Weekly
            });

            var postedItems = await _postedItemService.GetPostedItems(1000);
            if (postedItems != null && postedItems.Count > 0)
            {
                foreach (var postedItem in postedItems)
                {
                    nodes.Add(new SitemapNode()
                    {
                        Url = String.Format("{0}/PostedItems/{1}/{2}", canonicalLinkUrl, postedItem.Id, postedItem.Slug),
                        Frequency = SitemapFrequency.Weekly,
                        Priority = 0.9
                    });
                }

                var pages = Math.Ceiling((decimal)postedItems.Count / _configurationService.GetPostedItemCountPerPage());

                for (int pageNumber = 0; pageNumber < pages; pageNumber++)
                {
                    nodes.Add(new SitemapNode()
                    {
                        Url = String.Format("{0}/PostedItems/Page/{1}", canonicalLinkUrl, pageNumber),
                        Frequency = SitemapFrequency.Monthly,
                        Priority = 0.8
                    });
                }
            }

            return nodes;
        }

        private async Task<IReadOnlyCollection<NewsSitemapNode>> GetNewsSitemapNodes(string canonicalLinkUrl)
        {
            //var schema = urlHelper.RequestContext.HttpContext.Request.Url.Scheme;

            List<NewsSitemapNode> nodes = new List<NewsSitemapNode>();

            var postedItems = await _postedItemService.GetPostedItems(1000);
            if (postedItems != null && postedItems.Count > 0)
            {
                foreach (var postedItem in postedItems)
                {
                    nodes.Add(new NewsSitemapNode()
                    {
                        Url = String.Format("{0}/PostedItems/{1}/{2}", canonicalLinkUrl, postedItem.Id, postedItem.Slug),
                        //PublicationName = ,
                        Language = "en",
                        //Generes = ,
                        Publication_date = postedItem.Updated,
                        Title = postedItem.Title,
                        Keywords = postedItem.TagsTokenized,
                        CoverPhotoUrl = postedItem.PhotoUrl,
                        VideoUrl = postedItem.VideoSourceUrl
                        //Stock_tickers =
                    });
                }
            }

            return nodes;
        }

        public async Task<string> GetSitemapDocument(string canonicalLinkUrl)
        {
            var payload = await _cacheService.GetFromCache(_cacheKey);

            if (!string.IsNullOrEmpty(payload))
                return payload;

            XNamespace xmlns = XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9");
            XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
            XNamespace schemaLocation = XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd");

            XElement root = new XElement(xmlns + "urlset",
                    new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                    new XAttribute(xsi + "schemaLocation", schemaLocation)
            );

            var sitemapNodes = await GetSitemapNodes(canonicalLinkUrl);

            foreach (SitemapNode sitemapNode in sitemapNodes)
            {
                XElement urlElement = new XElement(
                    xmlns + "url",
                    new XElement(xmlns + "loc", sitemapNode.Url),
                    sitemapNode.LastModified == null ? null : new XElement(xmlns + "lastmod", sitemapNode.LastModified.Value.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:sszzz")),
                    sitemapNode.Frequency == null ? null : new XElement(xmlns + "changefreq", sitemapNode.Frequency.Value.ToString().ToLowerInvariant()),
                    sitemapNode.Priority == null ? null : new XElement(xmlns + "priority", sitemapNode.Priority.Value.ToString("F1", CultureInfo.InvariantCulture)));
                root.Add(urlElement);
            }
            XDocument document = new XDocument(root);

            payload = document.ToString();

            await _cacheService.AddToCache(_cacheKey, payload);

            return payload;
        }

        public async Task<string> GetNewsSitemapDocument(string canonicalLinkUrl)
        {
            var payload = await _cacheService.GetFromCache(_newsCacheKey);

            if (!string.IsNullOrEmpty(payload))
                return payload;

            XNamespace xmlns = XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9");
            XNamespace news = XNamespace.Get("http://www.google.com/schemas/sitemap-news/0.9");
            XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
            XNamespace schemaLocation = XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9 http://www.google.com/schemas/sitemap-news/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd");

            XElement root = new XElement(xmlns + "urlset",
                    new XAttribute(XNamespace.Xmlns + "news", news),
                    new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                    new XAttribute(xsi + "schemaLocation", schemaLocation)
            );

            var newsSitemapNodes = await GetNewsSitemapNodes(canonicalLinkUrl);

            foreach (NewsSitemapNode newsSitemapNode in newsSitemapNodes)
            {
                XElement newsElement = new XElement(news + "news",
                    new XElement(news + "publication",
                    newsSitemapNode.PublicationName == null ? null : new XElement(news + "name", newsSitemapNode.PublicationName),
                    newsSitemapNode.Language == null ? null : new XElement(news + "language", newsSitemapNode.Language)),
                    newsSitemapNode.Generes == null ? null : new XElement(news + "genres", newsSitemapNode.Generes),
                    newsSitemapNode.Publication_date == null ? null : new XElement(news + "publication_date", newsSitemapNode.Publication_date.Value.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:sszzz")),
                    newsSitemapNode.Title == null ? null : new XElement(news + "title", newsSitemapNode.Title),
                    newsSitemapNode.Keywords == null ? null : new XElement(news + "keywords", String.Join(", ", newsSitemapNode.Keywords)),
                    newsSitemapNode.Stock_tickers == null ? null : new XElement(news + "stock_tickers", newsSitemapNode.Stock_tickers));

                XElement urlElement = new XElement(
                            xmlns + "url",
                        new XElement(xmlns + "loc", newsSitemapNode.Url),
                        newsElement
                );
                root.Add(urlElement);
            }
            XDocument document = new XDocument(root);

            payload = document.ToString();

            await _cacheService.AddToCache(_newsCacheKey, payload);

            return payload;
        }
    }
}
