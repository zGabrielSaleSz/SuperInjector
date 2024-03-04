using SuperInjectorTests.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperInjectorTests.Model
{
    internal class CrossDependencyBeta : ICrossDependencyBeta
    {
        public CrossDependencyBeta(ICrossDependencyAlfa alfa) { 
        
        }
    }
}
