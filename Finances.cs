global using System;
using AshLib;
using AshLib.Folders;
using AshLib.AshFiles;

static class Finances{
	static Dependencies dep = null!;
	
	static Dictionary<DateOnly, Day> days = null!;
	
	public static AshFile config = null!;
	static AshFile daysFile = null!;
	
	public const string version = "1.1.0";
	
	public static int Main(string[] args){
		if(args.Length > 0){
			return CommandLineHandler.handle(args);
		}
		
		if(!Extensions.isConsoleInteractive()){
			Console.Error.WriteLine("This application needs an interactive console to be run.");
			Console.Error.WriteLine("Use -h for CLI help.");
			return 1;
		}
		
		initialize();
		
		load(findLatest());
		
		Console.Clear();
		
		if(days.Count == 0){
			Screens.doFirstOpen();
		}
		
		updateCurrentBalance();
		
		Screens.mainScreen();
		
		return 0;
	}
	
	static void initialize(){
		string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		dep = new Dependencies(appDataPath + "/ashproject/ashfinances", true, null, null);
		config = dep.config;
		
		days = new Dictionary<DateOnly, Day>();
		daysFile = dep.ReadAshFile("days.ash");
		
		Palette.initialize();
	}
	
	static DateOnly? findLatest(){
		DateOnly? l = null;
		foreach(var kvp in daysFile.data){
			string[] p = kvp.Key.Split(".");
			if(p.Length == 2 && p[1] == "s" && p[0].TryParseDate(out DateOnly d) && (l is null || d > l)){
				l = d;
			}
		}
		return l;
	}
	
	static DateOnly? findPrevious(DateOnly dn){
		DateOnly? l = null;
		DateOnly today = Extensions.Today;
		foreach(var kvp in daysFile.data){
			string[] p = kvp.Key.Split(".");
			if(p.Length == 2 && p[1] == "s" && p[0].TryParseDate(out DateOnly d) && ((l is null && d < dn) || (d < dn && d > l))){
				l = d;
				if(d.AddDays(1) == dn){
					return l;
				}
			}
		}
		return l;
	}
	
	static DateOnly? findAfter(DateOnly dn){
		DateOnly? l = null;
		foreach(var kvp in daysFile.data){
			string[] p = kvp.Key.Split(".");
			if(p.Length == 2 && p[1] == "s" && p[0].TryParseDate(out DateOnly d) && ((l is null && d > dn) || (d > dn && d < l))){
				l = d;
				if(d.AddDays(-1) == dn){
					return l;
				}
			}
		}
		return l;
	}
	
	static void load(DateOnly? dn){
		if(dn is null || days.ContainsKey((DateOnly) dn)){
			return;
		}
		
		DateOnly d = (DateOnly) dn;
		
		string n = d.ToStringDate();
		if(!daysFile.CanGetCamp(n + ".s", out float s)){
			return;
		}
		
		List<Transaction> tl = new();
		
		int t = 0;
		while(daysFile.CanGetCamp(n + "." + t, out float tv)){
			tl.Add(new Transaction(tv,
				daysFile.GetCampOrDefault<string?>(n + "." + t + ".c", null),
				daysFile.GetCampOrDefault<string?>(n + "." + t + ".d", null)));
			
			t++;
		}
		
		days[d] = new Day(s, tl);
	}
	
	public static Day[] requestDays(DateOnly start, int num){
		DateOnly end = start.AddDays(num);
		List<DateOnly> daysl = new List<DateOnly>();
		
		daysl.Add(start);
		
		DateOnly? dn = findAfter(start);
		while(dn is not null && dn! < end){
			DateOnly d = (DateOnly) dn;
			daysl.Add(d);
			dn = findAfter(d);
		}
		
		foreach(DateOnly dt in daysl){
			load(dt);
		}
		
		if(daysl.Count == 1 && !days.ContainsKey(start)){
			DateOnly? prev = findPrevious(start);
			load(prev);
			if(prev is null || !days.ContainsKey((DateOnly) prev)){
				return new Day[]{new Day(0)};
			}
			return new Day[]{new Day(days[(DateOnly) prev].end)};
		}
		
		return daysl.Where(k => days.ContainsKey(k)).Select(k => days[k]).ToArray();
	}
	
	static void updateForward(DateOnly d){
		load(d);
		DateOnly? dn = findAfter(d);
		while(dn is not null){
			load(dn);
			DateOnly dn2 = (DateOnly) dn;
			if(days.ContainsKey(dn2) && days.ContainsKey(d)){
				days[dn2].start = days[d].end;
			}
			d = dn2;
			dn = findAfter(d);
		}
	}
	
	public static void addFirstBalance(float i, string? c, string? d){
		if(i == 0){
			Finances.days[Extensions.Today] = new Day(0, new List<Transaction>());
		}else{
			Finances.days[Extensions.Today] = new Day(0, new List<Transaction>{new Transaction(i, c, d)});
		}
		
		save();
	}
	
	public static void addTransactionToday(float n, string c, string d){
		c = c.Trim();
		d = d.Trim();
		
		Transaction t = new Transaction(n, c == "" ? null : c, d == "" ? null : d);
		
		DateOnly today = Extensions.Today;
		DateOnly latest = findLatest() ?? today;
		
		if(days.ContainsKey(today)){
			days[today].addTransaction(t);
		}else if(days.ContainsKey(latest)){
			days[today] = new Day(days[latest].end, new List<Transaction>{t});
		}else{
			days[today] = new Day(0);
			days[today].addTransaction(t);
		}
		save();
	}
	
	public static void addTransaction(DateOnly dn, float n, string c, string d){
		c = c.Trim();
		d = d.Trim();
		
		load(dn);
		
		Transaction t = new Transaction(n, c == "" ? null : c, d == "" ? null : d);
		
		if(days.ContainsKey(dn)){
			days[dn].addTransaction(t);
		}else{
			DateOnly? prev = findPrevious(dn);
			load(prev);
			if(prev != null && days.ContainsKey((DateOnly) prev)){
				days[dn] = new Day(days[(DateOnly) prev]?.end ?? 0, new List<Transaction>{t});
			}else{
				days[dn] = new Day(0, new List<Transaction>{t});
			}
		}
		updateForward(dn);
		save();
	}
	
	static void updateCurrentBalance(){
		DateOnly latest = findLatest() ?? Extensions.Today;
		
		if(days.ContainsKey(latest)){
			Screens.updateCurrentBalance(days[latest].end);
		}
	}
	
	static void save(){
		foreach(var kvp in days){
			if(kvp.Value.transactions.Count == 0){
				continue;
			}
			string n = kvp.Key.ToStringDate();
			
			daysFile.SetCamp(n + ".s", kvp.Value.start);
			
			for(int i = 0; i < kvp.Value.transactions.Count; i++){
				daysFile.SetCamp(n + "." + i, kvp.Value.transactions[i].number);
				
				if(kvp.Value.transactions[i].category is not null){
					daysFile.SetCamp(n + "." + i + ".c", kvp.Value.transactions[i].category!);
				}
				
				if(kvp.Value.transactions[i].description is not null){
					daysFile.SetCamp(n + "." + i + ".d", kvp.Value.transactions[i].description!);
				}
			}
		}
		
		daysFile.Save();
		updateCurrentBalance();
	}
}