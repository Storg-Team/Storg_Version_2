using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using StorgCommon;

namespace StorgLibs
{
    public class SystemHelper
    {

        public ModelCurrentOS GetCurrentOS()
        {

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return new ModelCurrentOS(){ OS = "Windows"};
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return new ModelCurrentOS(){ OS = "Linux"};
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return new ModelCurrentOS(){ OS = "MacOS"};
            throw new PlatformNotSupportedException("OS non supporté !");

        }
    }
}
