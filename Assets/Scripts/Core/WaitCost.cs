using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Хранит ожидание и стоимость руки если этим ожиданием завершится рука
/// </summary>
    public class WaitCost
    {

        public WaitCost()
        {
            Costs = new List<(string Yaku, int Cost)>();
        }

        public Tile Wait { get; set; }
        public List<(string Yaku, int Cost)> Costs { get; set; }
    }

