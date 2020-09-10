/*
   普通节点的框架结构
   0x01 -> 更新HopsToSink
   ATCommand 0x4E 0x44 -> 网络邻居发现
   0x02 -> 查询邻居节点的信息
   0x03 -> 对邻居节点查询的响应
   0x04 -> Preamble packet
   0x05 -> ACK_Preamble packet
   0x06 -> Data packet
   0x07 -> 邻居发现
   0x08 -> 对邻居发现的响应
*/
#include<MsTimer2.h>
#include<math.h>
#include<ListLib.h>
#include<SimpleTimer.h>
#include<QList.h>
/*
 * 结构体定义
*/
struct Position
{
  byte x[4];
  byte y[4];
};
/* packet格式 */
typedef struct Packet
{
  byte OverheadTransmissions;
  byte Hops;
  byte WaitingTimes;
  byte SourceIPAddress[2];
  byte DestinationIPAddress[2];
} Packet;
/* 节点信息 */
typedef struct NodeInfo
{
  byte IPAddr[2];
  byte MACAddr_H[4];
  byte MACAddr_L[4];
} NodeInfo;
/* 邻居表中节点信息 */
typedef struct NeighborsTableEntry
{
  NodeInfo NeiNode;
} NeighborsTableEntry;
/*
 * 时间戳定义
*/
SimpleTimer simpleTimer;
unsigned long GlobalTimer;
unsigned long neighborsDiscoveryTimer;
unsigned long computedHopsTimer;

/*
 * 公共变量定义
*/
byte HopsToSink = 0x00;
bool isSink = true;
float CommunicationRangeRadius = 5.0;
unsigned long NumberOfDeliveryPacket=0;
unsigned long TotalWaitingTimes=0;
unsigned long TotalOverheadTransmissions=0;
struct Position MyPos = {{0x00,0x00,0x00,0x00},{0x40,0x00,0x00,0x00}};
byte myIPAddress[2] = {0x99, 0x99};
byte myMACAddress[8]={0x00,0x13,0xA2,0x00,0x41,0x91,0xD5,0x76};
/*sink节点拥有无限的能量*/
byte InitialEnergy[4]={0x40,0xA0,0x00,0x00};
byte CurrentEnergy[4]={0x40,0xA0,0x00,0x00};
byte broadcastAddr_64[8] = {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF};
List<NeighborsTableEntry> NeiTable;
byte FID = 1;
long responseDelay;

/*
 * Operations
*/
//Float->HEX
void FloatToHEX(float a, byte* buf)
{
  //float->binaryStr
  char str[33] = {'0'};
  unsigned long c = ((unsigned long*)&a)[0];
  for (unsigned int i = 0; i < 32; i++)
  {
    str[31 - i] = (char)(c & 1) + '0';
    c >>= 1;
  }
  str[32] = '\0';

  //binaryStr->HEX
  byte x = 0;
  for (unsigned int j = 0; j < 4; j++)
  {
    x = 128 * (str[j * 8] - '0');
    x += 64 * (str[j * 8 + 1] - '0');
    x += 32 * (str[j * 8 + 2] - '0');
    x += 16 * (str[j * 8 + 3] - '0');
    x += 8 * (str[j * 8 + 4] - '0');
    x += 4 * (str[j * 8 + 5] - '0');
    x += 2 * (str[j * 8 + 6] - '0');
    x += str[j * 8 + 7] - '0';
    buf[j] = x;
  }
}
//HEX->Float
float HexToFloat(byte* buf, int len)
{
  //HEX->binaryStr
  char str[33] = {'0'};
  for (unsigned int i = 0; i < len; i++)
  {
    int tmp = buf[i];
    int c = 7;
    while (c >= 0)
    {
      str[i * 8 + c] = (char)((tmp % 2) & 1) + '0';
      tmp = tmp / 2;
      c--;
    }
  }
  str[32] = '\0';

  //binaryStr->Float
  unsigned long flt = 0;
  for (int j = 0; j < 31; j++)
  {
    flt += (str[j] - '0');
    flt <<= 1;
  }
  flt += (str[31] - '0');
  float* ret = (float*)&flt;
  return *ret;
}
//通过sender的源地址寻找sender的MAC地址
boolean FindMacAddrByIpAddr(byte IPAddr_H, byte IPAddr_L, byte* buf)
{
  int neiLen = NeiTable.Count();
  unsigned int index = 255;
  for (int i = 0; i < neiLen; i++)
  {
    if (NeiTable[i].NeiNode.IPAddr[0] == IPAddr_H && NeiTable[i].NeiNode.IPAddr[1] == IPAddr_L)
    {
      index = i;
      break;
    }
  }
  if (index == 255)
  {
    return false;
  } else
  {
    byte* p = NeiTable[index].NeiNode.MACAddr_H;
    for (int j = 0; j < 8; j++)
    {
      buf[j] = *p;
      p++;
    }
    return true;
  }
}
//计算两个节点之间的距离
float DistanceBetweenSensors(Position sender, Position neighbor)
{
  float s_x = HexToFloat(sender.x, 4);
  float s_y = HexToFloat(sender.y, 4);
  float n_x = HexToFloat(neighbor.x, 4);
  float n_y = HexToFloat(neighbor.y, 4);
  float dx = (s_x - n_x);
  dx *= dx;
  float dy = (s_y - n_y);
  dy *= dy;
  return sqrt(dx + dy);
}

