using System;
using System.Linq;
using machineCRUD.DataOperations;
using machineCRUD.Models;

namespace machineCRUD
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var inventoryDal = new InventoryDAL();
            
            var list = inventoryDal.GetAllMachines();
            foreach (var machine in list)
            {
                Console.WriteLine(machine.ToString());
            }
        }
    }
}