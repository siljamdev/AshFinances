using System.Diagnostics;
using System.Runtime.InteropServices;
using AshLib;
using AshConsoleGraphics;
using AshConsoleGraphics.Interactive;

static class Screens{
	static TuiTwoLabels currentBalance = new TuiTwoLabels("Current balance: ", "0", Placement.TopLeft, 2, 1, null, Palette.Static);
	static TuiTwoLabels currentBalanceClone = new TuiTwoLabels("Current balance: ", "0", Placement.TopLeft, 2, 1, null, Palette.Static);
	
	public static bool doFirstOpen(){
		TuiFramedTextBoxFloat box = new TuiFramedTextBoxFloat("", 16, Placement.Center, 0, 0, null, null, null, Palette.User, Palette.User);
		
		TuiScreenInteractive dfo = null!;
		
		bool b = false;
		
		box.SubKeyEvent(ConsoleKey.Enter, (s, cki) => {
			if(float.TryParse(box.Text, out float i)){
				dfo.Stop();
				Finances.addFirstBalance(i, null, "Starting balance");
			}else{
				box.SelectedTextFormat = Palette.Error;
				if(dfo.Elements.Count < 3){
					dfo.Elements.Add(new TuiLabel("Invalid number. Try again", Placement.Center, 0, 4, Palette.Error));
				}
			}
		});
		
		dfo = new TuiScreenInteractive(20, 100,
			new TuiSelectable[,]{{
				box}},
			0, 0, null,
			new TuiLabel("Enter your starting balance:", Placement.Center, 0, -3));
		
		dfo.DeleteAllKeyEvents();
		
		dfo.SubKeyEvent(ConsoleKey.Escape, (s, ski) => {
			dfo.Stop();
			b = true;
		});
		
		dfo.WaitForKey = true;
		
		dfo.AutoResize = true;
		
		dfo.OnResize = s => Console.CursorVisible = false;
		
		Console.CursorVisible = false;
		dfo.Play();
		
		return b;
	}
	
	public static void mainScreen(){
		TuiScreenInteractive main = new TuiScreenInteractive(20, 100,
			new TuiSelectable[,]{
				{new TuiButton("Add transaction today", Placement.Center, 0, -4, null, Palette.User).SetAction((s, cki) => addTransactionTodayScreen())},
				{new TuiButton("Add transaction in the past", Placement.Center, 0, -2, null, Palette.User).SetAction((s, cki) => addTransactionPastScreen())},
				{new TuiButton("See stats", Placement.Center, 0, 0, null, Palette.User).SetAction((s, cki) => seeStatsScreen())},
				{new TuiButton("See transactions", Placement.Center, 0, 2, null, Palette.User).SetAction((s, cki) => seeTransactionsScreen())},
				{new TuiButton("Config", Placement.Center, 0, 7, null, Palette.User).SetAction((s, cki) => configScreen())},
				{new TuiButton("Credits", Placement.Center, 0, 9, null, Palette.User).SetAction((s, cki) => creditsScreen())}
			}, 0, 0, null,
			new TuiLabel("AshFinances", Placement.Center, 0, -8, Palette.Ash),
			currentBalance);
		
		main.WaitForKey = true;
		
		main.AutoResize = true;
		main.OnResize = s => Console.CursorVisible = false;
		
		Console.CursorVisible = false;
		main.Play();
	}
	
