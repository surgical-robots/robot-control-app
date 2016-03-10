using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace ROBOTIS
{
    class dynamixel
    {
        public const int MAX_ID             = 252;
        public const int BROADCAST_ID       = 254;

        public const int INST_PING          = 1;
        public const int INST_READ          = 2;
        public const int INST_WRITE         = 3;
        public const int INST_REG_WRITE     = 4;
        public const int INST_ACTION        = 5;
        public const int INST_RESET         = 6;
        public const int INST_SYNC_WRITE    = 131;
        public const int INST_BULK_READ     = 146;  // 0x92

        public const int INST_REBOOT        = 8;
        public const int INST_STATUS        = 85;   // 0x55
        public const int INST_SYNC_READ     = 130;  // 0x82
        public const int INST_BULK_WRITE    = 147;  // 0x93

        public const int ERRBIT_ALERT       = 128;

        public const int ERR_RESULT_FAIL    = 1;
        public const int ERR_INSTRUCTION    = 2;
        public const int ERR_CRC            = 3;
        public const int ERR_DATA_RANGE     = 4;
        public const int ERR_DATA_LENGTH    = 5;
        public const int ERR_DATA_LIMIT     = 6;
        public const int ERR_ACCESS         = 7;

        public const int COMM_TXSUCCESS     = 0;
        public const int COMM_RXSUCCESS     = 1;
        public const int COMM_TXFAIL        = 2;
        public const int COMM_RXFAIL        = 3;
        public const int COMM_TXERROR       = 4;
        public const int COMM_RXWAITING     = 5;
        public const int COMM_RXTIMEOUT     = 6;
        public const int COMM_RXCORRUPT     = 7;

        public static UInt16 DXL_MAKEWORD(UInt32 a, UInt32 b)   { return (UInt16)((a & 0xFF) | ((b & 0xFF) << 8)); }
        public static UInt32 DXL_MAKEDWORD(UInt32 a, UInt32 b)  { return (UInt32)((a & 0xFFFF) | ((b & 0xFFFF) << 16)); }
        public static UInt16 DXL_LOWORD(UInt32 l) { return (UInt16)(l & 0xFFFF); }
        public static UInt16 DXL_HIWORD(UInt32 l) { return (UInt16)((l >> 16) & 0xFFFF); }
        public static byte   DXL_LOBYTE(UInt16 w) { return (byte)(w & 0xFF); }
        public static byte   DXL_HIBYTE(UInt16 w) { return (byte)((w >> 8) & 0xFF); }

        [DllImport("dynamixel2_win64.dll")]
        public static extern int dxl_initialize(int port_num, int baud_rate);
        [DllImport("dynamixel2_win64.dll")]
        public static extern int dxl_change_baudrate(int baud_rate);
        [DllImport("dynamixel2_win64.dll")]
        public static extern int dxl_terminate();

        [DllImport("dynamixel2_win64.dll")]
        public static extern int dxl_get_comm_result();

        [DllImport("dynamixel2_win64.dll")]
        public static extern void dxl_tx_packet();
        [DllImport("dynamixel2_win64.dll")]
        public static extern void dxl_rx_packet();
        [DllImport("dynamixel2_win64.dll")]
        public static extern void dxl_txrx_packet();

        [DllImport("dynamixel2_win64.dll")]
        public static extern void dxl_set_txpacket_id(int id);
        [DllImport("dynamixel2_win64.dll")]
        public static extern void dxl_set_txpacket_instruction(int instruction);
        [DllImport("dynamixel2_win64.dll")]
        public static extern void dxl_set_txpacket_parameter(int index, int value);
        [DllImport("dynamixel2_win64.dll")]
        public static extern void dxl_set_txpacket_length(int length);
        [DllImport("dynamixel2_win64.dll")]
        public static extern int dxl_get_rxpacket_error(int error);
        [DllImport("dynamixel2_win64.dll")]
        public static extern int dxl_get_rxpacket_error_byte();
        [DllImport("dynamixel2_win64.dll")]
        public static extern int dxl_get_rxpacket_parameter(int index);
        [DllImport("dynamixel2_win64.dll")]
        public static extern int dxl_get_rxpacket_length();

        [DllImport("dynamixel2_win64.dll")]
        public static extern void dxl_ping(int id);
        [DllImport("dynamixel2_win64.dll")]
        public static extern int dxl_read_byte(int id, int address);
        [DllImport("dynamixel2_win64.dll")]
        public static extern void dxl_write_byte(int id, int address, int value);
        [DllImport("dynamixel2_win64.dll")]
        public static extern int dxl_read_word(int id, int address);
        [DllImport("dynamixel2_win64.dll")]
        public static extern void dxl_write_word(int id, int address, int value);

        [DllImport("dynamixel2_win64.dll")]
        public static extern void dxl2_tx_packet();
        [DllImport("dynamixel2_win64.dll")]
        public static extern void dxl2_rx_packet();
        [DllImport("dynamixel2_win64.dll")]
        public static extern void dxl2_txrx_packet();

        [DllImport("dynamixel2_win64.dll")]
        public static extern void dxl2_set_txpacket_id(byte id);
        [DllImport("dynamixel2_win64.dll")]
        public static extern void dxl2_set_txpacket_instruction(byte instruction);
        [DllImport("dynamixel2_win64.dll")]
        public static extern void dxl2_set_txpacket_parameter(UInt16 index, byte value);
        [DllImport("dynamixel2_win64.dll")]
        public static extern void dxl2_set_txpacket_length(UInt16 length);
        [DllImport("dynamixel2_win64.dll")]
        public static extern int dxl2_get_rxpacket_error_byte();
        [DllImport("dynamixel2_win64.dll")]
        public static extern int dxl2_get_rxpacket_parameter(int index);
        [DllImport("dynamixel2_win64.dll")]
        public static extern int dxl2_get_rxpacket_length();

        [DllImport("dynamixel2_win64.dll")]
        public static extern void dxl2_ping(byte id);
        [DllImport("dynamixel2_win64.dll")]
        public static extern int dxl2_get_ping_result(byte id, int info_num);
        [DllImport("dynamixel2_win64.dll")]
        public static extern void dxl2_broadcast_ping();

        [DllImport("dynamixel2_win64.dll")]
        public static extern void dxl2_reboot(byte id);
        [DllImport("dynamixel2_win64.dll")]
        public static extern void dxl2_factory_reset(byte id, int option);

        [DllImport("dynamixel2_win64.dll")]
        public static extern byte dxl2_read_byte(byte id, int address);
        [DllImport("dynamixel2_win64.dll")]
        public static extern void dxl2_write_byte(byte id, int address, byte value);
        [DllImport("dynamixel2_win64.dll")]
        public static extern UInt16 dxl2_read_word(byte id, int address);
        [DllImport("dynamixel2_win64.dll")]
        public static extern void dxl2_write_word(byte id, int address, UInt16 value);
        [DllImport("dynamixel2_win64.dll")]
        public static extern UInt32 dxl2_read_dword(byte id, int address);
        [DllImport("dynamixel2_win64.dll")]
        public static extern void dxl2_write_dword(byte id, int address, UInt32 value);

        [DllImport("dynamixel2_win64.dll")]
        public static extern byte dxl2_get_bulk_read_data_byte(byte id, UInt32 start_address);
        [DllImport("dynamixel2_win64.dll")]
        public static extern UInt16 dxl2_get_bulk_read_data_word(byte id, UInt32 start_address);
        [DllImport("dynamixel2_win64.dll")]
        public static extern UInt32 dxl2_get_bulk_read_data_dword(byte id, UInt32 start_address);

        [DllImport("dynamixel2_win64.dll")]
        public static extern byte dxl2_get_sync_read_data_byte(byte id, UInt32 start_address);
        [DllImport("dynamixel2_win64.dll")]
        public static extern UInt16 dxl2_get_sync_read_data_word(byte id, UInt32 start_address);
        [DllImport("dynamixel2_win64.dll")]
        public static extern UInt32 dxl2_get_sync_read_data_dword(byte id, UInt32 start_address);
    }
}