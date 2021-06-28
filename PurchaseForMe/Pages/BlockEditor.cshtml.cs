using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IronBlock;
using IronBlock.Blocks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using PurchaseForMe.Blocks;
using PurchaseForMe.Blocks.Error;
using PurchaseForMe.Blocks.Pipeline;
using PurchaseForMe.Blocks.Web;

namespace PurchaseForMe.Pages
{
    public class BlockEditorModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
