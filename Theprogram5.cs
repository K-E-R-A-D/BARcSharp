using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class method_house //this is a class that will house a number of methods
{
	public bool ext_check(string directori) //here's a method to check whether it's a .bars file or not
	{
		return (directori == ".bars") ? true : false;
	}

	public bool header_check(byte[] data) //this is a method for checking the header if it's a BARS
	{
		if (data[0] == 66 && data[1] == 65 && data[2] == 82 && data[3] == 83) return true;
		else return false;
	}

	public bool ext_check(FileAttributes dir) //this one is a method for checking if the input is a directory or not
	{
		return ((dir & FileAttributes.Directory) == FileAttributes.Directory) ? true : false;
	}

	public List<int> get_amta_offset(ref int[] offsets, ref uint amta_count) //a method to get offsets for AMTA, where the AMTA tag is located
	{
		int i, j, k;
		int amta_number;
		int the_interger;
		string number;
		string[] array_number = new string[] { };
		string joined = "";
		List<int> amta_offset = new List<int>();

		Array.Resize(ref array_number, 4);

		for (i = 0, amta_number = 0; i < offsets.Length; i += 8)
		{
			for (j = i + 3, k = 0; j >= i; j--)
			{
				number = String.Format("{0:X2}", offsets[j]);
				array_number.SetValue(number, k);
				k++;
				if (k == j + 3)
				{
					k = 0;
					break;
				}
				joined = String.Join("", array_number);
			}

			the_interger = Convert.ToInt32(joined, 16);

			amta_offset.Add(the_interger);

			if (amta_number < amta_count)
			{
				amta_number++;
				continue;
			}

			else if (amta_number == amta_count - 1) goto result;
		}

	result: return amta_offset;
	}

	public List<int> get_bwav_offset(ref int[] offsets, ref uint amta_count) //a method to get offsets for BWAV, where the BWAV file is located
	{
		int i, j, k;
		int amta_number;
		int the_interger;
		string number;
		string[] array_number = new string[] { };
		string joined = "";
		List<int> bwav_offset = new List<int>();

		Array.Resize(ref array_number, 4);

		for (i = 4, amta_number = 0; i < offsets.Length; i += 8)
		{
			for (j = i + 3, k = 0; j >= i; j--)
			{
				number = String.Format("{0:X2}", offsets[j]);
				array_number.SetValue(number, k);
				k++;
				if (k == j + 3)
				{
					k = 0;
					break;
				}
				joined = String.Join("", array_number);
			}

			the_interger = Convert.ToInt32(joined, 16);

			bwav_offset.Add(the_interger);

			if (amta_number < amta_count)
			{
				amta_number++;
				continue;
			}

			else if (amta_number == amta_count - 1) goto result;
		}

	result: return bwav_offset;
	}

	public static uint get_name_from_amta_array(ref uint[] amta_offset_name_start)
	{
		string number = "";
		string joined = "";
		uint name_start;
		string[] array_number = new string[4];

		for (int i = 0; i < amta_offset_name_start.Length; i++)
		{
			number = String.Format("{0:X2}", amta_offset_name_start[i]);
			array_number.SetValue(number, i);
		}

		joined = String.Join("", array_number);

		name_start = Convert.ToUInt32(joined, 16);

		return name_start;
	}

	public static uint get_amta_length(ref uint[] amta_length_container)
	{
		string number = "";
		string joined = "";
		uint amta_length;
		string[] array_number = new string[4];

		for (int i = 0; i < amta_length_container.Length; i++)
		{
			number = String.Format("{0:X2}", amta_length_container[i]);
			array_number.SetValue(number, i);
		}

		joined = String.Join("", array_number);

		amta_length = Convert.ToUInt32(joined, 16);

		return amta_length;
	}

	public List<string> get_amta_name_with_offset(ref byte[] data, ref string file_name_ext, ref List<int> amta_offset, ref uint amta_count) //this is a method to get the AMTA tags based on the available offset
	{
		uint offset_name_start, amta_length;
		uint[] amta_length_container = new uint[4];
		uint[] amta_offset_name_start = new uint[4];
		int amta_number;
		byte[] names;
		int j; //j is used to narrow the index in order to get the filenames
		string stringname;
		int character = 0;
		List<string> name = new List<string>();

		for (amta_number = 0; amta_number < amta_count;)
		{
			int i = amta_offset[amta_number];

			if (data[i] == 65 && data[i + 1] == 77 && data[i + 2] == 84 && data[i + 3] == 65)
			{
				if (data[i + 7] == 5)
				{

					int index_inc, index_dec;

					for (index_inc = 0, index_dec = 11; index_inc <= 3 && index_dec >= 8; index_inc++, index_dec--)
					{
						amta_length_container.SetValue(data[i + index_dec], index_inc);
					}

					for (index_inc = 0, index_dec = 39; index_inc <= 3 && index_dec >= 36; index_inc++, index_dec--)
					{
						amta_offset_name_start.SetValue(data[i + index_dec], index_inc);
					}

					offset_name_start = method_house.get_name_from_amta_array(ref amta_offset_name_start) + (uint)36;
					names = new byte[] { };
					amta_length = method_house.get_amta_length(ref amta_length_container);

					for (j = 0; j < amta_length; j++)
					{
						if (data[j + offset_name_start + i] < 33 || data[j + offset_name_start + i] > 126) break; //here to prevent any illegal characters to be encoded
						else
						{
							character++;
							if (data[i + j] == 65 && data[i + j + 1] == 77 && data[i + j + 2] == 84 && data[i + j + 3] == 65) character = 0 + 1;
						}
					}
					Array.Resize(ref names, character);

					for (j = 0; j < amta_length && j < names.Length; j++)
					{
						names.SetValue(data[i + offset_name_start + j], j);
					}
					stringname = Encoding.UTF8.GetString(names);
					name.Add(stringname);
					if (amta_number < amta_count - 1)
					{
						amta_number++;
						continue;
					}
					else if (amta_number == amta_count - 1) goto outside;
				}
				if (data[i + 7] != 5)
				{
					Console.Write("Even though {0} is a .bars file, the AMTA version seems not 5.\nKeep in mind that this tool is ONLY support AMTA v5!", file_name_ext);
					break;
				}
			}
		}

	outside: return name;
	}

	public List<byte[]> get_bwav_array_with_offset(ref byte[] data, ref List<int> bwav_offset, ref uint amta_count) //this is a method to get the BWAV files based on the available offset
	{
		int k;
		byte[] bwav;
		int bwavlength;
		int bwavstart = 0;
		int bwavend = 0;
		int bwavindex;
		int amta_number;
		List<byte[]> bwav_array = new List<byte[]>();

		for (amta_number = 0; amta_number < amta_count;)
		{
			int i = bwav_offset[amta_number];

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

				bwav = new byte[] { };

				Array.Resize(ref bwav, bwavlength);

				for (bwavindex = bwavstart; bwavindex < bwavend; bwavindex++)
				{
					bwav.SetValue(data[bwavindex], bwavindex - bwavstart);
				}

				bwav_array.Add(bwav);

				if (amta_number < amta_count - 1)
				{
					amta_number++;
					continue;
				}

				else if (amta_number == amta_count - 1) goto outside;
			}
		}

	outside: return bwav_array;
	}

	public uint get_amta_count(ref uint[] the_amta_count)
	{
		string number = "";
		string joined = "";
		uint the_amta_number;
		string[] array_number = new string[4];

		for (int i = 0; i < the_amta_count.Length; i++)
		{
			number = String.Format("{0:X2}", the_amta_count[i]);
			array_number.SetValue(number, i);
		}

		joined = String.Join("", array_number);

		the_amta_number = Convert.ToUInt32(joined, 16);

		return the_amta_number;
	}

	~method_house() //not sure if i called the destructor right
	{
		Console.WriteLine("the work is completed!");
	}
}

