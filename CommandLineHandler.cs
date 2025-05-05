public static class CommandLineHandler{
	
	public static int handle(string[] args){
		
		bool quiet = false;
		int clp = 0;
		float? number = null;
		string? category = null;
		string? description = null;
		DateOnly? date = null;
		
		while(clp < args.Length){
			switch(args[clp].ToLower()){
				case "-q":
				case "--quiet":
					quiet = true;
					break;
					
				case "-v":
				case "--version":
					Console.WriteLine("AshFinances v" + Finances.version);
					Console.WriteLine("Made sy Siljam");
					return 0;
					
				case "-h":
				case "--help":
					displayHelp();
					return 0;
					
				case "-t":
				case "--transaction":
					clp++;
					if(!(clp < args.Length)){
						Console.Error.WriteLine("A number was expected after '" + args[clp - 1] + "', instead there is nothing.");
						return 3;
					}
					if(!float.TryParse(args[clp], out float f)){
						Console.Error.WriteLine("A number was expected after '" + args[clp - 1] + "', instead there is '" + args[clp] + "'.");
						return 3;
					}
					if(f == 0f){
						Console.Error.WriteLine("The number '" + args[clp] + "' cannot be 0.");
						return 4;
					}
					number ??= f;
					break;
					
				case "-p":
				case "--past":
					clp++;
					if(!(clp < args.Length)){
						Console.Error.WriteLine("A number was expected after '" + args[clp - 1] + "', instead there is nothing.");
						return 3;
					}
					if(!float.TryParse(args[clp], out float f2)){
						Console.Error.WriteLine("A number was expected after '" + args[clp - 1] + "', instead there is '" + args[clp] + "'.");
						return 3;
					}
					if(f2 == 0f){
						Console.Error.WriteLine("The number '" + args[clp] + "' cannot be 0.");
						return 4;
					}
					number ??= f2;
					
					clp++;
					if(!(clp < args.Length)){
						Console.Error.WriteLine("A date was expected after '" + args[clp - 1] + "', instead there is nothing.");
						return 3;
					}
					if(!args[clp].TryParseDate(out DateOnly d)){
						Console.Error.WriteLine("A date in dd/mm/yyyy format was expected after '" + args[clp - 1] + "', instead there is '" + args[clp] + "'.");
						return 3;
					}
					if(d >= Extensions.Today){
						Console.Error.WriteLine("The date '" + args[clp] + "' cannot be today or the future.");
						return 4;
					}
					date ??= d;
					break;
					
				case "-c":
				case "--category":
					clp++;
					if(!(clp < args.Length)){
						Console.Error.WriteLine("A string(category name) was expected after '" + args[clp - 1] + "', instead there is nothing.");
						return 3;
					}
					category ??= args[clp];
					break;
					
				case "-d":
				case "--description":
					clp++;
					if(!(clp < args.Length)){
						Console.Error.WriteLine("A string(description) was expected after '" + args[clp - 1] + "', instead there is nothing.");
						return 3;
					}
					description ??= args[clp];
					break;
					
				default:
					Console.Error.WriteLine("Unknown flag '" + args[clp] + "'. Type -h for help.");
					return 2;
			}
			clp++;
		}
		
		if(date is not null && number is not null){
			Finances.addTransaction((DateOnly) date, (float) number, category, description);
			if(!quiet){
				Console.WriteLine("Added a transaction on the " + date + " succesfully.");
			}
			return 0;
		}else if(number is not null){
			Finances.addTransactionToday((float) number, category, description);
			if(!quiet){
				Console.WriteLine("Added a transaction today succesfully.");
			}
			return 0;
		}
		
		Console.Error.WriteLine("Not enough options for operation.");
		return 4;
	}
	
	static void displayHelp(){
		Console.WriteLine("Description");
		Console.WriteLine("  AshFinances is an interactive console application to manage money. It has a little CLI for adding transactions things.");
		Console.WriteLine();
		Console.WriteLine("CLI Usage");
		Console.WriteLine("  ashfinances <flags>");
		Console.WriteLine();
		Console.WriteLine("List of flags");
		Console.WriteLine("  -h, --help                        Outputs the help menu");
		Console.WriteLine("  -v, --version                     Outputs the current version menu");
		Console.WriteLine("  -q, --quiet                       Silences all non-error output");
		Console.WriteLine("  -t, --transaction <number>        Specifies the quantity of a transaction");
		Console.WriteLine("  -p, --past <number> <date>        Specifies the quantity and date of a transaction in the past");
		Console.WriteLine("  -c, --category <name>             Specifies the category. Optional");
		Console.WriteLine("  -d, --description <desc>          Specifies the description. Optional");
		Console.WriteLine();
		Console.WriteLine("Example");
		Console.WriteLine("  ashfinances -t 10.5 -c food");
	}
}