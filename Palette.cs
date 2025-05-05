using AshLib;
using AshLib.AshFiles;
using AshConsoleGraphics;

static class Palette{
	public static Color3? User {get; private set;}
	public static Color3? Static {get; private set;}
	public static Color3? Number {get; private set;}
	public static Color3? Continue {get; private set;}
	public static Color3? Error {get; private set;}
	public static Color3? Profit {get; private set;}
	public static Color3? Loss {get; private set;}
	public static Color3? Ash {get; private set;}
	
	public static void initialize(){
		AshFileModel m = new AshFileModel(
			new ModelInstance(ModelInstanceOperation.Type, "palette.user", Color3.Yellow),
			new ModelInstance(ModelInstanceOperation.Type, "palette.static", Color3.Yellow),
			new ModelInstance(ModelInstanceOperation.Type, "palette.number", new Color3("6BE4F4")),
			new ModelInstance(ModelInstanceOperation.Type, "palette.continue", new Color3("C8FFB5")),
			new ModelInstance(ModelInstanceOperation.Type, "palette.error", new Color3("D83F3C")),
			new ModelInstance(ModelInstanceOperation.Type, "palette.profit", Color3.Green),
			new ModelInstance(ModelInstanceOperation.Type, "palette.loss", new Color3("FF4535")),
			new ModelInstance(ModelInstanceOperation.Type, "palette.ash", new Color3("9150B2")),
			new ModelInstance(ModelInstanceOperation.Type, "useColors", true)
		);
		
		Finances.config *= m;
		
		if(isNoColorDefined() || (Finances.config.CanGetCamp("useColors", out bool b) && !b)){
			//AshConsoleGraphics.Buffer.NoColor = true; //Its broken :(
			return;
		}
		
		User = Finances.config.GetCamp<Color3>("palette.user");
		Static = Finances.config.GetCamp<Color3>("palette.static");
		Number = Finances.config.GetCamp<Color3>("palette.number");
		Continue = Finances.config.GetCamp<Color3>("palette.continue");
		Error = Finances.config.GetCamp<Color3>("palette.error");
		Profit = Finances.config.GetCamp<Color3>("palette.profit");
		Loss = Finances.config.GetCamp<Color3>("palette.loss");
		Ash = Finances.config.GetCamp<Color3>("palette.ash");
		
		Finances.config.Save();
	}
	
	static bool isNoColorDefined(){
		string noColor = Environment.GetEnvironmentVariable("NO_COLOR");
		return !string.IsNullOrEmpty(noColor);
	}
}