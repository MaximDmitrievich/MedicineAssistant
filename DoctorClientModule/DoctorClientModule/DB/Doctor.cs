using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorClientModule.DB
{
    public class Doctor
    {
        public string Id { get; set; }
        public string Login { get; set; } = "";
        public string Password { get; set; } = "";
        public string Name { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Patronymic { get; set; } = "";
        public string Specialization { get; set; } = "";
    }
}
