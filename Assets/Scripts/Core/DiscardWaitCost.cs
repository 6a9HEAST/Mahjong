using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/// <summary>
/// Хранит тайл который можно сбросить и ожидания которые после этого будут
/// </summary>
public class DiscardWaitCost
    {
        public DiscardWaitCost()
        {
            WaitsCosts = new List<WaitCost>();
        }

        public Tile Discard { get; set; }
        public List<WaitCost> WaitsCosts { get; set; }
    }

