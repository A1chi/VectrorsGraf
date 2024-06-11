using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VectorsGraf
{
    public class KeyControl
    {
        
        public delegate void Handler(ConsoleKey key);
        public event Handler usedKey;

        public void Listen()
        {
            usedKey?.Invoke(Console.ReadKey(true).Key);
            Listen();
        }
    }
}
