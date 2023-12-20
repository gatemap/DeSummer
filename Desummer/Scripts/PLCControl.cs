using System.Windows.Controls;

using XGCommLib;

namespace Desummer.Scripts
{
    public class PLCControl
    {
        CommObjectFactory20 factory;
        CommObject20 oCommDriver;

        TextBlock failureIndication;
        Button reconnectButton;

        bool connecting = false;

        readonly string plcIPAdress = "192.168.1.33:2004";

        public PLCControl(TextBlock textBlock, Button button) 
        { 
            factory = new CommObjectFactory20();
            oCommDriver = factory.GetMLDPCommObject20(plcIPAdress);

            failureIndication = textBlock;
            reconnectButton = button;

            ActiveTextBlock("", false);
        }

        /// <summary>
        /// PLC - PC 연결
        /// </summary>
        public void ConnectToPLC()
        {
            // 연결 실패시, 텍스트를 출력해주고 재통신 버튼을 활성화해줌
            if (oCommDriver.Connect("") == 0)
            {
                ActiveTextBlock("통신 실패!", true);
                connecting = false;
                return;
            }

            ActiveTextBlock("", false);
            connecting = true;
        }

        /// <summary>
        /// 데이터 전송
        /// </summary>
        /// <param name="aTempNormal">A보온로 정상, 비정상 체크</param>
        /// <param name="bTempNormal">B보온로 정상, 비정상 체크</param>
        /// <param name="cTempNormal">C보온로 정상, 비정상 체크</param>
        public void SendData(bool aTempNormal, bool bTempNormal, bool cTempNormal)
        {
            if (!connecting)
                return;

            byte[] bufWrite = new byte[6];

            DeviceInfo oDevice = factory.CreateDevice();
            
            bufWrite[0] = WriteDataInPLC(oDevice, 0, aTempNormal);
            bufWrite[1] = WriteDataInPLC(oDevice, 1, !aTempNormal);
            bufWrite[2] = WriteDataInPLC(oDevice, 2, bTempNormal);
            bufWrite[3] = WriteDataInPLC(oDevice, 3, !bTempNormal);
            bufWrite[4] = WriteDataInPLC(oDevice, 4, cTempNormal);
            bufWrite[5] = WriteDataInPLC(oDevice, 5, !cTempNormal);

            // 데이터 쓰기에 실패하면 연결도 끊음
            if (oCommDriver.WriteRandomDevice(bufWrite) != 1)
            {
                ActiveTextBlock("전송 실패!", true);
                DisconnectPLC(oCommDriver);
                return;
            }

            byte[] bufRead = new byte[6];

            // 제대로 전송이되고 plc와 데이터가 동일한지 체크
            if (oCommDriver.ReadRandomDevice(bufRead) != 1)
                ActiveTextBlock("읽기 실패!", true);
            else
            {
                if (bufWrite.SequenceEqual(bufRead))
                    ActiveTextBlock("", false);
                else
                    ActiveTextBlock("데이터 불일치!", true);
            }

            DisconnectPLC(oCommDriver);
        }

        void ActiveTextBlock(string text, bool enabled)
        {
            failureIndication.Text = text;
            reconnectButton.IsEnabled = enabled;
        }

        byte WriteDataInPLC(DeviceInfo oDevice, int offset, bool normal)
        {
            oDevice.ucDataType = (byte)'X';
            oDevice.ucDeviceType = (byte)'M';
            oDevice.lOffset = offset / 8;
            oDevice.lSize = offset % 8;
            oCommDriver.AddDeviceInfo(oDevice);

            return normal ? (byte)1 : (byte)0;
        }

        void DisconnectPLC(CommObject20 oCommDriver)
        {
            int nRetn = oCommDriver.Disconnect();

            if (nRetn == 1)
                ActiveTextBlock("", false);
            else
            {
                while(nRetn != 1)
                    nRetn = oCommDriver.Disconnect();
            }
        }
    }
}