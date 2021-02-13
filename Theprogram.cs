using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class methodhouse //this is a class that will house a number of methods
{
	public bool extcheck(string directori) //here's a method to check whether it's a .bars file or not
	{
		return (directori == ".bars") ? true : false;
	}

	public bool headercheck(byte[] data) //this is a method for checking the header if it's a BARS
    {
		if (data[0] == 66 && data[1] == 65 && data[2] == 82 && data[3] == 83) return true;
		else return false;
    }

	public bool extcheck(FileAttributes dir) //this one is a method for checking if the input is a directory or not
	{
		return ((dir & FileAttributes.Directory) == FileAttributes.Directory) ? true : false;
	}

	public List<string> getamtaname(ref byte[] data, ref string filenameext, out int i, ref int amtacount) //let's extract the contents of the .bars file by getting the names!
    {
		int offsetnamestart;
		int amtanumber;
		byte[] names;
		int j; //j is used to narrow the index in order to get the filenames
		string stringname;
		int character = 0;
		List<string> name = new List<string>();

		for (i = 0, amtanumber = 0; i < data.Length; i++)
        {
			if (data[i] == 65 && data[i + 1] == 77 && data[i + 2] == 84 && data[i + 3] == 65)
			{
				if (data[i + 7] == 5)
				{
					offsetnamestart = data[i + 36] + (int)36;
					names = new byte[] { }; //looking for a way to enable assigning array without changing array order

					for (j = 0; j < data[i + 8]; j++) //why data[i + 8]? apparently the Length of an AMTA array is located next to the version of AMTA (the data[i + 7]))
					{
						if (data[j + offsetnamestart + i] < 33 || data[j + offsetnamestart + i] > 126) break; //here to prevent any illegal characters to be encoded
						else
						{
							character++;
							if (data[i + j] == 65 && data[i + j + 1] == 77 && data[i + j + 2] == 84 && data[i + j + 3] == 65) character = 0 + 1;
						}
					}
					Array.Resize(ref names, character);

					for (j = 0; j < data[i + 8] && j < names.Length; j++)
					{
						names.SetValue(data[i + offsetnamestart + j], j);
					}
					stringname = Encoding.UTF8.GetString(names);
					name.Add(stringname);
					if (amtanumber < amtacount - 1)
                    {
						amtanumber++;
						continue;
					}
					else if (amtanumber == amtacount - 1) goto outside;
				}
				if (data[i + 7] != 5)
				{
					Console.Write("Even though {0} is a .bars file, the AMTA version seems not 5\nKeep in mind that this tool is ONLY support AMTA v5!", filenameext);
					break;
				}
			}
		}

		outside:  return name;
	}

	public List<byte[]> getbwavarray (ref byte[] data, out int i, ref int amtacount) //and also the BWAV files to write!
    {
		int k;
		byte[] bwav = new byte[] { };
		int bwavlength;
		int bwavstart = 0;
		int bwavend = 0;
		int bwavindex;
		int amtanumber;
		List<byte[]> bwavarray = new List<byte[]>();

		for (i = 0, amtanumber = 0; i < data.Length; i++)
        {
			if (data[i] == 66 && data[i + 1] == 87 && data[i + 2] == 65 && data[i + 3] == 86)
			{
				
				bwavstart = i;

				for (k = bwavstart + 4; k < data.Length; k++)
				{
					if (data[k] <= 255)
					{
						if (data[k] == 66 && data[k + 1] == 87 && data[k + 2] == 65 && data[k + 3] == 86) break;
						bwavend = k + 1;

						if (k == data.Length - 1) break;
					}
				}

				bwavlength = bwavend - bwavstart;

				Array.Resize(ref bwav, bwavlength);

				for (bwavindex = bwavstart; bwavindex < bwavend; bwavindex++)
				{
					bwav.SetValue(data[bwavindex], bwavindex - bwavstart);
				}
				
				bwavarray.Add(bwav);

				if (amtanumber < amtacount - 1)
				{
					amtanumber++;
					continue;
				}

				else if (amtanumber == amtacount - 1) goto outside;
			}
		}

		outside: return bwavarray;
	} 

	public List<int> getamtaoffset(ref int[] offsets, ref int amtacount) //a method to get offsets for AMTA, where the AMTA tag is located
    {
		int i, j, k;
		int amtanumber;
		int theinterger;
		string number;
		string[] arraynumber = new string[] { };
		string joined = "";
		List<int> amtaoffset = new List<int>();

		Array.Resize(ref arraynumber, 4);

		for (i = 0, amtanumber = 0; i < offsets.Length; i += 8)
        {
			for (j = i + 3, k = 0; j >= i; j--)
            {
				number = String.Format("{0:X2}", offsets[j]);
				arraynumber.SetValue(number, k);
				k++;
				if (k == j + 3)
				{
					k = 0;
					break;
				}
				joined = String.Join("", arraynumber);
			}

			theinterger = Convert.ToInt32(joined, 16);

			amtaoffset.Add(theinterger);

			if (amtanumber < amtacount)
			{
				amtanumber++;
				continue;
			}

			else if (amtanumber == amtacount - 1) goto result;
		}

		result: return amtaoffset;
	}

	public List<int> getbwavoffset(ref int[] offsets, ref int amtacount) //a method to get offsets for BWAV, where the BWAV file is located
    {
		int i, j, k;
		int amtanumber;
		int theinterger;
		string number;
		string[] arraynumber = new string[] { };
		string joined = "";
		List<int> bwavoffset = new List<int>();

		Array.Resize(ref arraynumber, 4);

		for (i = 4, amtanumber = 0; i < offsets.Length; i += 8)
		{
			for (j = i + 3, k = 0; j >= i; j--)
			{
				number = String.Format("{0:X2}", offsets[j]);  //Convert.ToString(offsets[j], 16);
				arraynumber.SetValue(number, k);
				k++;
				if (k == j + 3)
				{
					k = 0;
					break;
				}
				joined = String.Join("", arraynumber);
			}

			theinterger = Convert.ToInt32(joined, 16);

			bwavoffset.Add(theinterger);

			if (amtanumber < amtacount)
			{
				amtanumber++;
				continue;
			}

			else if (amtanumber == amtacount - 1) goto result;
		}

	result: return bwavoffset;
	}

	public List<string> getamtanamewithoffset(ref byte[] data, ref string filenameext, int i, ref int amtacount) //this is a method to get the AMTA tags based on the available offset
	{
		int offsetnamestart;
		int amtanumber;
		byte[] names;
		int j; //j is used to narrow the index in order to get the filenames
		string stringname;
		int character = 0;
		List<string> name = new List<string>();

		for (amtanumber = 0; amtanumber < amtacount; )
		{
			if (data[i] == 65 && data[i + 1] == 77 && data[i + 2] == 84 && data[i + 3] == 65)
			{
				if (data[i + 7] == 5)
				{
					offsetnamestart = data[i + 36] + (int)36;
					names = new byte[] { }; //looking for a way to enable assigning array without changing array order

					for (j = 0; j < data[i + 8]; j++) //why data[i + 8]? apparently the Length of an AMTA array is located next to the version of AMTA (the data[i + 7]))
					{
						if (data[j + offsetnamestart + i] < 33 || data[j + offsetnamestart + i] > 126) break; //here to prevent any illegal characters to be encoded
						else
						{
							character++;
							if (data[i + j] == 65 && data[i + j + 1] == 77 && data[i + j + 2] == 84 && data[i + j + 3] == 65) character = 0 + 1;
						}
					}
					Array.Resize(ref names, character);

					for (j = 0; j < data[i + 8] && j < names.Length; j++)
					{
						names.SetValue(data[i + offsetnamestart + j], j);
					}
					stringname = Encoding.UTF8.GetString(names);
					name.Add(stringname);
					if (amtanumber < amtacount - 1)
					{
						amtanumber++;
						continue;
					}
					else if (amtanumber == amtacount - 1) goto outside;
				}
				if (data[i + 7] != 5)
				{
					Console.Write("Even though {0} is a .bars file, the AMTA version seems not 5\nKeep in mind that this tool is ONLY support AMTA v5!", filenameext);
					break;
				}
			}
		}

	outside: return name;
	}

	public List<byte[]> getbwavarraywithoffset(ref byte[] data, int i, ref int amtacount) //this is a method to get the BWAV files based on the available offset
	{
		int k;
		byte[] bwav = new byte[] { };
		int bwavlength;
		int bwavstart = 0;
		int bwavend = 0;
		int bwavindex;
		int amtanumber;
		List<byte[]> bwavarray = new List<byte[]>();

		for (amtanumber = 0; amtanumber < amtacount;)
		{
			if (data[i] == 66 && data[i + 1] == 87 && data[i + 2] == 65 && data[i + 3] == 86)
			{

				bwavstart = i;

				for (k = bwavstart + 4; k < data.Length; k++)
				{
					if (data[k] <= 255)
					{
						if (data[k] == 66 && data[k + 1] == 87 && data[k + 2] == 65 && data[k + 3] == 86) break;
						bwavend = k + 1;

						if (k == data.Length - 1) break;
					}
				}

				bwavlength = bwavend - bwavstart;

				Array.Resize(ref bwav, bwavlength);

				for (bwavindex = bwavstart; bwavindex < bwavend; bwavindex++)
				{
					bwav.SetValue(data[bwavindex], bwavindex - bwavstart);
				}

				bwavarray.Add(bwav);

				if (amtanumber < amtacount - 1)
				{
					amtanumber++;
					continue;
				}

				else if (amtanumber == amtacount - 1) goto outside;
			}
		}

	outside: return bwavarray;
	}

	~methodhouse() //not sure if i called the destructor right
    {
		Console.WriteLine("the work is completed!");
    }
}