/* 
 *  发包
*/

/*
 * 邻居发现 -> 0x07
*/
void NeighborsDiscovery()
{
  byte packet[32] = {0x7E,0x00,0x00,0x00};
  unsigned int checkSum = 0;
  unsigned int idx = 4;
  //设置帧的ID
  packet[idx++] = FID;
  checkSum += FID;
  //设置广播地址
  for (int i = 0; i < 8; i++)
  {
    packet[idx++] = broadcastAddr_64[i];
    checkSum += broadcastAddr_64[i];
  }
  //设置options
  packet[idx++] = 0;
  //自定义包的类型0x07表示这是一个邻居发现的包
  packet[idx++] = 0x07;
  checkSum += 0x07;
  //设置MyPos.x
  for(unsigned int j=0;j<4;j++)
  {
    packet[idx++] = MyPos.x[j];
    checkSum += MyPos.x[j];
  }
  //设置MyPos.y
  for(unsigned int k=0;k<4;k++)
  {
    packet[idx++] = MyPos.y[k];
    checkSum += MyPos.y[k];
  }
  //设置校验和
  packet[idx++] = 0xFF - (checkSum & 0xFF);
  //设置帧的长度
  packet[2] = 0xFF & (idx - 4);
  Serial1.write(packet, idx);
  Serial.println();
  FID++;
}
/*
 * 响应邻居发现 -> 0x08
*/
void ResponsedNeighborsDiscovery()
{
  byte packet[64] = {0x7E,0x00,0x00,0x00};
  unsigned int checkSum = 0;
  unsigned int idx = 4;
  //设置帧的ID
  packet[idx++] = FID;
  checkSum += FID;
  //设置广播地址
  for (int i = 0; i < 8; i++)
  {
    packet[idx++] = broadcastAddr_64[i];
    checkSum += broadcastAddr_64[i];
  }
  //设置options
  packet[idx++] = 0;
  //自定义包的类型0x08表示这是一个响应邻居发现的包
  packet[idx++] =0x08;
  checkSum += 0x08;
  //MyPos -> IP地址 -> MAC地址
  //设置MyPos.x
  for(unsigned int j=0;j<4;j++)
  {
    packet[idx++] = MyPos.x[j];
    checkSum += MyPos.x[j];
  }
  //设置MyPos.y
  for(unsigned int k=0;k<4;k++)
  {
    packet[idx++] = MyPos.y[k];
    checkSum += MyPos.y[k];
  }
  //设置IP地址
  packet[idx++] = myIPAddress[0];
  checkSum += myIPAddress[0];
  packet[idx++] = myIPAddress[1];
  checkSum += myIPAddress[1];
  //设置MAC地址
  for(unsigned int m=0;m<8;m++)
  {
    packet[idx++] = myMACAddress[m];
    checkSum += myMACAddress[m];
  }
  //设置校验和
  packet[idx++] = 0xFF - (checkSum & 0xFF);
  //设置帧的长度
  packet[2] = 0xFF & (idx - 4);
  Serial1.write(packet, idx);
  Serial.println();
  FID++;
}

