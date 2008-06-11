unit ComPorts;

interface

uses
  Windows, Messages, SysUtils, ExtCtrls, Classes, Graphics, Controls, Forms, Dialogs;

type
  CommArray = array [0..31] of Byte;
  TPort = (COM1,COM2,COM3,COM4,COM5,COM6,COM7,COM8);
  TBaud = (b1200,b2400,b4800,b9600,b19200,b115200);
  TParity = (pNone,pOdd,pEven);
  TStop = (s1,s2);

  // Component Declaration
  //
  TComms = class(TComponent)
  private
    { Private declarations }
    FPort: TPort;
    FBaud: TBaud;
    FParity: TParity;
    FStop: TStop;
  protected
    { Protected declarations }
    DCB: TDCB;
    CommsID: integer;
    TimeOuts: _COMMTIMEOUTS;
  public
    { Public declarations }
    constructor Create(AOwner: TComponent); override;
    procedure InitComms;
    procedure CloseComms;
    function ReadComms(var Buf: CommArray) : DWORD;
    function WriteComms(var Buf: CommArray; NumOfBytes: DWORD): DWORD;
    function GetCommsID(): integer ;
  published
    { Published declarations }
    property Port: TPort read FPort write FPort;
    property BaudRate: TBaud read FBaud write FBaud;
    property Parity: TParity read FParity write FParity;
    property StopBits: TStop read FStop write FStop;
  end;

const EventMask: DWORD = EV_TXEMPTY;    // checks for TXBuffer empty

procedure Register;

//{$R *.DCR}

implementation

constructor TComms.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
  CommsID:= -1;
end;

procedure TComms.InitComms;
const LPort: PChar = 'COM1';
begin
{$R-}
 if CommsID < 0
 then begin
  case FPort of
    COM1: LPort:= 'COM1';
    COM2: LPort:= 'COM2';
    COM3: LPort:= 'COM3';
    COM4: LPort:= 'COM4';
    COM5: LPort:= 'COM5';
    COM6: LPort:= 'COM6';
    COM7: LPort:= 'COM7';
    COM8: LPort:= 'COM8';
  end;
  CommsID:= CreateFile(LPort,GENERIC_READ OR GENERIC_WRITE,0,nil,OPEN_EXISTING,0,0);
  if CommsID >= 0         (* if successful *)
  then begin
    GetCommState(CommsID,DCB);
    DCB.ByteSize:= 8;     (* 8 bits *)
    case FBaud of
      b1200: DCB.BaudRate:= 1200;
      b2400: DCB.BaudRate:= 2400;
      b4800: DCB.BaudRate:= 4800;
      b9600: DCB.BaudRate:= 9600;
      b19200: DCB.BaudRate:= 19200;
      b115200: DCB.BaudRate:= CBR_115200;
    end;
    case FParity of
      pNone: DCB.Parity:= NOPARITY;
      pOdd: DCB.Parity:= ODDPARITY;
      pEven: DCB.Parity:= EVENPARITY;
    end;
    case FStop of
      s1: DCB.StopBits:= ONESTOPBIT; 
      s2: DCB.StopBits:= TWOSTOPBITS;
    end;
    // added for XP to ensure RTS_CONTROL_TOGGLE is turned off
    // (user must also turn off UART FIFI in device manager)
    DCB.Flags:= DCB.Flags and $FFFFCFFFF;

    SetCommState(CommsID,DCB);
    GetCommTimeOuts(CommsID,TimeOuts);
    TimeOuts.ReadIntervalTimeout:= MAXDWORD;
    TimeOuts.ReadTotalTimeoutMultiplier:= 0;
    TimeOuts.ReadTotalTimeoutConstant:= 0;
    TimeOuts.WriteTotalTimeoutMultiplier:= 0;
    TimeOuts.WriteTotalTimeoutConstant:= 0;
    SetCommTimeOuts(CommsID,TimeOuts);
    PurgeComm(CommsID,PURGE_TXCLEAR OR PURGE_RXCLEAR);
    EscapeCommFunction(CommsID,CLRRTS);
    SetCommMask(CommsID,EventMask);
   end;
 end;
{$R+}
end;

function TComms.ReadComms(var Buf: CommArray) : DWORD;
begin
{$R-}
  ReadFile(CommsID,Buf,sizeof(Buf),Result,nil);
{$R+}
end;

function TComms.WriteComms(var Buf: CommArray; NumOfBytes: DWORD): DWORD ;
var Start, Finish: Int64;
begin
{$R-}
  Result:= 0;
  if NumOfBytes > 0
  then begin
    EscapeCommFunction(CommsID,SETRTS);
    WriteFile(CommsID,Buf,NumOfBytes,Result,nil);
    WaitCommEvent(CommsID,EventMask,nil);
    QueryPerformanceCounter(Start);
    repeat
       QueryPerformanceCounter(Finish);
    until (Finish - Start) > 4000;
    EscapeCommFunction(CommsID,CLRRTS);
  end;
{$R+}
end;

function TComms.GetCommsID(): integer ;
begin
  result:= CommsID;
end;

procedure TComms.CloseComms;
begin
{$R-}
  EscapeCommFunction(CommsID,CLRRTS);
  CloseHandle(CommsID);
  CommsID:= -1;
{$R+}
end;

procedure Register;
begin
  RegisterComponents('Samples', [TComms]);
end;

end.
