using System.Diagnostics;
using System.Runtime.InteropServices;
using AshLib;
using AshConsoleGraphics;
using AshConsoleGraphics.Interactive;

static class Screens{
	static TuiLabel currentBalance = new TuiLabel("0", Placement.TopLeft, 19, 1, Palette.Static, null);
	
	public static void doFirstOpen(){
		TuiFramedTextBox box = new TuiFramedTextBox("", 16, Placement.Center, 0, 0, null, null, null, null, null, null, Palette.User, null, Palette.User, null);
		
		TuiScreenInteractive dfo = null!;
		
		box.SubKeyEvent(ConsoleKey.Enter, (s, cki) => {
			if(float.TryParse(box.Text, out float i)){
				dfo.Stop();
				Finances.addFirstBalance(i, null, "Starting balance");
			}else{
				box.FgTextSelectedColor = Color3.Red;
				if(dfo.Elements.Count < 3){
					dfo.Elements.Add(new TuiLabel("Invalid number. Try again", Placement.Center, 0, 4, Palette.Error, null));
				}
			}
		});
		
		dfo = new TuiScreenInteractive(20, 100,
			new TuiSelectable[,]{{
				box}},
			0, 0, null, null,
			new TuiLabel("Enter your starting balance:", Placement.Center, 0, -3));
		
		dfo.AutoResize = true;
		
		dfo.OnResize = s => Console.CursorVisible = false;
		
		Console.CursorVisible = false;
		dfo.Play();
	}
	
	public static void mainScreen(){
		TuiScreenInteractive main = new TuiScreenInteractive(20, 100,
			new TuiSelectable[,]{
				{new TuiButton("Add transaction today", Placement.Center, 0, -4, null, null, Palette.User, null).SetAction((s, cki) => addTransactionTodayScreen())},
				{new TuiButton("Add transaction in the past", Placement.Center, 0, -2, null, null, Palette.User, null).SetAction((s, cki) => addTransactionPastScreen())},
				{new TuiButton("See stats", Placement.Center, 0, 0, null, null, Palette.User, null).SetAction((s, cki) => seeStatsScreen())},
				{new TuiButton("See transactions", Placement.Center, 0, 2, null, null, Palette.User, null).SetAction((s, cki) => seeTransactionsScreen())},
				{new TuiButton("Credits", Placement.Center, 0, 7, null, null, Palette.User, null).SetAction((s, cki) => creditsScreen())}
			}, 0, 0, null, null,
			new TuiLabel("AshFinances", Placement.Center, 0, -8, Palette.Ash, null),
			new TuiLabel("Current balance:", Placement.TopLeft, 2, 1), currentBalance);
		
		main.AutoResize = true;
		main.OnResize = s => Console.CursorVisible = false;
		
		Console.CursorVisible = false;
		main.Play();
	}
	
	public static void addTransactionTodayScreen(){
		TuiScreenInteractive att = new TuiScreenInteractive(20, 100, new TuiSelectable[,]{
				{new TuiFramedTextBox("", 16, Placement.Center, 0, -3, null, null, null, null, null, null, Palette.User, null, Palette.User, null)},
				{new TuiFramedTextBox("", 16, Placement.Center, 0, 0, null, null, null, null, null, null, Palette.User, null, Palette.User, null)},
				{new TuiFramedScrollingTextBox("", 512, 16, Placement.Center, 0, 3, null, null, null, null, null, null, Palette.User, null, Palette.User, null)},
				{new TuiButton("Done", Placement.Center, 0, 6, null, null, Palette.Continue, null, Palette.User, null)}
			}, 0, 0, null, null,
			new TuiLabel("Add transaction today", Placement.Center, 0, -8, Palette.Ash, null),
			new TuiLabel("Value:", Placement.Center, -15, -3),
			new TuiLabel("Category:", Placement.Center, -17, 0),
			new TuiLabel("Description:", Placement.Center, -18, 3),
			new TuiLabel("Current balance:", Placement.TopLeft, 2, 1),
			currentBalance
		);
		
		att.AutoResize = true;
		att.OnResize = s => Console.CursorVisible = false;
		
		((TuiButton) att.Elements[9]).SetAction((s, cki) => {
			if(float.TryParse(((TuiWritable) att.Elements[6]).Text, out float i)){
				if(i == 0){
					((TuiFramedTextBox) att.Elements[6]).FgTextColor = Palette.Error;
					((TuiFramedTextBox) att.Elements[6]).FgTextSelectedColor = Palette.Error;
					if(att.Elements.Count < 11){
						att.Elements.Add(new TuiLabel("Transaction cant be 0. Try again", Placement.Center, 0, 9, Palette.Error, null));
					}else{
						((TuiLabel)att.Elements[10]).Text = "Transaction cant be 0. Try again";
					}
				}else{
					att.Stop();
					Finances.addTransactionToday(i, ((TuiWritable) att.Elements[7]).Text, ((TuiWritable) att.Elements[8]).Text);
				}
			}else{
				((TuiFramedTextBox) att.Elements[6]).FgTextColor = Palette.Error;
				((TuiFramedTextBox) att.Elements[6]).FgTextSelectedColor = Palette.Error;
				if(att.Elements.Count < 11){
					att.Elements.Add(new TuiLabel("Invalid number. Try again", Placement.Center, 0, 9, Palette.Error, null));
				}else{
					((TuiLabel)att.Elements[10]).Text = "Invalid number. Try again";
				}
			}
		});
		
		Console.CursorVisible = false;
		att.Play();
	}
	
