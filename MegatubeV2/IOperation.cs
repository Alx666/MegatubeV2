using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MegatubeV2
{
    internal interface IOperation
    {
        void Execute();

        string ReturnActionName     { get; }
        string ReturnControllerName { get; }        
        object ReturnRouteValues    { get; }
    }
}
