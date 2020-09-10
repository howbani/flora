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
  byte ReduntantTransmissions;
  byte Hops;
  byte WaitingTimes;
  byte SourceIPAddress[2];
  byte DestinationIPAddress[2];
} Packet;
/* 用于存储哪些节点返回了ACK信息 */
typedef struct MatchFlow
{
  byte IPAddr[2];
} MatchFlow;
/* 节点信息 */
typedef struct NodeInfo
{
  byte IPAddr[2];
  byte MACAddr_H[4];
  byte MACAddr_L[4];
  struct Position pos;
  /*初始能量和当前能量*/
  byte InitialEnergy[4];
  byte CurrentEnergy[4];
} NodeInfo;
/* 邻居表中节点信息 */
typedef struct NeighborsTableEntry
{
  NodeInfo NeiNode;
  //IsCandidate=1 -> 是候选节点，在routing zone内
  byte IsCandidate;
  //Action=1 -> 可以转发数据包
  byte action;
  byte competencyValue[4];
} NeighborsTableEntry;

/*
 * 时间戳定义
*/
unsigned long GlobalTimer;
unsigned long neighborsDiscoveryTimer;
unsigned long queryInfosTimer;
unsigned long sendDataPacketTimer;
unsigned long updateHopsTimer;
unsigned long queueTimer;
unsigned long activeTimer;
unsigned long sleepTimer;
SimpleTimer simpleTimer;
//是否可以发送数据包 true -> 可以发送数据包
boolean sendPreamblePacketFlag = false;
boolean queueTimerFlag = false;
boolean active=false;
boolean sleep=false;
/*
 * 公共变量定义
*/
byte HopsToSink = 0x01;
boolean isSink = false;
float CommunicationRangeRadius = 5.0;
int pin=A0;
byte initialEnergy[4]={0x40,0xA0,0x00,0x00};
byte currentEnergy[4]={0x40,0xA0,0x00,0x00};
unsigned long TotalGeneratedPackets=0;
unsigned long TotalOverheadTransmission=0;
byte broadcastAddr_64[8] = {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF};
/*
 * weight[HopsToSink][0]->CloseTD; weight[1]->MediumTD; weight[2]->FarTD
 * weight[3]->ExtraSmallDA; weight[4]->SmallDA; weight[5]->MediumDA; 
 * weight[6]->LargeDA; weight[7]->SuperLargeDA
 * weight[8]->LowRE; weight[9]->MediumRE; weight[10]->HighRE
*/
float weight[6][11] = {
{0.0526316,0.0526315,0.0526315,0.4736842,0.0526315,0.0526315,0.0526315,0.0526315,0.0526315,0.0526315,0.0526315},
{0.0336879,0.0336879,0.0336879,0.2247140,0.2247140,0.1001468,0.0513532,0.0297226,0.0507236,0.0861941,0.1313675},
{0.0310445,0.0310445,0.0310445,0.2319725,0.2319725,0.1124605,0.0597081,0.0320294,0.0426739,0.0761439,0.1199054},
{0.0672069,0.0672069,0.0672069,0.1642781,0.1642781,0.1031509,0.0926260,0.0600762,0.0672069,0.0672069,0.0795561},
{0.0331122,0.0654553,0.1157354,0.1900792,0.1900792,0.1050205,0.0286421,0.0286421,0.0511338,0.0763644,0.1157354},
{0.0503865,0.0930536,0.1181216,0.2253365,0.2253365,0.1105302,0.0510559,0.0296466,0.0321774,0.0321774,0.0321774}
};
//锁机制
int lock = 0;
struct Position MyPos = {{0x00,0x00,0x00,0x00},{0x40,0xC0,0x00,0x00}};
byte myIPAddress[2] = {0x00,0x87};
byte myMACAddress[8]={0x00,0x13,0xA2,0x00,0x41,0x91,0xC9,0x68};
// 43 4F 00 00 -> 43 8C 85 1F
struct Position SinkPos = {{0x00,0x00,0x00,0x00},{0x40,0x00,0x00,0x00}};
byte sinkIPAddress[2] = {0x99, 0x99};

List<NeighborsTableEntry> NeiTable;
List<MatchFlow> flowEntry;
QList<Packet> WaitingPacketsQueue;
byte FID = 1;
boolean IsSendPacket=false;
boolean StartDutyCycle=false;
long randomDelay;
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
/* 获取当前电压->设置当前电压 */
float getEnergy()
{
  /*获取当前电压*/
  float sum=0.0;
  int sensorValue;
  float voltage;
  for(unsigned int i=0;i<500;i++){
    sensorValue=analogRead(pin);
    voltage=sensorValue*(5.0/1023.0);
    sum+=voltage;
  }
  float energy=(float)(floor((sum/500.0)*100))/100.0;
  return energy;
}
/* 判断邻居节点是否在RoutingZone内 */
//计算RoutingZone的四个角的坐标
void PredefinedRoutingZone(Position sender, Position sinkNode, float* zone)
{
  /*
   * c1(zone[0],zone[1]) -> c2(zone[2],zone[3])
   * c3(zone[4],zone[5]) -> c4(zone[6],zone[7])
  */
  float Ws = 2 * CommunicationRangeRadius;
  float Xb = HexToFloat(sinkNode.x, 4);
  float Yb = HexToFloat(sinkNode.y, 4);
  float Xs = HexToFloat(sender.x, 4);
  float Ys = HexToFloat(sender.y, 4);
  float D_sb = DistanceBetweenSensors(sender, sinkNode);
  float x, y;
  x = Xb - Ws * (Yb - Ys) / (2 * D_sb);
  y = Yb - Ws * (Xs - Xb) / (2 * D_sb);
  zone[0] = x;
  zone[1] = y;

  x = Xb + Ws * (Yb - Ys) / (2 * D_sb);
  y = Yb + Ws * (Xs - Xb) / (2 * D_sb);
  zone[2] = x;
  zone[3] = y;

  x = Xs - Ws * (Yb - Ys) / (2 * D_sb);
  y = Ys - Ws * (Xs - Xb) / (2 * D_sb);
  zone[4] = x;
  zone[5] = y;

  x = Xs + Ws * (Yb - Ys) / (2 * D_sb);
  y = Ys + Ws * (Xs - Xb) / (2 * D_sb);
  zone[6] = x;
  zone[7] = y;
}
boolean PointTestInTriangle(float* FourCorners, NodeInfo neighbor)
{
  float pX = HexToFloat(neighbor.pos.x, 4);
  float pY = HexToFloat(neighbor.pos.y, 4);
  //AP X AB=(c1.x-p.x)*(c1.y-c2.y)-(c1.x-c2.x)*(c1.y-p.y)
  float APAB = (FourCorners[0] - pX) * (FourCorners[1] - FourCorners[3]) - (FourCorners[0] - FourCorners[2]) * (FourCorners[1] - pY);
  //BP X BC=(c2.x-p.x)*(c2.y-c3.y)-(c2.x-c3.x)*(c2.y-p.y)
  float BPBC = (FourCorners[2] - pX) * (FourCorners[3] - FourCorners[5]) - (FourCorners[2] - FourCorners[4]) * (FourCorners[3] - pY);
  //CP X CA=(c3.x-p.x)*(c3.y-c1.y)-(c3.x-c1.x)*(c3.y-p.y)
  float CPCA = (FourCorners[4] - pX) * (FourCorners[5] - FourCorners[1]) - (FourCorners[4] - FourCorners[0]) * (FourCorners[5] - pY);
  if (APAB >= 0 && BPBC >= 0 && CPCA >= 0)
  {
    return true;
  } else if (APAB < 0 && BPBC < 0 && CPCA < 0)
  {
    return true;
  } else
  {
    return false;
  }
}
boolean IsNeighborWithinRoutingZone(NodeInfo neighbor, float* FourCorners)
{
  /*
     if neighbor==sink -> WithinRoutingZone of sender
     else 需要判断neighbor在不在RoutingZone内
  */
  if (neighbor.IPAddr[0] == sinkIPAddress[0] && neighbor.IPAddr[1] == sinkIPAddress[1])
  {
    return true;
  } else
  {
    float CornersABC[6] = {0};
    float CornersBCD[6] = {0};
    for (int i = 0; i < 8; i++)
    {
      if (i >= 0 && i <= 5)
      {
        CornersABC[i] = FourCorners[i];
      }
      if (i >= 2 && i <= 7)
      {
        CornersBCD[i - 2] = FourCorners[i];
      }
    }
    if (PointTestInTriangle(CornersABC, neighbor))
    {
      return true;
    } else if (PointTestInTriangle(CornersBCD, neighbor))
    {
      return true;
    } else
    {
      return false;
    }
  }
}