	public static void addTransactionPastScreen(){
		TuiScreenInteractive att = null!;
		att = new TuiScreenInteractive(20, 100, new TuiSelectable[,]{
				{	new TuiButton("-", Placement.Center, -12, -6, null, null, Palette.User, null).SetAction((s, cki) => {
						if(((TuiWritable) att.Elements[8]).Text.TryParseDate(out DateOnly d)){
							d = d.AddDays(-1);
							((TuiWritable) att.Elements[8]).Text = d.ToStringDate();
						}
					}),
					new TuiFramedTextBox(Extensions.Today.AddDays(-1).ToStringDate(), 16, Placement.Center, 0, -6, null, null, null, null, null, null, Palette.User, null, Palette.User, null),
					new TuiButton("+", Placement.Center, 11, -6, null, null, Palette.User, null).SetAction((s, cki) => {
						if(((TuiWritable) att.Elements[8]).Text.TryParseDate(out DateOnly d)){
							if(d >= Extensions.Today.AddDays(-1)){
								return;
							}
							d = d.AddDays(1);
							((TuiWritable) att.Elements[8]).Text = d.ToStringDate();
						}
					})},
				{null, new TuiFramedTextBox("", 16, Placement.Center, 0, -3, null, null, null, null, null, null, Palette.User, null, Palette.User, null), null},
				{null, new TuiFramedTextBox("", 16, Placement.Center, 0, 0, null, null, null, null, null, null, Palette.User, null, Palette.User, null), null},
				{null, new TuiFramedScrollingTextBox("", 512, 16, Placement.Center, 0, 3, null, null, null, null, null, null, Palette.User, null, Palette.User, null), null},
				{null, new TuiButton("Done", Placement.Center, 0, 6, null, null, Palette.Continue, null, Palette.User, null), null}
			}, 1, 1, null, null,
			new TuiLabel("Add transaction in the past", Placement.Center, 0, -8, Palette.Ash, null),
			new TuiLabel("Date (dd/mm/yyyy):", Placement.Center, -26, -6),
			new TuiLabel("Value:", Placement.Center, -15, -3),
			new TuiLabel("Category:", Placement.Center, -17, 0),
			new TuiLabel("Description:", Placement.Center, -18, 3),
			new TuiLabel("Current balance:", Placement.TopLeft, 2, 1),
			currentBalance
		);
		
		att.AutoResize = true;
		att.OnResize = s => Console.CursorVisible = false;
		
		((TuiButton) att.Elements[13]).SetAction((s, cki) => {
			if(float.TryParse(((TuiWritable) att.Elements[10]).Text, out float i)){
				if(i == 0){
					((TuiFramedTextBox) att.Elements[8]).FgTextColor = null;
					((TuiFramedTextBox) att.Elements[8]).FgTextSelectedColor = Palette.User;
					((TuiFramedTextBox) att.Elements[10]).FgTextColor = Palette.Error;
					((TuiFramedTextBox) att.Elements[10]).FgTextSelectedColor = Palette.Error;
					if(att.Elements.Count < 15){
						att.Elements.Add(new TuiLabel("Transaction cant be 0. Try again", Placement.Center, 0, 9, Palette.Error, null));
					}else{
						((TuiLabel)att.Elements[14]).Text = "Transaction cant be 0. Try again";
					}
				}else if(((TuiWritable) att.Elements[8]).Text.TryParseDate(out DateOnly d)){
					if(d > Extensions.Today){
						((TuiFramedTextBox) att.Elements[8]).FgTextColor = Palette.Error;
						((TuiFramedTextBox) att.Elements[8]).FgTextSelectedColor = Palette.Error;
						((TuiFramedTextBox) att.Elements[10]).FgTextColor = null;
						((TuiFramedTextBox) att.Elements[10]).FgTextSelectedColor = Palette.User;
						if(att.Elements.Count < 15){
							att.Elements.Add(new TuiLabel("Date cant be future! Try again", Placement.Center, 0, 9, Palette.Error, null));
						}else{
							((TuiLabel)att.Elements[14]).Text = "Date cant be future! Try again";
						}
					}else{
						att.Stop();
						Finances.addTransaction(d, i, ((TuiWritable) att.Elements[11]).Text, ((TuiWritable) att.Elements[12]).Text);
					}
				}else{
					((TuiFramedTextBox) att.Elements[8]).FgTextColor = Palette.Error;
					((TuiFramedTextBox) att.Elements[8]).FgTextSelectedColor = Palette.Error;
					((TuiFramedTextBox) att.Elements[10]).FgTextColor = null;
					((TuiFramedTextBox) att.Elements[10]).FgTextSelectedColor = Palette.User;
					if(att.Elements.Count < 15){
						att.Elements.Add(new TuiLabel("Invalid date. Try again", Placement.Center, 0, 9, Palette.Error, null));
					}else{
						((TuiLabel)att.Elements[14]).Text = "Invalid date. Try again";
					}
				}
			}else{
				((TuiFramedTextBox) att.Elements[8]).FgTextColor = null;
				((TuiFramedTextBox) att.Elements[8]).FgTextSelectedColor = Palette.User;
				((TuiFramedTextBox) att.Elements[10]).FgTextColor = Palette.Error;
				((TuiFramedTextBox) att.Elements[10]).FgTextSelectedColor = Palette.Error;
				if(att.Elements.Count < 15){
					att.Elements.Add(new TuiLabel("Invalid number. Try again", Placement.Center, 0, 9, Palette.Error, null));
				}else{
					((TuiLabel)att.Elements[14]).Text = "Invalid number. Try again";
				}
			}
		});
		
		Console.CursorVisible = false;
		att.Play();
	}
	