class the_program
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

		FileAttributes dir;

		try
		{
			dir = File.GetAttributes(argh[0]);
		}

		catch (FileNotFoundException)
		{
			Console.WriteLine("File does not exist!");
			return;
		}

		dir = File.GetAttributes(argh[0]);

		if ((dir & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
		{
			dir = RemoveAttribute(dir, FileAttributes.ReadOnly);
			File.SetAttributes(argh[0], dir);
		}

		string file_name_ext = Path.GetFileName(argh[0]);
		string file_name_only;
		string dir_ext = Path.GetFullPath(argh[0]);
		string ext = Path.GetExtension(dir_ext);

		method_house obj = new method_house();

		if (!obj.ext_check(dir))
		{
			if (obj.ext_check(ext)) //first, the code will check if it's a .bars file
			{
				byte[] data = File.ReadAllBytes(argh[0]);
				ext = Path.GetExtension(argh[0]);
				file_name_ext = Path.GetFileName(argh[0]);
				file_name_only = Path.GetFileNameWithoutExtension(argh[0]);

				if (obj.header_check(data))
				{

					Console.WriteLine("\n{0} is a .bars file", file_name_ext);
					Console.WriteLine("the .bars file size (in B): {0}\n", data.Length);

					uint amta_count;
					int bwav_count = 0;
					int index_inc, index_dec;

					uint[] the_amta_count = new uint[4];

					for (index_inc = 0, index_dec = 15; index_inc <= 3 && index_dec >= 12; index_inc++, index_dec--)
					{
						the_amta_count.SetValue(data[index_dec], index_inc);
					}

					amta_count = obj.get_amta_count(ref the_amta_count);

					for (int i = 0; i < data.Length; i++) //first things first, let's check how many BWAV it has
					{
						if (data[i] == 66 && data[i + 1] == 87 && data[i + 2] == 65 && data[i + 3] == 86) bwav_count++;
					}

					List<string> name; //a List of filenames from AMTA
					List<byte[]> bwav_array; //a List of BWAV array that will be used for extraction

					if (amta_count >= bwav_count) //meanwhile this statement will be executed if the number of AMTA contained is higher than the number of BWAV contained
					{

						Directory.CreateDirectory($"BARcSharp-output\\{file_name_only}");

						List<int> amta_offset; //an int typed List that will contain AMTA offsets
						List<int> bwav_offset; //an int typed List that will contain BWAV offsets
						int i;
						int[] offsets = new int[] { };
						int offset_length = (int)(amta_count * 4) * 2;
						int offset_start = (int)(16 + (amta_count * 4));

						Array.Resize(ref offsets, offset_length);

						for (i = offset_start; i < offset_length * 8; i++) //offset checking to get the offset values
						{
							if (data[i] == 65 && data[i + 1] == 77 && data[i + 2] == 84 && data[i + 3] == 65) break; //if the first AMTA tag is found, then it'll stop
							offsets.SetValue(data[i], i - offset_start);
						}

						amta_offset = new List<int>(obj.get_amta_offset(ref offsets, ref amta_count));
						bwav_offset = new List<int>(obj.get_bwav_offset(ref offsets, ref amta_count));

						name = new List<string>(obj.get_amta_name_with_offset(ref data, ref file_name_ext, ref amta_offset, ref amta_count));
						bwav_array = new List<byte[]>(obj.get_bwav_array_with_offset(ref data, ref bwav_offset, ref amta_count));

						for (int amta_number = 0; amta_number < amta_count; amta_number++)
						{
							Console.WriteLine("Extracting {0}.bwav ... ", name[amta_number]);
							File.WriteAllBytes($"BARcSharp-output\\{file_name_only}\\{name[amta_number]}.bwav", bwav_array[amta_number]);

							if (amta_number == amta_count - 1)
							{
								Console.WriteLine("All Done!");
								break;
							}
						}
					}

					else Console.WriteLine("AMTA is less than BWAV? Is it possible?");
				}

				else if (!obj.header_check(data)) Console.WriteLine("are you kidding me? the header in {0} is NOT in BARS!", file_name_ext);
			}

			else if (!obj.ext_check(ext)) Console.WriteLine("{0} IS NOT A .bars FILE!", file_name_ext); //otherwise, this happens
		}

		if (obj.ext_check(dir)) //meanwhile if the input is a directory, this happens. the process is technically the same though
		{
			List<string> ext2 = new List<string>(Directory.GetFiles(dir_ext, "*.bars"));

			if (ext2.Count == 0) Console.WriteLine($"No .bars file is found in {dir_ext}.\nAre you sure the directory (NOT its subdirectory) contains .bars files?");

			else
			{
				foreach (string bla in ext2)
				{
					byte[] data = File.ReadAllBytes(bla);
					ext = Path.GetExtension(bla);
					file_name_ext = Path.GetFileName(bla);
					file_name_only = Path.GetFileNameWithoutExtension(bla);

					if (obj.header_check(data))
					{

						Console.WriteLine("\n{0} is a .bars file", file_name_ext);
						Console.WriteLine("the .bars file size (in B): {0:#.###}\n", data.Length);

						uint amta_count;
						int bwav_count = 0;

						int index_inc, index_dec;

						uint[] the_amta_count = new uint[4];

						for (index_inc = 0, index_dec = 15; index_inc <= 3 && index_dec >= 12; index_inc++, index_dec--)
                        {
							the_amta_count.SetValue(data[index_dec], index_inc);
						}

						amta_count = obj.get_amta_count(ref the_amta_count);

						for (int i = 0; i < data.Length; i++) //first things first, let's check how many AMTA and BWAV it has
						{
							if (data[i] == 66 && data[i + 1] == 87 && data[i + 2] == 65 && data[i + 3] == 86) bwav_count++;
						}

						List<string> name;
						List<byte[]> bwav_array;

						if (amta_count >= bwav_count)
						{

							Directory.CreateDirectory($"BARcSharp-output\\{file_name_only}");

							List<int> amta_offset;
							List<int> bwav_offset;
							int i;
							int[] offsets = new int[] { };
							int offset_length = (int)(amta_count * 4) * 2;
							int offset_start = (int)(16 + (amta_count * 4));

							Array.Resize(ref offsets, offset_length);

							for (i = offset_start; i < offset_length * 8; i++)
							{
								if (data[i] == 65 && data[i + 1] == 77 && data[i + 2] == 84 && data[i + 3] == 65) break;
								offsets.SetValue(data[i], i - offset_start);
							}

							amta_offset = new List<int>(obj.get_amta_offset(ref offsets, ref amta_count));
							bwav_offset = new List<int>(obj.get_bwav_offset(ref offsets, ref amta_count));

							name = new List<string>(obj.get_amta_name_with_offset(ref data, ref file_name_ext, ref amta_offset, ref amta_count));
							bwav_array = new List<byte[]>(obj.get_bwav_array_with_offset(ref data, ref bwav_offset, ref amta_count));

							for (int amta_number = 0; amta_number < amta_count; amta_number++)
							{
								Console.WriteLine("Extracting {0}.bwav ... ", name[amta_number]);
								File.WriteAllBytes($"BARcSharp-output\\{file_name_only}\\{name[amta_number]}.bwav", bwav_array[amta_number]);

								if (amta_number == amta_count - 1)
								{
									Console.WriteLine("All Done!");
									break;
								}
							}
						}
					}

					else if (!obj.header_check(data)) Console.WriteLine("are you kidding me? the header in {0} is NOT in BARS!", file_name_ext);
				}
			}
		}

		File.SetAttributes(argh[0], File.GetAttributes(argh[0]) | FileAttributes.ReadOnly);
	}
}