// Jungo Connectivity Confidential. Copyright (c) 2023 Jungo Connectivity Ltd.  https://www.jungo.com

// Note: This code sample is provided AS-IS and as a guiding sample only.

using System;

using Jungo.wdapi_dotnet;
using DWORD = System.UInt32;

namespace Jungo.plx_lib
{
    public struct WDC_REG
    {
        public DWORD dwAddrSpace;       /* Number of address space in which the register resides */
        /* For PCI configuration registers, use WDC_AD_CFG_SPACE */
        public DWORD dwOffset;          /* Offset of the register in the dwAddrSpace address space */
        public DWORD dwSize;            /* Register's size (in bytes) */
        public WDC_DIRECTION direction; /* Read/write access mode - see WDC_DIRECTION options */
        public string  sName;           /* Register's name */
        public string  sDesc;           /* Register's description */

        public WDC_REG(DWORD _dwAddrSpace, DWORD _dwOffset, DWORD _dwSize,
            WDC_DIRECTION _direction, string  _sName, string _sDesc)
        {
            dwAddrSpace = _dwAddrSpace;
            dwOffset = _dwOffset;
            dwSize = _dwSize;
            direction = _direction;
            sName = _sName;
            sDesc = _sDesc;
        }
    };

    public class PLX_Regs
    {
        private const uint WDC_AD_CFG_SPACE = 0xFF;
        public static readonly WDC_REG[] gPLX_CfgRegs = new WDC_REG[]
        {
            new WDC_REG(WDC_AD_CFG_SPACE, (DWORD)PCI_CFG_REG.PCI_VID, wdc_lib_consts.WDC_SIZE_16, WDC_DIRECTION.WDC_READ_WRITE, "VID", "Vendor ID" ),
            new WDC_REG(WDC_AD_CFG_SPACE, (DWORD)PCI_CFG_REG.PCI_DID, wdc_lib_consts.WDC_SIZE_16, WDC_DIRECTION.WDC_READ_WRITE, "DID", "Device ID" ),
            new WDC_REG(WDC_AD_CFG_SPACE, (DWORD)PCI_CFG_REG.PCI_CR, wdc_lib_consts.WDC_SIZE_16, WDC_DIRECTION.WDC_READ_WRITE, "CMD", "Command" ),
            new WDC_REG(WDC_AD_CFG_SPACE, (DWORD)PCI_CFG_REG.PCI_SR, wdc_lib_consts.WDC_SIZE_16, WDC_DIRECTION.WDC_READ_WRITE, "STS", "Status" ),
            new WDC_REG(WDC_AD_CFG_SPACE, (DWORD)PCI_CFG_REG.PCI_REV, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "RID_CLCD", "Revision ID & Class Code" ),
            new WDC_REG(WDC_AD_CFG_SPACE, (DWORD)PCI_CFG_REG.PCI_CCSC, wdc_lib_consts.WDC_SIZE_8, WDC_DIRECTION.WDC_READ_WRITE, "SCC", "Sub Class Code" ),
            new WDC_REG(WDC_AD_CFG_SPACE, (DWORD)PCI_CFG_REG.PCI_CCBC, wdc_lib_consts.WDC_SIZE_8, WDC_DIRECTION.WDC_READ_WRITE, "BCC", "Base Class Code" ),
            new WDC_REG(WDC_AD_CFG_SPACE, (DWORD)PCI_CFG_REG.PCI_CLSR, wdc_lib_consts.WDC_SIZE_8, WDC_DIRECTION.WDC_READ_WRITE, "CALN", "Cache Line Size" ),
            new WDC_REG(WDC_AD_CFG_SPACE, (DWORD)PCI_CFG_REG.PCI_LTR, wdc_lib_consts.WDC_SIZE_8, WDC_DIRECTION.WDC_READ_WRITE, "LAT", "Latency Timer" ),
            new WDC_REG(WDC_AD_CFG_SPACE, (DWORD)PCI_CFG_REG.PCI_HDR, wdc_lib_consts.WDC_SIZE_8, WDC_DIRECTION.WDC_READ_WRITE, "HDR", "Header Type" ),
            new WDC_REG(WDC_AD_CFG_SPACE, (DWORD)PCI_CFG_REG.PCI_BISTR, wdc_lib_consts.WDC_SIZE_8, WDC_DIRECTION.WDC_READ_WRITE, "BIST", "Built-in Self Test" ),
            new WDC_REG(WDC_AD_CFG_SPACE, (DWORD)PCI_CFG_REG.PCI_BAR0, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "BADDR0", "Base Address 0" ),
            new WDC_REG(WDC_AD_CFG_SPACE, (DWORD)PCI_CFG_REG.PCI_BAR1, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "BADDR1", "Base Address 1" ),
            new WDC_REG(WDC_AD_CFG_SPACE, (DWORD)PCI_CFG_REG.PCI_BAR2, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "BADDR2", "Base Address 2" ),/* Mark end of chain */
            new WDC_REG(WDC_AD_CFG_SPACE, (DWORD)PCI_CFG_REG.PCI_BAR3, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "BADDR3", "Base Address 3" ),
            new WDC_REG(WDC_AD_CFG_SPACE, (DWORD)PCI_CFG_REG.PCI_BAR4, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "BADDR4", "Base Address 4" ),
            new WDC_REG(WDC_AD_CFG_SPACE, (DWORD)PCI_CFG_REG.PCI_BAR5, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "BADDR5", "Base Address 5" ),
            new WDC_REG(WDC_AD_CFG_SPACE, (DWORD)PCI_CFG_REG.PCI_CIS, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "CIS", "CardBus CIS pointer" ),
            new WDC_REG(WDC_AD_CFG_SPACE, (DWORD)PCI_CFG_REG.PCI_SVID, wdc_lib_consts.WDC_SIZE_16, WDC_DIRECTION.WDC_READ_WRITE, "SVID", "Sub-system Vendor ID" ),
            new WDC_REG(WDC_AD_CFG_SPACE, (DWORD)PCI_CFG_REG.PCI_SDID, wdc_lib_consts.WDC_SIZE_16, WDC_DIRECTION.WDC_READ_WRITE, "SDID", "Sub-system Device ID" ),
            new WDC_REG(WDC_AD_CFG_SPACE, (DWORD)PCI_CFG_REG.PCI_EROM, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "EROM", "Expansion ROM Base Address" ),
            new WDC_REG(WDC_AD_CFG_SPACE, (DWORD)PCI_CFG_REG.PCI_CAP, wdc_lib_consts.WDC_SIZE_8, WDC_DIRECTION.WDC_READ_WRITE, "NEW_CAP", "New Capabilities Pointer" ),
            new WDC_REG(WDC_AD_CFG_SPACE, (DWORD)PCI_CFG_REG.PCI_ILR, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "INTLN", "Interrupt Line" ),
            new WDC_REG(WDC_AD_CFG_SPACE, (DWORD)PCI_CFG_REG.PCI_IPR, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "INTPIN", "Interrupt Pin" ),
            new WDC_REG(WDC_AD_CFG_SPACE, (DWORD)PCI_CFG_REG.PCI_MGR, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "MINGNT", "Minimum Required Burst Period" ),
            new WDC_REG(WDC_AD_CFG_SPACE, (DWORD)PCI_CFG_REG.PCI_MLR, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "MAXLAT", "Maximum Latency" ),
            /* PLX-specific configuration registers */
            new WDC_REG(WDC_AD_CFG_SPACE, 0x40, wdc_lib_consts.WDC_SIZE_8, WDC_DIRECTION.WDC_READ_WRITE, "PMCAPID", "Power Management Capability ID" ),
            new WDC_REG(WDC_AD_CFG_SPACE, 0x41, wdc_lib_consts.WDC_SIZE_8, WDC_DIRECTION.WDC_READ_WRITE, "PMNEXT", "Power Management Next Capability Pointer" ),
            new WDC_REG(WDC_AD_CFG_SPACE, 0x42, wdc_lib_consts.WDC_SIZE_16, WDC_DIRECTION.WDC_READ_WRITE, "PMCAP", "Power Management Capabilities" ),
            new WDC_REG(WDC_AD_CFG_SPACE, 0x44, wdc_lib_consts.WDC_SIZE_16, WDC_DIRECTION.WDC_READ_WRITE, "PMCSR", "Power Management Control/Status" ),
            new WDC_REG(WDC_AD_CFG_SPACE, 0x48, wdc_lib_consts.WDC_SIZE_8, WDC_DIRECTION.WDC_READ_WRITE, "HS_CAPID", "Hot Swap Capability ID" ),
            new WDC_REG(WDC_AD_CFG_SPACE, 0x49, wdc_lib_consts.WDC_SIZE_8, WDC_DIRECTION.WDC_READ_WRITE, "HS_NEXT", "Hot Swap Next Capability Pointer" ),
            new WDC_REG(WDC_AD_CFG_SPACE, 0x4A, wdc_lib_consts.WDC_SIZE_8, WDC_DIRECTION.WDC_READ_WRITE, "HS_CSR", "Hot Swap Control/Status" ),
            new WDC_REG(WDC_AD_CFG_SPACE, 0x4C, wdc_lib_consts.WDC_SIZE_8, WDC_DIRECTION.WDC_READ_WRITE, "VPD_CAPID", "PCI Vital Product Data Control" ),
            new WDC_REG(WDC_AD_CFG_SPACE, 0x4D, wdc_lib_consts.WDC_SIZE_8, WDC_DIRECTION.WDC_READ_WRITE, "VPD_NEXT", "PCI Vital Product Next Capability Pointer" ),
            new WDC_REG(WDC_AD_CFG_SPACE, 0x4E, wdc_lib_consts.WDC_SIZE_16, WDC_DIRECTION.WDC_READ_WRITE, "VPD_ADDR", "PCI Vital Product Data Address" ),
            new WDC_REG(WDC_AD_CFG_SPACE, 0x50, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "VPD_DATA", "PCI VPD Data" ),
        };

