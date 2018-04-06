using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorClientModule.DB
{
    public class History
    {
        public DateTime Date { get; set; }
        public string DoctorID { get; set; } = "";
        public string Text { get; set; } = "";
    }
}
