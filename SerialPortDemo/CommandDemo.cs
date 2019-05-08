using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialPortDemo
{
    public enum CommandType
    {
        PluseUnlock = 0x01,//脉冲开锁
        Lock = 0x02,//闭锁
        Unlock = 0x03,//开锁
        QueryState = 0x04,//查询当前锁状态
    }
    public class CommandDemo
    {
        SerialPort serialPort;
        string[] portNames;
        //锁板地址
        List<byte> addresses;
        public CommandDemo()
        {
            Init();
        }
        void Init()
        {
            addresses = new List<byte>();
            portNames = SerialPort.GetPortNames();
            serialPort = new SerialPort();
            if(portNames.Length != 0)
            {
                //设置串口名，到时候按照具体插的哪个串口设置
                serialPort.PortName = portNames[0];
                //波特率，默认就是9600
                //serialPort.BaudRate = 9600;
                serialPort.DataReceived += ReceiveMessage;//接收到返回数据的事件
                serialPort.Open();
            }
            //锁板地址，有24个
            for(int i = 1;i <= 24;i++)
            {
                addresses.Add((byte)i);
            }
        }
        /// <summary>
        /// 操作，有什么操作看CommandType
        /// </summary>
        /// <param name="boardIndex">第几个板子，从0开始算</param>
        /// <param name="blockIndex">板子的第几个锁，从0开始算</param>
        /// <param name="commandType">要操作的类型</param>
        public void Operate(int boardIndex, int blockIndex, CommandType commandType)
        {
            List<byte> commond = new List<byte>();
            byte[] commondArray = null;
            if (boardIndex > addresses.Count - 1)
            {
                throw new IndexOutOfRangeException("没有那么多锁");
            }
            commond.Add(0x98);//命令头
            commond.Add(addresses[boardIndex]);//锁板地址
            commond.Add((byte)(blockIndex + 1));//锁地址
            commond.Add((byte)commandType);//命令
            for (int i = 0; i < 4; i++)//默认0位
            {
                commond.Add(0x00);
            }
            byte checksum = CheckSum(commond);//校验和
            commond.Add(checksum);
            commondArray = commond.ToArray();//最终的命令

            //发送命令
            SendCommand(commondArray, commondArray.Length);
        }
        void ReceiveMessage(object sender, SerialDataReceivedEventArgs e)
        {
            //暂时不知道那边回返回什么结尾符，所以先这样写
            if(e.EventType == SerialData.Eof)
            {
                string str = serialPort.ReadExisting();
                Console.WriteLine(str);
                Debug.WriteLine(str);
            }
        }
        void SendCommand(byte[] commond,int length)
        {
            serialPort.Write(commond, 0, length);

        }
        public void Test()
        {
            byte[] bytes = { 0x98, 0x01, 0x01, 0x01, 0, 0, 0, 0 };
            byte b = CheckSum(bytes);
            Debug.WriteLine(b.ToString("X"));
        }
        byte CheckSum(IEnumerable<byte> bytes)
        {
            byte sum = 0;
            foreach (var b in bytes)
            {
                sum += b;
            }
            return sum;
        }
    }
}
