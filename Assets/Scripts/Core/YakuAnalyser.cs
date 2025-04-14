using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class YakuAnalyser
{

    //List<KeyValuePair<Tile, List<Tile>>> waits
    // /\
    // ||
    // список пар вида <тайл для сброса, список ожиданий которые будут после сброса этого тайла>
    public void AnalyzeYaku(List<KeyValuePair<Tile, List<Tile>>> waits,List<List<Tile>> completeBlocks)
    {
        //foreach (var wait in waits)
        //{
        //    string output = string.Empty;
        //    foreach (var tile in wait.Value)
        //    {
        //        output += tile.ToString() + " ";
        //    }
        //    Debug.Log("Тайл для сброса: "+wait.Key.ToString() + " Ожидания: " + output);
        //}




    }
}