	public static void addTransactionTodayScreen(){
		TuiScreenInteractive att = new TuiScreenInteractive(20, 100, new TuiSelectable[,]{
				{new TuiFramedTextBoxFloat("", 16, Placement.Center, 0, -3, null, null, null, Palette.User, Palette.User)},
				{new TuiFramedTextBox("", 16, Placement.Center, 0, 0, null, null, null, Palette.User, Palette.User)},
				{new TuiFramedScrollingTextBox("", 512, 16, Placement.Center, 0, 3, null, null, null, Palette.User, Palette.User)},
				{new TuiButton("Done", Placement.Center, 0, 6, null, Palette.Continue, Palette.User)}
			}, 0, 0, null,
			new TuiLabel("Add transaction today", Placement.Center, 0, -8, Palette.Ash),
			new TuiLabel("Value:", Placement.Center, -15, -3),
			new TuiLabel("Category:", Placement.Center, -17, 0),
			new TuiLabel("Description:", Placement.Center, -18, 3),
			currentBalance
		);
		
		att.WaitForKey = true;
		
		att.AutoResize = true;
		att.OnResize = s => Console.CursorVisible = false;
		
		((TuiButton) att.Elements[8]).SetAction((s, cki) => {
			if(float.TryParse(((TuiWritable) att.Elements[5]).Text, out float i)){
				if(i == 0){
					((TuiFramedTextBox) att.Elements[5]).TextFormat = Palette.Error;
					((TuiFramedTextBox) att.Elements[5]).SelectedTextFormat = Palette.Error;
					if(att.Elements.Count < 10){
						att.Elements.Add(new TuiLabel("Transaction cant be 0. Try again", Placement.Center, 0, 9, Palette.Error));
					}else{
						((TuiLabel)att.Elements[9]).Text = "Transaction cant be 0. Try again";
					}
				}else{
					att.Stop();
					Finances.addTransactionToday(i, ((TuiWritable) att.Elements[6]).Text, ((TuiWritable) att.Elements[7]).Text);
				}
			}else{
				((TuiFramedTextBox) att.Elements[5]).TextFormat = Palette.Error;
				((TuiFramedTextBox) att.Elements[5]).SelectedTextFormat = Palette.Error;
				if(att.Elements.Count < 10){
					att.Elements.Add(new TuiLabel("Invalid number. Try again", Placement.Center, 0, 9, Palette.Error));
				}else{
					((TuiLabel)att.Elements[9]).Text = "Invalid number. Try again";
				}
			}
		});
		
		Console.CursorVisible = false;
		att.Play();
	}
	