class theprogram
{
	private static FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
	{
		return attributes & ~attributesToRemove;
	}

	static void Main(string[] argh)
	{
		if (argh.Length < 1) //if there's no "arguments" to use for the usage, this will execute
        {
			Console.WriteLine("hold up, you give the code nothing");
			Console.WriteLine("Usage: \"barcsharp [barsfiledirectory]\" or \"barcsharp [barsfile]\" \n(without quotation marks [except it's a directory] and square brackets!)");
			Console.WriteLine("Example: barcsharp \"C:\\..\\Sound\\Resource\\ or barchsarp doubutsugo_base.bars");
			return;
        }
		FileAttributes dir = File.GetAttributes(argh[0]);

		if ((dir & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
        {
			dir = RemoveAttribute(dir, FileAttributes.ReadOnly);
			File.SetAttributes(argh[0], dir);
        }

		string ext = Path.GetExtension(argh[0]);
		string filenameext = Path.GetFileName(argh[0]);
		string filenameonly = Path.GetFileNameWithoutExtension(argh[0]);

		methodhouse obj = new methodhouse();

		if (obj.extcheck(ext)) //first, the code will check if it's a .bars file
		{
			byte[] data = File.ReadAllBytes(argh[0]);
			ext = Path.GetExtension(argh[0]);
			filenameext = Path.GetFileName(argh[0]);
			filenameonly = Path.GetFileNameWithoutExtension(argh[0]);

			if (obj.headercheck(data))
			{

				Console.WriteLine("\n{0} is a .bars file", filenameext);
				Console.WriteLine("the .bars file size (in B): {0:#.###}", data.Length);

				int amtacount = 0;
				int bwavcount = 0;

				for (int i = 0; i < data.Length; i++) //first things first, let's check how many AMTA and BWAV it has
                {
					if (data[i] == 65 && data[i + 1] == 77 && data[i + 2] == 84 && data[i + 3] == 65) amtacount++;
					if (data[i] == 66 && data[i + 1] == 87 && data[i + 2] == 65 && data[i + 3] == 86) bwavcount++;
				}

				List<string> name; //a List of filenames from AMTA
				List<byte[]> bwavarray; //a List of BWAV array that will be used for extraction

				if (amtacount == bwavcount) //this statement will be executed if the number of AMTA contained is equal to the number of BWAV contained
                {
					Directory.CreateDirectory($"BARcSharp-output\\{filenameonly}"); //automatically creating the directory if it's not exist
					int i;

					for (int amtanumber = 0; amtanumber < amtacount; amtanumber++)
                    {
						name = new List<string> (obj.getamtaname(ref data, ref filenameext, out i, ref amtacount));
						bwavarray = new List<byte[]> (obj.getbwavarray(ref data, out i, ref amtacount));
						Console.WriteLine("Extracting {0}.bwav ...", name[amtanumber]);
						File.WriteAllBytes($"BARcSharp-output\\{filenameonly}\\{name[amtanumber]}.bwav", bwavarray[amtanumber]); //start the extraction
						if (amtanumber == amtacount - 1)
                        {
							Console.WriteLine("All Done!");
							break;
                        }
					}

				}

				else if (amtacount > bwavcount) //meanwhile this statement will be executed if the number of AMTA contained is higher than the number of BWAV contained
				{

					Directory.CreateDirectory($"BARcSharp-output\\{filenameonly}");

					List<int> amtaoffset; //an int typed List that will contain AMTA offsets
					List<int> bwavoffset; //an int typed List that will contain BWAV offsets
					int i;
					int[] offsets = new int[] { };
					int offsetlength = (amtacount * 4) * 2;
					int offsetstart = 16 + (amtacount * 4);

					Array.Resize(ref offsets, offsetlength);

					for (i = offsetstart; i < offsetlength * 2; i++) //offset checking to get the offset values
                    {
						if (data[i] == 65 && data[i + 1] == 77 && data[i + 2] == 84 && data[i + 3] == 65) break; //if the first AMTA tag is found, then it'll stop
						offsets.SetValue(data[i], i - offsetstart);
					}

					amtaoffset = new List<int>(obj.getamtaoffset(ref offsets, ref amtacount));
					bwavoffset = new List<int>(obj.getbwavoffset(ref offsets, ref amtacount));

					for (int amtanumber = 0; amtanumber < amtacount; amtanumber++)
					{
						name = new List<string>(obj.getamtanamewithoffset(ref data, ref filenameext, amtaoffset[amtanumber], ref amtacount));
						bwavarray = new List<byte[]>(obj.getbwavarraywithoffset(ref data, bwavoffset[amtanumber], ref amtacount));
						Console.WriteLine("Extracting {0}.bwav ...", name[amtanumber]);
						File.WriteAllBytes($"BARcSharp-output\\{filenameonly}\\{name[amtanumber]}.bwav", bwavarray[amtanumber]);
						if (amtanumber == amtacount - 1)
						{
							Console.WriteLine("All Done!");
							break;
						}
					}
				}
			}

			if (ext != ".bars") Console.WriteLine("{0} IS NOT A .bars FILE!", filenameext); //otherwise, this happens

			else if (!obj.headercheck(data)) Console.WriteLine("are you kidding me? the header in {0} is NOT in BARS!", filenameext);
		}

		if (obj.extcheck(dir)) //meanwhile if the input is a directory, this happens
        {
			List<string> ext2 = new List<string>(Directory.GetFiles(argh[0], ext));

			foreach (string bla in ext2)
            {
				byte[] data = File.ReadAllBytes(bla);
				ext = Path.GetExtension(bla);
				filenameext = Path.GetFileName(bla);
				filenameonly = Path.GetFileNameWithoutExtension(bla);

				if (obj.headercheck(data))
				{

					Console.WriteLine("\n{0} is a .bars file", filenameext);
					Console.WriteLine("the .bars file size (in B): {0:#.###}", data.Length);

					int amtacount = 0;
					int bwavcount = 0;

					for (int i = 0; i < data.Length; i++) //first things first, let's check how many AMTA and BWAV it has
					{
						if (data[i] == 65 && data[i + 1] == 77 && data[i + 2] == 84 && data[i + 3] == 65) amtacount++;
						if (data[i] == 66 && data[i + 1] == 87 && data[i + 2] == 65 && data[i + 3] == 86) bwavcount++;
					}

					List<string> name;
					List<byte[]> bwavarray;

					if (amtacount == bwavcount)
					{
						Directory.CreateDirectory($"BARcSharp-output\\{filenameonly}");
						int i;

						for (int amtanumber = 0; amtanumber < amtacount; amtanumber++)
						{
							name = new List<string>(obj.getamtaname(ref data, ref filenameext, out i, ref amtacount));
							bwavarray = new List<byte[]>(obj.getbwavarray(ref data, out i, ref amtacount));
							Console.WriteLine("Extracting {0}.bwav ...", name[amtanumber]);
							File.WriteAllBytes($"BARcSharp-output\\{filenameonly}\\{name[amtanumber]}.bwav", bwavarray[amtanumber]);
							if (amtanumber == amtacount - 1)
							{
								Console.WriteLine("All Done!");
								break;
							}
						}

					}

					else if (amtacount > bwavcount)
					{

						Directory.CreateDirectory($"BARcSharp-output\\{filenameonly}");

						List<int> amtaoffset;
						List<int> bwavoffset;
						int i;
						int[] offsets = new int[] { };
						int offsetlength = (amtacount * 4) * 2;
						int offsetstart = 16 + (amtacount * 4);

						Array.Resize(ref offsets, offsetlength);

						for (i = offsetstart; i < offsetlength * 2; i++)
						{
							if (data[i] == 65 && data[i + 1] == 77 && data[i + 2] == 84 && data[i + 3] == 65) break;
							offsets.SetValue(data[i], i - offsetstart);
						}

						amtaoffset = new List<int>(obj.getamtaoffset(ref offsets, ref amtacount));
						bwavoffset = new List<int>(obj.getbwavoffset(ref offsets, ref amtacount));

						for (int amtanumber = 0; amtanumber < amtacount; amtanumber++)
						{
							name = new List<string>(obj.getamtanamewithoffset(ref data, ref filenameext, amtaoffset[amtanumber], ref amtacount));
							bwavarray = new List<byte[]>(obj.getbwavarraywithoffset(ref data, bwavoffset[amtanumber], ref amtacount));
							Console.WriteLine("Extracting {0}.bwav ...", name[amtanumber]);
							File.WriteAllBytes($"BARcSharp-output\\{filenameonly}\\{name[amtanumber]}.bwav", bwavarray[amtanumber]);
							if (amtanumber == amtacount - 1)
							{
								Console.WriteLine("All Done!");
								break;
							}
						}
					}
				}

				if (ext != ".bars") Console.WriteLine("{0} IS NOT A .bars FILE!", filenameext); //otherwise, this happens

				else if (!obj.headercheck(data)) Console.WriteLine("are you kidding me? the header in {0} is NOT in BARS!", filenameext);
			}
		}

		File.SetAttributes(argh[0], File.GetAttributes(argh[0]) | FileAttributes.ReadOnly);
	}
}