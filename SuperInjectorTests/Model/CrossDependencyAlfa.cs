using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperInjectorTests.Model.Interface;

namespace SuperInjectorTests.Model
{
    internal class CrossDependencyAlfa : ICrossDependencyAlfa
    {
        public CrossDependencyAlfa(ICrossDependencyBeta beta) { 
        
        }
    }
}
