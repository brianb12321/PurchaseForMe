﻿using System;
using AngleSharp.Dom;
using IronBlock;
using IronBlock.Blocks;
using OpenQA.Selenium;

namespace PurchaseForMe.Blocks.Web.AngleSharp
{
    [RegisterBlock("web_sendKey", Category = "AngleSharp")]
    public class SendKeyBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            throw new PlatformNotSupportedException("AngleSharp does not support clicking.");
        }
    }
}