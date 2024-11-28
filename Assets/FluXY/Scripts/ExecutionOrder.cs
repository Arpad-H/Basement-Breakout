using System;

namespace Fluxy
{

    public class ExecutionOrder : Attribute
    {
        public int order;

        public ExecutionOrder(int order)
        {
            this.order = order;
        }
    }
}