        /* PLX run-time registers information array */
        public static readonly WDC_REG[] gPLX_M_Regs =
        {
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x00, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "LAS0RR", "Local Addr Space 0 Range for PCI-to-Local Bus" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x04, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "LAS0BA", "Local BAR (Remap) for PCI-to-Local Addr Space 0" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x08, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "MARBR", "Mode/DMA Arbitration" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x0C, wdc_lib_consts.WDC_SIZE_8, WDC_DIRECTION.WDC_READ_WRITE, "BIGEND", "Big/Little Endian Descriptor" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x0D, wdc_lib_consts.WDC_SIZE_8, WDC_DIRECTION.WDC_READ_WRITE, "LMISC", "Local Miscellananeous Control" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x0E, wdc_lib_consts.WDC_SIZE_8, WDC_DIRECTION.WDC_READ_WRITE, "PROT_AREA", "Serial EEPROM Write-Protected Addr Boundary" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x10, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "EROMRR", "Expansion ROM Range" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x14, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "EROMBA", "EROM Local BAR (Remap) & BREQ0 Control" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x18, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "LBRD0", "Local Addr Space 0 Bus Region Descriptor" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x1C, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "DMRR", "Local Range for PCI initiatior-to-PCI" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x20, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "DMLBAM", "Local Bus Addr for PCI Initiatior-to-PCI Mem" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x24, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "DMLBAI", "Local BAR for PCI Initiatior-to-PCI I/O" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x28, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "DMPBAM", "PCI BAR (Remap) for Initiatior-to-PCI Mem" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x2C, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "DMCFGA", "PCI Config Addr for PCI Initiatior-to-PCI I/O" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x30, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "OPQIS", "Outbound Post Queue Interrupt Status" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x34, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "OPQIM", "Outbound Post Queue Interrupt Mask" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x40, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "IQP", "Inbound Queue Post" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x44, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "OQP", "Outbound Queue Post" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x40, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "MBOX0_NO_I2O", "Mailbox 0 (I2O disabled)" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x44, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "MBOX1_NO_I2O", "Mailbox 1 (I2O disabled)" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x78, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "MBOXO", "Mailbox 0" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x7C, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "MBOX1", "Mailbox 1" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x48, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "MBOX2", "Mailbox 2" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x4C, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "MBOX3", "Mailbox 3" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x50, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "MBOX4", "Mailbox 4" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x54, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "MBOX5", "Mailbox 5" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x58, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "MBOX6", "Mailbox 6" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x5C, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "MBOX7", "Mailbox 7" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x60, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "P2LDBELL", "PCI-to-Local Doorbell" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x64, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "L2PDBELL", "Local-to-PCI Doorbell" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x68, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "INTCSR", "Interrupt Control/Status"  ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x6C, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "CNTRL", "Serial EEPROM/User I/O/Init Ctr & PCI Cmd Codes" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x70, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "PCIHIDR", "PCI Hardcoded Configuration ID" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x74, wdc_lib_consts.WDC_SIZE_16, WDC_DIRECTION.WDC_READ_WRITE, "PCIHREV", "PCI Hardcoded Revision ID" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x80, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "DMAMODE0", "DMA Channel 0 Mode" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x84, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "DMAPADR0", "DMA Channel 0 PCI Address" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x88, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "DMALADR0", "DMA Channel 0 Local Address" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x8C, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "DMASIZ0", "DMA Channel 0 Transfer Size (bytes)" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x90, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "DMADPR0", "DMA Channel 0 Descriptor Pointer" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x94, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "DMAMODE1", "DMA Channel 1 Mode" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x98, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "DMAPADR1", "DMA Channel 1 PCI Address" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x9C, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "DMALADR1", "DMA Channel 1 Local Address" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0xA0, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "DMASIZ1", "DMA Channel 1 Transfer Size (bytes)" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0xA4, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "DMADPR1", "DMA Channel 1 Descriptor Pointer" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0xA8, wdc_lib_consts.WDC_SIZE_8, WDC_DIRECTION.WDC_READ_WRITE, "DMACSR0", "DMA Channel 0 Command/Status" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0xA9, wdc_lib_consts.WDC_SIZE_8, WDC_DIRECTION.WDC_READ_WRITE, "DMACSR1", "DMA Channel 1 Command/Status" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0xAC, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "DMAARB", "DMA Arbitration" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0xB0, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "DMATHR", "DMA Threshold (Channel 0 only)" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0xB4, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "DMADAC0", "DMA 0 PCI Dual Address Cycle Address" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0xB8, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "DMADAC1", "DMA 1 PCI Dual Address Cycle Address" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0xC0, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "MQCR", "Messaging Queue Configuration" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0xC4, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "QBAR", "Queue Base Address" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0xC8, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "IFHPR", "Inbound Free Head Pointer" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0xCC, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "IFTPR", "Inbound Free Tail Pointer" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0xD0, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "IPHPR", "Inbound Post Head Pointer" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0xD4, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "IPTPR", "Inbound Post Tail Pointer" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0xD8, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "OFHPR", "Outbound Free Head Pointer" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0xDC, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "OFTPR", "Outbound Free Tail Pointer" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0xE0, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "OPHPR", "Outbound Post Head Pointer" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0xE4, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "OPTPR", "Outbound Post Tail Pointer" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0xE8, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "QSR", "Queue Status/Control" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0xF0, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "LAS1RR", "Local Addr Space 1 Range for PCI-to-Local Bus" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0xF4, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "LAS1BA", "Local Addr Space 1 Local BAR (Remap)" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0xF8, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "LBRD1", "Local Addr Space 1 Bus Region Descriptor" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0xFC, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "DMDAC", "PCI Initiatior PCI Dual Address Cycle" ),
        };

