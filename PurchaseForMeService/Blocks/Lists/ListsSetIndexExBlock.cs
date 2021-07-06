using System;
using System.Collections.Generic;
using System.Linq;
using IronBlock;
using IronBlock.Blocks;

namespace PurchaseForMeService.Blocks.Lists
{
    public class ListsSetIndexExBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            List<object> list = (List<object>)Values.Evaluate("LIST", context);
            //Object list element will be.
            object to = Values.Evaluate("TO", context);
            string mode = Fields.Get("MODE");
            //If using an index
            int at = 0;
            bool insertMode = false;
            if (this.Values.Any(x => x.Name == "AT"))
            {
                at = (int)Values.Evaluate("AT", context);
            }

            switch (mode)
            {
                case "SET":
                    insertMode = false;
                    break;
                case "INSERT":
                    insertMode = true;
                    break;
            }

            string where = Fields.Get("WHERE");
            switch (where)
            {
                case "FROM_START":
                    if (insertMode)
                    {
                        list.Insert(at, to);
                    }
                    else
                    {
                        list[at] = to;
                    }
                    break;
                case "FROM_END":
                    if (insertMode)
                    {
                        list.Insert(list.Count - at, to);
                    }
                    else
                    {
                        list[^at] = to;
                    }
                    break;
                case "FIRST":
                    if (insertMode)
                    {
                        list.Insert(0, to);
                    }
                    else
                    {
                        list[0] = to;
                    }
                    break;
                case "LAST":
                    if (insertMode)
                    {
                        list.Add(to);
                    }
                    else
                    {
                        list[^1] = to;
                    }
                    break;
                case "RANDOM":
                    Random random = new Random();
                    if (insertMode)
                    {
                        list.Insert(random.Next(0, list.Count), to);
                    }
                    else
                    {
                        list[random.Next(0, list.Count)] = to;
                    }
                    break;
            }

            base.Evaluate(context);
            return null;
        }
    }
}