# BARcSharp
A tool to extract the BWAV files from Switch .bars files (for AMTA v5 only, particularly found in a game Animal Crossing New Horizons), 100% written in C# !

Just my "casual" take on a tool development, inspired from this tool: https://github.com/jackz314/bars-to-bwav

Although the tool itself may be far from perfect, it should be able to extract most Switch .bars files

# How to use

-Since the release in a .zip file, extract all the contents first.

-Next, open the command prompt, locate the BarcSharp.exe file, and then type "barcsharp [barsfiledirectory]" or "barcsharp [barsfile]" (without quotation marks [except it's a directory] and square brackets!)

Example: barcsharp "C:\\..\Sound\\Resource\\" or barchsarp doubutsugo_base.bars

-The output file(s) would be somewhere in <executablelocation>\\BARcSharp-output\\<barsfilename>

# What's next after you get the .bwav files?

you can play the .bwav files using a plugin called vgmstream for a foobar2000 player: https://www.foobar2000.org/components/view/foo_input_vgmstream , and you can even convert them to .wav files