        public static readonly WDC_REG[] gPLX_T_Regs =
        {
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x00, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "LAS0RR", "Local Addr Space 0 Range" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x04, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "LAS1RR", "Local Addr Space 1 Range" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x08, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "LAS2RR", "Local Addr Space 2 Range" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x0C, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "LAS3RR", "Local Addr Space 3 Range" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x10, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "EROMRR", "Expansion ROM Range" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x14, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "LAS0BA", "Local Addr Space 0 Local BAR (Remap)" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x18, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "LAS1BA", "Local Addr Space 1 Local BAR (Remap)" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x1C, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "LAS2BA", "Local Addr Space 2 Local BAR (Remap)" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x20, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "LAS3BA", "Local Addr Space 3 Local BAR (Remap)" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x24, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "EROMBA", "Expansion ROM Local BAR (Remap)" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x28, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "LAS0BRD", "Local Addr Space 0 Bus Region Descriptors" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x2C, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "LAS1BRD", "Local Addr Space 1 Bus Region Descriptors" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x30, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "LAS2BRD", "Local Addr Space 2 Bus Region Descriptors" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x34, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "LAS3BRD", "Local Addr Space 3 Bus Region Descriptors" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x38, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "EROMBRD", "Expansion ROM Bus Region Descriptors" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x3C, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "CS0BASE", "Chip Select 0 Base Address" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x40, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "CS1BASE", "Chip Select 1 Base Address" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x44, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "CS2BASE", "Chip Select 2 Base Address" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x48, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "CS3BASE", "Chip Select 3 Base Address" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x4C, wdc_lib_consts.WDC_SIZE_16, WDC_DIRECTION.WDC_READ_WRITE, "INTCSR", "Interrupt Control/Status" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x4E, wdc_lib_consts.WDC_SIZE_16, WDC_DIRECTION.WDC_READ_WRITE, "PROT_AREA", "Serial EEPROM Write-Protected Addr Boundary" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x50, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "CNTRL", "PCI Target Response; Serial EEPROM; Init Ctr" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x54, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "GPIOC", "General Purpose I/O Control" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x70, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "PMDATASEL", "Hidden 1 Power Management Data Select" ),
            new WDC_REG((DWORD)PLX_ADDR.PLX_ADDR_REG, 0x74, wdc_lib_consts.WDC_SIZE_32, WDC_DIRECTION.WDC_READ_WRITE, "PMDATASCALE", "Hidden 2 Power Management Data Scale" ),
        };
    };
}

