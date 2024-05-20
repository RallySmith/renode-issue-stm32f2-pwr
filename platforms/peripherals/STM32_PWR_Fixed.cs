// Derived from 1.15.0 STM32_PWR.cs
//
// Fix required to track PWR_CR:DBP
//
// Original assignments:
//
// Copyright (c) 2010-2023 Antmicro
// Copyright (c) 2022 SICK AG
//
// This file is licensed under the MIT License.
// Full license text is available in 'licenses/MIT.txt'.
//

using System.Collections.Generic;
using Antmicro.Renode.Core;
using Antmicro.Renode.Core.Structure.Registers;
using Antmicro.Renode.Peripherals.Bus;
using Antmicro.Renode.Logging;
using Antmicro.Renode.Utilities;
using Antmicro.Renode.Exceptions;

namespace Antmicro.Renode.Peripherals.Miscellaneous
{
    [AllowedTranslations(AllowedTranslation.ByteToDoubleWord | AllowedTranslation.WordToDoubleWord)]
    public sealed class STM32_PWR_Fixed : BasicDoubleWordPeripheral, IKnownSize
    {
        public STM32_PWR_Fixed(string stm32Family, IMachine machine) : base(machine)
        {
            if(null == stm32Family)
            {
                throw new ConstructionException("stm32Family was null");
            }
            this.Model = stm32Family;
            DefineRegisters();
        }
        
        private void DefineRegisters()
        {
            // TODO: PWR_CR
            // Model                            reset           mask
            // "F2"                             0x00000000      0x000003FF
            // "F405" "F407" "F415" "F417"      0x00004000      0x000043FF
            // "F42" "F43"                      0x0000C000      0x000FEFFF
            // "F7"                             0x0000C000      0x000FEFFB
            //
            // "H7"     CONSIDER: providing specific model since has extra functionality
            // "L4"     CONSIDER: providing specific model since has extra functionality
            // "L0"     NOTE: has own model in 1.14.0: Peripherals/Miscellaneous/STM32L0_PWR.cs
            Registers.PowerControl.Define(this, 0xFCC00, name: "PWR_CR")
                .WithTaggedFlag("LPDS", 0)
                .WithTaggedFlag("PDDS", 1)
                .WithTaggedFlag("CWUF", 2)
                .WithTaggedFlag("CSBF", 3)
                .WithTaggedFlag("PVDE", 4)
                .WithEnumField<DoubleWordRegister, PvdLevelSelection>(5, 3, name: "PLS")
                .WithFlag(8, out disableBackupProtection, name: "DBP", changeCallback:(_, value) => { AccessRTC(value); })
                .WithTaggedFlag("FPDS", 9)
                .WithTaggedFlag("LPUDS", 10)
                .WithTaggedFlag("MRUDS", 11)
                .WithReservedBits(12, 1)
                .WithTaggedFlag("ADCD1", 13)
                .WithEnumField<DoubleWordRegister, RegulatorVoltageScalingOutputSelection>(14, 2, out vosValue, name: "VOS", writeCallback: (_, value) =>
                    {
                        if(value == RegulatorVoltageScalingOutputSelection.Reserved)
                        {
                            vosValue.Value = RegulatorVoltageScalingOutputSelection.ScaleMode3;
                        }
                    })
                .WithFlag(16, name: "ODEN", writeCallback: (_, value) =>
                    {
                        if(!value)
                        {
                            odswenValue.Value = false;
                        }

                        odrdyValue.Value = value;
                    })
                .WithFlag(17, out odswenValue, name: "ODSWEN", writeCallback: (_, value) => { odswrdyValue.Value = value; })
                .WithEnumField<DoubleWordRegister, UnderDriveEnableInStopMode>(18, 2, name: "UDEN")
                .WithReservedBits(20, 12);
                
            Registers.PowerControlStatus.Define(this, name: "PWR_CSR")
                .WithTaggedFlag("WUF", 0)
                .WithTaggedFlag("SBF", 1)
                .WithTaggedFlag("PVDO", 2)
                .WithTaggedFlag("BRR", 3)
                .WithReservedBits(4, 4)
                .WithTaggedFlag("EWUP", 8)
                .WithTaggedFlag("BER", 9)
                .WithReservedBits(10, 4)
                .WithTaggedFlag("VOSRDY", 14)
                .WithReservedBits(15, 1)
                .WithFlag(16, out odrdyValue, FieldMode.Read, name: "ODRDY")
                .WithFlag(17, out odswrdyValue, FieldMode.Read, name: "ODSWRDY")
                .WithEnumField<DoubleWordRegister, UnderDriveReady>(18, 2, FieldMode.Read | FieldMode.WriteOneToClear, name: "UDRDY")
                .WithReservedBits(20, 12);
        }

        private void AccessRTC(bool dbp)
        {
            this.Log(LogLevel.Debug, "TODO: AccessRTC: dbp {0}", dbp);
            // TODO:IMPLEMENT: We should control access to the RTC and RTC Backup registers and backup SRAM via a GPIO signal
        }

        public long Size => 0x400;

        public string Model { get; }

        private IEnumRegisterField<RegulatorVoltageScalingOutputSelection> vosValue;
        private IFlagRegisterField odswenValue;
        private IFlagRegisterField odrdyValue;
        private IFlagRegisterField odswrdyValue;
        private IFlagRegisterField disableBackupProtection;

        private enum Registers
        {
            PowerControl = 0x0,
            PowerControlStatus = 0x4
        }

        private enum PvdLevelSelection
        {
            V2_0,
            V2_1,
            V2_3,
            V2_5,
            V2_6,
            V2_7,
            V2_8,
            V2_9
        }

        private enum RegulatorVoltageScalingOutputSelection
        {
            Reserved,
            ScaleMode3,
            ScaleMode2,
            ScaleMode1
        }

        private enum UnderDriveEnableInStopMode
        {
            UnderDriveDisable,
            Reserved1,
            Reserved2,
            UnderDriveEnable
        }

        private enum UnderDriveReady
        {
            UnderDriveDisabled,
            Reserved1,
            Reserved2,
            UnderDriveActivated
        }
    }
}