	public static void seeStatsScreen(){
		TuiScreenInteractive see = null!;
		
		DateOnly startDate = Extensions.Today;
		int numberOfDays = 1;
		
		TuiFramedTextBox inputDate = new TuiFramedTextBox("", 16, Placement.TopCenter, 0, 4, null, null, null, null, null, null, Palette.User, null, Palette.User, null);
		TuiLabel inputDateError = new TuiLabel("", Placement.TopCenter, 0, 1, Palette.Error, null);
		
		TuiLabel fromDate = new TuiLabel("", Placement.TopRight, 2, 4, Palette.Static, null);
		TuiLabel toDate = new TuiLabel("", Placement.TopRight, 2, 5, Palette.Static, null);
		
		inputDate.SubKeyEvent(ConsoleKey.Enter, (s, cki) => {
			if(inputDate.Text.TryParseDate(out DateOnly d)){
				startDate = d;
				update();
			}else{
				inputDateError.Text = "Invalid date. Try again";
				inputDate.FgTextColor = Palette.Error;
				inputDate.FgTextSelectedColor = Palette.Error;
			}
		});
		
		TuiHorizontalLine separator = new TuiHorizontalLine(20, '─', Placement.TopCenter, 0, 11);
		TuiHorizontalLine separator2 = new TuiHorizontalLine(20, '─', Placement.TopCenter, 0, 16);
		
		//16 elements
		see = new TuiScreenInteractive(20, 100, new TuiSelectable[,]{
				{new TuiButton("See 1 day", Placement.TopLeft, 4, 4, null, null, Palette.User, null).SetAction((s, ski) => {numberOfDays = 1; update();}), inputDate},
				{new TuiButton("See 7 days", Placement.TopLeft, 4, 7, null, null, Palette.User, null).SetAction((s, ski) => {numberOfDays = 7; update();}), new TuiButton("+", Placement.TopCenter, 0, 7, null, null, Palette.User, null).SetAction((s, ski) => {startDate = startDate.AddDays(numberOfDays); update();})},
				{new TuiButton("See 30 days", Placement.TopLeft, 4, 10, null, null, Palette.User, null).SetAction((s, ski) => {numberOfDays = 30; update();}), new TuiButton("-", Placement.TopCenter, 0, 10, null, null, Palette.User, null).SetAction((s, ski) => {startDate = startDate.AddDays(-numberOfDays); update();})},
			}, 0, 0, null, null,
			new TuiLabel("From:", Placement.TopRight, 13, 4), fromDate,
			new TuiLabel("To:", Placement.TopRight, 13, 5), toDate,
			inputDateError,
			new TuiLabel("Categories", Placement.TopCenter, 0, 17, Palette.Ash, null),
			new TuiLabel("Current balance:", Placement.TopLeft, 2, 1),
			currentBalance,
			separator,
			separator2
		);
		
		void update(){
			if(startDate.AddDays(numberOfDays - 1) > Extensions.Today){
				startDate = Extensions.Today.AddDays(-(numberOfDays - 1));
			}
			inputDate.Text = startDate.ToStringDate();
			fromDate.Text = startDate.ToStringDate();
			toDate.Text = startDate.AddDays(numberOfDays - 1).ToStringDate();
			
			inputDateError.Text = "";
			inputDate.FgTextColor = null;
			inputDate.FgTextSelectedColor = Palette.User;
			
			if(16 < see.Elements.Count){
				see.Elements.RemoveRange(16, see.Elements.Count - 16);
			}
			
			Day[] days = Finances.requestDays(startDate, numberOfDays);
			
			Transaction[] transactions = days.SelectMany(d => d.transactions).ToArray();
			
			float s = days[0].start;
			float e = days[^1].end;
			float p = e - s;
			
			see.Elements.Add(new TuiLabel("Start balance:", Placement.TopLeft, 4, 12));
			see.Elements.Add(new TuiLabel(s.ToString("F2"), Placement.TopLeft, 19, 12, s < 0 ? Palette.Loss : Palette.Static, null));
			
			see.Elements.Add(new TuiLabel("Profit:", Placement.TopCenter, -p.ToString("F2").Length / 2 - 1, 12));
			see.Elements.Add(new TuiLabel(p.ToString("F2"), Placement.TopCenter, 4, 12, p < 0 ? Palette.Loss : Palette.Profit, null));
			
			see.Elements.Add(new TuiLabel("End balance:", Placement.TopRight, e.ToString("F2").Length + 5, 12));
			see.Elements.Add(new TuiLabel(e.ToString("F2"), Placement.TopRight, 4, 12, e < 0 ? Palette.Loss : Palette.Static, null));
			
			Transaction[] positive = transactions.Where(t => t.number > 0).ToArray();
			Transaction[] negative = transactions.Where(t => t.number < 0).ToArray();
			
			see.Elements.Add(new TuiLabel("Income:", Placement.TopLeft, 4, 14));
			see.Elements.Add(new TuiLabel(positive.Sum(t => t.number).ToString("F2"), Placement.TopLeft, 12, 14, Palette.Profit, null));
			see.Elements.Add(new TuiLabel("Transactions:", Placement.TopLeft, 4, 15));
			see.Elements.Add(new TuiLabel(positive.Length.ToString(), Placement.TopLeft, 18, 15, Palette.Number, null));
			
			string n = (-negative.Sum(t => t.number)).ToString("F2");
			string nl = negative.Length.ToString();
			see.Elements.Add(new TuiLabel("Expenses:", Placement.TopRight, n.Length + 5, 14));
			see.Elements.Add(new TuiLabel(n, Placement.TopRight, 4, 14, Palette.Loss, null));
			see.Elements.Add(new TuiLabel("Transactions:", Placement.TopRight, nl.Length + 5, 15));
			see.Elements.Add(new TuiLabel(nl, Placement.TopRight, 4, 15, Palette.Number, null));
			
			string tl = transactions.Length.ToString();
			see.Elements.Add(new TuiLabel("Total transactions:", Placement.TopCenter, -tl.Length / 2 - 6, 15));
			see.Elements.Add(new TuiLabel(tl, Placement.TopCenter, 5, 15, Palette.Number, null));
			
			Dictionary<string, float> categories = new();
			
			foreach(Transaction t in transactions){
				string c = t.category;
				if(t.category is null){
					c = "No category";
				}
				if(categories.ContainsKey(c)){
					categories[c] += t.number;
				}else{
					categories[c] = t.number;
				}
			}
			
			int x = 4;
			int y = 19;
			
			int w = 0;
			
			foreach(var kvp in categories){
				see.Elements.Add(new TuiLabel(kvp.Key + ":", Placement.TopLeft, x, y));
				string v = kvp.Value.ToString("F2");
				see.Elements.Add(new TuiLabel(v, Placement.TopLeft, x + kvp.Key.Length + 2, y, kvp.Value < 0 ? Palette.Loss : Palette.Profit, null));
				w = Math.Max(w, kvp.Key.Length + 2 + v.Length);
				y++;
				if(y > see.Ysize - 1){
					y = 19;
					x += w + 3;
					w = 0;
				}
			}
		}
		
		update();
		
		see.AutoResize = true;
		see.OnResize = s => {
			Console.CursorVisible = false;
			separator.Xsize = see.Xsize;
			separator2.Xsize = see.Xsize;
			update();
		};
		
		Console.CursorVisible = false;
		see.Play();
	}
	
