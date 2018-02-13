using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prosegur.Processo.TestarBuscarLatLong
{
    class Program
    {
        static void Main(string[] args)
        {
            var ProcessoBuscarLatLong = new Prosegur.Processo.BuscarLatLong.Processo();
            Console.WriteLine("Chamando processo BuscarLatLong");
            ProcessoBuscarLatLong.Executar();
            Console.WriteLine("Encerrando processo BuscarLatLong");
            Console.ReadLine();
        }
    }
}
