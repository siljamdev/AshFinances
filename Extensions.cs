using System.Globalization;

static class Extensions{
	public static bool TryParseDate(this string s, out DateOnly d){
		return DateOnly.TryParseExact(s, "dd/MM/yyyy", out d);
	}
	
	public static string ToStringDate(this DateOnly d){
		return d.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
	}
	
	public static DateOnly Today{get{
		return DateOnly.FromDateTime(DateTime.Today);
	}}
	
	public static int Max(params int[] i){
		return i.Max();
	}
}