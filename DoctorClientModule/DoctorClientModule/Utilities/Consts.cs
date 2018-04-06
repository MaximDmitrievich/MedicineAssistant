using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorClientModule.Utilities
{
    public static class Consts
    {
        public const string EndPoint = @"http://itts-worker.westeurope.cloudapp.azure.com/";


        public const string Dependency = EndPoint + @"api/dependency";
        public const string Doctors = EndPoint + @"api/doctors";
        public const string Pacients = EndPoint + @"api/pacientsdata";
        public const string Chart = EndPoint + @"api/chart";

    }
}
