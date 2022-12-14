using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Exceptions
{
    public class InventoryException : Exception
    {
        public InventoryException()
        {
        }

        public InventoryException(string message)
            : base(message)
        {
        }

        public InventoryException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
