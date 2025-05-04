# AshFinances
<img src="res/icon.png" width="200"/>

AshFinances is a console application powered by [AshConsoleGraphics](https://github.com/siljamdev/AshConsoleGraphics) that will help you keep track of money and transactions.

## Usage
On the first boot, you will be prompted to input your starting balance. All numbers are floating point numbers.  
Then, you have access to the menu, where you move around with arrows and click buttons with enter. You can go back with escape  
You can add a transaction in the current day, in a past day, or see stats and transactions.  
All dates are in the format dd/mm/yyyy, if you use any other format you should get a long holiday and spend some time thinking where it all went wrong.

## Installation
You can install AshFinances for windows x64 or x86 with the portable executable.
There are no pre built executables for linux or mac but it should be compatible, you could build it.

## Internal operation
The app folder is in %appdata%/ashproject/ashfinances, and in there there is an [AshFile](https://github.com/Dumbelfo08/AshLib) (days.ash) that holds all information.