/*
   更新HopsToSink信息,一开始由sink来发送 -> 0x01
*/
void HopsToSinkComputation()
{
  byte packet[32] = {0x7E, 0x00, 0x00, 0x00};
  //64位的广播包地址
  //byte broadcastAddr_64[8] = {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF};
  unsigned int checkSum = 0;
  unsigned int idx = 4;
  //设置帧的ID
  packet[idx++] = FID;
  checkSum += FID;
  //设置广播地址
  for (int i = 0; i < 8; i++)
  {
    packet[idx++] = broadcastAddr_64[i];
    checkSum += broadcastAddr_64[i];
  }
  //设置options
  packet[idx++] = 0;
  //自定义包的类型0x01表示这是一个广播跳数的包
  packet[idx++] = 0x01;
  checkSum += 0x01;
  /*设置节点的位置坐标*/
  //设置MyPos.x
  for(unsigned int j=0;j<4;j++)
  {
    packet[idx++] = MyPos.x[j];
    checkSum += MyPos.x[j];
  }
  //设置MyPos.y
  for(unsigned int k=0;k<4;k++)
  {
    packet[idx++] = MyPos.y[k];
    checkSum += MyPos.y[k];
  }
  //设置到sink的跳数
  packet[idx++] = HopsToSink;
  checkSum += HopsToSink;
  //设置校验和
  packet[idx++] = 0xFF - (checkSum & 0xFF);
  //设置帧的长度
  packet[2] = 0xFF & (idx - 4);
  Serial1.write(packet, idx);
  Serial.println();
  FID++;
}
/*
   响应邻居节点的信息查询请求 -> 0x03
*/
void ResponseQuery(byte IPAddr_H, byte IPAddr_L)
{
  int neiLen = NeiTable.Count();
  byte packet[64] = {0x7E, 0x00, 0x00, 0x00};
  unsigned int checkSum = 0;
  unsigned int idx = 4;
  //设置帧的ID
  packet[idx++] = FID;
  checkSum += FID;
  //查找sender节点的MAC地址
  unsigned int senderIndex = 255;
  for (unsigned int i1 = 0; i1 < neiLen; i1++)
  {
    if (NeiTable[i1].NeiNode.IPAddr[0] == IPAddr_H && NeiTable[i1].NeiNode.IPAddr[1] == IPAddr_L)
    {
      senderIndex = i1;
      break;
    }
  }
  if (senderIndex == 255)
  {
    Serial.println(F("cant responseQuery"));
    return;
  }
  //设置sender节点MAC的地址
  byte* addrMAC = NeiTable[senderIndex].NeiNode.MACAddr_H;
  for (unsigned int i2 = 0; i2 < 8; i2++)
  {
    packet[idx++] += *addrMAC;
    checkSum += *addrMAC;
    addrMAC++;
  }
  //设置Options
  packet[idx++] = 0;
  //0x03表示这是对邻居节点查询信息的响应
  packet[idx++] = 0x03;
  checkSum += 0x03;
  /*
     设置请求数据
  */
  //设置节点的横坐标
  for (unsigned int j1 = 0; j1 < 4; j1++)
  {
    packet[idx++] = MyPos.x[j1];
    checkSum += MyPos.x[j1];
  }
  //设置节点的纵坐标
  for (unsigned int j2 = 0; j2 < 4; j2++)
  {
    packet[idx++] = MyPos.y[j2];
    checkSum += MyPos.y[j2];
  }
  //设置初始能量
  for(unsigned int j3=0;j3<4;j3++)
  {
    packet[idx++] = InitialEnergy[j3];
    checkSum += InitialEnergy[j3];
  }
  //设置当前能量
  for(unsigned int j4=0;j4<4;j4++)
  {
    packet[idx++] = CurrentEnergy[j4];
    checkSum += CurrentEnergy[j4];
  }
  //设置校验和
  packet[idx++] = 0xFF - (checkSum & 0xFF);
  //设置帧的长度
  packet[2] = 0xFF & (idx - 4);
  Serial1.write(packet, idx);
  FID++;
}

