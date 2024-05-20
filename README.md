# STM32F2 PWR

The provided `ecos_stm3220g_eval_plf_pwr` ELF file executes on a STM3220G-EVAL based
platform accessing the PWR_CR register to check for DBP flag operation.

Currently this is a **mirror** (and not a fork) of the
renode-issue-reproduction-template because github limits users to a
single fork.

## branches

| Branch  | Description
|:--------|:-------------------------------------------------------------------
| `main`  | test executes against latest and stable renode worlds
| `fixed` | updated model to allow successful execution of the application

## github workflow

(**INVESTIGATING**) Currently there is some issue with the github
workflow execution for the "main" branch where it reports success
(green), where a local execution of the test against the 1.15.0
triggers the expected failure:

```
renode-issue-stm32f2-pwr$ ~/work/emulation/renode_1.15.0_portable/renode-test test.robot
Preparing suites
Started Renode instance on port 49152; pid 1763610
Starting suites
Running test.robot
+++++ Starting test 'test.Should Run Test Case'
!!!!! Emulation's state saved to "/fastspace/github_issue_merging/mirrors/renode-issue-stm32f2-pwr//snapshots/test.Should_Run_Test_Case.fail0.save"
!!!!! Log saved to "/fastspace/github_issue_merging/mirrors/renode-issue-stm32f2-pwr//logs/test.Should_Run_Test_Case.fail0.log"
+++++ Finished test 'test.Should Run Test Case' in 3.49 seconds with status failed
      ╔═
      ║ KeywordException: Unexpected line detected in the log: pwr: Unhandled write to offset 0x0. Unhandled bits: [8] when writing value 0xFCD00. Tags: DBP (0x1).
      ║ 
      ╚═
Suite test.robot failed in 4.16 seconds.
Cleaning up suites
Closing Renode pid 1763610
Aggregating all robot results
Output:  /fastspace/github_issue_merging/mirrors/renode-issue-stm32f2-pwr/robot_output.xml
Log:     /fastspace/github_issue_merging/mirrors/renode-issue-stm32f2-pwr/log.html
Report:  /fastspace/github_issue_merging/mirrors/renode-issue-stm32f2-pwr/report.html
Some tests failed :( See the list of failed tests below and logs for details!
Failed robot critical tests:
        1. test.Should Run Test Case
------
```
