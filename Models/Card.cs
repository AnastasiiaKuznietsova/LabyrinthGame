using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthGame.Models
{
    public class Card
    {
        public string Id { get; set; } = string.Empty;

        public string ImagePath { get; set; } = string.Empty;

        public double ImageAngle { get; set; }
    }
}