	public static void addTransactionPastScreen(){
		TuiScreenInteractive att = null!;
		att = new TuiScreenInteractive(20, 100, new TuiSelectable[,]{
				{	new TuiButton("-", Placement.Center, -12, -6, null, Palette.User).SetAction((s, cki) => {
						if(((TuiWritable) att.Elements[7]).Text.TryParseDate(out DateOnly d)){
							d = d.AddDays(-1);
							((TuiWritable) att.Elements[7]).Text = d.ToStringDate();
						}
					}),
					new TuiFramedScrollingTextBoxDate(Extensions.Today.AddDays(-1).ToStringDate(), 16, Placement.Center, 0, -6, null, null, null, Palette.User, Palette.User),
					new TuiButton("+", Placement.Center, 11, -6, null, Palette.User).SetAction((s, cki) => {
						if(((TuiWritable) att.Elements[7]).Text.TryParseDate(out DateOnly d)){
							if(d >= Extensions.Today.AddDays(-1)){
								return;
							}
							d = d.AddDays(1);
							((TuiWritable) att.Elements[7]).Text = d.ToStringDate();
						}
					})},
				{null, new TuiFramedTextBoxFloat("", 16, Placement.Center, 0, -3, null, null, null, Palette.User, Palette.User), null},
				{null, new TuiFramedTextBox("", 16, Placement.Center, 0, 0, null, null, null, Palette.User, Palette.User), null},
				{null, new TuiFramedScrollingTextBox("", 512, 16, Placement.Center, 0, 3, null, null, null, Palette.User, Palette.User), null},
				{null, new TuiButton("Done", Placement.Center, 0, 6, null, Palette.Continue, Palette.User), null}
			}, 1, 1, null,
			new TuiLabel("Add transaction in the past", Placement.Center, 0, -8, Palette.Ash),
			new TuiLabel("Date (dd/mm/yyyy):", Placement.Center, -26, -6),
			new TuiLabel("Value:", Placement.Center, -15, -3),
			new TuiLabel("Category:", Placement.Center, -17, 0),
			new TuiLabel("Description:", Placement.Center, -18, 3),
			currentBalance
		);
		
		att.WaitForKey = true;
		
		att.AutoResize = true;
		att.OnResize = s => Console.CursorVisible = false;
		
		((TuiButton) att.Elements[12]).SetAction((s, cki) => {
			if(float.TryParse(((TuiWritable) att.Elements[9]).Text, out float i)){
				if(i == 0){
					((TuiFramedTextBox) att.Elements[7]).TextFormat = null;
					((TuiFramedTextBox) att.Elements[7]).SelectedTextFormat = Palette.User;
					((TuiFramedTextBox) att.Elements[9]).TextFormat = Palette.Error;
					((TuiFramedTextBox) att.Elements[9]).SelectedTextFormat = Palette.Error;
					if(att.Elements.Count < 14){
						att.Elements.Add(new TuiLabel("Transaction cant be 0. Try again", Placement.Center, 0, 9, Palette.Error));
					}else{
						((TuiLabel)att.Elements[13]).Text = "Transaction cant be 0. Try again";
					}
				}else if(((TuiWritable) att.Elements[7]).Text.TryParseDate(out DateOnly d)){
					if(d > Extensions.Today){
						((TuiFramedTextBox) att.Elements[7]).TextFormat = Palette.Error;
						((TuiFramedTextBox) att.Elements[7]).SelectedTextFormat = Palette.Error;
						((TuiFramedTextBox) att.Elements[9]).TextFormat = null;
						((TuiFramedTextBox) att.Elements[9]).SelectedTextFormat = Palette.User;
						if(att.Elements.Count < 14){
							att.Elements.Add(new TuiLabel("Date cant be future! Try again", Placement.Center, 0, 9, Palette.Error));
						}else{
							((TuiLabel)att.Elements[13]).Text = "Date cant be future! Try again";
						}
					}else{
						att.Stop();
						Finances.addTransaction(d, i, ((TuiWritable) att.Elements[10]).Text, ((TuiWritable) att.Elements[11]).Text);
					}
				}else{
					((TuiFramedTextBox) att.Elements[7]).TextFormat = Palette.Error;
					((TuiFramedTextBox) att.Elements[7]).SelectedTextFormat = Palette.Error;
					((TuiFramedTextBox) att.Elements[9]).TextFormat = null;
					((TuiFramedTextBox) att.Elements[9]).SelectedTextFormat = Palette.User;
					if(att.Elements.Count < 14){
						att.Elements.Add(new TuiLabel("Invalid date. Try again", Placement.Center, 0, 9, Palette.Error));
					}else{
						((TuiLabel)att.Elements[13]).Text = "Invalid date. Try again";
					}
				}
			}else{
				((TuiFramedTextBox) att.Elements[7]).TextFormat = null;
				((TuiFramedTextBox) att.Elements[7]).SelectedTextFormat = Palette.User;
				((TuiFramedTextBox) att.Elements[9]).TextFormat = Palette.Error;
				((TuiFramedTextBox) att.Elements[9]).SelectedTextFormat = Palette.Error;
				if(att.Elements.Count < 14){
					att.Elements.Add(new TuiLabel("Invalid number. Try again", Placement.Center, 0, 9, Palette.Error));
				}else{
					((TuiLabel)att.Elements[13]).Text = "Invalid number. Try again";
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
		
		TuiFramedScrollingTextBoxDate inputDate = new TuiFramedScrollingTextBoxDate("", 16, Placement.TopCenter, 0, 4, null, null, null, Palette.User, Palette.User);
		TuiLabel inputDateError = new TuiLabel("", Placement.TopCenter, 0, 3, Palette.Error);
		
		TuiLabel fromDate = new TuiLabel("", Placement.TopRight, 2, 4, Palette.Static);
		TuiLabel toDate = new TuiLabel("", Placement.TopRight, 2, 5, Palette.Static);
		
		inputDate.SubKeyEvent(ConsoleKey.Enter, (s, cki) => {
			if(inputDate.Text.TryParseDate(out DateOnly d)){
				startDate = d;
				update();
			}else{
				inputDateError.Text = "Invalid date. Try again";
				inputDate.TextFormat = Palette.Error;
				inputDate.SelectedTextFormat = Palette.Error;
			}
		});
		
		TuiHorizontalLine separator = new TuiHorizontalLine(20, '─', Placement.TopCenter, 0, 11);
		TuiHorizontalLine separator2 = new TuiHorizontalLine(20, '─', Placement.TopCenter, 0, 16);
		
		//15 elements
		see = new TuiScreenInteractive(20, 100, new TuiSelectable[,]{
				{new TuiButton("See 1 day", Placement.TopLeft, 4, 4, null, Palette.User).SetAction((s, ski) => {numberOfDays = 1; update();}), inputDate},
				{new TuiButton("See 7 days", Placement.TopLeft, 4, 7, null, Palette.User).SetAction((s, ski) => {numberOfDays = 7; update();}), new TuiButton("+", Placement.TopCenter, 0, 7, null, Palette.User).SetAction((s, ski) => {startDate = startDate.AddDays(numberOfDays); update();})},
				{new TuiButton("See 30 days", Placement.TopLeft, 4, 10, null, Palette.User).SetAction((s, ski) => {numberOfDays = 30; update();}), new TuiButton("-", Placement.TopCenter, 0, 10, null, Palette.User).SetAction((s, ski) => {startDate = startDate.AddDays(-numberOfDays); update();})},
			}, 0, 0, null,
			new TuiLabel("From:", Placement.TopRight, 13, 4), fromDate,
			new TuiLabel("To:", Placement.TopRight, 13, 5), toDate,
			inputDateError,
			new TuiLabel("Categories", Placement.TopCenter, 0, 17, Palette.Ash),
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
			inputDate.TextFormat = null;
			inputDate.SelectedTextFormat = Palette.User;
			
			if(15 < see.Elements.Count){
				see.Elements.RemoveRange(15, see.Elements.Count - 15);
			}
			
			Day[] days = Finances.requestDays(startDate, numberOfDays);
			
			Transaction[] transactions = days.SelectMany(d => d.transactions).ToArray();
			
			float s = days[0].start;
			float e = days[^1].end;
			float p = e - s;
			
			see.Elements.Add(new TuiTwoLabels("Start balance: ", s.ToString("F2"), Placement.TopLeft, 4, 12, null, s < 0 ? Palette.Loss : Palette.Static));
			
			see.Elements.Add(new TuiTwoLabels("Profit: ", p.ToString("F2"), Placement.TopCenter, 0, 12, null, p < 0 ? Palette.Loss : Palette.Profit));
			
			see.Elements.Add(new TuiTwoLabels("End balance: ", e.ToString("F2"), Placement.TopRight, 4, 12, null, e < 0 ? Palette.Loss : Palette.Static));
			
			Transaction[] positive = transactions.Where(t => t.number > 0).ToArray();
			Transaction[] negative = transactions.Where(t => t.number < 0).ToArray();
			
			see.Elements.Add(new TuiTwoLabels("Income: ", positive.Sum(t => t.number).ToString("F2"), Placement.TopLeft, 4, 14, null, Palette.Profit));
			
			see.Elements.Add(new TuiTwoLabels("Transactions: ", positive.Length.ToString(), Placement.TopLeft, 4, 15, null, Palette.Number));
			
			see.Elements.Add(new TuiTwoLabels("Expenses: ", (negative.Sum(t => -t.number)).ToString("F2"), Placement.TopRight, 4, 14, null, Palette.Loss));
			
			see.Elements.Add(new TuiTwoLabels("Transactions: ", negative.Length.ToString(), Placement.TopRight, 4, 15, null, Palette.Number));
			
			see.Elements.Add(new TuiTwoLabels("Total transactions: ", transactions.Length.ToString(), Placement.TopCenter, 0, 15, null, Palette.Number));
			
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
				string v = kvp.Value.ToString("F2");
				see.Elements.Add(new TuiTwoLabels(kvp.Key + ": ", v, Placement.TopLeft, x, y, null, kvp.Value < 0 ? Palette.Loss : Palette.Profit));
				w = Math.Max(w, kvp.Key.Length + 2 + v.Length);
				y++;
				if(y > see.Ysize - 1){
					y = 19;
					x += w + 3;
					w = 0;
				}
			}
		}
		
		see.WaitForKey = true;
		
		see.AutoResize = true;
		see.OnResize = s => {
			Console.CursorVisible = false;
			separator.Xsize = see.Xsize;
			separator2.Xsize = see.Xsize;
			update();
		};
		
		update();
		
		Console.CursorVisible = false;
		see.Play();
	}
	
	public static void seeTransactionsScreen(){
		TuiScreenInteractive see = null!;
		
		DateOnly startDate = Extensions.Today;
		int numberOfDays = 1;
		
		KeyValuePair<DateOnly, Transaction>[] transactions = null;
		
		TuiFramedScrollingTextBoxDate inputDate = new TuiFramedScrollingTextBoxDate("", 16, Placement.TopCenter, 0, 4, null, null, null, Palette.User, Palette.User);
		TuiLabel inputDateError = new TuiLabel("", Placement.TopCenter, 0, 3, Palette.Error);
		
		TuiButton datePlus = new TuiButton("+", Placement.TopCenter, 0, 7, null, Palette.User);
		TuiButton dateMinus = new TuiButton("-", Placement.TopCenter, 0, 10, null, Palette.User);
		
		TuiFramedTextBoxUInt inputDelete = new TuiFramedTextBoxUInt("", 16, Placement.TopCenter, 0, 5, null, null, null, Palette.Number, Palette.User);
		TuiLabel inputDeleteError = new TuiLabel("", Placement.TopCenter, 0, 4, Palette.Error);
		TuiLabel deleteLabel = new TuiLabel("Enter transaction number to delete:", Placement.TopCenter, 0, 3);
		
		TuiLabel fromDate = new TuiLabel("", Placement.TopRight, 2, 4, Palette.Static);
		TuiLabel toDate = new TuiLabel("", Placement.TopRight, 2, 5, Palette.Static);
		
		datePlus.SetAction((s, ski) => {startDate = startDate.AddDays(numberOfDays); update();});
		dateMinus.SetAction((s, ski) => {startDate = startDate.AddDays(-numberOfDays); update();});
		
		inputDate.SubKeyEvent(ConsoleKey.Enter, (s, cki) => {
			if(inputDate.Text.TryParseDate(out DateOnly d)){
				startDate = d;
				update();
			}else{
				inputDateError.Text = "Invalid date. Try again";
				inputDate.TextFormat = Palette.Error;
				inputDate.SelectedTextFormat = Palette.Error;
			}
		});
		
		inputDelete.SubKeyEvent(ConsoleKey.Enter, (s, cki) => {
			if(uint.TryParse(inputDelete.Text, out uint del) && (int) del < transactions.Length){
				Finances.deleteTransaction(transactions[del].Key, transactions[del].Value);
				update();
			}else{
				inputDeleteError.Text = "Invalid number. Try again";
				inputDelete.TextFormat = Palette.Error;
				inputDelete.SelectedTextFormat = Palette.Error;
			}
		});
		
		TuiHorizontalLine separator = new TuiHorizontalLine(20, '─', Placement.TopCenter, 0, 11);
		
		//17 elements
		see = new TuiScreenInteractive(20, 100, new TuiSelectable[,]{
				{new TuiButton("See 1 day", Placement.TopLeft, 4, 4, null, Palette.User).SetAction((s, ski) => {numberOfDays = 1; update();}), inputDate, inputDelete},
				{new TuiButton("See 7 days", Placement.TopLeft, 4, 7, null, Palette.User).SetAction((s, ski) => {numberOfDays = 7; update();}), datePlus, inputDelete},
				{new TuiButton("See 30 days", Placement.TopLeft, 4, 10, null, Palette.User).SetAction((s, ski) => {numberOfDays = 30; update();}), dateMinus, inputDelete},
			}, 0, 0, null,
			new TuiLabel("From:", Placement.TopRight, 13, 4), fromDate,
			new TuiLabel("To:", Placement.TopRight, 13, 5), toDate,
			inputDateError,
			inputDeleteError,
			deleteLabel,
			new TuiLabel("Transactions", Placement.TopCenter, 0, 13, Palette.Ash),
			currentBalanceClone,
			separator
		);
		
		void update(){
			if(startDate.AddDays(numberOfDays - 1) > Extensions.Today){
				startDate = Extensions.Today.AddDays(-(numberOfDays - 1));
			}
			inputDate.Text = startDate.ToStringDate();
			fromDate.Text = startDate.ToStringDate();
			toDate.Text = startDate.AddDays(numberOfDays - 1).ToStringDate();
			
			inputDate.OffsetX = -(int) see.Xsize/4 + 4;
			inputDateError.OffsetX = -(int) see.Xsize/4 + 4;
			datePlus.OffsetX = -(int) see.Xsize/4 + 4;
			dateMinus.OffsetX = -(int) see.Xsize/4 + 4;
			
			inputDelete.OffsetX = (int) see.Xsize/4 - 4;
			inputDeleteError.OffsetX = (int) see.Xsize/4 - 4;
			deleteLabel.OffsetX = (int) see.Xsize/4 - 4;
			
			inputDateError.Text = "";
			inputDate.TextFormat = null;
			inputDate.SelectedTextFormat = Palette.User;
			
			inputDeleteError.Text = "";
			inputDelete.TextFormat = Palette.Number;
			inputDelete.SelectedTextFormat = Palette.Number;
			
			if(17 < see.Elements.Count){
				see.Elements.RemoveRange(17, see.Elements.Count - 17);
			}
			
			KeyValuePair<DateOnly, Day>[] days = Finances.requestDaysWithDate(startDate, numberOfDays);
			
			transactions = days.SelectMany(d => d.Value.transactions.Select(t => new KeyValuePair<DateOnly, Transaction>(d.Key, t))).ToArray();
			
			string tl = transactions.Length.ToString();
			see.Elements.Add(new TuiLabel("Total transactions:", Placement.TopLeft, 4, 13));
			see.Elements.Add(new TuiLabel(tl, Placement.TopLeft, 24, 13, Palette.Number));
			
			int x = 4;
			int y = 15;
			
			int w = 0;
			
			int num = 0;
			foreach(var kvp in transactions){
				Transaction t = kvp.Value;
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
					
					see.Elements.Add(new TuiTwoLabels(num.ToString() + ". ", v2, Placement.TopLeft, x, y, Palette.Number, t.number < 0 ? Palette.Loss : Palette.Profit));
					y++;
					
					see.Elements.Add(new TuiTwoLabels("Category: ", v, Placement.TopLeft, x, y, null, Palette.Static));
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
					
					see.Elements.Add(new TuiTwoLabels(num.ToString() + ". ", v2, Placement.TopLeft, x, y, Palette.Number, t.number < 0 ? Palette.Loss : Palette.Profit));
					y++;
					
					see.Elements.Add(new TuiTwoLabels("Category: ", v, Placement.TopLeft, x, y, null, Palette.Static));
					y++;
					y++;
				}
				num++;
			}
		}
		
		see.WaitForKey = true;
		
		see.AutoResize = true;
		see.OnResize = s => {
			Console.CursorVisible = false;
			separator.Xsize = see.Xsize;
			update();
		};
		
		update();
		
		Console.CursorVisible = false;
		see.Play();
	}
	
	public static void creditsScreen(){
		TuiScreenInteractive dfo = new TuiScreenInteractive(20, 100,
			new TuiSelectable[,]{{
				new TuiButton("Open Github repo", Placement.Center, 0, 5, null, Palette.User).SetAction((s, cki) => {openUrl("https://github.com/siljamdev/AshFinances");})
			}},
			0, 0, null,
			new TuiLabel("AshFinances v" + Finances.version, Placement.Center, 0, -1, Palette.Ash),
			new TuiLabel("Made by Siljam", Placement.Center, 0, 0),
			new TuiLabel("This software is licensed under the MIT License.", Placement.Center, 0, 2)
			);
		
		dfo.WaitForKey = true;
		
		dfo.AutoResize = true;
		
		dfo.OnResize = s => Console.CursorVisible = false;
		
		Console.CursorVisible = false;
		dfo.Play();
	}
	
	public static void configScreen(){
		TuiFramedCheckBox useCols = new TuiFramedCheckBox(' ', 'X', Finances.config.GetCamp<bool>("useColors"), Placement.Center, 4, -6, null, null, null, Palette.User, Palette.User);
		
		TuiButton done = new TuiButton("Done", Placement.Center, 0, 10, null, Palette.Continue, Palette.User);
		
		TuiLabel errorLabel = new TuiLabel("", Placement.Center, 0, 9, Palette.Error);
		
		TuiScreenInteractive dfo = new TuiScreenInteractive(20, 100,
			new TuiSelectable[,]{
				{useCols, useCols},
				
				{new TuiFramedScrollingTextBoxColor3(Palette.User.foreground.ToString(), 8, Placement.Center, -15, -1, null, null, Palette.User, Palette.User, Palette.User),
				new TuiFramedScrollingTextBoxColor3(Palette.Error.foreground.ToString(), 8, Placement.Center, 15, -1, null, null, Palette.Error, Palette.Error, Palette.User)},
				
				{new TuiFramedScrollingTextBoxColor3(Palette.Static.foreground.ToString(), 8, Placement.Center, -15, 2, null, null, Palette.Static, Palette.Static, Palette.User),
				new TuiFramedScrollingTextBoxColor3(Palette.Profit.foreground.ToString(), 8, Placement.Center, 15, 2, null, null, Palette.Profit, Palette.Profit, Palette.User)},
				
				{new TuiFramedScrollingTextBoxColor3(Palette.Number.foreground.ToString(), 8, Placement.Center, -15, 5, null, null, Palette.Number, Palette.Number, Palette.User),
				new TuiFramedScrollingTextBoxColor3(Palette.Loss.foreground.ToString(), 8, Placement.Center, 15, 5, null, null, Palette.Loss, Palette.Loss, Palette.User)},
				
				{new TuiFramedScrollingTextBoxColor3(Palette.Continue.foreground.ToString(), 8, Placement.Center, -15, 8, null, null, Palette.Continue, Palette.Continue, Palette.User),
				new TuiFramedScrollingTextBoxColor3(Palette.Ash.foreground.ToString(), 8, Placement.Center, 15, 8, null, null, Palette.Ash, Palette.Ash, Palette.User)},
				
				{done, done}
			},
			0, 0, null,
			new TuiLabel("Config", Placement.Center, 0, -8, Palette.Ash),
			new TuiLabel("Use colors:", Placement.Center, -4, -6),
			new TuiLabel("Colors", Placement.Center, 0, -3),
			
			new TuiLabel("User:", Placement.Center, -24, -1), new TuiLabel("Error:", Placement.Center, 6, -1),
			new TuiLabel("Static:", Placement.Center, -25, 2), new TuiLabel("Profit:", Placement.Center, 5, 2),
			new TuiLabel("Number:", Placement.Center, -25, 5), new TuiLabel("Loss:", Placement.Center, 6, 5),
			new TuiLabel("Continue:", Placement.Center, -26, 8), new TuiLabel("Ash:", Placement.Center, 7, 8),
			
			errorLabel //11
			);
		
		dfo.WaitForKey = true;
		
		dfo.AutoResize = true;
		
		dfo.OnResize = s => Console.CursorVisible = false;
		
		done.SetAction((s, cki) => {
			if(!Color3.TryParse(((TuiWritable) dfo.Elements[13]).Text, out Color3 user)){
				errorLabel.Text = "Invalid user color. Try again";
			}
			if(!Color3.TryParse(((TuiWritable) dfo.Elements[14]).Text, out Color3 error)){
				errorLabel.Text = "Invalid error color. Try again";
			}
			if(!Color3.TryParse(((TuiWritable) dfo.Elements[15]).Text, out Color3 stat)){
				errorLabel.Text = "Invalid static color. Try again";
			}
			if(!Color3.TryParse(((TuiWritable) dfo.Elements[16]).Text, out Color3 profit)){
				errorLabel.Text = "Invalid profit color. Try again";
			}
			if(!Color3.TryParse(((TuiWritable) dfo.Elements[17]).Text, out Color3 number)){
				errorLabel.Text = "Invalid number color. Try again";
			}
			if(!Color3.TryParse(((TuiWritable) dfo.Elements[18]).Text, out Color3 loss)){
				errorLabel.Text = "Invalid loss color. Try again";
			}
			if(!Color3.TryParse(((TuiWritable) dfo.Elements[19]).Text, out Color3 contin)){
				errorLabel.Text = "Invalid continue color. Try again";
			}
			if(!Color3.TryParse(((TuiWritable) dfo.Elements[20]).Text, out Color3 ash)){
				errorLabel.Text = "Invalid ash color. Try again";
			}
			
			errorLabel.Text = "";
			
			Finances.config.SetCamp("useColors", useCols.Checked);
			
			Finances.config.SetCamp("palette.user", user);
			Finances.config.SetCamp("palette.error", error);
			Finances.config.SetCamp("palette.static", stat);
			Finances.config.SetCamp("palette.profit", profit);
			Finances.config.SetCamp("palette.number", number);
			Finances.config.SetCamp("palette.loss", loss);
			Finances.config.SetCamp("palette.continue", contin);
			Finances.config.SetCamp("palette.ash", ash);
			
			Finances.config.Save();
			
			Palette.initialize();
		});
		
		Console.CursorVisible = false;
		dfo.Play();
	}
	
	static void openUrl(string url){
		try{
			if(OperatingSystem.IsWindows()){
				Process.Start(new ProcessStartInfo{
					FileName = url,
					UseShellExecute = true
				});
			}
			else if(OperatingSystem.IsLinux()){
				Process.Start("xdg-open", url);
			}
			else if(OperatingSystem.IsMacOS()){
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
		currentBalance.RightText = currentBalanceClone.RightText = n.ToString("F2");
		if(n < 0){
			currentBalance.RightFormat = currentBalanceClone.RightFormat = Palette.Loss;
		}else{
			currentBalance.RightFormat = currentBalanceClone.RightFormat = Palette.Static;
		}
	}
}