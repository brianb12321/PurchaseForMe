using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using IronBlock;
using IronBlock.Blocks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PurchaseForMeService.Blocks.Web.Selenium
{
    [RegisterBlock("web_selectDropDown", Category = "Selenium")]
    public class SelectDropdownBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            IWebElement element = (IWebElement) Values.Evaluate("element", context);
            string selectValue = Values.Evaluate("selectValue", context).ToString();
            //create select element object 
            var selectElement = new SelectElement(element);
            // select by text
            selectElement.SelectByText(selectValue);
            base.Evaluate(context);
            return null;
        }
    }
}