/* 按照**排序邻居表NeiTable */
void Swap(NeighborsTableEntry* x, NeighborsTableEntry* y)
{
  NeighborsTableEntry tmp = {0};
  /*
     按照排序要求进行修改
  */
  if (HexToFloat(x->competencyValue,4) < HexToFloat(y->competencyValue,4))
  {
    tmp = *x;
    *x = *y;
    *y = tmp;
  }
}
void SortNeiTable()
{
  int neiLen = NeiTable.Count();
  //sssint neiLen=NeiTable.size();
  for (int i = 0; i < neiLen - 1; i++)
  {
    for (int j = neiLen - 1; j > i; j--)
    {
      Swap(&NeiTable[i], &NeiTable[j]);
    }
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
/* Fuzzy Model */
//计算TransmissionDistance
float TransmissionDistance(Position sender, Position neighbor)
{
  float distance = DistanceBetweenSensors(sender, neighbor);
  return distance;
}
/*
 * 计算DirectionAngle
 * s->sender;n->neighbor;d->sink
*/
float DirectionAngle(Position sender, Position neighbor, Position sinkNode)
{
  float s_x = HexToFloat(sender.x, 4);
  float s_y = HexToFloat(sender.y, 4);
  float n_x = HexToFloat(neighbor.x, 4);
  float n_y = HexToFloat(neighbor.y, 4);
  float d_x = HexToFloat(sinkNode.x, 4);
  float d_y = HexToFloat(sinkNode.y, 4);
  float molecule = (n_x - s_x) * (d_x - s_x) + (n_y - s_y) * (d_y - s_y);
  float denominator = DistanceBetweenSensors(sender, sinkNode) * DistanceBetweenSensors(sender, neighbor);
  float theta = 0.0;
  if (denominator == 0)
  {
    theta = 1.0;
  } else
  {
    theta = molecule / denominator;
    if (theta > 1.0)
    {
      theta = 1.0;
    }
  }
  float angle = acos(theta) / M_PI * 180;
  return angle;
}
/*
 * TransmissionDistanceFuzzySet
*/
float TDClose(float transCrisp)
{
  float tdClose=0.0;
  if(transCrisp>=0.0 && transCrisp<0.29)
  {
    tdClose=0.6+0.34483*transCrisp;
  }else if(transCrisp>=0.29 && transCrisp<0.5)
  {
    tdClose=1.66667-3.33333*transCrisp;
  }else
  {
    tdClose=0.0;
  }
  return tdClose;
}
float TDMedium(float transCrisp)
{
  float tdMedium=0.0;
  if(transCrisp>=0.1 && transCrisp<0.29)
  {
    tdMedium=-0.01053+2.10526*transCrisp;
  }else if(transCrisp>=0.29 && transCrisp<0.67)
  {
    tdMedium=0.52368+0.26316*transCrisp;
  }else if(transCrisp>=0.67 && transCrisp<0.8)
  {
    tdMedium=3.27692-3.84615*transCrisp;
  }else
  {
    tdMedium=0.2;
  }
  return tdMedium;
}
float TDFar(float transCrisp)
{
  float tdFar=0.0;
  if(transCrisp>=0.2 && transCrisp<0.67)
  {
    tdFar=-0.25532+1.2766*transCrisp;
  }else if(transCrisp>=0.67 && transCrisp<=1.0)
  {
    tdFar=0.39697+0.30303*transCrisp;
  }else
  {
    tdFar=0.0;
  }
  return tdFar;
}
/*
 * DirectionAngleFuzzySet
*/
float DAExtraSmall(float angCrisp)
{
  float daExtraSmall=0.0;
  if(angCrisp>=0.0 && angCrisp<0.0555)
  {
    daExtraSmall=1.0-3.6036*angCrisp;
  }else if(angCrisp>=0.0555 && angCrisp<0.5)
  {
    daExtraSmall=0.89989-1.79978*angCrisp;
  }else
  {
    daExtraSmall=0.0;
  }
  return daExtraSmall;
}
float DASmall(float angCrisp)
{
  float daSmall=0.0;
  if(angCrisp>=0.0 && angCrisp<0.0111)
  {
    daSmall=0.6+9.00901*angCrisp;
  }else if(angCrisp>=0.0111 && angCrisp<0.25)
  {
    daSmall=0.70232-0.20929*angCrisp;
  }else if(angCrisp>=0.25 && angCrisp<0.6)
  {
    daSmall=1.07857-1.71429*angCrisp;
  }else
  {
    daSmall=0.05;
  }
  return daSmall;
}
float DAMedium(float angCrisp)
{
  float daMedium=0.0;
  if(angCrisp>=0.0 && angCrisp<0.25)
  {
    daMedium=0.5+1.2*angCrisp;
  }else if(angCrisp>=0.25 && angCrisp<0.5)
  {
    daMedium=0.95-0.6*angCrisp;
  }else if(angCrisp>=0.5 && angCrisp<0.8)
  {
    daMedium=1.56667-1.83333*angCrisp;
  }else
  {
    daMedium=0.1;
  }
  return daMedium;
}
float DALarge(float angCrisp)
{
  float daLarge=0.0;
  if(angCrisp>=0.0 && angCrisp<0.5)
  {
    daLarge=0.3+1.0*angCrisp;
  }else if(angCrisp>=0.5 && angCrisp<0.75)
  {
    daLarge=1.1-0.6*angCrisp;
  }else if(angCrisp>=0.75 && angCrisp<0.85)
  {
    daLarge=4.4-5.0*angCrisp;
  }else
  {
    daLarge=0.15;
  }
  return daLarge;
}
float DASuperLarge(float angCrisp)
{
  float daSuperLarge=0.0;
  if(angCrisp>=0.3 && angCrisp<0.75)
  {
    daSuperLarge=-0.53333+1.77778*angCrisp;
  }else if(angCrisp>=0.75 && angCrisp<=1.0)
  {
    daSuperLarge=1.1-0.4*angCrisp;
  }else
  {
    daSuperLarge=0.0;
  }
  return daSuperLarge;
}
/*
 * ResidualEnergyFuzzySet
*/
float RELow(float enerCrisp)
{
  float reLow=0.0;
  if(enerCrisp>=0.0 && enerCrisp<0.3)
  {
    reLow=0.75+0.5*enerCrisp;
  }else if(enerCrisp>=0.3 && enerCrisp<0.7)
  {
    reLow=1.575-2.25*enerCrisp;
  }else
  {
    reLow=0.0;
  }
  return reLow;
}
float REMedium(float enerCrisp)
{
  float reMedium=0.0;
  if(enerCrisp>=0.1 && enerCrisp<0.3)
  {
    reMedium=-0.075+2.75*enerCrisp;
  }else if(enerCrisp>=0.3 && enerCrisp<0.7)
  {
    reMedium=0.725+0.25*enerCrisp;
  }else if(enerCrisp>=0.7 && enerCrisp<0.9)
  {
    reMedium=3.35-3.5*enerCrisp;
  }else
  {
    reMedium=0.2;
  }
  return reMedium;
}
float REHigh(float enerCrisp)
{
  float reHigh=0.0;
  if(enerCrisp>=0.5 && enerCrisp<0.7)
  {
    reHigh=-1.875+3.75*enerCrisp;
  }else if(enerCrisp>=0.7 && enerCrisp<=1.0)
  {
    reHigh=0.4+0.5*enerCrisp;
  }else
  {
    reHigh=0.0;
  }
  return reHigh;
}

//计算competency value
float CompetencyValue(NodeInfo NeiNode)
{
  int hops;
  if(HopsToSink<7)
  {
    hops=(int)HopsToSink;
  }else
  {
    hops=6;
  }
  float TDCrisp=TransmissionDistance(MyPos,NeiNode.pos)/CommunicationRangeRadius;
  float DACrisp=DirectionAngle(MyPos,NeiNode.pos,SinkPos)/180.0;
  float RECrisp=HexToFloat(NeiNode.CurrentEnergy,4)/HexToFloat(NeiNode.InitialEnergy,4);
  if(RECrisp>1.0){RECrisp=1.0;}
  /*
   * TD->Close
  */
  //TD->Close && DA->ExtraSmall && RE->Low
  if((TDCrisp>=0.0 && TDCrisp<=0.29) && (DACrisp>=0.0 && DACrisp<(1.6/180.0)) && (RECrisp>=0.0 && RECrisp<=0.3))
  {
    return weight[hops-1][0]*TDClose(TDCrisp)+weight[hops-1][3]*DAExtraSmall(DACrisp)+weight[hops-1][8]*RELow(RECrisp);
  }
  //TD->Close && DA->ExtraSmall && RE->Medium
  else if((TDCrisp>=0.0 && TDCrisp<=0.29) && (DACrisp>=0.0 && DACrisp<(1.6/180.0)) &&(RECrisp>0.3 && RECrisp<=0.7))
  {
    return weight[hops-1][0]*TDClose(TDCrisp)+weight[hops-1][3]*DAExtraSmall(DACrisp)+weight[hops-1][9]*REMedium(RECrisp);
  }
  //TD->Close && DA->ExtraSmall && RE->High
  else if((TDCrisp>=0.0 && TDCrisp<=0.29) && (DACrisp>=0.0 && DACrisp<(1.6/180.0)) && (RECrisp>0.7 && RECrisp<=1.0))
  {
    return weight[hops-1][0]*TDClose(TDCrisp)+weight[hops-1][3]*DAExtraSmall(DACrisp)+weight[hops-1][10]*REHigh(RECrisp);
  }

  
  //TD->Close && DA->Small && RE->Low
  else if((TDCrisp>=0.0 && TDCrisp<=0.29) && (DACrisp>=(1.6/180.0) && DACrisp<(60.0/180.0)) && (RECrisp>=0.0 && RECrisp<=0.3))
  {
    return weight[hops-1][0]*TDClose(TDCrisp)+weight[hops-1][4]*DASmall(DACrisp)+weight[hops-1][8]*RELow(RECrisp);
  }
  //TD->Close && DA->Small && RE->Medium
  else if((TDCrisp>=0.0 && TDCrisp<=0.29) && (DACrisp>=(1.6/180.0) && DACrisp<(60.0/180.0)) && (RECrisp>0.3 && RECrisp<=0.7))
  {
    return weight[hops-1][0]*TDClose(TDCrisp)+weight[hops-1][4]*DASmall(DACrisp)+weight[hops-1][9]*REMedium(RECrisp);
  }
  //TD->Close && DA->Small && RE->High
  else if((TDCrisp>=0.0 && TDCrisp<=0.29) && (DACrisp>=(1.6/180.0) && DACrisp<(60.0/180.0)) && (RECrisp>0.7 && RECrisp<=1.0))
  {
    return weight[hops-1][0]*TDClose(TDCrisp)+weight[hops-1][4]*DASmall(DACrisp)+weight[hops-1][10]*REHigh(RECrisp);
  }


  //TD->Close && DA->Medium && RELow
  else if((TDCrisp>=0.0 && TDCrisp<=0.29) && (DACrisp>=(60.0/180.0) && DACrisp<(86.0/180.0)) && (RECrisp>=0.0 && RECrisp<=0.3))
  {
    return weight[hops-1][0]*TDClose(TDCrisp)+weight[hops-1][5]*DAMedium(DACrisp)+weight[hops-1][8]*RELow(RECrisp);
  }
  //TD->Close && DA->Medium && REMedium
  else if((TDCrisp>=0.0 && TDCrisp<=0.29) && (DACrisp>=(60.0/180.0) && DACrisp<(86.0/180.0)) && (RECrisp>0.3 && RECrisp<=0.7))
  {
    return weight[hops-1][0]*TDClose(TDCrisp)+weight[hops-1][5]*DAMedium(DACrisp)+weight[hops-1][9]*REMedium(RECrisp);
  }
  //TD->Close && DA->Medium && REHigh
  else if((TDCrisp>=0.0 && TDCrisp<=0.29) && (DACrisp>=(60.0/180.0) && DACrisp<(86.0/180.0)) && (RECrisp>0.7 && RECrisp<=1.0))
  {
    return weight[hops-1][0]*TDClose(TDCrisp)+weight[hops-1][5]*DAMedium(DACrisp)+weight[hops-1][10]*REHigh(RECrisp);
  }
  
  //TD->Close && DA->Large && RE->Low
  else if((TDCrisp>=0.0 && TDCrisp<=0.29) && (DACrisp>=(86.0/180.0) && DACrisp<(135.0/180.0)) && (RECrisp>=0.0 && RECrisp<=0.3))
  {
    return weight[hops-1][0]*TDClose(TDCrisp)+weight[hops-1][6]*DALarge(DACrisp)+weight[hops-1][8]*RELow(RECrisp);
  }
  //TD->Close && DA->Large && RE->Medium
  else if((TDCrisp>=0.0 && TDCrisp<=0.29) && (DACrisp>=(86.0/180.0) && DACrisp<(135.0/180.0)) && (RECrisp>0.3 && RECrisp<=0.7))
  {
    return weight[hops-1][0]*TDClose(TDCrisp)+weight[hops-1][6]*DALarge(DACrisp)+weight[hops-1][9]*REMedium(RECrisp);
  }
  //TD->Close && DA->Large && RE->High
  else if((TDCrisp>=0.0 && TDCrisp<=0.29) && (DACrisp>=(86.0/180.0) && DACrisp<(135.0/180.0)) && (RECrisp>0.7 && RECrisp<=1.0))
  {
    return weight[hops-1][0]*TDClose(TDCrisp)+weight[hops-1][6]*DALarge(DACrisp)+weight[hops-1][10]*REHigh(RECrisp);
  }
  
  //TD->Close && DA->SuperLarge && RE->Low
  else if((TDCrisp>=0.0 && TDCrisp<=0.29) && (DACrisp>=(135.0/180.0) && DACrisp<=1.0) && (RECrisp>=0.0 && RECrisp<=0.3))
  {
    return weight[hops-1][0]*TDClose(TDCrisp)+weight[hops-1][7]*DASuperLarge(DACrisp)+weight[hops-1][8]*RELow(RECrisp);
  }
  //TD->Close && DA->SuperLarge && RE->Medium
  else if((TDCrisp>=0.0 && TDCrisp<=0.29) && (DACrisp>=(135.0/180.0) && DACrisp<=1.0) && (RECrisp>0.3 && RECrisp<=0.7))
  {
    return weight[hops-1][0]*TDClose(TDCrisp)+weight[hops-1][7]*DASuperLarge(DACrisp)+weight[hops-1][9]*REMedium(RECrisp);
  }
  //TD->Close && DA->SuperLarge && RE->High
  else if((TDCrisp>=0.0 && TDCrisp<=0.29) && (DACrisp>=(135.0/180.0) && DACrisp<=1.0) && (RECrisp>0.7 && RECrisp<=1.0))
  {
    return weight[hops-1][0]*TDClose(TDCrisp)+weight[hops-1][7]*DASuperLarge(DACrisp)+weight[hops-1][10]*REHigh(RECrisp);
  }

  
  /*
   * TD->Medium
  */
  //TD->Medium && DA->ExtraSmall && RE->Low
  else if((TDCrisp>0.29 && TDCrisp<=0.67) && (DACrisp>=0.0 && DACrisp<(1.6/180.0)) && (RECrisp>=0.0 && RECrisp<=0.3))
  {
    return weight[hops-1][1]*TDMedium(TDCrisp)+weight[hops-1][3]*DAExtraSmall(DACrisp)+weight[hops-1][8]*RELow(RECrisp);
  }
  //TD->Medium && DA->ExtraSmall && RE->Medium
  else if((TDCrisp>0.29 && TDCrisp<=0.67) && (DACrisp>=0.0 && DACrisp<(1.6/180.0)) && (RECrisp>0.3 && RECrisp<=0.7))
  {
    return weight[hops-1][1]*TDMedium(TDCrisp)+weight[hops-1][3]*DAExtraSmall(DACrisp)+weight[hops-1][9]*REMedium(RECrisp);
  }
  //TD->Medium && DA->ExtraSmall && RE->High
  else if((TDCrisp>0.29 && TDCrisp<=0.67) && (DACrisp>=0.0 && DACrisp<(1.6/180.0)) && (RECrisp>0.7 && RECrisp<=1.0))
  {
    return weight[hops-1][1]*TDMedium(TDCrisp)+weight[hops-1][3]*DAExtraSmall(DACrisp)+weight[hops-1][10]*REHigh(RECrisp);
  }
  
  //TD->Medium && DA->Small && RE->Low
  else if((TDCrisp>0.29 && TDCrisp<=0.67) && (DACrisp>=(1.6/180.0) && DACrisp<(60.0/180.0)) && (RECrisp>=0.0 && RECrisp<=0.3))
  {
    return weight[hops-1][1]*TDMedium(TDCrisp)+weight[hops-1][4]*DASmall(DACrisp)+weight[hops-1][8]*RELow(RECrisp);
  }
  //TD->Medium && DA->Small && RE->Medium
  else if((TDCrisp>0.29 && TDCrisp<=0.67) && (DACrisp>=(1.6/180.0) && DACrisp<(60.0/180.0)) && (RECrisp>0.3 && RECrisp<=0.7))
  {
    return weight[hops-1][1]*TDMedium(TDCrisp)+weight[hops-1][4]*DASmall(DACrisp)+weight[hops-1][9]*REMedium(RECrisp);
  }
  //TD->Medium && DA->Small && RE->High
  else if((TDCrisp>0.29 && TDCrisp<=0.67) && (DACrisp>=(1.6/180.0) && DACrisp<(60.0/180.0)) && (RECrisp>0.7 && RECrisp<=1.0))
  {
    return weight[hops-1][1]*TDMedium(TDCrisp)+weight[hops-1][4]*DASmall(DACrisp)+weight[hops-1][10]*REHigh(RECrisp);
  }

  
  //TD->Medium && DA->Medium && RE->Low
  else if((TDCrisp>0.29 && TDCrisp<=0.67) && (DACrisp>=(60.0/180.0) && DACrisp<(86.0/180.0)) && (RECrisp>=0.0 && RECrisp<=0.3))
  {
    return weight[hops-1][1]*TDMedium(TDCrisp)+weight[hops-1][5]*DAMedium(DACrisp)+weight[hops-1][8]*RELow(RECrisp);
  }
  //TD->Medium && DA->Medium && RE->Medium
  else if((TDCrisp>0.29 && TDCrisp<=0.67) && (DACrisp>=(60.0/180.0) && DACrisp<(86.0/180.0)) && (RECrisp>0.3 && RECrisp<=0.7))
  {
    return weight[hops-1][1]*TDMedium(TDCrisp)+weight[hops-1][5]*DAMedium(DACrisp)+weight[hops-1][9]*REMedium(RECrisp);
  }
  //TD->Medium && DA->Medium && RE->High
  else if((TDCrisp>0.29 && TDCrisp<=0.67) && (DACrisp>=(60.0/180.0) && DACrisp<(86.0/180.0)) && (RECrisp>0.7 && RECrisp<=1.0))
  {
    return weight[hops-1][1]*TDMedium(TDCrisp)+weight[hops-1][5]*DAMedium(DACrisp)+weight[hops-1][10]*REHigh(RECrisp);
  }
  
  //TD->Medium && DA->Large && RE->Low
  else if((TDCrisp>0.29 && TDCrisp<=0.67) && (DACrisp>=(86.0/180.0) && DACrisp<(135.0/180.0)) && (RECrisp>=0.0 && RECrisp<=0.3))
  {
    return weight[hops-1][1]*TDMedium(TDCrisp)+weight[hops-1][6]*DALarge(DACrisp)+weight[hops-1][8]*RELow(RECrisp);
  }
  //TD->Medium && DA->Large && RE->Medium
  else if((TDCrisp>0.29 && TDCrisp<=0.67) && (DACrisp>=(86.0/180.0) && DACrisp<(135.0/180.0)) && (RECrisp>0.3 && RECrisp<=0.7))
  {
    return weight[hops-1][1]*TDMedium(TDCrisp)+weight[hops-1][6]*DALarge(DACrisp)+weight[hops-1][9]*REMedium(RECrisp);
  }
  //TD->Medium && DA->Large && RE->High
  else if((TDCrisp>0.29 && TDCrisp<=0.67) && (DACrisp>=(86.0/180.0) && DACrisp<(135.0/180.0)) && (RECrisp>0.7 && RECrisp<=1.0))
  {
    return weight[hops-1][1]*TDMedium(TDCrisp)+weight[hops-1][6]*DALarge(DACrisp)+weight[hops-1][10]*REHigh(RECrisp);
  }

  //TD->Medium && DA->SuperLarge && RE->Low
  else if((TDCrisp>0.29 && TDCrisp<=0.67) && (DACrisp>=(135.0/180.0) && DACrisp<=1.0) && (RECrisp>=0.0 && RECrisp<=0.3))
  {
    return weight[hops-1][1]*TDMedium(TDCrisp)+weight[hops-1][7]*DASuperLarge(DACrisp)+weight[hops-1][8]*RELow(RECrisp);
  }
  //TD->Medium && DA->SuperLarge && RE->Medium
  else if((TDCrisp>0.29 && TDCrisp<=0.67) && (DACrisp>=(135.0/180.0) && DACrisp<=1.0) && (RECrisp>0.3 && RECrisp<=0.7))
  {
    return weight[hops-1][1]*TDMedium(TDCrisp)+weight[hops-1][7]*DASuperLarge(DACrisp)+weight[hops-1][9]*REMedium(RECrisp);
  }
  //TD->Medium && DA->SuperLarge && RE->High
  else if((TDCrisp>0.29 && TDCrisp<=0.67) && (DACrisp>=(135.0/180.0) && DACrisp<=1.0) && (RECrisp>0.7 && RECrisp<=1.0))
  {
    return weight[hops-1][1]*TDMedium(TDCrisp)+weight[hops-1][7]*DASuperLarge(DACrisp)+weight[hops-1][10]*REHigh(RECrisp);
  }
  
  /*
   * TD->Far
  */
  //TD->Far && DA->ExtraSmall && RE->Low
  else if((TDCrisp>0.67 && TDCrisp<=1.0) && (DACrisp>=0.0 && DACrisp<(1.6/180.0)) && (RECrisp>=0.0 && RECrisp<=0.3))
  {
    return weight[hops-1][2]*TDFar(TDCrisp)+weight[hops-1][3]*DAExtraSmall(DACrisp)+weight[hops-1][8]*RELow(RECrisp);
  }
  //TD->Far && DA->ExtraSmall && RE->Medium
  else if((TDCrisp>0.67 && TDCrisp<=1.0) && (DACrisp>=0.0 && DACrisp<(1.6/180.0)) && (RECrisp>0.3 && RECrisp<=0.7))
  {
    return weight[hops-1][2]*TDFar(TDCrisp)+weight[hops-1][3]*DAExtraSmall(DACrisp)+weight[hops-1][9]*REMedium(RECrisp);
  }
  //TD->Far && DA->ExtraSmall && RE->High
  else if((TDCrisp>0.67 && TDCrisp<=1.0) && (DACrisp>=0.0 && DACrisp<(1.6/180.0)) && (RECrisp>0.7 && RECrisp<=1.0))
  {
    float temp=weight[hops-1][2]*TDFar(TDCrisp)+weight[hops-1][3]*DAExtraSmall(DACrisp)+weight[hops-1][10]*REHigh(RECrisp);
    return weight[hops-1][2]*TDFar(TDCrisp)+weight[hops-1][3]*DAExtraSmall(DACrisp)+weight[hops-1][10]*REHigh(RECrisp);
  }

   
  //TD->Far && DA->Small && RE->Low
  else if((TDCrisp>0.67 && TDCrisp<=1.0) && (DACrisp>=(1.6/180.0) && DACrisp<(60.0/180.0)) && (RECrisp>=0.0 && RECrisp<=0.3))
  {
    return weight[hops-1][2]*TDFar(TDCrisp)+weight[hops-1][4]*DASmall(DACrisp)+weight[hops-1][8]*RELow(RECrisp);
  }
  //TD->Far && DA->Small && RE->Medium
  else if((TDCrisp>0.67 && TDCrisp<=1.0) && (DACrisp>=(1.6/180.0) && DACrisp<(60.0/180.0)) && (RECrisp>0.3 && RECrisp<=0.7))
  {
    return weight[hops-1][2]*TDFar(TDCrisp)+weight[hops-1][4]*DASmall(DACrisp)+weight[hops-1][9]*REMedium(RECrisp);
  }
  //TD->Far && DA->Small && RE->High
  else if((TDCrisp>0.67 && TDCrisp<=1.0) && (DACrisp>=(1.6/180.0) && DACrisp<(60.0/180.0)) && (RECrisp>0.7 && RECrisp<=1.0))
  {
    float temp=weight[hops-1][2]*TDFar(TDCrisp)+weight[hops-1][4]*DASmall(DACrisp)+weight[hops-1][10]*REHigh(RECrisp);
    return weight[hops-1][2]*TDFar(TDCrisp)+weight[hops-1][4]*DASmall(DACrisp)+weight[hops-1][10]*REHigh(RECrisp);
  }

  
  //TD->Far && DA->Medium && RE->Low
  else if((TDCrisp>0.67 && TDCrisp<=1.0) && (DACrisp>=(60.0/180.0) && DACrisp<(86.0/180.0)) && (RECrisp>=0.0 && RECrisp<=0.3))
  {
    return weight[hops-1][2]*TDFar(TDCrisp)+weight[hops-1][5]*DAMedium(DACrisp)+weight[hops-1][8]*RELow(RECrisp);
  }
  //TD->Far && DA->Medium && RE->Medium
  else if((TDCrisp>0.67 && TDCrisp<=1.0) && (DACrisp>=(60.0/180.0) && DACrisp<(86.0/180.0)) && (RECrisp>0.3 && RECrisp<=0.7))
  {
    return weight[hops-1][2]*TDFar(TDCrisp)+weight[hops-1][5]*DAMedium(DACrisp)+weight[hops-1][9]*REMedium(RECrisp);
  }
  //TD->Far && DA->Medium && RE->High
  else if((TDCrisp>0.67 && TDCrisp<=1.0) && (DACrisp>=(60.0/180.0) && DACrisp<(86.0/180.0)) && (RECrisp>0.7 && RECrisp<=1.0))
  {
    return weight[hops-1][2]*TDFar(TDCrisp)+weight[hops-1][5]*DAMedium(DACrisp)+weight[hops-1][10]*REHigh(RECrisp);
  }
  
  
  //TD->Far && DA->Large && RE->Low
  else if((TDCrisp>0.67 && TDCrisp<=1.0) && (DACrisp>=(86.0/180.0) && DACrisp<(135.0/180.0)) && (RECrisp>=0.0 && RECrisp<=0.3))
  {
    return weight[hops-1][2]*TDFar(TDCrisp)+weight[hops-1][6]*DALarge(DACrisp)+weight[hops-1][8]*RELow(RECrisp);
  }
  //TD->Far && DA->Large && RE->Medium
  else if((TDCrisp>0.67 && TDCrisp<=1.0) && (DACrisp>=(86.0/180.0) && DACrisp<(135.0/180.0)) && (RECrisp>0.3 && RECrisp<=0.7))
  {
    return weight[hops-1][2]*TDFar(TDCrisp)+weight[hops-1][6]*DALarge(DACrisp)+weight[hops-1][9]*REMedium(RECrisp);
  }
  //TD->Far && DA->Large && RE->High
  else if((TDCrisp>0.67 && TDCrisp<=1.0) && (DACrisp>=(86.0/180.0) && DACrisp<(135.0/180.0)) && (RECrisp>0.7 && RECrisp<=1.0))
  {
    return weight[hops-1][2]*TDFar(TDCrisp)+weight[hops-1][6]*DALarge(DACrisp)+weight[hops-1][10]*REHigh(RECrisp);
  }
  
  //TD->Far && DA->SuperLarge && RE->Low
  else if((TDCrisp>0.67 && TDCrisp<=1.0) && (DACrisp>=(135.0/180.0) && DACrisp<=1.0) && (RECrisp>=0.0 && RECrisp<=0.3))
  {
    return weight[hops-1][2]*TDFar(TDCrisp)+weight[hops-1][7]*DASuperLarge(DACrisp)+weight[hops-1][8]*RELow(RECrisp);
  }
  //TD->Far && DA->SuperLarge && RE->Medium
  else if((TDCrisp>0.67 && TDCrisp<=1.0) && (DACrisp>=(135.0/180.0) && DACrisp<=1.0) && (RECrisp>0.3 && RECrisp<=0.7))
  {
    return weight[hops-1][2]*TDFar(TDCrisp)+weight[hops-1][7]*DASuperLarge(DACrisp)+weight[hops-1][9]*REMedium(RECrisp);
  }
  //TD->Far && DA->SuperLarge && RE->High
  else if((TDCrisp>0.67 && TDCrisp<=1.0) && (DACrisp>=(135.0/180.0) && DACrisp<=1.0) && (RECrisp>0.7 && RECrisp<=1.0))
  {
    return weight[hops-1][2]*TDFar(TDCrisp)+weight[hops-1][7]*DASuperLarge(DACrisp)+weight[hops-1][10]*REHigh(RECrisp);
  }
  return 0.0;
}

//通过sender的源地址寻找sender的MAC地址
boolean FindMacAddrByIpAddr(byte IPAddr_H, byte IPAddr_L, byte* buf)
{
  int neiLen = NeiTable.Count();
  //sssint neiLen=NeiTable.size();
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

/*
 * 发包
*/

/* 获取16位IP地址 */
void getIPAddress()
{
  byte packet[] = {0x7E,0x00,0x04,0x08,0x01,0x4D,0x59,0x50};
  byte packetLength = 8;
  Serial1.write(packet,packetLength);
  Serial.println();
}
/* 获取64位MAC地址 */
void getMAC_SH()
{
  byte packet[] = {0x7E,0x00,0x04,0x08,0x01,0x53,0x48,0x5B};
  byte packetLength = 8;
  Serial1.write(packet,packetLength);
  Serial.println();
}
void getMAC_SL()
{
  byte packet[] = {0x7E,0x00,0x04,0x08,0x01,0x53,0x4C,0x57};
  byte packetLength = 8;
  Serial1.write(packet,packetLength);
  Serial.println();
}
/*
 * 形成数据包
*/
void GenerateDataPacket()
{
  Packet packet={0};
  packet.ReduntantTransmissions=0;
  packet.Hops=0;
  packet.WaitingTimes=0;
  packet.SourceIPAddress[0]=myIPAddress[0];
  packet.SourceIPAddress[1]=myIPAddress[1];
  packet.DestinationIPAddress[0]=sinkIPAddress[0];
  packet.DestinationIPAddress[1]=sinkIPAddress[1];
  WaitingPacketsQueue.push_back(packet);
}
/* 邻居发现 -> 0x07 */
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
/* 响应邻居发现 -> 0x08 */
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
/* 更新HopsToSink信息,一开始由sink来发送 -> 0x01 */
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
   查询邻居节点的信息 -> 0x02
   ResidualEnergy + TransmissionDistance + DirectionAngle
*/
void QueryInfosForNeighbors()
{
  Serial.println(F("\n\r request Info for Neighbors"));
  byte packet[25] = {0x7E, 0x00, 0x00, 0x00};
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
  //0x02表示这是一个查询邻居信息的包
  packet[idx++] = 0x02;
  checkSum += 0x02;
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
  Serial.println(F("ResponseQuery"));
  delay(10);
  int neiLen = NeiTable.Count();
  //sssint neiLen=NeiTable.size();
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
  //设置节点的初始能量
  for(unsigned int j3=0;j3<4;j3++)
  {
    packet[idx++] = initialEnergy[j3];
    checkSum += initialEnergy[j3];
  }
  /*
   * 插入获取节点当前能量的代码
  */
  FloatToHEX(getEnergy(),currentEnergy);
  delay(10);
  //设置节点的当前能量
  for(unsigned int j4=0;j4<4;j4++)
  {
    packet[idx++] = currentEnergy[j4];
    checkSum += currentEnergy[j4];
  }
  //设置校验和
  packet[idx++] = 0xFF - (checkSum & 0xFF);
  //设置帧的长度
  packet[2] = 0xFF & (idx - 4);
  Serial1.write(packet, idx);
  FID++;
}
/*
   发送Preamble packet -> 0x04
   Length = 128bits
   帧格式：
*/
void SendPreamblePacket()
{
  IsSendPacket=true;
  activeTimer=millis();
  byte packet[64] = {0x7E, 0x00, 0x00, 0x00};
  //byte broadcastAddr_64[8] = {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF};
  unsigned int checkSum = 0;
  unsigned int idx = 4;
  //设置帧的ID
  packet[idx++] = FID;
  checkSum += FID;
  //设置下一跳的MAC地址
  for (int i = 0; i < 8; i++)
  {
    packet[idx++] = broadcastAddr_64[i];
    checkSum += broadcastAddr_64[i];
  }
  //设置Options
  packet[idx++] = 0;
  //0x04表示这是一个Preamble packet
  packet[idx++] = 0x04;
  checkSum += 0x04;
  /*
   * 设置the IP address of potential forwards
  */
  unsigned int index=idx;
  //设置候选节点的个数
  packet[idx++]=0;
  //设置候选节点的IP地址
  int neiLen=NeiTable.Count();
  //sssint neiLen=NeiTable.size();
  for(int j=0;j<neiLen;j++)
  {
    if(NeiTable[j].action==1)
    {
      packet[idx++]=NeiTable[j].NeiNode.IPAddr[0];
      checkSum+=NeiTable[j].NeiNode.IPAddr[0];
      packet[idx++]=NeiTable[j].NeiNode.IPAddr[1];
      checkSum+=NeiTable[j].NeiNode.IPAddr[1];
    }
  }
  //设置候选节点的个数
  packet[index]=idx-index-1;
  checkSum+=packet[index];
  //设置校验和
  packet[idx++] = 0xFF - (checkSum & 0xFF);
  //设置帧的长度
  packet[2] = 0xFF & (idx - 4);
  Serial1.write(packet, idx);
  Serial.println(F("Spreamble packet"));
  MsTimer2::set(2000, SendDataPacket);
  MsTimer2::start();
  FID++;
}
/*
 * 发送ACK_Preamble packet -> 0x05
 * Length = 128bits
 * 帧格式:
*/
void SendAckToPreamblePacket(byte* addr)
{
  activeTimer=millis();
  byte packet[64] = {0x7E, 0x00, 0x00, 0x00};
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
  Serial.println(F("\n\r SAck"));
  FID++;
}
//发送数据包
void SendDataPacket()
{
  MsTimer2::stop();
  int flowLen=flowEntry.Count();
  if (flowLen != 0)
  {
    while(WaitingPacketsQueue.size()!=0){
      activeTimer=millis();
      Packet dataPacket=WaitingPacketsQueue.front();
      WaitingPacketsQueue.pop_front();
      if(dataPacket.WaitingTimes<7){
        byte packet[64]={0x7E,0x00,0x00,0x00};
        unsigned int checkSum=0;
        unsigned int idx=4;
        int neiLen=NeiTable.Count();
        //修改冗余传输的次数
        dataPacket.ReduntantTransmissions+=flowLen-1;
        //设置帧的ID
        packet[idx++]=FID;
        checkSum+=FID;
        //设置下一跳的MAC地址
        byte* nextMACAddr=NULL;
        for(int i=0;i<neiLen;i++)
        {
          for(int j=0;j<flowLen;j++)
          {
            if(NeiTable[i].NeiNode.IPAddr[0]==flowEntry[j].IPAddr[0] && NeiTable[i].NeiNode.IPAddr[1]==flowEntry[j].IPAddr[1])
            {
              nextMACAddr=NeiTable[i].NeiNode.MACAddr_H;
            }
          }
        }
        for(int k=0;k<8;k++)
        {
          packet[idx++]=*nextMACAddr;
          checkSum+=*nextMACAddr;
          nextMACAddr++;
        }
        //设置Options
        packet[idx++]=0;
        //0x06表示这是一个数据包
        packet[idx++]=0x06;
        checkSum+=0x06;
        byte* p=&dataPacket.ReduntantTransmissions;
        for(int c=0;c<7;c++)
        {
          packet[idx++]=*p;
          checkSum+=*p;
          p++;
        }
        //设置校验和
        packet[idx++]=0xFF - (checkSum & 0xFF);
        //设置帧的长度
        packet[2]=0xFF & (idx-4);
        Serial1.write(packet,idx);
        Serial.println(F("Sending data packet"));
        FID++;
        delay(100);
      }else{
        Serial.print(F("Abandon this data packet"));
      }
    }
    flowEntry.Clear();
  } else
  {
    /*
     * 暂时没有节点醒来 -> Adding the data packet into Queue;
    */
    for(int i=0;i<WaitingPacketsQueue.size();i++){
      WaitingPacketsQueue[i].WaitingTimes++;
    }
    Serial.println(F("without awake"));
  }
  IsSendPacket=false;
}
//发送位于队列中的数据包
void DeliveryPacketsInQueue()
{
  /*
   * 定时检查队列，当队列中有数据包的时候，先发送PreamblePacket
  */
  int queueLen=WaitingPacketsQueue.size();
  //queueLen>0 && lock==0
  if(queueLen>0 && IsSendPacket==false)
  {
    Serial.println(F("DePInQueue"));
    SendPreamblePacket();
  }
}
/*
//网络邻居发现
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
  Serial.print(F("len: "));
  Serial.println(len);
  int neiLen = NeiTable.Count();
  //sssint neiLen=NeiTable.size();
  //开辟一个接收读入数据的空间
  byte Info[32] = {0};
  //Info[0]是Command Status
  Serial.println(F("printInfo"));
  for (unsigned int c = 0; c < len; c++)
  {
    Info[c] = SerialInput();
    if(Info[c]<0x10){Serial.print(F("0"));}
    Serial.print(Info[c],HEX);
    checkSumTemp += Info[c];
  }
  Serial.println();
  //根据校验和检查接收的数据是否有问题
  unsigned int checkSum = SerialInput();
  if (checkSum != (0xFF - (checkSumTemp & 0xFF)))
  {
    Serial.print(F("\nErrorAddNeiTable\n\r"));
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
      byte* p1=tmp.NeiNode.IPAddr;
      for(int j=0;j<10;j++)
      {
        *p1=Info[j];
        p1++;
      }
      delay(10);
      NeiTable.Add(tmp);
      //sssNeiTable.push_back(tmp);
    }
  }
}

//更新邻居表信息
void UpdateNeiTable(byte ipAddr_H, byte ipAddr_L, unsigned int len, unsigned int chkSum)
{
  int neiLen = NeiTable.Count();
  //sssint neiLen=NeiTable.size();
  byte Info[32] = {0};
  Serial.println(F("ResponseInformation"));
  for (unsigned c = 0; c < len; c++)
  {
    Info[c] = SerialInput();
    if(Info[c]<0x10){Serial.print(F("0"));}
    Serial.print(Info[c],HEX);
    chkSum += Info[c];
  }
  Serial.println();
  //读取校验和
  unsigned int checkSum = SerialInput();
  if (checkSum != (0xFF - (chkSum & 0xFF)))
  {
    Serial.println(F("ErrorUpdateNeiTable"));
  }else{
    unsigned int idx = 100;
    for (unsigned int i = 0; i < neiLen; i++)
    {
      if (ipAddr_H == NeiTable[i].NeiNode.IPAddr[0] && ipAddr_L == NeiTable[i].NeiNode.IPAddr[1])
      {
        idx = i;
        break;
      }
    }
    if (idx < neiLen)
    {
      for(int j=0;j<4;j++)
      {
        NeiTable[idx].NeiNode.pos.x[j] = Info[j];
        NeiTable[idx].NeiNode.pos.y[j] = Info[j+4];
        NeiTable[idx].NeiNode.InitialEnergy[j] = Info[j+8];
        NeiTable[idx].NeiNode.CurrentEnergy[j] = Info[j+12];
      }
    }
  }
}

//打印邻居表
void PrintNeiTable()
{
  int neiLen = NeiTable.Count();
  //sssint neiLen=NeiTable.size();
  for (unsigned int i = 0; i < neiLen; i++)
  {
    Serial.print(F("\n\r"));
    byte* p = NeiTable[i].NeiNode.IPAddr;
    for (unsigned int c = 0; c < 32; c++)
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
  Serial.println();
}
//是否开始发送DataPacket
void EnabledSendPreamblePacket()
{
  sendPreamblePacketFlag = true;
}
//启用队列时间戳
void EnabledQueueTimer()
{
  queueTimerFlag=true;
}
//计算forwarder set
void ComputedForwarderSet()
{
  float sum=0.0;
  float fourCorners[8]={0};
  PredefinedRoutingZone(MyPos,SinkPos,fourCorners);
  int neiLen=NeiTable.Count();
  //sssint neiLen=NeiTable.size();
  float cv=0.0;
  byte bufTemp[4]={0};
  for(int i=0;i<neiLen;i++)
  {
    bufTemp[0]=0;
    bufTemp[1]=0;
    bufTemp[2]=0;
    bufTemp[3]=0;
    if(IsNeighborWithinRoutingZone(NeiTable[i].NeiNode,fourCorners))
    {
      NeiTable[i].IsCandidate=1;
    }else
    {
      NeiTable[i].IsCandidate=0;
    }
    //cv=CompetencyValue(MyPos,NeiTable[i].NeiNode.pos,SinkPos);
    cv=CompetencyValue(NeiTable[i].NeiNode);
    FloatToHEX(cv,bufTemp);
    NeiTable[i].competencyValue[0]=bufTemp[0];
    NeiTable[i].competencyValue[1]=bufTemp[1];
    NeiTable[i].competencyValue[2]=bufTemp[2];
    NeiTable[i].competencyValue[3]=bufTemp[3];
    sum+=cv;
  }
  for(int j=0;j<neiLen;j++)
  {
    bufTemp[0]=0;
    bufTemp[1]=0;
    bufTemp[2]=0;
    bufTemp[3]=0;
    cv=HexToFloat(NeiTable[j].competencyValue,4)/sum;
    FloatToHEX(cv,bufTemp);
    NeiTable[j].competencyValue[0]=bufTemp[0];
    NeiTable[j].competencyValue[1]=bufTemp[1];
    NeiTable[j].competencyValue[2]=bufTemp[2];
    NeiTable[j].competencyValue[3]=bufTemp[3];
  }
  /*
   * 根据competencyValue排序
  */
  SortNeiTable();
  int forwardersCount=0;
  float average=1.0/((float)neiLen);
  float n=(float)neiLen+1.0;
  int Ftheashoeld=(int)ceil(sqrt(sqrt(n)));
  for(int k=0;k<neiLen;k++)
  {
    cv=HexToFloat(NeiTable[k].competencyValue,4);
    if(forwardersCount<=Ftheashoeld && cv>=average && NeiTable[k].IsCandidate==1)
    {
      NeiTable[k].action=1;
    }else
    {
      NeiTable[k].action=0;
    }
  }
}
void printHops()
{
  Serial.print(F("Hops: "));
  Serial.println(HopsToSink);
}
void ActiveToSleep()
{
  pinMode(2,OUTPUT);
  digitalWrite(2,HIGH);
}
void SleepToActive()
{
  pinMode(2,OUTPUT);
  digitalWrite(2,LOW);
}
void Initialization()
{
  SleepToActive();
  GlobalTimer=millis();
  neighborsDiscoveryTimer=millis();
  queryInfosTimer=millis();
  sendDataPacketTimer=millis();
  queueTimer=millis();
  updateHopsTimer=millis();
  randomSeed(analogRead(A0));
}

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  Serial1.begin(9600);
  
  
  if (isSink)
  {
    MsTimer2::set(3000, HopsToSinkComputation);
    MsTimer2::start();
  }
  /*
   * 前30s用于初始化工作
  */
  /*设置初始能量*/
  FloatToHEX(getEnergy(),initialEnergy);
  Initialization();
}

void loop() {
  // put your main code here, to run repeatedly:
  //simpleTimer.run();
  if(millis()-GlobalTimer<60000){
  }else if(millis()-GlobalTimer<75000){
    if(millis()-neighborsDiscoveryTimer>3000){
      neighborsDiscoveryTimer=millis();
      NeighborsDiscovery();
    }
  }else if(millis()-GlobalTimer<85000){
    if(millis()-queryInfosTimer>2000){
      queryInfosTimer=millis();
      QueryInfosForNeighbors();
    }
  }else if((millis()-GlobalTimer>85000) && (millis()-GlobalTimer<90000)){
    if(millis()-updateHopsTimer>1500){
      updateHopsTimer=millis();
      HopsToSinkComputation();
    }
  }else if(millis()-GlobalTimer>120000){
    if(StartDutyCycle==false){
      ComputedForwarderSet();
      delay(50);
      sendPreamblePacketFlag=true;
      StartDutyCycle=true;
      active=true;
      activeTimer=millis();
      randomDelay=random(0,2000);
    }
  }  
  if(StartDutyCycle==true){
    if((millis()-activeTimer>=0) && (millis()-activeTimer<=3000)){
      SleepToActive();
      delay(10);
      //如果节点从Sleep -> Active：检查队列中是否有数据包需要转发
      if(active==true){
        DeliveryPacketsInQueue();
        active=false;
        queueTimer=millis();
      }else{
        //每隔n秒检查队列中是否有数据包需要转发
        if(millis()-queueTimer>1500){
          queueTimer=millis();
          DeliveryPacketsInQueue();
        }
      }
    }else if((millis()-activeTimer>3000) && (millis()-activeTimer)<=4000){
      ActiveToSleep();
    }else{
      activeTimer=millis();
      active=true;
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
        //读取帧的ID
        frameID = SerialInput();
        //读取AT Command
        byte ATC_H = SerialInput();
        byte ATC_L = SerialInput();
        chkSum += frameID + ATC_H + ATC_L;
        /*
        if (ATC_H == 0x4E && ATC_L == 0x44)
        {
          //说明是一个用于网络节点发现的包
          AddNeiTable(byteCount - 4, chkSum);
          Serial.print(F("\n\rPrint NeiTable: "));
          PrintNeiTable();
          Serial.print(F("\n\r"));
        }
        */
        if(ATC_H == 0x4D && ATC_L == 0x59)
        {
          /*
           * 获取IP地址
          */
          //读取Status
          byte Status = SerialInput();
          chkSum += Status;
          //读取IP地址的高位
          myIPAddress[0] = SerialInput();
          chkSum += myIPAddress[0];
          //读取IP地址的低位
          myIPAddress[1] = SerialInput();
          chkSum += myIPAddress[1];
          //读取校验和
          unsigned int checkSum = SerialInput();
          if (checkSum != (0xFF - (chkSum & 0xFF)))
          {
            Serial.println(F("getIPAddress Failed"));
          }
        }else if(ATC_H == 0x53 && ATC_L == 0x48)
        {
          /*
           * 获取MAC地址的高32位
          */
          //读取Status
          byte Status = SerialInput();
          chkSum += Status;
          //读取MAC地址的高32位
          for(unsigned int i=0;i<4;i++)
          {
            myMACAddress[i] = SerialInput();
            chkSum += myMACAddress[i];
          }
          //读取校验和
          unsigned int checkSum = SerialInput();
          if (checkSum != (0xFF - (chkSum & 0xFF)))
          {
            Serial.println(F("getMAC_SH Failed"));
          }
        }else if(ATC_H == 0x53 && ATC_L == 0x4C)
        {
          /*
           * 获取MAC地址的低32位
          */
          //读取Status
          byte Status = SerialInput();
          chkSum += Status;
          //读取MAC地址的低32位
          for(unsigned int i=0;i<4;i++)
          {
            myMACAddress[i+4] = SerialInput();
            chkSum += myMACAddress[i+4];
          }
          //读取校验和
          unsigned int checkSum = SerialInput();
          if (checkSum != (0xFF - (chkSum & 0xFF)))
          {
            Serial.println(F("getMAC_SL Failed"));
          }
        }
        
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
            Serial.println(F("Error 0x00"));
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
        }else if (packetType == 0x01)
        {
          /*
            更新HopsToSink的包
          */
          //读取节点的位置坐标
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
          //读取HopsToSink
          byte hops = SerialInput();
          chkSum += hops;
          //读取校验和
          unsigned int checkSum = SerialInput();
          //验证数据包的有效性
          if (checkSum != (0xFF - (chkSum & 0xFF)))
          {
            Serial.println(F("Update Hops Failed"));
          } else
          {
            //更新HopsToSink
            if ((DistanceBetweenSensors(MyPos,NeiPos) <= CommunicationRangeRadius) && (hops + 1 < HopsToSink))
            {
              HopsToSink = hops + 1;
              HopsToSinkComputation();
              Serial.print(F("HopsToSink: "));
              Serial.println(HopsToSink, DEC);
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
        else if (packetType == 0x03)
        {
          /*
           * 邻居节点对信息查询的响应
          */
          //更新邻居表
          Serial.println(F("Receive Info From Neighbors"));
          Serial.print(IPAddr_L,HEX);
          Serial.print(F("--->RSSI: "));
          Serial.println(rssiValue,DEC);
          delay(10);
          UpdateNeiTable(IPAddr_H, IPAddr_L, 16, chkSum);
          Serial.print(F("\n\rUpdate NeiTable: "));
          delay(10);
          PrintNeiTable();
          Serial.print(F("\n\r"));
        }
        else if (packetType == 0x04)
        {
          /*
           * this is a preamble packet
           * 读取the address of potential forwarders
           * 如果自己是 a potential forwarder，则返回ACK_PreamblePacket
           * 
           */
          boolean enabledAck=false;
          Serial.println(F("this is a preamble packet"));
          //读取候选节点的个数
          unsigned int candidatesCount=SerialInput();
          chkSum+=candidatesCount;
          candidatesCount=candidatesCount/2;
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
            if(addressHigh==myIPAddress[0] && addressLow==myIPAddress[1])
            {
              enabledAck=true;
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
              activeTimer=millis();
              responseDelay=random(0,150);
              delay(responseDelay);
              SendAckToPreamblePacket(senderMacAddr);
            } else
            {
              Serial.println(F("\n Can't Find MAC address"));
            }
          }       
        }
        else if (packetType == 0x05)
        {
          Serial.println(F("this is a packetType 0x05"));
          activeTimer=millis();
          /*
           * this is a ACK_PreamblePacket
           * adding nodes that are awake and send back ACK into flowEntry
           */
          //读取校验和
          unsigned int checkSum = SerialInput();
          if (checkSum != (0xFF - (chkSum & 0xFF)))
          {
            Serial.print(F("\nError 0x05\n\r"));
          }else{
            MatchFlow tmp = {0};
            tmp.IPAddr[0] = IPAddr_H;
            tmp.IPAddr[1] = IPAddr_L;
            bool contain=false;
            for(int i=0;i<flowEntry.Count();i++){
              if(flowEntry[i].IPAddr[0]==tmp.IPAddr[0] && flowEntry[i].IPAddr[1]==tmp.IPAddr[1]){
                contain=true;
                break;
              }
            }
            if(contain==false){
              flowEntry.Add(tmp);
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
           activeTimer=millis();
           Packet packetTemp={0};
           /*
            * 读取packet的内容
           */
           //读取ReduntantTransmissions
           packetTemp.ReduntantTransmissions=SerialInput();
           chkSum+=packetTemp.ReduntantTransmissions;
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
            Serial.print(F("\nError 0x06\n\r"));
           }else{
            WaitingPacketsQueue.push_back(packetTemp);
            lock++;
            //SendPreamblePacket();
            Serial.print(F("Source IP Address:"));
            Serial.print(packetTemp.SourceIPAddress[0],HEX);
            Serial.print(F("->"));
            Serial.println(packetTemp.SourceIPAddress[1],HEX);
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
