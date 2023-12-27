using System.Diagnostics;

using XGCommLib;

namespace Desummer.Scripts
{
    public class PLCControl
    {
        CommObjectFactory20 factory;
        CommObject20 oCommDriver;

        readonly string plcIPAdress = "192.168.1.33:2004";

        public PLCControl()
        { 
            // plc 연결 설정
            factory = new CommObjectFactory20();
            oCommDriver = factory.GetMLDPCommObject20(plcIPAdress);
        }

        /// <summary>
        /// PLC - PC 연결
        /// </summary>
        bool ConnectToPLC()
        {
            oCommDriver.RemoveAll();

            // 연결 실패시, false return을 해준다
            if (oCommDriver.Connect("") == 0)
                return false;

            return true;
        }

        /// <summary>
        /// 데이터 전송
        /// </summary>
        /// <param name="aTempNormal">A보온로 정상, 비정상 체크</param>
        /// <param name="bTempNormal">B보온로 정상, 비정상 체크</param>
        /// <param name="cTempNormal">C보온로 정상, 비정상 체크</param>
        public void SendData(bool aTempNormal, bool bTempNormal, bool cTempNormal)
        {
            // 연결 실패시 아무 작동하지 않음
            if (!ConnectToPLC())
                return;

            // 최대 6개의 데이터만 보낼 것이기 때문에 6으로 설정
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
                Debug.WriteLine("데이터 작성 실패");
                DisconnectPLC(oCommDriver);
                return;
            }

            byte[] bufRead = new byte[6];

            // 제대로 전송이되고 plc와 데이터가 동일한지 체크
            if (oCommDriver.ReadRandomDevice(bufRead) != 1)
            {
                Debug.WriteLine("데이터 읽기 실패");
                DisconnectPLC(oCommDriver);
                return;
            }
            else
            {
                if (!bufWrite.SequenceEqual(bufRead))
                    Debug.WriteLine("데이터 불일치");
                
                DisconnectPLC(oCommDriver);
            }
        }


        /// <summary>
        /// PLC의 내부 메모리에 %MX로 보낼 데이터 작성
        /// </summary>
        /// <param name="oDevice"></param>
        /// <param name="offset"></param>
        /// <param name="normal">정상, 비정상</param>
        /// <returns></returns>
        byte WriteDataInPLC(DeviceInfo oDevice, int offset, bool normal)
        {
            oDevice.ucDataType = (byte)'X';
            oDevice.ucDeviceType = (byte)'M';
            oDevice.lOffset = offset / 8;
            oDevice.lSize = offset % 8;
            oCommDriver.AddDeviceInfo(oDevice);

            return normal ? (byte)1 : (byte)0;
        }

        /// <summary>
        /// PLC-PC 통신 종료
        /// </summary>
        /// <param name="oCommDriver"></param>
        void DisconnectPLC(CommObject20 oCommDriver)
        {
            int nRetn = oCommDriver.Disconnect();

            if (nRetn == 1)
                Debug.WriteLine("연결 정상 종료");
            else
            {
                while (nRetn != 1)
                {
                    Debug.WriteLine("연결 종료 실패! 재시도");
                    nRetn = oCommDriver.Disconnect();
                }

                Debug.WriteLine("연결 정상 종료");
            }
        }
    }
}