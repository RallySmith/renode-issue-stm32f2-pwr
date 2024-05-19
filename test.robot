*** Settings ***
Suite Setup                   Setup
Suite Teardown                Teardown
Test Setup                    Reset Emulation
Test Teardown                 Test Teardown
Resource                      ${RENODEKEYWORDS}

*** Variables ***
${SCRIPT}                     ${CURDIR}/test.resc
${UART}                       sysbus.uart4


*** Keywords ***
Load Script
    Execute Script            ${SCRIPT}
    Create Terminal Tester    ${UART}


*** Test Cases ***
Should Run Test Case
    Load Script
    Start Emulation
    Should Not Be In Log      pwr: Unhandled write to offset 0x0. Unhandled bits: [8] when writing value 0xFCD00. Tags: DBP (0x1).
    Should Not Be On Uart     FAIL:<BD_PROTECT(0):PWR_CR:DBP not updated> Line: 61, File: /home/jsmith/work/ecos/hg/ecospro/packages/hal/cortexm/stm32/stm32x0g_eval/current/tests/pwr.c
    Wait For Line On Uart     PASS:<STM32X0G-EVAL PWR test done>
    Wait For Line On Uart     EXIT:<done>

