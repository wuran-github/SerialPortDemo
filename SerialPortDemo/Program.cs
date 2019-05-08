using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialPortDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandDemo demo = new CommandDemo();
            demo.Operate(0, 0, CommandType.Lock);//锁上第1个板子的第一个锁
        }
    }
}