	public static void seeTransactionsScreen(){
		TuiScreenInteractive see = null!;
		
		DateOnly startDate = Extensions.Today;
		int numberOfDays = 1;
		
		TuiFramedTextBox inputDate = new TuiFramedTextBox("", 16, Placement.TopCenter, 0, 4, null, null, null, null, null, null, Palette.User, null, Palette.User, null);
		TuiLabel inputDateError = new TuiLabel("", Placement.TopCenter, 0, 1, Palette.Error, null);
		
		TuiLabel fromDate = new TuiLabel("", Placement.TopRight, 2, 4, Palette.Static, null);
		TuiLabel toDate = new TuiLabel("", Placement.TopRight, 2, 5, Palette.Static, null);
		
		inputDate.SubKeyEvent(ConsoleKey.Enter, (s, cki) => {
			if(inputDate.Text.TryParseDate(out DateOnly d)){
				startDate = d;
				update();
			}else{
				inputDateError.Text = "Invalid date. Try again";
				inputDate.FgTextColor = Palette.Error;
				inputDate.FgTextSelectedColor = Palette.Error;
			}
		});
		
		TuiHorizontalLine separator = new TuiHorizontalLine(20, '─', Placement.TopCenter, 0, 11);
		
		//15 elements
		see = new TuiScreenInteractive(20, 100, new TuiSelectable[,]{
				{new TuiButton("See 1 day", Placement.TopLeft, 4, 4, null, null, Palette.User, null).SetAction((s, ski) => {numberOfDays = 1; update();}), inputDate},
				{new TuiButton("See 7 days", Placement.TopLeft, 4, 7, null, null, Palette.User, null).SetAction((s, ski) => {numberOfDays = 7; update();}), new TuiButton("+", Placement.TopCenter, 0, 7, null, null, Palette.User, null).SetAction((s, ski) => {startDate = startDate.AddDays(numberOfDays); update();})},
				{new TuiButton("See 30 days", Placement.TopLeft, 4, 10, null, null, Palette.User, null).SetAction((s, ski) => {numberOfDays = 30; update();}), new TuiButton("-", Placement.TopCenter, 0, 10, null, null, Palette.User, null).SetAction((s, ski) => {startDate = startDate.AddDays(-numberOfDays); update();})},
			}, 0, 0, null, null,
			new TuiLabel("From:", Placement.TopRight, 13, 4), fromDate,
			new TuiLabel("To:", Placement.TopRight, 13, 5), toDate,
			inputDateError,
			new TuiLabel("Transactions", Placement.TopCenter, 0, 13, Palette.Ash, null),
			new TuiLabel("Current balance:", Placement.TopLeft, 2, 1),
			currentBalance,
			separator
		);
		
		void update(){
			if(startDate.AddDays(numberOfDays - 1) > Extensions.Today){
				startDate = Extensions.Today.AddDays(-(numberOfDays - 1));
			}
			inputDate.Text = startDate.ToStringDate();
			fromDate.Text = startDate.ToStringDate();
			toDate.Text = startDate.AddDays(numberOfDays - 1).ToStringDate();
			
			inputDateError.Text = "";
			inputDate.FgTextColor = null;
			inputDate.FgTextSelectedColor = Palette.User;
			
			if(15 < see.Elements.Count){
				see.Elements.RemoveRange(15, see.Elements.Count - 15);
			}
			
			Day[] days = Finances.requestDays(startDate, numberOfDays);
			
			Transaction[] transactions = days.SelectMany(d => d.transactions).ToArray();
			
			string tl = transactions.Length.ToString();
			see.Elements.Add(new TuiLabel("Total transactions:", Placement.TopLeft, 4, 13));
			see.Elements.Add(new TuiLabel(tl, Placement.TopLeft, 24, 13, Palette.Number, null));
			
			int x = 4;
			int y = 15;
			
			int w = 0;
			
			foreach(Transaction t in transactions){
				string v = t.category ?? "No category";
				string v2 = t.number.ToString("F2");
				
				w = Extensions.Max(w, v.Length + 10, v2.Length + 8);
				
				if(t.description is not null){
					int descw = w;
					string[] l = splitString(t.description, descw - 1);
					if(y + l.Length + 3 > see.Ysize - 1){
						y = 15;
						x += w + 3;
						w = Extensions.Max(v.Length + 10, v2.Length + 8, descw);
					}
					
					see.Elements.Add(new TuiLabel("Category:", Placement.TopLeft, x, y));
					see.Elements.Add(new TuiLabel(v, Placement.TopLeft, x + 10, y, Palette.Static, null));
					y++;
					
					see.Elements.Add(new TuiLabel("Amount:", Placement.TopLeft, x, y));
					see.Elements.Add(new TuiLabel(v2, Placement.TopLeft, x + 8, y, t.number < 0 ? Palette.Loss : Palette.Profit, null));
					y++;
					
					see.Elements.Add(new TuiLabel("Description:", Placement.TopLeft, x, y));
					y++;
					foreach(string line in l){
						see.Elements.Add(new TuiLabel(line, Placement.TopLeft, x + 1, y));
						y++;
					}
					y++;
				}else{
					if(y + 2 > see.Ysize - 1){
						y = 15;
						x += w + 3;
						w = Extensions.Max(v.Length + 10, v2.Length + 8);
					}
					
					see.Elements.Add(new TuiLabel("Category:", Placement.TopLeft, x, y));
					see.Elements.Add(new TuiLabel(v, Placement.TopLeft, x + 10, y, Palette.Static, null));
					y++;
					
					see.Elements.Add(new TuiLabel("Amount:", Placement.TopLeft, x, y));
					see.Elements.Add(new TuiLabel(v2, Placement.TopLeft, x + 8, y, t.number < 0 ? Palette.Loss : Palette.Profit, null));
					y++;
					y++;
				}
			}
		}
		
		update();
		
		see.AutoResize = true;
		see.OnResize = s => {
			Console.CursorVisible = false;
			separator.Xsize = see.Xsize;
			update();
		};
		
		Console.CursorVisible = false;
		see.Play();
	}
	
