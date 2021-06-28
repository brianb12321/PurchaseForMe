using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.XPath;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ScrapySharp.Extensions;
using ScrapySharp.Network;

namespace PurchaseForMe.Pages
{
    [Authorize]
    public class ScrapeModel : PageModel
    {
        private readonly ScrapingBrowser _browser;
        [BindProperty(SupportsGet = true)]
        public List<string> Result { get; set; }
        public ScrapeModel()
        {
            _browser = new ScrapingBrowser();
        }
        public async Task OnGet()
        {
            WebPage page = await _browser.NavigateToPageAsync(new Uri(
                @"https://www.ebay.com/sch/i.html?_from=R40&_trksid=p2334524.m570.l1313&_nkw=garurumon+statue&_sacat=0&LH_TitleDesc=0&_odkw=garurumon+statue&_osacat=0"));

            Result = new List<string>();
            var titles = page.Html.CssSelect(".s-item__title");
            foreach (var title in titles)
            {
                if (!string.IsNullOrEmpty(title.InnerText))
                {
                    Result.Add(title.InnerText);
                }
            }
        }
    }
}