/*
   发送ACK_Preamble packet -> 0x05
   Length = 128bits
   帧格式:
*/
void SendAckToPreamblePacket(byte* addr)
{
  for(int i=0;i<8;i++)
  {
    if(addr[i]<0x10){Serial.print(F("0"));}
    Serial.print(addr[i],HEX);
  }
  Serial.println();
  byte packet[128] = {0x7E, 0x00, 0x00, 0x00};
  unsigned int checkSum = 0;
  unsigned int idx = 4;
  //设置帧的ID
  packet[idx++] = FID;
  checkSum += FID;
  //设置下一跳的MAC地址
  for (int i = 0; i < 8; i++)
  {
    packet[idx++] = *addr;
    checkSum += *addr;
    addr++;
  }
  //设置Options
  packet[idx++] = 0;
  //0x05表示这是一个ACK_Preamble packet
  packet[idx++] = 0x05;
  checkSum += 0x05;
  /*
     填充满128bits数据？
  */
  //设置校验和
  packet[idx++] = 0xFF - (checkSum & 0xFF);
  //设置帧的长度
  packet[2] = 0xFF & (idx - 4);
  Serial1.write(packet, idx);
  Serial.println(F("\n\r SendACK"));
  FID++;
}

//网络邻居发现
/*
void NeighborsDiscovery()
{
  byte packet[] = {0x7E, 0x00, 0x04, 0x08, 0x01, 0x4E, 0x44, 0x64};
  byte packetLength = 8;
  Serial1.write(packet, packetLength);
  Serial.println();
}
*/
byte SerialInput()
{
  return (Serial1.read());
}

// len是将要读入的数据长度
void AddNeiTable(unsigned int len, unsigned int checkSumTemp)
{
  int neiLen = NeiTable.Count();
  //开辟一个接收读入数据的空间
  byte Info[30] = {0};
  //Info[0]是Command Status
  for (unsigned int c = 0; c < len; c++)
  {
    Info[c] = SerialInput();
    checkSumTemp += Info[c];
  }
  //根据校验和检查接收的数据是否有问题
  unsigned int checkSum = SerialInput();
  if (checkSum != (0xFF - (checkSumTemp & 0xFF)))
  {
    Serial.print(F("\nCheckSum Error from AddNeiTable\n\r"));
  }else{
    unsigned int flag = 0;
    //提前设置了邻居表空间，找到空的位置；idx==空的位置
    for (unsigned int i = 0; i < neiLen; i++)
    {
      if (NeiTable[i].NeiNode.IPAddr[0] == Info[0] && NeiTable[i].NeiNode.IPAddr[1] == Info[1])
      {
        flag = 1;
        break;
      }
    }
  
    if (flag == 0)
    {
      NeighborsTableEntry tmp = {0};
      byte* p1 = tmp.NeiNode.IPAddr;
      for(int j=0;j<10;j++)
      {
        *p1 = Info[j];
        p1++;
      }
      delay(10);
      NeiTable.Add(tmp);
    }
  }
}

//打印邻居表
void PrintNeiTable()
{
  int neiLen = NeiTable.Count();
  for (unsigned int i = 0; i < neiLen; i++)
  {
    Serial.print(F("\n\r"));
    byte* p = NeiTable[i].NeiNode.IPAddr;
    for (unsigned int c = 0; c < 10; c++)
    {
      if (*p < 0x10)
      {
        Serial.print(F("0"));
      }
      Serial.print(*p, HEX);
      Serial.print(F(""));
      p++;
    }
  }
}
void Initialization()
{
  GlobalTimer=millis();
  neighborsDiscoveryTimer=millis();
  computedHopsTimer=millis();
}
void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  Serial1.begin(9600);
  if (isSink)
  {
  }
  Initialization();
}

