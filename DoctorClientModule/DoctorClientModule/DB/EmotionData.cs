using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorClientModule.DB
{
    public class EmotionData
    {
        public string Id { get; set; } = "";
        public DateTime Date { get; set; } = DateTime.MinValue;
        public string Emotion { get; set; } = "";
        public string UserId { get; set; } = "";
    }
}
