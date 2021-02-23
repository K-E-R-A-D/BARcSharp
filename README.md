# BARcSharp
A tool to extract the BWAV files from Switch .bars files (for AMTA v5 only, particularly found in a game Animal Crossing New Horizons), 100% written in C# !

Just my "casual" take on a tool development, inspired from this tool: https://github.com/jackz314/bars-to-bwav

Now that the tool itself should be close to perfect, it should be able to extract most (if not all) Switch .bars files, but i'll try to keep improving them sometimes

# How to use

-Since the release in a .zip file, extract all the contents first.

-Next, open the command prompt, locate the BarcSharp.exe file, and then type "barcsharp [barsfiledirectory]" or "barcsharp [barsfile]" (without quotation marks [except it's a directory] and square brackets!)

Example: barcsharp "C:\\..\Sound\\Resource\\" or barchsarp doubutsugo_base.bars

-The output file(s) would be somewhere in [executablelocation]\\BARcSharp-output\\[barsfilename]\\

# What's next after you get the .bwav files?

-You can try play the .bwav files using a plugin called vgmstream for a foobar2000 player: https://www.foobar2000.org/components/view/foo_input_vgmstream , and you can even convert them to .wav files using that player as well.

# A warning that must not be ignored

-DO NOT attempt to move the executable file alone without the DLLs. Those DLLs are the working components in order to run the program. The same applies to when you extract the contents from the .zip file, DO NOT just extract the executable alone without the DLLs.