void loop() {
  // put your main code here, to run repeatedly:
  if(millis()-GlobalTimer<60000){
  }else if(millis()-GlobalTimer<70000){
    if(millis()-neighborsDiscoveryTimer>2000){
      neighborsDiscoveryTimer=millis();
      NeighborsDiscovery();
    }
  }else if((millis()-GlobalTimer>75000) && (millis()-GlobalTimer<85000)){
    if(millis()-computedHopsTimer>2000){
      computedHopsTimer=millis();
      HopsToSinkComputation();
    }
  }
  if (Serial1.available() > 0)
  {
    delay(200);
    unsigned int chkSum = 0;
    unsigned int length_H;
    unsigned int length_L;
    unsigned int byteCount;
    byte frameID;
    byte testData = SerialInput();
    if (testData == 0x7E)
    {
      Serial.println(F("0x7E"));
      //读取帧的长度
      length_H = SerialInput();
      length_L = SerialInput();
      byteCount = (length_H * 256) + length_L;

      //读取帧的类型
      byte frameType = SerialInput();
      chkSum += frameType;
      if (frameType == 0x88)
      {
        Serial.println(F("0x88"));
        /*
        //读取帧的ID
        frameID = SerialInput();
        //读取AT Command
        byte ATC_H = SerialInput();
        byte ATC_L = SerialInput();
        chkSum += frameID + ATC_H + ATC_L;
        if (ATC_H == 0x4E && ATC_L == 0x44)
        {
          //说明是一个用于网络节点发现的包
          AddNeiTable(byteCount - 4, chkSum);
        }
        Serial.print(F("\n\rPrint NeiTable: "));
        PrintNeiTable();
        Serial.print(F("\n\r"));
        */
      } else if (frameType == 0x81)
      {
        byte IPAddr_H = SerialInput();
        byte IPAddr_L = SerialInput();
        byte rssiValue = SerialInput();
        //获取options的值
        byte Opt = SerialInput();
        //读取自己设置的包的类型
        byte packetType = SerialInput();
        chkSum += IPAddr_H + IPAddr_L + rssiValue + Opt + packetType;
        if(packetType == 0x07)
        {
          struct Position NeiPos={0};
          //读取邻居节点横坐标
          for(unsigned int i=0;i<4;i++)
          {
            NeiPos.x[i]=SerialInput();
            chkSum += NeiPos.x[i];
          }
          //读取邻居节点纵坐标
          for(unsigned int j=0;j<4;j++)
          {
            NeiPos.y[j] = SerialInput();
            chkSum += NeiPos.y[j];
          }
          //读取校验和
          unsigned int checkSum = SerialInput();
          //验证数据包的有效性
          if (checkSum != (0xFF - (chkSum & 0xFF)))
          {
            Serial.print(F("\nError 0x00\n\r"));
          }else{
            //如果节点之间的距离 < 传输范围 --> 响应邻居发现
            if(DistanceBetweenSensors(MyPos,NeiPos) <= CommunicationRangeRadius)
            {
              Serial.println(F("ResponsedNeighborsDiscovery"));
              ResponsedNeighborsDiscovery();
            }
          }
        }
        else if(packetType == 0x08)
        {
          /*
           * 来自邻居节点对邻居发现的响应
          */
          // 读取邻居节点的位置 -> IP地址 ->MAC地址
          struct Position NeiPos={0};
          //读取邻居节点横坐标
          for(unsigned int i=0;i<4;i++)
          {
            NeiPos.x[i]=SerialInput();
            chkSum += NeiPos.x[i];
          }
          //读取邻居节点纵坐标
          for(unsigned int j=0;j<4;j++)
          {
            NeiPos.y[j] = SerialInput();
            chkSum += NeiPos.y[j];
          }
          //如果节点之间的距离 < 传输范围 -->加入到邻居表中
          if(DistanceBetweenSensors(MyPos,NeiPos) <= CommunicationRangeRadius)
          {
            AddNeiTable(byteCount-14,chkSum);
            Serial.print(F("\n\rPrint NeiTable: "));
            PrintNeiTable();
            Serial.print(F("\n\r"));
          }else{
            //读取脏字节
            byte buf[20] = {0};
            int len = byteCount-14;
            for(int k=0;k<len;k++){
              buf[k] = SerialInput();
              chkSum += buf[k];
            }
            //读取校验和
            unsigned int checkSum = SerialInput();
            //验证数据包的有效性
            if (checkSum != (0xFF - (chkSum & 0xFF)))
            {
              Serial.print(F("\nError 0x00\n\r"));
            }
          }
        }
        else if (packetType == 0x02)
        {
          /*
            来自邻居节点的信息查询
          */
          //读取校验和
          unsigned int checkSum = SerialInput();
          if (checkSum != (0xFF - (chkSum & 0xFF)))
          {
            Serial.print(F("\nError 0x02\n\r"));
          }else{
            //设置被响应的邻居节点的地址
            ResponseQuery(IPAddr_H, IPAddr_L);
          } 
        }
        else if (packetType == 0x04)
        {
          /*
           * this is a preamble packet
           * 读取the address of potential forwarders
           * 如果自己是 a potential forwarder，则返回ACK_PreamblePacket
          */
          boolean enabledAck=false;
          Serial.println(F("this is a preamble packet"));
          //读取候选节点的个数
          byte candidatesCount=SerialInput();
          chkSum+=candidatesCount;
          candidatesCount=candidatesCount/2;
          Serial.print(F("candidatesCount="));
          Serial.println(candidatesCount);
          /*
           * 判断自己的IpAddress在不在Preamble packet内
          */
          byte addressHigh=0;
          byte addressLow=0;
          for(int i=0;i<candidatesCount;i++)
          {
            addressHigh=SerialInput();
            chkSum+=addressHigh;
            addressLow=SerialInput();
            chkSum+=addressLow;
            Serial.print(F("addressHigh="));
            Serial.println(addressHigh,HEX);
            Serial.print(F("addressLow="));
            Serial.println(addressLow,HEX);
            delay(10);
            if(addressHigh==myIPAddress[0] && addressLow==myIPAddress[1])
            {
              enabledAck=true;
              Serial.println(F("enabledAck==ture"));
            }
          }
          //读取校验和
          unsigned int checkSum = SerialInput();
          if (checkSum != (0xFF - (chkSum & 0xFF)))
          {
            Serial.print(F("\nError 0x04\n\r"));
          }else{
            //寻找sender的MAC地址
            byte senderMacAddr[8] = {0};
            if (enabledAck && FindMacAddrByIpAddr(IPAddr_H, IPAddr_L, senderMacAddr))
            {
              responseDelay=random(0,150);
              delay(responseDelay);
              SendAckToPreamblePacket(senderMacAddr);
            } else
            {
              Serial.println(F("Cant findMAC address"));
            }
          } 
        }
        else if(packetType == 0x06)
        {
          /*
           * this is a data packet
           * adding the data packet into WaitingPacketsQueue, and then send a preamble packet
           */
           Serial.println(F("this is a packetType 0x06"));
           Packet packetTemp={0};
           /*
            * 读取packet的内容
           */
           //读取OverheadTransmissions
           packetTemp.OverheadTransmissions=SerialInput();
           chkSum+=packetTemp.OverheadTransmissions;
           //读取Hops；
           packetTemp.Hops=SerialInput();
           chkSum+=packetTemp.Hops;
           packetTemp.Hops++;
           //读取WaitingTimes
           packetTemp.WaitingTimes=SerialInput();
           chkSum+=packetTemp.WaitingTimes;
           //读取SourceIPAddress
           packetTemp.SourceIPAddress[0]=SerialInput();
           chkSum+=packetTemp.SourceIPAddress[0];
           packetTemp.SourceIPAddress[1]=SerialInput();
           chkSum+=packetTemp.SourceIPAddress[1];
           //读取DestinationIPAddress;
           packetTemp.DestinationIPAddress[0]=SerialInput();
           chkSum+=packetTemp.DestinationIPAddress[0];
           packetTemp.DestinationIPAddress[1]=SerialInput();
           chkSum+=packetTemp.DestinationIPAddress[1];
           //读取校验和
           unsigned int checkSum = SerialInput();
           if (checkSum != (0xFF - (chkSum & 0xFF)))
           {
            Serial.print(F("\n Error 0x06\n\r"));
           }else{
             /*
              * 成功接收的数据包数量加1；
             */
             NumberOfDeliveryPacket++;
             TotalOverheadTransmissions+=packetTemp.OverheadTransmissions;
             TotalWaitingTimes+=packetTemp.WaitingTimes;
             Serial.print(F("Source IP Address:"));
             Serial.print(packetTemp.SourceIPAddress[0],HEX);
             Serial.print(F("->"));
             Serial.println(packetTemp.SourceIPAddress[1],HEX);
             Serial.print(F("NumberofDeliveryPacket: "));
             Serial.println(NumberOfDeliveryPacket,DEC);
             Serial.print(F("TotalWaitingTimes: "));
             Serial.println(TotalWaitingTimes,DEC);
             Serial.print(F("TotalOverheadTransmissions: "));
             Serial.println(TotalOverheadTransmissions,DEC);
             delay(10);
           }
        }
      } else if (frameType == 0x89)
      {
        //读取帧的ID
        frameID = SerialInput();
        //读取投递状态
        byte dStatus = SerialInput();
        Serial.print(F("DeliveryStatus="));
        Serial.println(dStatus);
        //读取校验和
        unsigned int checkSum = SerialInput();
      }
    }
  }
}
