using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorClientModule.DB
{
    public class Pacient
    {
        public string Id { get; set; } = "";
        public string SNILS { get; set; } = "";
        public string Name { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Patronymic { get; set; } = "";
        public DateTime BirthdayDate { get; set; }
        public string DeviceID { get; set; } = "";
        public List<History> DiseaseHistory { get; set; } = new List<History>();
        public List<HumanTags> HumanTagses { get; set; } = new List<HumanTags>();
    }
}
