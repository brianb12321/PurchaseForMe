using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronBlock;

namespace PurchaseForMe.Blocks.Pipeline
{
    [RegisterBlock("pipeline_createObject")]
    public class CreateObjectBlock : IBlock
    {
        public override object Evaluate(Context context)
        {
            dynamic returnObject = new System.Dynamic.ExpandoObject();
            for (int i = 0; i < this.Fields.Count; i++)
            {
                Field propertyName = this.Fields[i];
                Value propertyValue = this.Values[i];
                ((IDictionary<string, Object>)returnObject).Add(propertyName.Value, propertyValue.Evaluate(context));
            }

            return returnObject;
        }
    }
}