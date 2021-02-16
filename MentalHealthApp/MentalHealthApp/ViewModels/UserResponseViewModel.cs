using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MentalHealthApp.ViewModels
{
    public class UserResponseViewModel
    {
        public int Id { get; set; }    
        public string Name { get; set; }
        public string Emotion { get; set; }
        public string Type { get; set; }
        public string Response { get; set; }
        public DateTime Date { get; set; }
    }
}