	public static void creditsScreen(){
		TuiScreenInteractive dfo = new TuiScreenInteractive(20, 100,
			new TuiSelectable[,]{{
				new TuiButton("Open Github repo", Placement.Center, 0, 5, null, null, Palette.User, null).SetAction((s, cki) => {openUrl("https://github.com/siljamdev/AshFinances");})
			}},
			0, 0, null, null,
			new TuiLabel("AshFinances v" + Finances.version, Placement.Center, 0, -1, Palette.Ash, null),
			new TuiLabel("Made by Siljam", Placement.Center, 0, 0),
			new TuiLabel("This software is licensed under the MIT License.", Placement.Center, 0, 2)
			);
		
		dfo.AutoResize = true;
		
		dfo.OnResize = s => Console.CursorVisible = false;
		
		Console.CursorVisible = false;
		dfo.Play();
	}
	
	static void openUrl(string url){
		try{
			if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows)){
				Process.Start(new ProcessStartInfo{
					FileName = url,
					UseShellExecute = true
				});
			}
			else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux)){
				Process.Start("xdg-open", url);
			}
			else if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX)){
				Process.Start("open", url);
			}
		}
		catch(Exception e){}
	}
	
	static string[] splitString(string input, int chunkSize){
		int c = (input.Length + chunkSize - 1) / chunkSize;
		string[] result = new string[c];
		
		for(int i = 0; i < c; i++){
			int start = i * chunkSize;
			int length = Math.Min(chunkSize, input.Length - start);
			result[i] = input.Substring(start, length);
		}
		
		return result;
	}
	
	public static void updateCurrentBalance(float n){
		currentBalance.Text = n.ToString("F2");
		if(n < 0){
			currentBalance.FgColor = Palette.Loss;
		}else{
			currentBalance.FgColor = Palette.Static;
		}
	}
}